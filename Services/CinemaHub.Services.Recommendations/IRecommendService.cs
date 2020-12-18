namespace CinemaHub.Services.Recommendations
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using CinemaHub.Services.Recommendations.Training.Models;

    public interface IRecommendService
    {
        Task<IEnumerable<string>> GetMediaIdsBasedOnKeywords(string userId);

        Task<IEnumerable<string>> GetMediaIdsBasedOnOtherUsers(string userId, string mediaId);

        MovieRatingPrediction GetSinglePrediciton(string userId, string mediaId);

        Task TrainModel(string path);
    }
}
