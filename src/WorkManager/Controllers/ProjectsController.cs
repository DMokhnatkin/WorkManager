using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WorkManager.Data;
using WorkManager.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using WorkManager.Authorization;
using WorkManager.Models.ProjectsViewModels;
using System.Globalization;
using WorkManager.Services.Projects;
using WorkManager.Services.Norms;

namespace WorkManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IProjectsService _projects;
        private readonly INormService _norms;

        public ProjectsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService,
            IProjectsService projects,
            INormService norms)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _projects = projects;
            _norms = norms;
        }

        // GET: Projects
        public async Task<IActionResult> List()
        {
            var userId = _userManager.GetUserId(User);
            return View(await _projects.GetProjectsForOwner(userId).ToListAsync());
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projects.GetProjectAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            if (!await CanAccessToProject(project))
                return NotFound();

            // Generate days list for culture
            var culture = _projects.GetCulture(project);
            var days = new List<DayOfWeek>();
            for (int i = (int)culture.DateTimeFormat.FirstDayOfWeek; i < 7; i++)
                days.Add((DayOfWeek)i);
            for (int i = 0; days.Count != 7; i++)
                days.Add((DayOfWeek)i);

            DetailsViewModel model = new DetailsViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                TimeZone = project.TimeZone,
                Culture = _projects.GetCulture(project),
                Days = days,
                Norm = await _norms.GetNorm(project) 
            };

            return View(model);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View(new CreateViewModel());
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Title,TimeZone,Culture")] CreateViewModel model)
        {
            Project project = new Project()
            {
                Description = model.Description,
                Title = model.Title,
                TimeZone = model.TimeZone,
                Culture = model.Culture
            };
            if (ModelState.IsValid)
            {
                project.Owner = await _userManager.GetUserAsync(User);
                await _projects.CreateProject(project);
                return RedirectToAction("List");
            }
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projects.GetProjectAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            if (!await CanAccessToProject(project))
                return NotFound();

            var viewModel = new EditViewModel()
            {
                Id = project.Id,
                Title = project.Title,
                Description = project.Description,
                TimeZone = project.TimeZone,
                Culture = project.Culture,
            };

            return View(viewModel);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Description,Title,TimeZone,Culture")] Project project)
        {
            if (id != project.Id)
            {
                return NotFound();
            }

            if (!await CanAccessToProject(project))
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(project);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _projects.ExistsAsync(project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("List");
            }
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projects.GetProjectAsync(id.Value);
            if (project == null)
            {
                return NotFound();
            }

            if (!await CanAccessToProject(project))
                return NotFound();

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var project = await _projects.GetProjectAsync(id);

            if (!await CanAccessToProject(project))
                return NotFound();

            await _projects.Remove(project);
            return RedirectToAction("List");
        }

        private async Task<bool> CanAccessToProject(Project project)
        {
            // If OwnerId is null, load it from database
            if (project.OwnerId == null)
                project.OwnerId = _context.Projects
                    .Where(x => x.Id == project.Id)
                    .Select(x => x.OwnerId)
                    .Single();
            return await _authorizationService.AuthorizeAsync(User, project, "IsOwner");
        }
    }
}
