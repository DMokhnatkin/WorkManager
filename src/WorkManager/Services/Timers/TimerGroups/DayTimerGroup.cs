using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Services.Timers
{
    public class DayTimerGroup : ITimerGroup
    {
        public DateTime Date { get; set; }

        public IQueryable Childs { get; set; }

        public TimeSpan Duration { get; set; }
    }
}
