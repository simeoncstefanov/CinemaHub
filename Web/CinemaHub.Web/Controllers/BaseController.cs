namespace CinemaHub.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    public class BaseController : Controller
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (TempData["ModelState"] != null && !ModelState.Equals(TempData["ModelState"]))
                ModelState.Merge((ModelStateDictionary)TempData["ModelState"]);

            base.OnActionExecuted(filterContext);
        }
    }
}
