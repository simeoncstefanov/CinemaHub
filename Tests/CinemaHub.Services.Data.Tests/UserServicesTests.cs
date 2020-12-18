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

    public class UserServicesTests
    {
        private readonly IRepository<ApplicationUser> userRepo;

        public UserServicesTests()
        {
            this.userRepo = new Mock<IRepository<ApplicationUser>>().Object;
        }

        [Fact]
        public async Task AddToUserWatchlistCreatesWatcherIfItDoesNotExist()
        {
            var mockRepo = this.GetMock<MediaWatcher>(new List<MediaWatcher>());
            var expectedWatchlistCount = 1;
        }

        private Mock<IRepository<T>> GetMock<T>(List<T> entityList)
            where T : class
        {
            var repoMock = new Mock<IRepository<T>>();
            var mock = entityList.AsQueryable().BuildMock();
            repoMock.Setup(x => x.AllAsNoTracking()).Returns(mock.Object);
            repoMock.Setup(x => x.All()).Returns(mock.Object);
            repoMock.Setup(x => x.AddAsync(It.IsAny<T>())).Callback((T entity) => entityList.Add(entity));

            return repoMock;
        }

        public List<MediaWatcher> GetWatch()
        {
            var watchers = new List<MediaWatcher>();
            return watchers;
        }
    }
}
