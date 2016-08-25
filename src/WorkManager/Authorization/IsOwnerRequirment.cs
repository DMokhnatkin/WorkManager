using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WorkManager.Models;

namespace WorkManager.Authorization
{
    public class IsOwnerRequirment : IAuthorizationRequirement
    {
    }

    public class IsOwnerHandler : AuthorizationHandler<IsOwnerRequirment, Project>
    {
        UserManager<ApplicationUser> _userManager;
        public IsOwnerHandler(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsOwnerRequirment requirement, Project project)
        {
            var userId = _userManager.GetUserId(context.User);
            if (userId == project.OwnerId)
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
