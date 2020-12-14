namespace ContactManager.Authorization
{
    using System.Threading.Tasks;

    using CinemaHub.Common;
    using CinemaHub.Data.Common.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Infrastructure;

    public class EntityAdministratorAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, ICreatedByEntity>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            OperationAuthorizationRequirement requirement,
            ICreatedByEntity resource)
        {
            if (context.User == null)
            {
                return Task.CompletedTask;
            }

            // Administrators can do anything.
            if (context.User.IsInRole(GlobalConstants.AdministratorRoleName))
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}