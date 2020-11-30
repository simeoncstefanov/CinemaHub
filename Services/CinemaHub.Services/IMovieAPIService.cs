namespace CinemaHub.Services
{
    using System.Threading.Tasks;

    using CinemaHub.Services.Models;
    using CinemaHub.Web.ViewModels.Media;

    public interface IMovieAPIService
    {
        Task<MediaDetailsInputModel> GetDetailsFromApiAsync(string id, string mediaType, string title);
    }
}
