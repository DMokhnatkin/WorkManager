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
            if (opened_timer != null)
                throw new ArgumentException("Already started");

            var new_timer = new Timer()
            {
                Started = GetNowTime(project),
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

            opened_timer.Stopped = GetNowTime(project);
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

        public IQueryable<Timer> GetTimersInInterval(Project project, DateTime? from, DateTime? to)
        {
            var query = _context.Timers
                .Where(x => x.ProjectId == project.Id);
            if (from != null)
                query = query.Where(x => x.Started >= from);
            if (to != null)
                query = query.Where(x => x.Stopped <= to);
            return query;
        }

        public DateTime GetNowTime(Project project)
        {
            var timeZone = _projects.GetTimeZone(project);
            return TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, timeZone);
        }

        public TimeSpan GetDuration(Timer timer)
        {
            if (timer.Stopped != null)
                return timer.Stopped.Value - timer.Started;
            else
                return GetNowTime(timer.Project) - timer.Started;
        }

        public IQueryable<ITimerGroup> GroupTimers(IQueryable<Timer> timers, TimersGroupFlags groupBy)
        {
            IQueryable<ITimerGroup> grouped = null;
            if ((groupBy & TimersGroupFlags.day) == TimersGroupFlags.day)
            {
                grouped = timers
                        .Include(x => x.Project) // Todo Lazy loading is not implemented in ef core 1.0. So use eager loading
                        .GroupBy(x => x.Started.Date)
                        .Select(g => new DayTimerGroup{
                            Date = g.Key,
                            Childs = g.AsQueryable(),
                            Duration = GetDuration(g.AsQueryable<Timer>())
                        });
            }
            if ((groupBy & TimersGroupFlags.month) == TimersGroupFlags.month)
            {
                if ((groupBy & TimersGroupFlags.day) == TimersGroupFlags.day)
                {
                    // We have already grouped by day. Just add group by month.
                    grouped = grouped
                        .GroupBy(x => new DateTime(x.Date.Year, x.Date.Month, 0))
                        .Select(g => new MonthTimerGroup
                        {
                            Date = g.Key,
                            Childs = g.AsQueryable()
                        });
                }
                else
                {
                    grouped = timers
                        .GroupBy(x => new DateTime(x.Started.Date.Year, x.Started.Date.Month, 0))
                        .Select(g => new MonthTimerGroup
                        {
                            Date = g.Key,
                            Childs = g.AsQueryable()
                        });
                }
            }

            return grouped;
        }

        public TimeSpan GetDuration(IQueryable<Timer> timers)
        {

            // TODO: use aggregate (not implemented in ef core 1.0)
            //return Task.Run<TimeSpan>(() => 
            //    timers.Aggregate(new TimeSpan(0), (sum, next) => sum.Add(GetDuration(next))));
            var duration = new TimeSpan(0);
            foreach (var z in timers.ToList())
                duration += GetDuration(z);
            return duration;
        }
    }
}
