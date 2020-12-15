namespace CinemaHub.Services.Recommendations
{
    using System.Threading.Tasks;
    using System.Collections.Generic;

    public interface IRecommendService
    {
        Task<IEnumerable<string>> GetMediaIdsBasedOnKeywords(string userId);
    }
}
