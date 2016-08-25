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

namespace WorkManager.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Timers")]
    public class TimersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;

        public TimersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
        }

        [HttpGet("start/{projectId:int}")]
        // Start timer for a project
        public async Task<IActionResult> Start(int projectId)
        {
            var project = _context.Projects.SingleOrDefault(x => x.Id == projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            var opened_timer = GetOpenedTimer(project);
            // If some timer is opened we don't need to do anything
            if (opened_timer != null)
                return Ok();

            var new_timer = new Timer()
            {
                Started = DateTime.UtcNow,
                Stopped = null,
                ProjectId = project.Id
            };
            _context.Timers.Add(new_timer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("stop/{projectId:int}")]
        // Stop timer for a project
        public async Task<IActionResult> Stop(int projectId)
        {
            var project = _context.Projects.SingleOrDefault(x => x.Id == projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            var opened_timer = GetOpenedTimer(project);
            if (opened_timer == null)
                return BadRequest();

            opened_timer.Stopped = DateTime.UtcNow;
            _context.Timers.Update(opened_timer);
            await _context.SaveChangesAsync();

            return Ok();
        }

        private Timer GetOpenedTimer(Project project)
        {
            var opened_timer = _context.Timers
                .Where(x => x.ProjectId == project.Id)
                .Where(x => x.Stopped == null)
                .SingleOrDefault();
            return opened_timer;
        }
    }
}