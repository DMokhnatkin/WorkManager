using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Data;
using WorkManager.Models;

namespace WorkManager.Services.Projects
{
    public class ProjectsService : IProjectsService
    {
        private readonly ApplicationDbContext _context;

        public ProjectsService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> ExistsAsync(int id)
        {
            return await _context.Projects.AnyAsync(e => e.Id == id);
        }

        public async Task<Project> GetProjectAsync(int id)
        {
            return await _context.Projects.SingleOrDefaultAsync(x => x.Id == id);
        }

        public IQueryable<Project> GetProjectsForOwner(string ownerId)
        {
            return _context.Projects.Where(x => x.OwnerId == ownerId);
        }

        /// <summary>
        /// Get timezone specifed for project
        /// </summary>
        /// <returns>Timezone, if is founded on computer</returns>
        public TimeZoneInfo GetTimeZone(Project project)
        {
            return TimeZoneInfo.FindSystemTimeZoneById(project.TimeZone);
        }

        public CultureInfo GetCulture(Project project)
        {
            if (project.Culture == null)
                throw new ArgumentNullException("Culture is null");
            return new CultureInfo(project.Culture);
        }

        public async Task Remove(Project project)
        {
            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
        }
    }
}
