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
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.Reviews;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

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
        public async Task<IActionResult> Add(string mediaId)
        {
            var userId = await this.userManager.GetUserAsync(this.User);
            var media = await this.mediaService.GetDetailsAsync<MediaPathViewModel>(mediaId);

            if (media == null)
            {
                return this.Redirect("/");
            }

            int currentUserRating = await this.reviewService.GetRatingForMedia(userId.Id, mediaId);
            return this.View(new ReviewInputModel() { MediaId = mediaId, CurrentUserRating = currentUserRating});
        }

        [HttpPost]
        public async Task<IActionResult> Add(ReviewInputModel inputModel)
        {
            var userId = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.View(inputModel);
            }

            try
            {
                await this.reviewService.CreateReview(userId.Id, inputModel);
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError(string.Empty, e.Message);
                return this.View(inputModel);
            }

            var media = await this.mediaService.GetDetailsAsync<MediaPathViewModel>(inputModel.MediaId);
            return this.Redirect(media.MediaPath);
        }

    }
}
