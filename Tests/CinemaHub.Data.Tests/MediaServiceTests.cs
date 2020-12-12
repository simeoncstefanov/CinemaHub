namespace CinemaHub.Data.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Web.ViewModels.Media;

    using Moq;

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
                                ReleaseDate = DateTime.Now
                            };

            movie.Genres = new List<MediaGenre>() { new MediaGenre {Genre = new Genre() { Id = 1, Name = "Action", ApiId = 1, }, Media = movie } };
            return new List<Media>()
                       {
                           movie,
                           new Show() { Title = "Borat TV Series", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now},
                           new Show() { Title = "Mr. Robot", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now},
                           new Movie() { Title = "Inception", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now},
                           new Movie() { Title = "Shrek 2077", Budget = 1000, Overview = "ijweifgjwiegjweigoweg", Runtime = 100, ReleaseDate = DateTime.Now}
                       };
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
            Assert.Equal(expectedGenre, mediaRepoMock.Object.All()
                .FirstOrDefault(x => x.Id == editMediaId).Genres
                .FirstOrDefault().Genre.Name);
        }

        [Fact]
        public void GetPageAsyncGetsProperAmountOfPagesAndResult()
        {
            var mediaList = this.GetMedia();
            var mediaRepoMock = new Mock<IRepository<Media>>();
            mediaRepoMock.Setup(x => x.AllAsNoTracking()).Returns(mediaList.AsQueryable);

            // Arrange
            var mediaService = new MediaService(
                this.mediaRepo,
                this.genreRepo,
                this.keywordRepo,
                this.mediaKeywordRepo);

            var totalResults = this.mediaRepo.AllAsNoTracking().Count();

            var expectedPages1 = 1;
            var query1 = new MediaQueryDTO() { Page = expectedPages1, ElementsPerPage = 20, };

            var expectedPages2 = 3;
            var query2 = new MediaQueryDTO() { Page = expectedPages2, ElementsPerPage = 2, };

            // Act
            MediaResultDTO results1 = new MediaResultDTO();
            MediaResultDTO results2 = new MediaResultDTO();
            Task.Run(
                async () =>
                    {
                        results1 = await mediaService.GetPageAsync(query1);
                    }).GetAwaiter().GetResult();
            Task.Run(
                async () =>
                    {
                        results2 = await mediaService.GetPageAsync(query1);
                    }).GetAwaiter().GetResult();


            // Assert
            Assert.Equal(expectedPages1, results1.Pages);
            Assert.Equal(expectedPages2, results2.Pages);
            Assert.Equal(query1.ElementsPerPage, results1.Results.Count()); // result at first page
            Assert.Equal(query2.ElementsPerPage * expectedPages2 - totalResults, results2.Results.Count()); // results at the last page of query2
        }

        [Fact]
        public void EditDetailsReplacesKeywordListFromInputModel()
        {
            // Arrange
            var mediaList = this.GetMedia();

            var media = mediaList[0];
            var keywords = this.GetKeywords();

            var mediaKeywordList = new List<MediaKeyword>();
            var mediaKeywordMock = new Mock<IRepository<MediaKeyword>>();

            mediaKeywordMock.Setup(x => x.AddAsync(It.IsAny<MediaKeyword>()))
                .Callback((MediaKeyword model) => mediaKeywordList.Add(model));

            foreach (var keyword in keywords)
            {
                mediaKeywordList.Add(new MediaKeyword() { Media = media, Keyword = keyword, });
            }

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
            };

            // Act
            Task.Run(async () =>
            {
                await mediaService.EditDetailsAsync(inputModel, string.Empty, string.Empty);
            }).GetAwaiter().GetResult();

            // Assert
            Assert.Equal(expectedCount, mediaRepoMock.Object.All().Count());
            Assert.Equal(expectedTitle, mediaRepoMock.Object.All().FirstOrDefault(x => x.Id == editMediaId).Title);
            Assert.Equal(expectedGenre, mediaRepoMock.Object.All()
                .FirstOrDefault(x => x.Id == editMediaId).Genres
                .FirstOrDefault().Genre.Name);
        }
    }
}
