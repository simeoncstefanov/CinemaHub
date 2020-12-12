using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHub.Services.Recommendation
{
    using System.Threading.Tasks;

    using CinemaHub.Services.Recommendation.Models;

    public interface IRecommendService
    {
        Task<IEnumerable<string>> GetMediaIdsBasedOnKeywords(string userId, string mediaId);
    }
}
