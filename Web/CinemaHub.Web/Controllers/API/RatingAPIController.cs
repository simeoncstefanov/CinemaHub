namespace CinemaHub.Web.Controllers
{
    using System.Collections.Generic;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CinemaHub.Services.Data;
    using CinemaHub.Web.Controllers;
    using CinemaHub.Web.ViewModels.Reviews;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/reviews")]
    public class RatingAPIController : ControllerBase
    {
        private readonly IReviewsService reviewService;

        public RatingAPIController(IReviewsService reviewsService)
        {
            this.reviewService = reviewsService;
        }

        [HttpGet]
        public async Task<IEnumerable<ReviewViewModel>> GetReviews(string mediaId, int page, string sortType)
        {
            var reviews = await this.reviewService.GetReviews<ReviewViewModel>(mediaId, page, sortType);
            return reviews;

        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<RatingResponseModel>> Post(RatingInputModel input)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            await this.reviewService.CreateRating(input.MediaId, userId, input.Value);

            var ratingService = await this.reviewService.GetRatingAverageCount(input.MediaId);

            var average = ratingService.Item1;
            var count = ratingService.Item2;

            var response = new RatingResponseModel() { TotalVotes = count, AverageVote = average, };
            return response;
        }
    }
}