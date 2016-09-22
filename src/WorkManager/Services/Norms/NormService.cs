using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Data;
using WorkManager.Models;
using WorkManager.Models.Norms;
using WorkManager.Services.Timers;
using WorkManager.Helpers;
using WorkManager.Services.Projects;

namespace WorkManager.Services.Norms
{
    public class NormService : INormService
    {
        private readonly ITimerService _timers;
        private readonly ApplicationDbContext _context;
        private readonly IProjectsService _projects;

        public NormService(ITimerService timers, ApplicationDbContext context, IProjectsService projects)
        {
            _timers = timers;
            _context = context;
            _projects = projects;
        }

        public async Task<Norm> GetNormAsync(Project project)
        {
            return await _context.Norms.SingleAsync(x => x.ProjectId == project.Id);
        }

        // Calc and return progress
        public async Task<NormProgress> GetProgressAsync(Project project, DateTime date)
        {
            Norm norm = await GetNormAsync(project);
            switch (norm.Type)
            {
                case NormType.Day:
                    var duration = _timers.GetDuration(_timers.GetTimersInInterval(project, date.Date, date.Date.AddDays(1)));
                    return new NormProgress(norm, norm.Goal, duration);
                case NormType.Week:
                    var startOfWeek = DateHelpers.GetStartOfWeek(date, _projects.GetCulture(project));
                    // Get [(start of week)..date] duration
                    var curWeekDuration = _timers.GetDuration(_timers.GetTimersInInterval(project, startOfWeek, date));
                    return new NormProgress(norm, norm.Goal, curWeekDuration);
                case NormType.Month:
                    var startOfMonth = DateHelpers.GetStartOfMonth(date);
                    // Get [(start of month)..date] duration
                    var curMonthDuration = _timers.GetDuration(_timers.GetTimersInInterval(project, startOfMonth, date));
                    return new NormProgress(norm, norm.Goal, curMonthDuration);
                case NormType.Project:
                    var timers = _timers.GetDuration(_timers.GetTimersInInterval(project, null, date));
                    return new NormProgress(norm, norm.Goal, timers);
                case NormType.None:
                    return new NormProgress(norm, new TimeSpan(0), new TimeSpan(0));
            }
            throw new ArgumentException();
        }
    }
}
