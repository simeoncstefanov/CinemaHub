using CinemaHub.Common;
using Hangfire.Annotations;
using Hangfire.Dashboard;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CinemaHub.Web.Filters.Authorization.Hangfire
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {
            var httpContext = context.GetHttpContext();
            return httpContext.User.Identity.IsAuthenticated &&
                httpContext.User.IsInRole(GlobalConstants.AdministratorRoleName);
        }
    }
}
