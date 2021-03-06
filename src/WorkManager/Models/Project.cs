﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using WorkManager.Models.Norms;

namespace WorkManager.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string TimeZone { get; set; }
        public string Culture { get; set; }

        public string OwnerId { get; set; }
        public ApplicationUser Owner { get; set; }

        public virtual Norm Norm { get; set; }
    }
}
