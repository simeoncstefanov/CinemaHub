namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;

    using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<IEnumerable<MediaGridDTO>> Get(string query)
        {
            var media = await this.mediaService.GetPageAsync(new MediaQueryDTO() { SearchQuery = query, Page = 1, ElementsPerPage = 10 });
            return media.Results;
        }

        [IgnoreAntiforgeryToken]
        [HttpPost]
        [AllowAnonymous]
        public async Task<MediaResultDTO> Query([FromBody] MediaQueryDTO query)
        {
            var user = this.User.FindFirst(ClaimTypes.NameIdentifier);
            if (user != null)
            {
                query.UserId = user.Value;
            }

            var result = await this.mediaService.GetPageAsync(query);
            return result;
        }
    }
}
