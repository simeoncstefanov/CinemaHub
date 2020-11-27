namespace CinemaHub.Services.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CinemaHub.Services.Data.Models;

    public interface IKeywordService
    {
        Task<IEnumerable<KeywordDTO>> GetAllAsync(string query, int results);
    }
}
