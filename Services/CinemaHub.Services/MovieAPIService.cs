namespace CinemaHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Models;

    using Newtonsoft.Json;

    public class MovieAPIService
    {
        private readonly IRepository<Media> mediaRepository;
        private readonly HttpClient client;

        private readonly string rootPath;

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

        public async Task InsertMovieDetailsAsync(string title)
        {
            await this.InsertDetails(title, "movie");

        }

        public async Task InsertShowDetailsAsync(string title)
        {
            await this.InsertDetails(title, "tv");
        }

        // Add id if you fill details on already created Movie
        public async Task InsertDetails(string apiId, string mediaType)
        {
            var mediaString = await this.client.GetStringAsync(
                                  $"https://api.themoviedb.org/3/{mediaType}/{apiId}?api_key=238b937765146aa0e189640d869591e7&language=en-US&append_to_response=keywords");
            var mediaJson = JsonConvert.DeserializeObject<List<MovieApiDTO>>(mediaString);

            var mediaDb = new Movie(); // TODO MAPping
        }

        private async Task<string> DownloadImageAsync(string url, string movieGuid)
        {
            var response = await this.client.GetAsync(url);
            var imagePath = "\\images\\posters\\" +
                $"poster-{movieGuid}{System.IO.Path.GetExtension(url)}";

            await using var fileStream = new FileStream(this.rootPath + imagePath, FileMode.CreateNew);

            if (response != null)
            {
                await response.Content.CopyToAsync(fileStream);
            }

            return imagePath;
        }
    }
}
