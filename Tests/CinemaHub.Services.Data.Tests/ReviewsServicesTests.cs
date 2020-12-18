namespace CinemaHub.Services.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.Reviews;
    using MockQueryable.Core;
    using MockQueryable.Moq;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public class ReviewsServicesTests
    {
        private readonly IRepository<Media> mediaRepo;

        public ReviewsServicesTests()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CinemaHub.Services.Data.Tests"));

            var medias = this.GetMedia();
            this.mediaRepo = this.GetMediaMock(medias).Object;
        }

        [Fact]
        public async Task GetReviewsGetsCorrectData()
        {
            // Arange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);
            var expectedMediaId = "2";
            var expectedReviews = reviewList.Where(x => x.MediaId == expectedMediaId).ToList();
            var expectedIds = expectedReviews.Select(x => x.Rating.Id).ToList();

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, this.mediaRepo);

            // Act
            var reviews = await reviewService.GetReviews<ReviewViewModel>(expectedMediaId, 1);

            var reviewsIds = reviews.Select(x => x.RatingId).ToList();

            // Assert
            Assert.All(reviewsIds, x => expectedIds.Contains(x));
        }

        [Fact]
        public async Task GetRatingAverageCountGetsCorrectData()
        {   
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var medias = this.GetMedia();
            var media = medias.LastOrDefault();
            var mediaMock = this.GetMediaMock(medias);
            var expectedAverage = media.Ratings.Average(x => x.Score);
            var expectedCount = media.Ratings.Count();

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, mediaMock.Object);

            // Act
            var result = await reviewService.GetRatingAverageCount(media.Id);

            // Assert
            Assert.Equal(expectedAverage, result.Item1);
            Assert.Equal(expectedCount, result.Item2);
        }

        [Fact]
        public async Task CreateReviewsWorksCorrectly()
        {
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var expectedReviewCount = reviewList.Count + 1;

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, this.mediaRepo);

            var inputModel = new ReviewInputModel()
            {
                CurrentUserRating = 0,
                MediaId = "1",
                ReviewText = "Da",
                Title = "Da",
            };

            // Act
            await reviewService.CreateReview("2", inputModel);

            // Assert
            Assert.Equal(expectedReviewCount, reviewMock.Object.All().Count());
            Assert.Contains(reviewMock.Object.All(), x => x.Title == inputModel.Title && x.ReviewText == inputModel.ReviewText);
        }

        [Fact]
        public async Task CreatingReviewWithoutRatingThrowsError()
        {
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, this.mediaRepo);

            var inputModel = new ReviewInputModel()
            {
                CurrentUserRating = 0,
                MediaId = "1",
                ReviewText = "Da",
                Title = "Da",
            };

            // Act
            Func<Task> act = () => reviewService.CreateReview("1", inputModel);

            // Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(act);
            Assert.Contains("You need to rate the media to submit a review.", ex.Message);
        }

        [Fact]
        public async Task CreatingSecondReviewThrowsError()
        {
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, this.mediaRepo);

            var inputModel = new ReviewInputModel()
            {
                CurrentUserRating = 0,
                MediaId = "2",
                ReviewText = "Da",
                Title = "Da",
            };

            // Act
            Func<Task> act = () => reviewService.CreateReview("2", inputModel);

            // Assert
            Exception ex = await Assert.ThrowsAsync<Exception>(act);
            Assert.Contains("You have already created a review. Delete it to create new one.", ex.Message);
        }

        [Fact]
        public async Task CreateRatingAddsNewIfItDoesNotExist()
        {
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var expectedRatingCount = ratingList.Count() + 1;

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, this.mediaRepo);

            var inputModel = new RatingInputModel()
            {
                MediaId = "4",
                Value = 5,
            };

            // Act
            await reviewService.CreateRating(inputModel.MediaId, "2", inputModel.Value);

            // Assert
            Assert.Equal(expectedRatingCount, ratingMock.Object.All().Count());
            Assert.Contains(ratingMock.Object.All(), x => x.MediaId == inputModel.MediaId && x.Score == inputModel.Value);
        }

        [Fact]
        public async Task CreateRatingChangesRatingsIfItAlreadyExists()
        {
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var creatorId = "2";
            var expectedRatingCount = ratingList.Count();

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, this.mediaRepo);

            var inputModel = new RatingInputModel()
            {
                MediaId = "3",
                Value = 5,
            };

            // Act
            await reviewService.CreateRating(inputModel.MediaId, creatorId, inputModel.Value);

            var newValue = ratingMock.Object.All().Where(x => x.MediaId == inputModel.MediaId && x.CreatorId == creatorId).FirstOrDefault().Score;
            // Assert
            Assert.Equal(expectedRatingCount, ratingMock.Object.All().Count());
            Assert.Equal(inputModel.Value, newValue);
        }

        [Fact]
        public async Task GetReviewCountGetsCorrectCount()
        {
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var medias = this.GetMedia();
            var media = medias.FirstOrDefault();
            var mediaReviews = media.Reviews;
            var mediaMock = this.GetMediaMock(medias);
            var expectedCount = mediaReviews.Count();

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, mediaMock.Object);

            // Act
            var count = await reviewService.GetReviewCount(media.Id);

            // Assert
            Assert.Equal(expectedCount, count);
        }

        [Fact]
        public async Task GetRatingForMediaGetsCorrectRating()
        {
            // Arrange
            var ratingList = this.GetRatings();
            var reviewList = this.GetReviews();
            var ratingMock = this.GetRatingMock(ratingList);
            var reviewMock = this.GetReviewMock(reviewList);

            var medias = this.GetMedia();
            var media = medias.LastOrDefault();
            var mediaMock = this.GetMediaMock(medias);
            var expectedAverage = media.Ratings.Average(x => x.Score);
            var expectedCount = media.Ratings.Count();

            var reviewService = new ReviewsService(reviewMock.Object, ratingMock.Object, mediaMock.Object);

            // Act
            var result = await reviewService.GetRatingAverageCount(media.Id);

            // Assert
            Assert.Equal(expectedAverage, result.Item1);
            Assert.Equal(expectedCount, result.Item2);
        }

        private Mock<IRepository<Review>> GetReviewMock(List<Review> reviews)
        {
            var repoMock = new Mock<IRepository<Review>>();
            var mock = reviews.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<Review>())).Callback((Review review) => reviews.Add(review));

            return repoMock;
        }

        private Mock<IRepository<Rating>> GetRatingMock(List<Rating> ratings)
        {
            var repoMock = new Mock<IRepository<Rating>>();
            var mock = ratings.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<Rating>())).Callback((Rating x) => ratings.Add(x));

            return repoMock;
        }

        private Mock<IRepository<Media>> GetMediaMock(List<Media> medias)
        {
            var repoMock = new Mock<IRepository<Media>>();
            var mock = medias.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<Media>())).Callback((Media comment) => medias.Add(comment));

            return repoMock;
        }

        public List<Media> GetMedia()
        {
            var mediaList = new List<Media>()
                       {
                           new Show() { Title = "24", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                           new Movie() { Title = "Inception", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                           new Show() { Title = "Borat TV Series", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                           new Movie() { Title = "Tenet", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now,
                           Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                           Ratings = new List<Rating>() { new Rating() { Score = 10 }, new Rating() { Score = 1 }, new Rating() { Score = 4 }},  },
                       };

            foreach (var media in mediaList)
            {
                media.Genres = new List<MediaGenre>() { new MediaGenre { Genre = new Genre() { Id = 1, Name = "Action", ApiId = 1, }, Media = media } };
                media.Keywords = new List<MediaKeyword>();
                media.Reviews = new List<Review>() { new Review(), new Review(), new Review() };
            }

            return mediaList;
        }

        public List<Rating> GetRatings()
        {
            var ratings = new List<Rating>()
            {
                new Rating()
                {
                    MediaId = "3",
                    Creator = new ApplicationUser(),
                    CreatorId = "2",
                    Score = 10,
                },
                new Rating()
                {
                    MediaId = "1",
                    Creator = new ApplicationUser(),
                    Score = 8,
                    CreatorId = "2"
                },
                new Rating()
                {
                    MediaId = "2",
                    Creator = new ApplicationUser(),
                    Score = 1,
                    CreatorId = "1",
                },
                new Rating()
                {
                    MediaId = "2",
                    Creator = new ApplicationUser(),
                    Score = 3,
                    CreatorId = "2",
                    Review = new Review(),
                },
            };

            return ratings;
        }

        public List<Review> GetReviews()
        {
            var reviews = new List<Review>()
            {
                new Review()
                {
                    MediaId = "1",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    Title = "That's pretty good",
                    ReviewText = "I liked it a lot man would watch again",
                    Rating = new Rating()
                    {
                        Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                        Score = 10,
                        MediaId = "1",
                    },
                    CreatedOn = DateTime.Now,
                },
                new Review()
                {
                    MediaId = "1",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    Title = "Mediocre",
                    ReviewText = "I didn't like it a lot man wouldn't watch again",
                    Rating = new Rating()
                    {
                        Score = 8,
                        MediaId = "1",
                        Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    },
                    CreatedOn = DateTime.Now,
                },
                new Review()
                {
                    MediaId = "2",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    Title = "That's pretty good",
                    ReviewText = "I liked it a lot man would watch again",
                    Rating = new Rating()
                    {
                        Score = 10,
                        MediaId = "2",
                        Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    },
                    CreatedOn = DateTime.Now,
        },
                new Review()
                {
                    MediaId = "2",
                    Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    Title = "That's pretty good",
                    ReviewText = "I liked it a lot man would watch again",
                    Rating = new Rating()
                    {
                        Score = 4,
                        MediaId = "2",
                        Creator = new ApplicationUser()
                    {
                        UserName = "cryptosbg",
                        AvatarImage = new AvatarImage()
                        {
                            Path = "",
                        },
                    },
                    },
                    CreatedOn = DateTime.Now,
        },
            };

            return reviews;
        }
    }
}
