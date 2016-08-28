﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManager.Models.ProjectsViewModels
{
    public class DetailsViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }

        public string TimeZone { get; set; }
        public string Culture { get; set; }
    }
}
