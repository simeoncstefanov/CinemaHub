namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Recommendations;
    using CinemaHub.Web.ViewModels.Recommendations;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.View;

    public class RecommendationsController : BaseController
    {
        private readonly IRecommendService recommendService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMediaService mediaService;

        public RecommendationsController(
            UserManager<ApplicationUser> userManager,
            IRecommendService recommendService,
            IMediaService mediaService)
        {
            this.recommendService = recommendService;
            this.userManager = userManager;
            this.mediaService = mediaService;
        }

        [Authorize]
        public async Task<IActionResult> Index()
        {
            var userId = this.userManager.GetUserId(this.User);

            var recommendedIds = await this.recommendService.GetMediaIdsBasedOnKeywords(userId);
            var recommendedIdsOnModel = await this.recommendService.GetMediaIdsBasedOnOtherUsers(userId);

            var mergedIds = recommendedIds.Union(recommendedIdsOnModel);
            var recommendedMovies = await this.mediaService.GetMediaBatch<MediaRecommendationViewModel>(mergedIds);

            var viewModel = new RecommendationResponseViewModel()
                                {
                                    TotalRecommendations = recommendedIds.Count(),
                                    Recommendations = recommendedMovies,
                                };

            return this.View("~/Views/Recommendations/Recommend.cshtml", viewModel);
        }
    }
}
