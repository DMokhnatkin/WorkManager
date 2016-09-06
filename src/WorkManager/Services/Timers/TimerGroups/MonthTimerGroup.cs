using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Services.Timers
{
    public class MonthTimerGroup : ITimerGroup
    {
        public DateTime Date { get; set; }

        public IQueryable Childs { get; set; }
    }
}
