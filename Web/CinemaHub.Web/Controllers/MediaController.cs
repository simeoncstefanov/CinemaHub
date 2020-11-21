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

        [HttpGet("Media/Movies/{id}")]
        public async Task<IActionResult> MoviesDetails(string id)
        {
            var viewModel = await this.mediaService.GetMovieDetailsAsync(id);
            if (viewModel.MediaType != "Movie" || viewModel is null)
            {
                // TODO: ADD ERROR VIEW;
                return this.NotFound();
            }

            return this.View("MediaDetails", viewModel);
        }

        [HttpGet("Media/Shows/{id}")]
        public async Task<IActionResult> ShowsDetails(string id)
        {
            var viewModel = await this.mediaService.GetMovieDetailsAsync(id);
            if (viewModel.MediaType != "Show" || viewModel is null)
            {
                // TODO: ADD ERROR VIEW;
                return this.NotFound();
            }

            return this.View("MediaDetails", viewModel);
        }

        public async Task<IActionResult> Movies([FromForm] string query, [FromQuery] int page = 1)
        {
            double resultsFound = (double)this.mediaService.SearchMedia(query, MediaEnum.Movie);
            var viewModel = await this.mediaService.GetPageElementsAsync(page, 20);

            var model = new MediaGridViewModel()
            {
                ResultsFound = (int)resultsFound,
                ElementsPerPage = 20,
                Medias = viewModel,
                MediaType = "Show",
                TotalPages = (int)Math.Ceiling(resultsFound / 20.0),
                CurrentPage = page,
            };

            return this.View(model);
        }

        public async Task<IActionResult> Shows([FromForm] string query, [FromQuery] int page = 1)
        {
            double resultsFound = (double)this.mediaService.SearchMedia(query, MediaEnum.Show);
            var viewModel = await this.mediaService.GetPageElementsAsync(page, 20);

            var model = new MediaGridViewModel()
            {
                ResultsFound = (int)resultsFound,
                ElementsPerPage = 20,
                Medias = viewModel,
                MediaType = "Show",
                TotalPages = (int)Math.Ceiling(resultsFound / 20.0),
                CurrentPage = page,
            };

            return this.View(model);
        }
    }
}
