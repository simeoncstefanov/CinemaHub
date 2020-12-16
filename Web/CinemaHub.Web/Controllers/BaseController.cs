namespace CinemaHub.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Result is NotFoundResult)
            {
                context.Result = new ViewResult()
                                     {
                                         ViewName = "~/Views/Home/NotFound.cshtml",
                                         StatusCode = 404,
                                     };
            }

            base.OnActionExecuted(context);
        }
    }
}