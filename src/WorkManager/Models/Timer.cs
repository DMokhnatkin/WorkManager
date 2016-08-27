﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace WorkManager.Models
{
    [DataContract]
    public class Timer
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public DateTime Started { get; set; }
        [DataMember]
        public DateTime? Stopped { get; set; }

        [DataMember]
        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}