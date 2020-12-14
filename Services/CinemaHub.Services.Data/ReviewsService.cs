namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Common;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels.Reviews;

    using Microsoft.EntityFrameworkCore;

    public class ReviewsService : IReviewsService
    {
        private const int DefaultReviewsPerPage = 5;
        private readonly IRepository<Review> reviewRepository;
        private readonly IRepository<Rating> ratingRepository;
        private readonly IRepository<Media> mediaRepository;

        public ReviewsService(
            IRepository<Review> reviewRepository,
            IRepository<Rating> ratingRepository,
            IRepository<Media> mediaRepository)
        {
            this.reviewRepository = reviewRepository;
            this.ratingRepository = ratingRepository;
            this.mediaRepository = mediaRepository;
        }

        public async Task CreateRating(string mediaId, string userId, byte rating)
        {
            var ratingDb = await this.ratingRepository.All()
                .Where(x => x.CreatorId == userId && x.MediaId == mediaId)
                .FirstOrDefaultAsync();

            if (ratingDb != null)
            {
                ratingDb.Score = rating;
            }
            else
            {
                var newRating = new Rating()
                                    {
                                        CreatorId = userId,
                                        MediaId = mediaId,
                                        Score = rating,
                                    };

                await this.ratingRepository.AddAsync(newRating);
            }

            await this.ratingRepository.SaveChangesAsync();
        }

        public async Task CreateReview(string userId, ReviewInputModel inputModel)
        {
            var ratingDb = await this.ratingRepository.All()
                               .Include(x => x.Review)
                               .Where(x => x.CreatorId == userId && x.MediaId == inputModel.MediaId)
                               .FirstOrDefaultAsync();

            if (ratingDb == null)
            {
                throw new Exception("You need to rate the media to submit a review.");
            }

            if (ratingDb.Review != null)
            {
                throw new Exception("You have already created a review. Delete it to create new one.");
            }

            var newReview = new Review()
                                {
                                    Rating = ratingDb,
                                    Title = inputModel.Title,
                                    ReviewText = inputModel.ReviewText,
                                    MediaId = inputModel.MediaId,
                                    CreatorId = userId,
                                };

            await this.reviewRepository.AddAsync(newReview);
            await this.reviewRepository.SaveChangesAsync();
        }

        public async Task<Tuple<double, int>> GetRatingAverageCount(string mediaId)
        {
            var media = await this.mediaRepository.AllAsNoTracking().Include(x => x.Ratings)
                            .FirstOrDefaultAsync(x => x.Id == mediaId);

            if (media == null)
            {
                throw new Exception(string.Format(GlobalExceptions.MediaDoesNotExist, mediaId));
            }

            double average = media.Ratings.Select(x => x.Score).DefaultIfEmpty().Average(x => x);
            var count = media.Ratings.Count();

            return new Tuple<double, int>(average, count);
        }

        public async Task<int> GetReviewCount(string mediaId)
        {
            var media = await this.mediaRepository.AllAsNoTracking().Include(x => x.Reviews)
                .FirstOrDefaultAsync(x => x.Id == mediaId);

            if (media == null)
            {
                throw new Exception(string.Format(GlobalExceptions.MediaDoesNotExist, mediaId));
            }

            int count = media.Reviews.Count();
            return count;
        }

        public async Task<IEnumerable<T>> GetReviews<T>(string mediaId, int page, string sortType = "")
        {
            int pagination = (page - 1) * DefaultReviewsPerPage;
            var query = this.reviewRepository.AllAsNoTracking().Include(x => x.Rating)
                .Where(x => x.Rating.MediaId == mediaId);

            switch (sortType)
            {
                case "Oldest":
                    query = query.OrderBy(x => x.CreatedOn);
                    break;
                case "Latest":
                    query = query.OrderByDescending(x => x.CreatedOn);
                    break;
                case "Rating Ascending":
                    query = query.OrderBy(x => x.Rating.Score);
                    break;
                case "Rating Descending":
                    query = query.OrderByDescending(x => x.Rating.Score);
                    break;
            }

            var reviews = await query.Skip(pagination)
                              .Take(DefaultReviewsPerPage)
                              .To<T>().ToListAsync();
            return reviews;
        }

        public async Task<int> GetRatingForMedia(string userId, string mediaId)
        {
            return await this.ratingRepository.All().Where(x => x.CreatorId == userId && x.MediaId == mediaId)
                               .Select(x => x.Score).FirstOrDefaultAsync();
        }
    }
}
