namespace CinemaHub.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using CinemaHub.Services.Models;
    using CinemaHub.Web.ViewModels.Media;

    public interface IMovieAPIService
    {
        Task<MediaDetailsInputModel> GetDetailsFromApiAsync(int apiId, string mediaType);

        Task<int> GetIdFromTitle(string title, string mediaType);

        Task<IEnumerable<int>> GetMoviesIds(int page);

        Task<IEnumerable<int>> GetShowsIds(int page);
    }
}
