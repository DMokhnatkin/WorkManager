using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Services.Projects
{
    public interface IProjectsService
    {
        Task<Project> CreateProject(Project project);
        Task<Project> GetProjectAsync(int id);
        TimeZoneInfo GetTimeZone(Project project);
        CultureInfo GetCulture(Project project);
        IQueryable<Project> GetProjectsForOwner(string ownerId);
        Task Remove(Project project);
        Task<bool> ExistsAsync(int id);
        Task<Project> UpdateAsync(Project project);
    }
}
