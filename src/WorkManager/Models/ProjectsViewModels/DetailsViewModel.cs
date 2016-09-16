using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManager.Models.ProjectsViewModels
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string TimeZone { get; set; }
        public CultureInfo Culture { get; set; }
        public Norm Norm { get; set; }

        public IEnumerable<DayOfWeek> Days { get; set; }
    }
}
