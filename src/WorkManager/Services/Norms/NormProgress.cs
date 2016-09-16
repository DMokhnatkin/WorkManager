using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Services.Norms
{
    public class NormProgress
    {
        public Norm Norm { get; private set; }

        public TimeSpan GoalTime { get; private set; }
        public TimeSpan Excecuted { get; private set; }

        public float Percent { get { return Excecuted.Milliseconds / GoalTime.Milliseconds; } }

        public NormProgress(Norm norm, TimeSpan goalTime, TimeSpan excecuted)
        {
            Norm = norm;
            GoalTime = goalTime;
            Excecuted = excecuted;
        }
    }
}
