using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WorkManager.Models.ProjectsViewModels
{
    public class EditViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public string TimeZone { get; set; }
        public List<SelectListItem> TimeZoneList { get; set; }

        public string Culture { get; set; }
        public IEnumerable<SelectListItem> CultureList { get; set; }
    }
}
