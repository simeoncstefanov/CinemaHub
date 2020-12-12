namespace CinemaHub.Services.Recommendation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Security.Cryptography.X509Certificates;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Recommendation.Models;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.ML.Transforms;

    public class RecommendService : IRecommendService
    {
        private readonly IRepository<Rating> ratingRepo;
        private readonly IRepository<Media> mediaRepo;
        private readonly IRepository<ApplicationUser> userRepo;
        private readonly IRepository<Keyword> keywordsRepo;

        public RecommendService(IRepository<Rating> ratingRepo,
                                IRepository<ApplicationUser> userRepo,
                                IRepository<Media> mediaRepo,
                                IRepository<Keyword> keywordsRepo)
        {
            this.ratingRepo = ratingRepo;
            this.userRepo = userRepo;
            this.mediaRepo = mediaRepo;
            this.keywordsRepo = keywordsRepo;
        }

        public async Task<IEnumerable<string>> GetMediaIdsBasedOnKeywords(string userId, string mediaId)
        {
            // Get user
            var top20UserRatings = this.ratingRepo.AllAsNoTracking().Where(x => x.CreatorId == userId)
                .OrderByDescending(x => x.Score).ThenByDescending(x => x.CreatedOn).Take(20);

            // Rank best keywords
            var bestKeywordsUser = top20UserRatings
                .SelectMany(y => y.Media.Keywords.Select(x => new { Id = x.KeywordId, Score = y.Score }))
                .GroupBy(p => p.Id)
                .Select(g => new
                                 {
                                     KeywordId = g.Key,
                                     Score = g.Average(x => x.Score),
                                 })
                .OrderByDescending(x => x.Score)
                .Select(x => x.KeywordId)
                .Take(20)
                .ToList();

            var recommendedMovies = await this.mediaRepo.AllAsNoTracking()
                .Where(x => !x.Watchers.Any(x => x.UserId == userId && (x.WatchType == WatchType.Completed || x.WatchType == WatchType.OnWatchlist)))
                .OrderByDescending(x => x.Keywords.Count(x => bestKeywordsUser.Contains(x.KeywordId)))
                .Select(x => x.Id)
                .Take(20)
                .ToListAsync();

            return recommendedMovies;

        }
    }
}
