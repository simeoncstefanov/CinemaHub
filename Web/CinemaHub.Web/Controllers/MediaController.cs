namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Web.Filters.Action.InputModelTransfer;
    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.Reviews;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Components.Forms;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.CodeAnalysis.Operations;

    using Newtonsoft.Json;

    public class MediaController : BaseController
    {
        private const int DefaultPerPage = 20;
        private readonly IMediaService mediaService;
        private readonly IMovieAPIService apiService;
        private readonly IUserService userService;
        private readonly IReviewsService reviewsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public MediaController(IMediaService mediaService,
                               IMovieAPIService apiService,
                               IWebHostEnvironment webHostEnvironment,
                               IReviewsService reviewsService,
                               IUserService userService,
                               UserManager<ApplicationUser> userManager)
        {
            this.mediaService = mediaService;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
            this.apiService = apiService;
            this.reviewsService = reviewsService;
            this.userService = userService;
        }

        // both controllers get the searched media with ajax
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Movies()
        {
            return this.View(new MediaGridViewModel() { MediaType = "Movie" });
        }

        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Shows()
        {
            return this.View(new MediaGridViewModel() { MediaType = "Show" });
        }

        [Route("/[controller]/[action]/{id}")]
        public async Task<IActionResult> Movies(string id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            // Fills the details if the information is not full because of the weird api
            var title = this.mediaService.IsMediaDetailsFullAsync(id);
            if (title != null)
            {
                var apiInputModel = await this.apiService.GetDetailsFromApiAsync(title, "Movie", id);
                await this.mediaService.EditDetailsAsync(apiInputModel, "", this.webHostEnvironment.WebRootPath);
            }

            var viewModel = await this.mediaService.GetDetailsAsync<MediaDetailsViewModel>(id);
            if (viewModel.MediaType != "Movie" || viewModel is null)
            {
                // TODO: ADD ERROR VIEW;
                return this.NotFound();
            }

            var ratings = await this.reviewsService.GetRatingAverageCount(id);
            viewModel.AverageRating = ratings.Item1;
            viewModel.RatingCount = ratings.Item2;
            if (user != null)
            {
                viewModel.CurrentUserRating = await this.reviewsService.GetRatingForMedia(user.Id, id);
                viewModel.UserWatchType = await this.userService.GetWatchtypeUserAsync(user.Id, id);
            }
            viewModel.ReviewCount = await this.reviewsService.GetReviewCount(id);
            return this.View("MediaDetails", viewModel);
        }

        [Route("/[controller]/[action]/{id}")]
        public async Task<IActionResult> Shows(string id)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            // Fills the details if the information is not full because of the weird api
            var title = this.mediaService.IsMediaDetailsFullAsync(id);
            if (title != null)
            {
                var apiInputModel = await this.apiService.GetDetailsFromApiAsync(title, "Show", id);
                await this.mediaService.EditDetailsAsync(apiInputModel, "", this.webHostEnvironment.WebRootPath);
            }

            var viewModel = await this.mediaService.GetDetailsAsync<MediaDetailsViewModel>(id);
            if (viewModel.MediaType != "Show" || viewModel is null)
            {
                // TODO: ADD ERROR VIEW;
                return this.NotFound();
            }

            var ratings = await this.reviewsService.GetRatingAverageCount(id);
            viewModel.AverageRating = ratings.Item1;
            viewModel.RatingCount = ratings.Item2;
            if (user != null)
            {
                viewModel.CurrentUserRating = await this.reviewsService.GetRatingForMedia(user.Id, id);
                viewModel.UserWatchType = await this.userService.GetWatchtypeUserAsync(user.Id, id);
            }
            viewModel.ReviewCount = await this.reviewsService.GetReviewCount(id);
            return this.View("MediaDetails", viewModel);
        }

        public async Task<IActionResult> All(string query)
        {
            return this.View("Error");
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            return this.View(new MediaDetailsInputModel());
        }

        [Authorize]
        [ImportModelState]
        [ImportInputModel(ClassName = nameof(MediaDetailsInputModel))]
        public async Task<IActionResult> Edit(string id)
        {
            var media = await this.mediaService.GetDetailsAsync<MediaDetailsInputModel>(id);
            return this.View(media);
        }

        // Edits or adds new movie if there is no id in the input model
        [Authorize]
        [HttpPost]
        [ExportModelState]
        [ExportInputModel]
        public async Task<IActionResult> Edit(MediaDetailsInputModel edit)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Edit", "Media", new { id = edit.Id });
            }

            if (edit.ReleaseDateString == "01-01-0001" || edit.ReleaseDate == null)
            {
                this.ModelState.AddModelError(string.Empty, "Release date is required");
                return this.RedirectToAction("Edit", "Media", new { id = edit.Id });
            }

            try
            {
                await this.mediaService.EditDetailsAsync(edit, user.Id, this.webHostEnvironment.WebRootPath);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.RedirectToAction("Edit", "Media", new { id = edit.Id });
            }

            return this.Redirect($"/Media/{edit.MediaType}s/{edit.Id}");
        }
    }
}
