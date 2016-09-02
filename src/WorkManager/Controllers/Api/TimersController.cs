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

namespace WorkManager.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/Timers")]
    public class TimersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ITimerService _timers;
        private readonly IProjectsService _projects;

        public TimersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService,
            ITimerService timers,
            IProjectsService projects)
        {
            _context = context;
            _userManager = userManager;
            _authorizationService = authorizationService;
            _timers = timers;
            _projects = projects;
        }

        [HttpGet("start/{projectId:int}")]
        // Start timer for a project
        public async Task<IActionResult> Start(int projectId)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            await _timers.StartTimerAsync(project);

            return Ok();
        }

        [HttpGet("stop/{projectId:int}")]
        // Stop timer for a project
        public async Task<IActionResult> Stop(int projectId)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            await _timers.StopTimerAsync(project);

            return Ok();
        }

        // Get timers in time interval.
        [HttpGet("statistics/{projectId:int}")]
        public async Task<IActionResult> Statistics(int projectId, [FromQuery]DateTime from, [FromQuery]DateTime? to)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            TimeZoneInfo timeZone = _projects.GetTimeZone(project);

            return new OkObjectResult(
                _timers.GetTimersInInterval(project, from, to));
        }

        // Get today timers
        [HttpGet("today/{projectId:int}")]
        public async Task<IActionResult> TodayStatistics(int projectId)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            TimeZoneInfo timeZone = _projects.GetTimeZone(project);
            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, timeZone);

            return new OkObjectResult(
                _timers.GetTimersInInterval(project, now.Date, null));
        }

        // Get current week timers
        [HttpGet("week/{projectId:int}")]
        public async Task<IActionResult> WeekStatistics(int projectId)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            TimeZoneInfo timeZone = _projects.GetTimeZone(project);
            CultureInfo _culture = _projects.GetCulture(project);

            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, timeZone);
            var startOfWeek = StartOfWeek(now, _culture.DateTimeFormat.FirstDayOfWeek);

            var timers = _timers.GetTimersInInterval(project, startOfWeek, null);
            var grouped = timers.GroupBy(x => x.Started.Date).Select(g => new { Day = g.Key, Timers = g.ToList() });

            return new OkObjectResult(grouped);
        }

        // Get current month timers
        [HttpGet("week/{projectId:int}")]
        public async Task<IActionResult> MonthStatistics(int projectId)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            TimeZoneInfo timeZone = _projects.GetTimeZone(project);
            CultureInfo _culture = _projects.GetCulture(project);

            var now = TimeZoneInfo.ConvertTime(DateTime.UtcNow, TimeZoneInfo.Utc, timeZone);
            var startOfMonth = StartOfMonth(now);

            var timers = _timers.GetTimersInInterval(project, startOfMonth, null);
            var grouped = timers.GroupBy(x => x.Started.Date).Select(g => new { Day = g.Key, Timers = g.ToList() });

            return new OkObjectResult(grouped);
        }

        private DateTime StartOfWeek(DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }

        private DateTime StartOfMonth(DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 0);
        }
    }
}