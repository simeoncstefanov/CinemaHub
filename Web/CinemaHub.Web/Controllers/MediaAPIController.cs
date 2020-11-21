namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/media/")]
    [ApiController]
    public class MediaAPIController : ControllerBase
    {
        private readonly IMediaService mediaService;

        public MediaAPIController(IMediaService mediaService)
        {
            this.mediaService = mediaService;
        }

        [HttpGet]
        public async Task<IEnumerable<MediaGridDTO>> Get(string query)
        {
            this.mediaService.SearchMedia(query, MediaEnum.All);
            var media = await this.mediaService.GetPageElementsAsync(1, 10);
            return media;
        }
    }
}
