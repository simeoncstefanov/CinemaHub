namespace CinemaHub.Web.Tests
{
    using System.Collections.Generic;
    using System.Net;
    using System.Threading.Tasks;

    using AutoMapper;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using Moq;
    using Xunit;

    public class MediaServicesTests
    {
        private readonly IMediaService mediaService;

        public MediaServicesTests()
        {
        }

        [Fact]
        public void ServiceReturnsRightAmountElementsOnPagination()
        { 
        }      
    }
}
