namespace CinemaHub.Web.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Common;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Common.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.VisualBasic;

    public class EntityIsCreatedByAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, ICreatedByEntity>
    {
        private readonly UserManager<ApplicationUser> userManager;

        public EntityIsCreatedByAuthorizationHandler(UserManager<ApplicationUser>
                                                     userManager)
        {
            this.userManager = userManager;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       OperationAuthorizationRequirement requirement,
                                                       ICreatedByEntity resource)
        {
            if (context.User == null || resource == null)
            {
                return Task.CompletedTask;
            }

            // If not asking for CRUD permission, return.

            if (requirement.Name != GlobalConstants.CreateOperationName &&
                requirement.Name != GlobalConstants.ReadOperationName &&
                requirement.Name != GlobalConstants.UpdateOperationName &&
                requirement.Name != GlobalConstants.DeleteOperationName)
            {
                return Task.CompletedTask;
            }

            if (resource.CreatorId == this.userManager.GetUserId(context.User))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
