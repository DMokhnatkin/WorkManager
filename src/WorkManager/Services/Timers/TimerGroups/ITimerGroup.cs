using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Services.Timers
{
    [Flags]
    public enum TimersGroupFlags
    {
        none = 0x0,
        day = 0x1,
        month = 0x2,
    }

    // Timer group is used to group timers by some date (f.e. day or month)
    public interface ITimerGroup
    {
        DateTime Date { get; set; }
        IQueryable Childs { get; set; }
    }
}
