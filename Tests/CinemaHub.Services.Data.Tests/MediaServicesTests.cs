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
    using MockQueryable.Core;
    using MockQueryable.Moq;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public class MediaServiceTests
    {
        private IRepository<Media> mediaRepo;
        private IRepository<Genre> genreRepo;
        private IRepository<Keyword> keywordRepo;
        private IRepository<MediaKeyword> mediaKeywordRepo;

        public List<Genre> GetGenres()
        {
            return new List<Genre>()
                       {
                           new Genre() { Id = 1, Name = "Action", ApiId = 1, },
                           new Genre() { Id = 2, Name = "Adventure", ApiId = 2, },
                           new Genre() { Id = 3, Name = "Crime", ApiId = 3, },
                       };
        }

        public List<Keyword> GetKeywords()
        {
            return new List<Keyword>()
                       {
                           new Keyword() { Id = 1, Name = "hackers"},
                           new Keyword() { Id = 2, Name = "anime"},
                           new Keyword() { Id = 3, Name = "leonardo dicaprio"},
                       };
        }

        public List<Media> GetMedia()
        {
            var movie = new Movie()
            {
                Title = "Borat",
                Budget = 1000,
                Overview = "ijweifgjwiegjweigoweg",
                Runtime = 100,
                ReleaseDate = DateTime.Now,
            };

            movie.Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } };
            movie.Ratings = new List<Rating>() { new Rating() { Score = 10 } };

            var mediaList = new List<Media>()
                       {
                           movie,
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
                           Ratings = new List<Rating>() { new Rating() { Score = 10 } }, },
                       };

            foreach (var media in mediaList)
            {
                media.Genres = new List<MediaGenre>() { new MediaGenre { Genre = new Genre() { Id = 1, Name = "Action", ApiId = 1, }, Media = media } };
                media.Keywords = new List<MediaKeyword>();
            }

            movie.Genres = new List<MediaGenre>() { new MediaGenre { Genre = new Genre() { Id = 1, Name = "Adventure", ApiId = 1, }, Media = movie } };

            return mediaList;
        }

        public MediaServiceTests()
        {
            var mediaRepoMock = new Mock<IRepository<Media>>();
            var genreRepoMock = new Mock<IRepository<Genre>>();
            var keywordRepoMock = new Mock<IRepository<Keyword>>();
            var mediaKeyword = new Mock<IRepository<MediaKeyword>>();

            genreRepoMock.Setup(x => x.All()).Returns(this.GetGenres().AsQueryable());
            genreRepoMock.Setup(x => x.AllAsNoTracking()).Returns(this.GetGenres().AsQueryable);

            mediaRepoMock.Setup(x => x.All()).Returns(this.GetMedia().AsQueryable());
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(this.GetMedia().AsQueryable);

            keywordRepoMock.Setup(x => x.All()).Returns(this.GetKeywords().AsQueryable());
            keywordRepoMock.Setup(x => x.AllAsNoTracking()).Returns(this.GetKeywords().AsQueryable());

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CinemaHub.Services.Data.Tests"));

            this.mediaRepo = mediaRepoMock.Object;
            this.genreRepo = genreRepoMock.Object;
            this.keywordRepo = keywordRepoMock.Object;
            this.mediaKeywordRepo = mediaKeyword.Object;
        }

        [Fact]
        public void EditDetailsAddsModelIfIdDoesNotExists()
        {
            // Arrange
            var mediaList = this.GetMedia();
            var expectedCount = mediaList.Count + 1;
            var mediaRepoMock = new Mock<IRepository<Media>>();
            mediaRepoMock.Setup(x => x.All())
                .Returns(mediaList.AsQueryable());
            mediaRepoMock.Setup(x => x.AddAsync(It.IsAny<Media>()))
                .Callback((Media model) => mediaList.Add(model));


            var mediaService = new MediaService(mediaRepoMock.Object, this.genreRepo, this.keywordRepo, this.mediaKeywordRepo);

            var inputModel = new MediaDetailsInputModel()
            {
                Title = "Film",
                Overview = "dadadafewfwfewfwefwefwefewfwefwefwefewfwefwefweffew",
                ReleaseDate = DateTime.Now,
                Runtime = 100,
                Budget = 100,
                MediaType = "Movie",
                Genres = "Action, Adventure",
            };

            // Act
            Task.Run(async () =>
            {
                await mediaService.EditDetailsAsync(inputModel, string.Empty, string.Empty);
            }).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expectedCount, mediaRepoMock.Object.All().Count());
            Assert.True(mediaRepoMock.Object.All().Any(x => x.Title == inputModel.Title));
        }

        [Fact]
        public void EditDetailsEditsModelIfIdDoesExist()
        {
            // Arrange
            var mediaList = this.GetMedia();
            var mediaRepoMock = new Mock<IRepository<Media>>();

            mediaRepoMock.Setup(x => x.All())
                .Returns(mediaList.AsQueryable);
            mediaRepoMock.Setup(x => x.AllAsNoTracking())
                .Returns(mediaList.AsQueryable);

            var title = "Borat";
            var expectedTitle = title + "Film";
            var expectedGenre = "Adventure";
            var editMediaId = mediaList.FirstOrDefault(x => x.Title == title).Id;
            var expectedCount = mediaList.Count;

            var mediaService = new MediaService(mediaRepoMock.Object, this.genreRepo, this.keywordRepo, this.mediaKeywordRepo);

            var inputModel = new MediaDetailsInputModel()
            {
                Id = editMediaId,
                Title = expectedTitle,
                Overview = "dadadafewfwfewfwefwefwefewfwefwefwefewfwefwefweffew",
                ReleaseDate = DateTime.Now,
                Runtime = 100,
                Budget = 100,
                MediaType = "Movie",
                Genres = expectedGenre,
            };

            // Act
            Task.Run(async () =>
            {
                await mediaService.EditDetailsAsync(inputModel, string.Empty, string.Empty);
            }).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expectedCount, mediaRepoMock.Object.All().Count());
            Assert.Equal(expectedTitle, mediaRepoMock.Object.All().FirstOrDefault(x => x.Id == editMediaId).Title);
            Assert.Equal(expectedGenre, mediaRepoMock.Object.All().FirstOrDefault(x => x.Id == editMediaId).Genres.FirstOrDefault().Genre.Name);
        }

        [Fact]
        public async Task GetPageAsyncGetsProperAmountOfPagesAndResult()
        {
            // Arrange
            var mediaRepoMock = new Mock<IRepository<Media>>();

            var mock = this.GetMedia().AsQueryable().BuildMock();
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);

            var mediaService = new MediaService(
                mediaRepoMock.Object,
                this.genreRepo,
                this.keywordRepo,
                this.mediaKeywordRepo);

            var expectedPages1 = 1;
            var query1 = new MediaQueryDTO() { Page = expectedPages1, ElementsPerPage = 20, };

            var expectedPages2 = 3;
            var query2 = new MediaQueryDTO() { Page = expectedPages2, ElementsPerPage = 2, };
            // Act
            var totalResults = this.mediaRepo.AllAsNoTracking().Count();

            var results1 = await mediaService.GetPageAsync(query1);
            var results2 = await mediaService.GetPageAsync(query2);

            // Assert
            Assert.Equal(expectedPages1, results1.Pages);
            Assert.Equal(expectedPages2, results2.Pages);
            Assert.Equal((query2.ElementsPerPage * results2.Pages) - totalResults, results2.Results.Count()); // results at the last page of query2
        }

        [Fact]
        public void EditDetailsReplacesKeywordListFromInputModel()
        {
            // Arrange
            var mediaList = this.GetMedia();

            var media = mediaList[0];
            var keywords = this.GetKeywords();
            var mediaKeywordsList = new List<MediaKeyword>();

            foreach (var keyword in keywords)
            {
                mediaKeywordsList.Add(new MediaKeyword() { Media = media, Keyword = keyword, KeywordId = keyword.Id });
            }

            media.Keywords = mediaKeywordsList;

            var mediaMock = new Mock<IRepository<Media>>();
            var keywordsMock = new Mock<IRepository<Keyword>>();
            var mediaKeywordsMock = new Mock<IRepository<MediaKeyword>>();

            mediaMock.Setup(x => x.All()).Returns(mediaList.AsQueryable);
            mediaMock.Setup(x => x.AddAsync(It.IsAny<Media>()))
                .Callback((Media model) => mediaList.Add(model));
            keywordsMock.Setup(x => x.AddAsync(It.IsAny<Keyword>()))
                .Callback((Keyword model) => keywords.Add(model));
            mediaKeywordsMock.Setup(x => x.Delete(It.IsAny<MediaKeyword>()))
                .Callback((MediaKeyword model) => mediaKeywordsList.Remove(model));


            var title = "Borat";
            var expectedTitle = title + "Film";
            var editMediaId = mediaList.FirstOrDefault(x => x.Title == title).Id;
            var expectedCount = mediaList.Count;
            var expectedGenre = "Adventure";

            var mediaService = new MediaService(mediaMock.Object, this.genreRepo, keywordsMock.Object, mediaKeywordsMock.Object);

            var keywordInput = this.GetKeywords().Select(x => new KeywordViewModel() { Value = x.Name, Id = x.Id }).ToList();

            var newKeyword = new KeywordViewModel() { Value = "new keyword" };
            keywordInput.Add(newKeyword);
            var expectedToBeRemoved = mediaKeywordsList[0];
            keywordInput.RemoveAt(0);

            var serializedKeywords = JsonConvert.SerializeObject(keywordInput);

            var inputModel = new MediaDetailsInputModel()
            {
                Id = editMediaId,
                Title = expectedTitle,
                Budget = 1000,
                Overview = "ijweifgjwiegjweigoweg",
                Runtime = 100,
                ReleaseDate = DateTime.Now,
                MediaType = "Movie",
                Keywords = serializedKeywords,
                Genres = expectedGenre,
            };

            // Act
            Task.Run(async () =>
            {
                await mediaService.EditDetailsAsync(inputModel, string.Empty, string.Empty);
            }).GetAwaiter().GetResult();

            // Assert
            Assert.False(media.Keywords.Contains(expectedToBeRemoved));
            Assert.Contains(media.Keywords, x => x.Keyword.Name == newKeyword.Value);
        }

        [Fact]
        public async Task SearchQueryFindsCorrectMedia()
        {
            var mediaRepoMock = new Mock<IRepository<Media>>();

            var mediaList = this.GetMedia();

            var keyword = new Keyword() { Name = "keyword", Id = 5 };
            var mediaKeyword = new MediaKeyword() { KeywordId = 5, Keyword = keyword, Media = mediaList.FirstOrDefault() };
            var show = new Show()
            {
                Title = "Mr. Robot",
                Budget = 1000,
                Overview = "ijweifgjwiegjweigoweg",
                Runtime = 100,
                ReleaseDate = DateTime.Now,
                Images = new List<MediaImage>() { new MediaImage { Path = "", ImageType = ImageType.Poster } },
                Ratings = new List<Rating>() { new Rating() { Score = 10 } },
                Keywords = new List<MediaKeyword>() { mediaKeyword },
                Genres = new List<MediaGenre>() { new MediaGenre { Genre = new Genre() { Id = 1, Name = "Adventure", ApiId = 1, } } },
            };

            mediaList.Add(show);

            var mock = mediaList.AsQueryable().BuildMock();
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            mediaRepoMock.Setup(x => x.All()).Returns(mock.Object);

            var mediaService = new MediaService(
                mediaRepoMock.Object,
                this.genreRepo,
                this.keywordRepo,
                this.mediaKeywordRepo);

            var query1 = new MediaQueryDTO() { Page = 1, ElementsPerPage = 20, SearchQuery = "Mr. R" };
            var query2 = new MediaQueryDTO() { Page = 1, ElementsPerPage = 2, Genres = "Adventure" };
            var query3 = new MediaQueryDTO() { Page = 1, ElementsPerPage = 20, Keywords = "5" };

            // Act
            var totalResults = this.mediaRepo.AllAsNoTracking().Count();

            var results1 = await mediaService.GetPageAsync(query1);
            var results2 = await mediaService.GetPageAsync(query2);
            var results3 = await mediaService.GetPageAsync(query3);

            // Assert
            Assert.Contains(results1.Results, x => x.Title == "Mr. Robot");
            Assert.Equal(1, results1.ResultCount);
            Assert.Contains(results2.Results, x => x.Title == "Borat");
            Assert.Equal(2, results2.ResultCount);
            Assert.Contains(results3.Results, x => x.Title == "Mr. Robot");
            Assert.Equal(1, results3.ResultCount);
        }

        [Fact]
        public async Task GetDetailsGetsCorrectDetails()
        {
            // Arrange
            var mediaRepoMock = new Mock<IRepository<Media>>();

            var mediaList = this.GetMedia();

            var media = mediaList[0];
            var mock = mediaList.AsQueryable().BuildMock();
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);

            var expectedMedia = mediaList.FirstOrDefault();

            var mediaService = new MediaService(
                mediaRepoMock.Object,
                this.genreRepo,
                this.keywordRepo,
                this.mediaKeywordRepo
                );

            var expectedKeywords = media.Keywords.Select(x => new
            {
                Value = x.Keyword.Name,
                Id = x.KeywordId,
            });

            // Act
            var viewModel = await mediaService.GetDetailsAsync<MediaDetailsViewModel>(media.Id);

            // Arrange
            Assert.Equal(media.Title, viewModel.Title);
            Assert.Equal(media.Overview, viewModel.Overview);
            Assert.Equal(JsonConvert.SerializeObject(expectedKeywords), JsonConvert.SerializeObject(viewModel.Keywords));

        }
    }
}
