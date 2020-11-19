namespace CinemaHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Services.Models;
    public interface IMovieAPIService
    {
        Task<MovieApiDTO> GetMediaResources(string apiId);
    }
}
