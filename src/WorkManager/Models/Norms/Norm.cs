using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;

namespace WorkManager.Models.Norms
{
    public enum NormType { Day, Week, Month, Project, None };

    [DataContract]
    public class Norm
    {
        [Required, Key, DataMember]
        public int ProjectId { get; set; }
        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }

        [DataMember]
        public NormType Type { get; set; } = NormType.None;
        // How much to work (every day/week/month/..)
        [DataMember]
        public TimeSpan Goal { get; set; }
    }
}
