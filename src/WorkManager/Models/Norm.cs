using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace WorkManager.Models
{
    public enum NormType { Day, Week, Month, Project, None };

    public class Norm
    {
        [Required]
        [Key]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        public NormType Type { get; set; } = NormType.None;
        // How much to work (every day/week/month/..)
        public TimeSpan Goal { get; set; }
    }
}
