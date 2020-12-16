namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using CinemaHub.Common;
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
    using Microsoft.Extensions.Logging.Abstractions;

    using Newtonsoft.Json;

    public class MediaController : BaseController
    {
        private const int DefaultPerPage = 20;
        private readonly IMediaService mediaService;
        private readonly IMovieAPIService apiService;
        private readonly IUserService userService;
        private readonly IReviewsService reviewsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthorizationService authorizationService;
        private readonly IWebHostEnvironment webHostEnvironment;
        private readonly IMediaEditService mediaEditService;

        public MediaController(IMediaService mediaService,
                               IMovieAPIService apiService,
                               IWebHostEnvironment webHostEnvironment,
                               IReviewsService reviewsService,
                               IUserService userService,
                               IAuthorizationService authorizationService,
                               UserManager<ApplicationUser> userManager,
                               IMediaEditService mediaEditService)
        {
            this.mediaService = mediaService;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
            this.apiService = apiService;
            this.reviewsService = reviewsService;
            this.userService = userService;
            this.authorizationService = authorizationService;
            this.mediaEditService = mediaEditService;
        }

        // controllers get the searched media with ajax
        [AllowAnonymous]
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Movies()
        {
            return this.View(new MediaGridViewModel() { MediaType = "Movie" });
        }

        // controllers get the searched media with ajax
        [AllowAnonymous]
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Shows()
        {
            return this.View(new MediaGridViewModel() { MediaType = "Show" });
        }

        [AllowAnonymous]
        [Route("/[controller]/[action]/{id}")]
        public async Task<IActionResult> Movies(string id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            MediaDetailsViewModel viewModel = null;
            try
            {
                viewModel = await this.AllMediaInfo(id, "Movie", user);

                if (viewModel is null)
                {
                    return this.NotFound();
                }

            }
            catch (Exception ex)
            {
                return this.NotFound();
            }

            return this.View("MediaDetails", viewModel);
        }

        [AllowAnonymous]
        [Route("/[controller]/[action]/{id}")]
        public async Task<IActionResult> Shows(string id)
        {
            var user = await this.userManager.GetUserAsync(this.User);
            MediaDetailsViewModel viewModel = null;
            try
            {
                viewModel = await this.AllMediaInfo(id, "Show", user);

                if (viewModel is null)
                {
                    return this.NotFound();
                }

            }
            catch (Exception ex)
            {
                return this.NotFound();
            }

            return this.View("MediaDetails", viewModel);
        }

        [AllowAnonymous]
        public async Task<IActionResult> All(string query)
        {
            return this.View("Error");
        }

        public async Task<IActionResult> Add()
        {
            return this.View(new MediaDetailsInputModel());
        }

        [ImportModelState]
        [ImportInputModel(ClassName = nameof(MediaDetailsInputModel))]
        public async Task<IActionResult> Edit(string id)
        {
            var media = await this.mediaService.GetDetailsAsync<MediaDetailsInputModel>(id);
            return this.View(media);
        }

        // Will request edit to an administrator if it is a regular user. Administrator can straight edit.
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

            if (edit.Id == null && edit.PosterImageFile == null)
            {
                this.ModelState.AddModelError(string.Empty, "When you add new media you must upload poster!");
                return this.RedirectToAction("Edit", "Media", new { id = edit.Id });
            }

            var isAdmin = this.User.IsInRole(GlobalConstants.AdministratorRoleName);
            try
            {
                if (!isAdmin)
                {
                    await this.mediaEditService.ApplyEditForApproval(edit, user.Id, this.webHostEnvironment.WebRootPath);
                }
                else
                {
                    await this.mediaService.EditDetailsAsync(edit, user.Id, this.webHostEnvironment.WebRootPath);
                }
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
                return this.RedirectToAction("Edit", "Media", new { id = edit.Id });
            }

            return this.Redirect($"/Media/{edit.MediaType}s/{edit.Id}");
        }

        [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
        public async Task<IActionResult> CommitEdit(MediaDetailsInputModel edit)
        {
            return this.View();
        }

        private async Task<MediaDetailsViewModel> AllMediaInfo(string id, string mediaType, ApplicationUser user)
        {
            // Fills the details if the information is not full.
            var title = this.mediaService.IsMediaDetailsFullAsync(id);
            if (title != null)
            {
                var apiId = await this.apiService.GetIdFromTitle(title, mediaType);
                var apiInputModel = await this.apiService.GetDetailsFromApiAsync(apiId, mediaType);
                await this.mediaService.EditDetailsAsync(apiInputModel, string.Empty, this.webHostEnvironment.WebRootPath);
            }

            var viewModel = await this.mediaService.GetDetailsAsync<MediaDetailsViewModel>(id);
            if (viewModel.MediaType != mediaType || viewModel is null)
            {
                return null;
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

            return viewModel;
        }
    }
}
