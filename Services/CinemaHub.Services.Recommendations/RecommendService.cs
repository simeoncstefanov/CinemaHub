namespace CinemaHub.Services.Recommendations
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
    using CinemaHub.Services.Recommendations.Training.Models;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.ML;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.ML;
    using CinemaHub.Services.Recommendations.Training;

    public class RecommendService : IRecommendService
    {
        private readonly IRepository<Rating> ratingRepo;
        private readonly IRepository<Media> mediaRepo;
        private readonly IRepository<ApplicationUser> userRepo;
        private readonly IRepository<Keyword> keywordsRepo;
        private readonly PredictionEnginePool<MovieRating, MovieRatingPrediction> predictionEnginePool;
        private readonly IServiceProvider serviceProvider;

        public RecommendService(IRepository<Rating> ratingRepo,
                                IRepository<ApplicationUser> userRepo,
                                IRepository<Media> mediaRepo,
                                IRepository<Keyword> keywordsRepo,
                                PredictionEnginePool<MovieRating, MovieRatingPrediction> predictionEnginePool,
                                IServiceProvider serviceProvider)
        {
            this.ratingRepo = ratingRepo;
            this.userRepo = userRepo;
            this.mediaRepo = mediaRepo;
            this.keywordsRepo = keywordsRepo;
            this.predictionEnginePool = predictionEnginePool;
            this.serviceProvider = serviceProvider;
        }

        public async Task<IEnumerable<string>> GetMediaIdsBasedOnKeywords(string userId)
        {
            // Get user
            var top20UserRatings = this.ratingRepo.AllAsNoTracking().Where(x => x.CreatorId == userId)
                .OrderByDescending(x => x.Score).ThenByDescending(x => x.CreatedOn).Take(20);

            // Rank best keywords
            var bestKeywords = await top20UserRatings
                .SelectMany(y => y.Media.Keywords.Select(x => new { Id = x.KeywordId, Score = y.Score }))
                .GroupBy(p => p.Id)
                .Select(g => new
                                 {
                                     KeywordId = g.Key,
                                     Score = g.Average(x => x.Score),
                                     Count = g.Count(),
                                 })
                .ToListAsync();

            var bestKeywordsUser = bestKeywords.OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Count)
                .Select(x => x.KeywordId)
                .ToList();


            var recommendedMovies = await this.mediaRepo.AllAsNoTracking()
                .Where(x => !x.Watchers.Any(x => x.UserId == userId))
                .OrderByDescending(x => x.Keywords.Count(x => bestKeywordsUser.Contains(x.KeywordId)))
                .Take(10)
                .Select(x => x.Id)
                .ToListAsync();

            return recommendedMovies;
        }

        public async Task<IEnumerable<string>> GetMediaIdsBasedOnOtherUsers(string userId)
        {
            var mediaSelection = await this.mediaRepo.AllAsNoTracking()
                .Where(x => x.Ratings.Select(x => x.Id).Count() > 0)
                .Where(x => !x.Watchers.Any(x => x.UserId == userId))
                .Select(x => x.Id)
                .ToListAsync();

            var predictions = new List<MovieIdRating>();

            foreach(var id in mediaSelection)
            {
                var score = this.GetSinglePrediction(userId, id);
                
                if (score == null)
                {
                    continue;
                }

                predictions.Add(new MovieIdRating()
                {
                    MediaId = id,
                    Score = score.Score,
                });
            }

            return predictions.OrderByDescending(x => x.Score).Where(x => x.Score != Double.NaN).Select(x => x.MediaId).Take(10);        
        }

        public MovieRatingPrediction GetSinglePrediction(string userId, string mediaId)
        {
            MovieRating rating = new MovieRating()
            {
                userId = userId,
                movieId = mediaId,
            };

            try
            {
                var prediction = this.predictionEnginePool.Predict(modelName: "MovieRecommendation", example: rating);
                return prediction;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task TrainModel(string path)
        {
            var ratings = this.ratingRepo.All();

            if (ratings.Count() > 10)
            {
                var recommendationTrainer = this.serviceProvider.GetRequiredService<IRecommendationModelTrainer>();

                try 
                {
                    var data = await recommendationTrainer.LoadData();
                    var model = await recommendationTrainer.BuildAndTrainModel(data.training);
                    await recommendationTrainer.EvaluateModel(data.testing, model);
                    await recommendationTrainer.SaveModel(data.training, model, path);
                }
                catch (Exception ex)
                {
                    var mess = ex.Message;
                    Console.WriteLine(mess);
                }
            }
        }
    }
}
