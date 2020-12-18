namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Threading.Tasks;

    using CinemaHub.Web.ViewModels.Reviews;

    public interface IReviewsService
    {
        Task<IEnumerable<T>> GetReviews<T>(string mediaId, int page, string sort);

        Task<Tuple<double, int>> GetRatingAverageCount(string mediaId);

        Task CreateReview(string userId, ReviewInputModel inputModel);

        Task CreateRating(string mediaId, string userId, byte rating);

        Task<int> GetReviewCount(string mediaId);

        Task<int> GetRatingForMedia(string userId, string mediaId);

        Task<int> GetRatingCount();
    }
}
