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
    using CinemaHub.Common;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.Watchlist;
    using MockQueryable.Core;
    using MockQueryable.Moq;
    using Moq;
    using Newtonsoft.Json;
    using Xunit;

    public class UserServicesTests
    {
        private readonly IRepository<ApplicationUser> userRepo;

        public UserServicesTests()
        {
            this.userRepo = new Mock<IRepository<ApplicationUser>>().Object;

            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(Assembly.Load("CinemaHub.Services.Data.Tests"));
        }

        [Fact]
        public async Task AddToUserWatchlistCreatesWatcherIfItDoesNotExist()
        {
            // Arrange
            var watcherMockRepo = this.GetMock<MediaWatcher>(new List<MediaWatcher>());
            var avatarMockRepo = new Mock<IRepository<AvatarImage>>();
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();
            var expectedWatchlistCount = 1;

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            await service.AddToUserWatchlistAsync("1", "1", WatchType.Completed);

            // Assert
            Assert.Equal(expectedWatchlistCount, watcherMockRepo.Object.All().Count());
        }

        [Fact]
        public async Task AddToUserWatchlistJustChangesWatchtypeIfItExists()
        {
            // Arrange
            var watcherMockRepo = this.GetMock<MediaWatcher>(this.GetWatch());
            var avatarMockRepo = new Mock<IRepository<AvatarImage>>();
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();
;
            var expectedWatchtype = WatchType.Completed;

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            await service.AddToUserWatchlistAsync("1", "1", WatchType.Completed);

            var result = watcherMockRepo.Object.AllAsNoTracking().FirstOrDefault(x => x.MediaId == "1" && x.UserId == "1");

            // Assert
            Assert.Equal(expectedWatchtype, result.WatchType);
        }

        [Fact]
        public async Task DeleteWatchlistAsyncDeletesIfExists()
        {
            var watcherMockRepo = this.GetMock<MediaWatcher>(this.GetWatch());
            var avatarMockRepo = new Mock<IRepository<AvatarImage>>();
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();

            var expectedCount = watcherMockRepo.Object.All().Count() - 1;

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            await service.DeleteWatchlistAsync("1", "1");

            var result = watcherMockRepo.Object.AllAsNoTracking();

            // Assert
            Assert.Equal(expectedCount, result.Count());
            Assert.DoesNotContain(result, x => x.MediaId == "1" && x.UserId == "1");
        }

        [Fact]
        public async Task GetAvatarPathGetProperPath()
        {
            // Arrange
            var watcherMockRepo = new Mock<IRepository<MediaWatcher>>();
            var avatarMockRepo = this.GetMock<AvatarImage>(this.GetAvatars());
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();

            var expectedPath = avatarMockRepo.Object.All().FirstOrDefault(x => x.UserId == "1").Path;

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            var path = await service.GetAvatarPath("1");

            // Assert
            Assert.Equal(expectedPath, path);
        }

        [Fact]
        public async Task GetAvatarPathGivesDefaultPathIfItDoesNotExist()
        {
            var watcherMockRepo = new Mock<IRepository<MediaWatcher>>();
            var avatarMockRepo = this.GetMock<AvatarImage>(this.GetAvatars());
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();

            var expectedPath = GlobalConstants.DefaultAvatarImagePath;

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            var path = await service.GetAvatarPath("3");

            // Assert
            Assert.Equal(expectedPath, path);
        }

        [Fact]
        public async Task GetUserWatchListAsyncGetsCorrectData()
        {
            //Arrange
            var watcherMockRepo = this.GetMock<MediaWatcher>(this.GetWatch());
            var avatarMockRepo = new Mock<IRepository<AvatarImage>>();
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();

            var expectedMedias = new List<string>() { "2", "3" };

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            var result = await service.GetUserWatchListAsync<WatchlistListViewModel>("3", 1, WatchType.Completed);

            var medias = result.Select(x => x.MediaId).ToList();

            // Assert
            Assert.Equal(expectedMedias, medias);
        }

        [Fact]
        public async Task GetWatchtypeUserAsyncGetsCorrectWatchtype()
        {
            //Arrange
            var watcherMockRepo = this.GetMock<MediaWatcher>(this.GetWatch());
            var avatarMockRepo = new Mock<IRepository<AvatarImage>>();
            var userMocKRepo = new Mock<IRepository<ApplicationUser>>();

            var expectedWatchType = watcherMockRepo.Object.AllAsNoTracking().FirstOrDefault().WatchType;
            var expectedUser = watcherMockRepo.Object.AllAsNoTracking().FirstOrDefault().UserId;
            var expectedId = watcherMockRepo.Object.AllAsNoTracking().FirstOrDefault().MediaId;

            var service = new UserService(watcherMockRepo.Object, avatarMockRepo.Object, userMocKRepo.Object);

            // Act
            var result = await service.GetWatchtypeUserAsync(expectedUser, expectedId);

            // Assert
            Assert.Equal(expectedWatchType.ToString(), result);
        }

        private Mock<IRepository<T>> GetMock<T>(List<T> entityList)
            where T : class
        {
            var repoMock = new Mock<IRepository<T>>();
            var mock = entityList.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<T>())).Callback((T entity) => entityList.Add(entity));
            repoMock.Setup(x => x.Delete(It.IsAny<T>())).Callback((T entity) => entityList.Remove(entity));

            return repoMock;
        }

        public List<MediaWatcher> GetWatch()
        {
            var watchers = new List<MediaWatcher>()
            {
                new MediaWatcher()
                {
                    Id = 1,
                    MediaId = "1",
                    UserId = "1",
                    WatchType = WatchType.OnWatchlist,
                },
                new MediaWatcher()
                {
                    Id = 2,
                    MediaId = "2",
                    UserId = "1",
                    WatchType = WatchType.Dropped,
                },
                new MediaWatcher()
                {
                    Id = 3,
                    UserId = "3",
                    MediaId = "2",
                    WatchType = WatchType.Completed,
                },
                new MediaWatcher()
                {
                    Id = 3,
                    UserId = "3",
                    MediaId = "3",
                    WatchType = WatchType.Completed,
                },
                new MediaWatcher()
                {
                    Id = 3,
                    UserId = "3",
                    MediaId = "1",
                    WatchType = WatchType.OnWatchlist,
                },
            };

            return watchers;
        }

        public List<AvatarImage> GetAvatars()
        {
            var avatars = new List<AvatarImage>()
            {
                new AvatarImage()
                {
                    Path = "path1",
                    UserId = "1",
                },
                new AvatarImage()
                {
                    Path = "path2",
                    UserId = "2",
                },
            };

            return avatars;
        }
    }
}
