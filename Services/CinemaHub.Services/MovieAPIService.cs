namespace CinemaHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Models;

    public class MovieAPIService : IMovieAPIService
    {
        private readonly IRepository<Media> mediaRepository;
        private readonly HttpClient client;

        public MovieAPIService(IRepository<Media> mediaRepository)
        {
            this.mediaRepository = mediaRepository;
            this.client = new HttpClient();
        }

        public async Task<MovieApiDTO> GetMediaResources(string id)
        {
            var content = await this.client.GetStringAsync($"https://api.themoviedb.org/3/movie/popular?api_key=238b937765146aa0e189640d869591e7&language=en-US&page=");
            return new MovieApiDTO();
        }
    }
}
