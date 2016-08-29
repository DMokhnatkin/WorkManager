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

namespace WorkManager.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly IProjectsService _projects;

        public ProjectsController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService,
            IProjectsService projects)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _projects = projects;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
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

            DetailsViewModel model = new DetailsViewModel()
            {
                Title = project.Title,
                Description = project.Description,
                TimeZone = project.TimeZone,
                Culture = project.Culture
            };

            return View(model);
        }

        // GET: Projects/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Projects/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Description,Title")] Project project)
        {
            if (ModelState.IsValid)
            {
                project.Owner = await _userManager.GetUserAsync(User);
                _context.Add(project);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
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

                TimeZoneList = TimeZoneInfo.GetSystemTimeZones().Select(x => new SelectListItem() { Text = x.Id, Value = x.Id }).ToList(),
                CultureList = new List<SelectListItem>()
                {
                    new SelectListItem() { Text = "Russian", Value = "ru-Ru" },
                    new SelectListItem() { Text = "English(US)", Value = "en-US" }
                },
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
                return RedirectToAction("Index");
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

            _projects.Remove(project);
            return RedirectToAction("Index");
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
