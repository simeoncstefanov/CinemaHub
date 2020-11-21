namespace CinemaHub.Services
{
    using System.Threading.Tasks;

    using CinemaHub.Services.Models;

    public interface IMovieAPIService
    {
        Task<MovieApiDTO> GetMediaResources(string apiId);
    }
}
