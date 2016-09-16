using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Data;
using WorkManager.Models;
using WorkManager.Models.Norms;
using WorkManager.Services.Timers;

namespace WorkManager.Services.Norms
{
    public class NormService : INormService
    {
        private readonly ITimerService _timers;
        private readonly ApplicationDbContext _context;

        public NormService(ITimerService timers, ApplicationDbContext context)
        {
            _timers = timers;
            _context = context;
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
                    throw new NotImplementedException();
                    break;
                case NormType.Month:
                    throw new NotImplementedException();
                    break;
                case NormType.Project:
                    var timers = _timers.GetDuration(_timers.GetTimersInInterval(project, null, date));
                    return new NormProgress(norm, norm.Goal, timers);
                case NormType.None:
                    return null;
            }
            throw new ArgumentException();
        }
    }
}
