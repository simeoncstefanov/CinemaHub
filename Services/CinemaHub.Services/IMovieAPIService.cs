namespace CinemaHub.Services
{
    using System.Threading.Tasks;

    using CinemaHub.Services.Models;

    public interface IMovieAPIService
    {
        Task InsertMovieDetails(string title);

        Task InsertShowDetails(string title);
    }
}
