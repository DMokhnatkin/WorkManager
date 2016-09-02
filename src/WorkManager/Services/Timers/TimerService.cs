using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Data;
using WorkManager.Models;
using WorkManager.Services.Projects;

namespace WorkManager.Services.Timers
{
    public class TimerService : ITimerService
    {
        private readonly IProjectsService _projects;
        private readonly ApplicationDbContext _context;

        public TimerService(IProjectsService projects, ApplicationDbContext context)
        {
            _projects = projects;
            _context = context;
        }

        public async Task<Timer> StartTimerAsync(Project project)
        {
            var opened_timer = await GetOpenedTimerAsync(project);
            // If some timer is opened throw exception
            if (opened_timer != null)
                throw new ArgumentException("Already started");

            var new_timer = new Timer()
            {
                Started = DateTime.UtcNow,
                Stopped = null,
                ProjectId = project.Id,
                TimeZoneId = project.TimeZone
            };
            _context.Timers.Add(new_timer);
            await _context.SaveChangesAsync();

            return new_timer;
        }

        public async Task<Timer> StopTimerAsync(Project project)
        {
            var opened_timer = await GetOpenedTimerAsync(project);
            if (opened_timer == null)
                throw new ArgumentException("Already stoped");

            opened_timer.Stopped = DateTime.UtcNow;
            _context.Timers.Update(opened_timer);
            await _context.SaveChangesAsync();

            return opened_timer;
        }

        public async Task<Timer> GetOpenedTimerAsync(Project project)
        {
            var opened_timer = await _context.Timers
                .Where(x => x.ProjectId == project.Id)
                .Where(x => x.Stopped == null)
                .SingleOrDefaultAsync();
            return opened_timer;
        }

        public IQueryable<Timer> GetTimersInInterval(Project project, DateTime from, DateTime? to)
        {
            var query = _context.Timers
                .Where(x => x.ProjectId == project.Id)
                .Where(x => x.Started >= from);
            if (to != null)
                query.Where(x => x.Stopped <= to);
            return query;
        }
    }
}
