namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Services;
    using CinemaHub.Services.Data;

    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class TestController : Controller
    {
        private readonly IMovieAPIService apiService;

        private readonly IMediaService mediaService;

        private readonly IWebHostEnvironment environment;

        public TestController(IMovieAPIService apiService, IMediaService mediaService, IWebHostEnvironment environment)
        {
            this.environment = environment;
            this.apiService = apiService;
            this.mediaService = mediaService;
        }

        public async Task<IActionResult> Index(string apiId)
        {
            return this.View("Error");
        }
    }
}
