namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services;
    using CinemaHub.Services.Data;
    using CinemaHub.Web.Filters.Action;
    using CinemaHub.Web.Filters.Action.InputModelTransfer;
    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.Reviews;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    using Newtonsoft.Json;

    public class ReviewsController : BaseController
    {
        private readonly IReviewsService reviewService;

        private readonly IMediaService mediaService;
        private readonly UserManager<ApplicationUser> userManager;


        public ReviewsController(IReviewsService reviewService, IMediaService mediaService, UserManager<ApplicationUser> userManager)
        {
            this.reviewService = reviewService;
            this.mediaService = mediaService;
            this.userManager = userManager;
        }

        [Authorize]
        [ImportInputModel(ClassName = nameof(ReviewInputModel))]
        [ImportModelState]
        public async Task<IActionResult> Add(string mediaId)
        {
            var userId = await this.userManager.GetUserAsync(this.User);

            // Check if media exists
            var media = await this.mediaService.GetDetailsAsync<MediaPathViewModel>(mediaId);

            if (media == null)
            {
                return this.Redirect("/");
            }

            int currentUserRating = await this.reviewService.GetRatingForMedia(userId.Id, mediaId);

            return this.View(new ReviewInputModel() { MediaId = mediaId, CurrentUserRating = currentUserRating });
        }

        [Authorize]
        [HttpPost]
        [ExportInputModel]
        [ExportModelState]
        public async Task<IActionResult> Add(ReviewInputModel inputModel)
        {
            var userId = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Add", "Reviews", new { mediaId = inputModel.MediaId });
            }

            try
            {
                await this.reviewService.CreateReview(userId.Id, inputModel);
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError(string.Empty, e.Message);
                return this.RedirectToAction("Add", "Reviews", new { mediaId = inputModel.MediaId });
            }

            var media = await this.mediaService.GetDetailsAsync<MediaPathViewModel>(inputModel.MediaId);
            return this.Redirect(media.MediaPath);
        }

    }
}
