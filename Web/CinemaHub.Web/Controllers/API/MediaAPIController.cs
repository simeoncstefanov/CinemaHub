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
            await this.mediaService.ApplyQueryAsync(new MediaQueryDTO() { SearchQuery = query });
            var media = await this.mediaService.GetPageAsync(1, 10);
            return media.Results;
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        public async Task<MediaResultDTO> Query([FromBody] MediaQueryDTO query)
        {
            await this.mediaService.ApplyQueryAsync(query);
            var result = await this.mediaService.GetPageAsync(query.Page, query.ElementsPerPage);
            return result;
        }
    }
}
