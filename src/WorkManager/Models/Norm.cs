﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace WorkManager.Models
{
    public enum NormType { Day, Week, Month, Project };

    public class Norm
    {
        public int Id { get; set; }

        public int ProjectId { get; set; }
        public virtual Project Project { get; set; }

        public NormType Type { get; set; }
        // How much to work (every day/week/month/..)
        public TimeSpan Goal { get; set; }
    }
}
