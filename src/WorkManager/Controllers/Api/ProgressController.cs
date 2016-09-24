using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WorkManager.Data;
using WorkManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using WorkManager.Services.Timers;
using WorkManager.Services.Projects;
using WorkManager.Services.Norms;
using WorkManager.Helpers;

namespace WorkManager.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/progress")]
    public class ProgressController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ITimerService _timers;
        private readonly IProjectsService _projects;
        private readonly INormService _norms;

        public ProgressController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService,
            ITimerService timers,
            IProjectsService projects,
            INormService norms)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _timers = timers;
            _projects = projects;
            _norms = norms;
        }

        // Get progress
        [HttpGet("current/{projectId:int}")]
        public async Task<IActionResult> Progress(int projectId)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            var now = _timers.GetNowTime(project);

            return new OkObjectResult(new
            {
                isRunning = await _timers.GetOpenedTimerAsync(project) != null,
                progress = await _norms.GetProgressAsync(project, now.Date)
            });
        }
    }
}
