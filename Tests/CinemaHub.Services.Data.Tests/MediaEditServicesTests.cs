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
    using CinemaHub.Data.Common.Models;
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

    public class MediaEditServicesTests
    {
        public MediaEditServicesTests()
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CinemaHub.Services.Data.Tests"));
        }

        [Fact]
        public async Task ApplyEditForApprovalCreatesNewMediaEdit()
        {
            // Arrange
            var inputModel = new MediaDetailsInputModel()
            {
                Title = "Daa",
                Overview = "fiwejfoiwejfoiwj",
                Language = "en",
                Id = "1",
                ReleaseDate = DateTime.Now,
                Runtime = 100,
                Budget = 1000,
                YoutubeTrailerUrl = "www.youtube.com",
                Keywords = "keywords",
                Genres = "Adventure, Action",
                MediaType = "Movie",
                PosterPath = "/yes.jpg",
            };

            var mock = this.GetMock<MediaEdit>(new List<MediaEdit>());
            var expectedCount = 1;
            var service = new MediaEditService(mock.Object);

            // Act
            await service.ApplyEditForApproval(inputModel, "1", "");

            var newEdit = mock.Object.All().FirstOrDefault();
            // Assert
            Assert.Equal(expectedCount, mock.Object.All().Count());
            Assert.Equal(inputModel.Title, newEdit.Title);
            Assert.Equal(inputModel.Runtime, newEdit.Runtime);
            Assert.Equal(inputModel.Keywords, newEdit.KeywordsJson);
            Assert.Equal(inputModel.PosterPath, newEdit.PosterPath);
        }

        [Fact]
        public async Task GetEditForApprovalGetsProperEdit()
        {
            // Arrange
            var list = this.GetMediaEdits();
            var expectedEdit = list.FirstOrDefault();
            var mock = this.GetMock<MediaEdit>(list);

            var service = new MediaEditService(mock.Object);

            // Act
            var result = await service.GetEditForApproval<MediaDetailsInputModel>(expectedEdit.Id);

            // Assert
            Assert.Equal(expectedEdit.Title, result.Title);
            Assert.Equal(expectedEdit.KeywordsJson, result.Keywords);
            Assert.Equal(expectedEdit.PosterPath, result.PosterPath);
            Assert.Equal(expectedEdit.MediaType, result.MediaType);
            Assert.Equal(expectedEdit.Genres, result.Genres);
        }

        [Fact]
        public async Task GetEditForApprovalThrowsExceptionIfItDoesNotExist()
        {
            // Arrange
            var list = this.GetMediaEdits();
            var mock = this.GetMock<MediaEdit>(list);

            var service = new MediaEditService(mock.Object);

            // Act / Assert
            await Assert.ThrowsAsync<Exception>(() => service.GetEditForApproval<MediaDetailsInputModel>("weghuweuoighowehg"));
        }

        [Fact]
        public async Task GetEditsForApprovalGetsProperEdits()
        {
            // Arrange
            var list = this.GetMediaEdits();
            var expectedCount = list.Count;
            var mock = this.GetMock<MediaEdit>(list);

            var service = new MediaEditService(mock.Object);

            // Act
            var result = await service.GetEditsForApproval<MediaDetailsInputModel>(1, 10);

            // Assert
            var listTitles = list.Select(x => x.Title).ToList();
            var resultTitle = result.Select(x => x.Title).ToList();
            Assert.Equal(expectedCount, result.Count());
            Assert.True(listTitles.SequenceEqual(resultTitle));
        }

        [Fact]
        public async Task GetEditsForApprovalGivesCorrectCount()
        {
            // Arrange
            var list = this.GetMediaEdits();
            var expectedCount = list.Count;
            var mock = this.GetMock<MediaEdit>(list);

            var service = new MediaEditService(mock.Object);

            // Act
            var result = await service.GetEditsForApprovalCount();

            // Assert
            Assert.Equal(expectedCount, result);
        }

        [Fact]
        public async Task GetAndApproveEditApprovesThenDeletesEditAndReturnsProperModel()
        {
            // Arrange
            var list = this.GetMediaEdits();
            var expectedEdit = list.LastOrDefault();
            var mock = this.GetDeletableMock(list);

            var service = new MediaEditService(mock.Object);

            // Act
            var result = await service.GetAndApproveEdit<MediaDetailsInputModel>(expectedEdit.Id);

            // Assert
            Assert.True(expectedEdit.IsDeleted);
            Assert.True(expectedEdit.IsApproved);
            Assert.Equal(expectedEdit.Title, result.Title);
            Assert.Equal(expectedEdit.PosterPath, result.PosterPath);
            Assert.Equal(expectedEdit.Overview, result.Overview);
        }

        [Fact]
        public async Task RejectEditsDeletesEdit()
        {
            // Arrange
            var list = this.GetMediaEdits();
            var expectedEdit = list.LastOrDefault();
            var mock = this.GetDeletableMock(list);

            var service = new MediaEditService(mock.Object);

            // Act
            await service.RejectEdit(expectedEdit.Id);

            // Assert
            Assert.True(expectedEdit.IsDeleted);
        }

        private Mock<IDeletableEntityRepository<T>> GetMock<T>(List<T> entityList)
           where T : class, IDeletableEntity
        {
            var repoMock = new Mock<IDeletableEntityRepository<T>>();
            var mock = entityList.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<T>())).Callback((T entity) => entityList.Add(entity));
            repoMock.Setup(x => x.Delete(It.IsAny<T>())).Callback((T entity) => entityList.Remove(entity));

            return repoMock;
        }

        private Mock<IDeletableEntityRepository<MediaEdit>> GetDeletableMock(List<MediaEdit> entityList)
        {
            var repoMock = new Mock<IDeletableEntityRepository<MediaEdit>>();
            var mock = entityList.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<MediaEdit>())).Callback((MediaEdit entity) => entityList.Add(entity));
            repoMock.Setup(x => x.Delete(It.IsAny<MediaEdit>())).Callback((MediaEdit entity) => entity.IsDeleted = true);

            return repoMock;
        }

        private List<MediaEdit> GetMediaEdits()
        {
            var mediaEdits = new List<MediaEdit>()
            {
                new MediaEdit()
                {
                Title = "Daa",
                Overview = "fiwejfoiwejfoiwj",
                Language = "en",
                MediaId = "1",
                ReleaseDate = DateTime.Now,
                Runtime = 100,
                Budget = 1000,
                YoutubeTrailerUrl = "www.youtube.com",
                KeywordsJson = "keywords",
                Genres = "Adventure, Action",
                MediaType = "Movie",
                PosterPath = "/yes.jpg",
                CreatorId = "1",
                },
                new MediaEdit()
                {
                Title = "Daa",
                Overview = "fiwejfoiwejfoiwj",
                Language = "en",
                MediaId = "1",
                ReleaseDate = DateTime.Now,
                Runtime = 100,
                Budget = 1000,
                YoutubeTrailerUrl = "www.youtube.com",
                KeywordsJson = "keywords",
                Genres = "Adventure, Action",
                MediaType = "Movie",
                PosterPath = "/yes.jpg",
                CreatorId = "2",
                },
                new MediaEdit()
                {
                Title = "Test",
                Overview = "Tetsetestsets",
                Language = "en",
                MediaId = "2",
                ReleaseDate = DateTime.Now,
                Runtime = 100,
                Budget = 1000,
                YoutubeTrailerUrl = "www.twitter.com",
                KeywordsJson = "keywords2",
                Genres = "Adventure, Action, Adventure",
                MediaType = "Show",
                PosterPath = "/no.png",
                CreatorId = "1",
                },
            };

            return mediaEdits;
        }
    }
}
