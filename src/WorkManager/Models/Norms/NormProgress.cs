using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Models.Norms
{
    [DataContract]
    public class NormProgress
    {
        [DataMember]
        public Norm Norm { get; private set; }

        [DataMember]
        public TimeSpan GoalTime { get; private set; }
        [DataMember]
        public TimeSpan Excecuted { get; private set; }
        [DataMember]
        public bool IsCompleted { get; private set; }

        public float Percent { get { return Excecuted.Milliseconds / GoalTime.Milliseconds; } }

        public NormProgress(Norm norm, TimeSpan goalTime, TimeSpan excecuted)
        {
            Norm = norm;
            GoalTime = goalTime;
            Excecuted = excecuted;
            IsCompleted = Excecuted > GoalTime;
        }
    }
}
