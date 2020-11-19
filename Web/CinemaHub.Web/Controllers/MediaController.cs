namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Services.Data;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Media;
    using Microsoft.AspNetCore.Mvc;

    public class MediaController : BaseController
    {
        private readonly IMediaService mediaService;

        public MediaController(IMediaService mediaService)
        {
            this.mediaService = mediaService;
        }

        [HttpGet("Movies/Media/{id}")]
        public async Task<IActionResult> Movies(string id)
        {
            var viewModel = await this.mediaService.GetMovieDetailsAsync(id);
            return this.View("MovieDetails", viewModel);
        }

        public async Task<IActionResult> Movies([FromQuery] int page = 1)
        {
            var viewModel = await this.mediaService.GetPageElementsAsync(page, 20);

            return this.View(viewModel);
        }

        public IActionResult Shows()
        {
            return this.View();
        }

    }
}
