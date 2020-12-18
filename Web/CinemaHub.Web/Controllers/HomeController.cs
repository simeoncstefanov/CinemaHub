namespace CinemaHub.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [AllowAnonymous]
    public class HomeController : BaseController
    {
        private readonly IMediaService mediaService;
        public HomeController(IMediaService mediaService)
        {
            this.mediaService = mediaService;
        }

        [Route("/")]
        public async Task<IActionResult> Index(string returnUrl = null)
        {
            var result = await this.mediaService.GetPageAsync(new MediaQueryDTO() { Page = 1, ElementsPerPage = 10, SortType = "date-desc" });
            return this.View(result);
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
