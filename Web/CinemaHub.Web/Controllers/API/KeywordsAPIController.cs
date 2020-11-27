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

    [Route("api/keywords")]
    [ApiController]
    public class KeywordsAPIController : ControllerBase
    {
        private readonly IKeywordService keywordService;

        public KeywordsAPIController(IKeywordService keywordService)
        {
            this.keywordService = keywordService;
        }

        [HttpGet]
        public async Task<IEnumerable<KeywordDTO>> Get(string query, int results)
        {
            var keywords = await this.keywordService.GetAllAsync(query, results);
            return keywords;
        }
    }
}
