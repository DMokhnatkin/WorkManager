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
        private readonly ProjectsService _projects;

        public TimersController(ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            IAuthorizationService authorizationService,
            ITimerService timers,
            ProjectsService projects)
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

            TimeZoneInfo timeZone = TimeZoneInfo.Utc;
            try
            {
                timeZone = TimeZoneInfo.FindSystemTimeZoneById(project.TimeZone);
            }
            catch (InvalidTimeZoneException)
            {
                return BadRequest("Ivalid timezone");
            }

            return new OkObjectResult(
                _timers.GetTimersInInterval(project, from, to));
        }

        // Get today timers
        [HttpGet("today/{projectId:int}")]
        public IActionResult TodayStatistics(int projectId, [FromQuery]string timezoneId)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.Utc;
            try
            {
                if (timezoneId != null)
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }
            catch (InvalidTimeZoneException e)
            {
                return BadRequest("Ivalid timezone id");
            }

            var timeofday = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone).TimeOfDay;
            // When current local day started in utc time
            var localday_started_in_utc = DateTime.UtcNow - timeofday;
            return RedirectToAction("Statistics",
                new
                {
                    projectId = projectId,
                    from = localday_started_in_utc,
                });
        }

        // Get current week timers
        [HttpGet("week/{projectId:int}")]
        public IActionResult WeekStatistics(int projectId, [FromQuery]string timezoneId, [FromQuery]string culture)
        {
            TimeZoneInfo timeZone = TimeZoneInfo.Utc;
            try
            {
                if (timezoneId != null)
                    timeZone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);
            }
            catch (InvalidTimeZoneException e)
            {
                return BadRequest("Ivalid timezone id");
            }

            CultureInfo _culture = new CultureInfo("en-US");
            try
            {
                if (culture != null)
                    _culture = new CultureInfo(culture);
            }
            catch (CultureNotFoundException)
            {
                return BadRequest("Ivalid culture");
            }
            var first_week_day = _culture.DateTimeFormat.FirstDayOfWeek;

            var local_date = TimeZoneInfo.ConvertTime(DateTime.UtcNow, timeZone);
            // When week was started in local timezone
            var local_week_start = StartOfWeek(local_date, first_week_day);
            // Convert local week started time to utc
            var local_week_start_utc = TimeZoneInfo.ConvertTime(local_week_start, TimeZoneInfo.Utc);
            return RedirectToAction("Statistics",
                new
                {
                    projectId = projectId,
                    from = local_week_start_utc,
                    group_by_days = true
                });
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
    }
}