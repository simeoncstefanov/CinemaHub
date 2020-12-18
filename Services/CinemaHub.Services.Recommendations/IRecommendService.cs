namespace CinemaHub.Services.Recommendations
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using CinemaHub.Services.Recommendations.Training.Models;

    public interface IRecommendService
    {
        Task<IEnumerable<string>> GetMediaIdsBasedOnKeywords(string userId);

        Task<IEnumerable<string>> GetMediaIdsBasedOnOtherUsers(string userId);

        MovieRatingPrediction GetSinglePrediction(string userId, string mediaId);

        Task TrainModel(string path);
    }
}
