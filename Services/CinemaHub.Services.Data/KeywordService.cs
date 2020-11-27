namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class KeywordService : IKeywordService
    {
        private readonly IRepository<Keyword> keywordRep;

        public KeywordService(IRepository<Keyword> keywordRep, IRepository<Media> mediaRepository)
        {
            this.keywordRep = keywordRep;
        }

        public async Task<IEnumerable<KeywordDTO>> GetAllAsync(string query, int results)
        {
            var keywords = await this.keywordRep.All().Include(x => x.Medias)
                .Where(x => x.Name.Contains(query))
                .OrderBy(x => x.Medias.Count)
                .Take(results)
                .ToListAsync();

            var dto = keywords.Select(x => new KeywordDTO
            {
                Id = x.Id,
                Value = x.Name,
            });

            return dto;
        }
    }
}
