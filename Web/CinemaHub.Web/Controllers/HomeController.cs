namespace CinemaHub.Web.Controllers
{
    using System.Diagnostics;

    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        [Route("/")]
        public IActionResult Index(string returnUrl = null)
        {
            return this.View();
        }

        public IActionResult About()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }

        public IActionResult NotFound()
        {
            return this.View();
        }
    }
}
