namespace CinemaHub.Web.Filters.Action.ModelStateTransfer
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Mvc.Filters;

    public abstract class TransferModelState : ActionFilterAttribute
    {
        protected const string Key = nameof(TransferModelState);
    }
}
