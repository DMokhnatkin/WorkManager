using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models.Norms;

namespace WorkManager.Models.ProjectsViewModels
{
    public class DetailsViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string TimeZone { get; set; }
        public string Culture { get; set; }
        public CultureInfo GetCulture()
        {
            return new CultureInfo(Culture);
        }
        public Norm Norm { get; set; }

        public IEnumerable<DayOfWeek> Days { get; set; }
    }
}
