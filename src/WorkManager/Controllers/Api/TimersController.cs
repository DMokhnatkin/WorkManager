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
    [Route("api/timers")]
    public class TimersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuthorizationService _authorizationService;
        private readonly ITimerService _timers;
        private readonly IProjectsService _projects;
        private readonly INormService _norms;

        public TimersController(ApplicationDbContext context,
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
        public async Task<IActionResult> Statistics(int projectId, [FromQuery]DateTime start, [FromQuery]DateTime? end, [FromQuery]List<string> groupBy)
        {
            var project = await _projects.GetProjectAsync(projectId);
            if (project == null)
                return NotFound();

            if (!await _authorizationService.AuthorizeAsync(User, project, "IsOwner"))
                return NotFound();

            TimeZoneInfo timeZone = _projects.GetTimeZone(project);
            var timers = _timers.GetTimersInInterval(project, start, end);

            IQueryable grouped = null;
            if (groupBy.Count != 0)
            {
                TimersGroupFlags groupByFlag = TimersGroupFlags.none;
                if (groupBy.Contains("day"))
                    groupByFlag |= TimersGroupFlags.day;
                if (groupBy.Contains("month"))
                    groupByFlag |= TimersGroupFlags.month;

                grouped = _timers.GroupTimers(timers, groupByFlag);
            }

            return new OkObjectResult(grouped ?? timers);
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

            var now = _timers.GetNowTime(project);
            var timers = _timers.GetTimersInInterval(project, now.Date, null);
            var duration = _timers.GetDuration(timers);

            return new OkObjectResult(new {
                timers = timers,
                duration = duration,
                isRunning = await _timers.GetOpenedTimerAsync(project) != null,
                progress = await _norms.GetProgressAsync(project, now.Date)
            });
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
            var startOfWeek = DateHelpers.GetStartOfWeek(now, _culture);
            var endOfWeek = DateHelpers.GetEndOfWeek(now, _culture);

            var timers = _timers.GetTimersInInterval(project, startOfWeek, endOfWeek);
            var grouped = _timers.GroupTimers(timers, TimersGroupFlags.day);

            return new OkObjectResult(grouped);
        }

        // Get current month timers
        [HttpGet("month/{projectId:int}")]
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
            var startOfMonth = DateHelpers.GetStartOfMonth(now);
            var endOfMonth = DateHelpers.GetEndOfMonth(now);

            var timers = _timers.GetTimersInInterval(project, startOfMonth, endOfMonth);
            var grouped = timers.GroupBy(x => x.Started.Date).Select(g => new { Day = g.Key, Timers = g.ToList() });

            return new OkObjectResult(grouped);
        }
    }
}