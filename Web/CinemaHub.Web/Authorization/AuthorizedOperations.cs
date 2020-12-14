namespace CinemaHub.Web.Authorization
{
    using CinemaHub.Common;

    using Microsoft.AspNetCore.Authorization.Infrastructure;
    using Microsoft.VisualBasic;

    public static class AuthorizedOperations
    {
        public static OperationAuthorizationRequirement Create =
            new OperationAuthorizationRequirement { Name = GlobalConstants.CreateOperationName };

        public static OperationAuthorizationRequirement Read =
            new OperationAuthorizationRequirement { Name = GlobalConstants.ReadOperationName };

        public static OperationAuthorizationRequirement Update =
            new OperationAuthorizationRequirement { Name = GlobalConstants.UpdateOperationName };

        public static OperationAuthorizationRequirement Delete =
            new OperationAuthorizationRequirement { Name = GlobalConstants.DeleteOperationName };

        public static OperationAuthorizationRequirement Approve =
            new OperationAuthorizationRequirement { Name = GlobalConstants.ApproveOperationName };

        public static OperationAuthorizationRequirement Reject =
            new OperationAuthorizationRequirement { Name = GlobalConstants.RejectOperationName };
    }
}