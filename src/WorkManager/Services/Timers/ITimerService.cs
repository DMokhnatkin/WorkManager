﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Services.Timers
{
    public interface ITimerService
    {
        Task<Timer> StartTimerAsync(Project project);
        Task<Timer> StopTimerAsync(Project project);
        Task<Timer> GetOpenedTimerAsync(Project project);
        IQueryable<Timer> GetTimersInInterval(Project project, DateTime? from, DateTime? to);
        TimeSpan GetDuration(Timer timer);
        TimeSpan GetDuration(IQueryable<Timer> timers);
        DateTime GetNowTime(Project project);
        IQueryable<ITimerGroup> GroupTimers(IQueryable<Timer> timers, TimersGroupFlags groupBy);
    }
}
