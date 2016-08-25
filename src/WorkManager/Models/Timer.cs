using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManager.Models
{
    public class Timer
    {
        public int Id { get; set; }
        public DateTime Started { get; set; }
        public DateTime? Stopped { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; }
    }
}
