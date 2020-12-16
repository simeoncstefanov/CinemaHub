namespace CinemaHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;
    using AutoMapper.Configuration;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Models;
    using CinemaHub.Web.ViewModels.Media;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Http.Internal;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    public class MovieAPIService : IMovieAPIService
    {
        private readonly IRepository<Keyword> keywordRepository;
        private readonly IRepository<Media> mediaRepository;
        private readonly HttpClient client;
        private readonly IMapper mapper;

        private readonly string rootPath;

        public MovieAPIService(IRepository<Keyword> keywordRepository, IRepository<Media> mediaRepository)
        {
            this.keywordRepository = keywordRepository;
            var config = new MapperConfiguration(
                cfg => cfg.CreateMap<MovieApiDTO, MediaDetailsInputModel>()
                    .ForMember(
                        x => x.ReleaseDate,
                        opt => opt.MapFrom(
                            x => DateTime.ParseExact(x.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)))
                    .ForMember(
                        x => x.Genres,
                        opt => opt.MapFrom(x => string.Join(", ", x.Genres.Select(x => x.Name)))));

            this.mapper = new Mapper(config);
            this.client = new HttpClient();
            this.mediaRepository = mediaRepository;
        }

        public async Task<MediaDetailsInputModel> GetDetailsFromApiAsync(int apiId, string mediaType)
        {
            string urlMediaType = string.Empty;
            if (mediaType == "Movie")
            {
                urlMediaType = "movie";
            }
            else if (mediaType == "Show")
            {
                urlMediaType = "tv";
            }

            var mediaString = await this.client.GetStringAsync(
                                  $"https://api.themoviedb.org/3/{urlMediaType}/{apiId}?api_key=238b937765146aa0e189640d869591e7&language=en-US&append_to_response=keywords");
            var mediaJson = JsonConvert.DeserializeObject<MovieApiDTO>(mediaString);
            var inputModel = this.mapper.Map<MediaDetailsInputModel>(mediaJson);

            // Check to see if the movie already exists so we can edit it if it does
            var mediaDb = this.mediaRepository.AllAsNoTracking().FirstOrDefault(x => x.Title == mediaJson.Title);
            if (mediaDb != null)
            {
                inputModel.Id = mediaDb.Id;
            }

            // IF the keyword doesn't exist make its id 0 so the Media Service knows it does not exist 
            var keywordList = new List<KeywordViewModel>();
            foreach (var keyword in mediaJson.KeywordsListApi.KeywordsList)
            {
                var keywordDb = await this.keywordRepository.AllAsNoTracking()
                                    .FirstOrDefaultAsync(x => x.Name == keyword.Name);
                int keywordId = keywordDb?.Id ?? 0;

                keywordList.Add(new KeywordViewModel() { Id = keywordId, Value = keyword.Name, });
            }

            inputModel.Keywords = JsonConvert.SerializeObject(keywordList);

            // Add image and backdrop as FormFIle
            var posterResponse = await this.client.GetAsync($"https://image.tmdb.org/t/p/w500{mediaJson.PosterPath}");
            var posterBytes = await posterResponse.Content.ReadAsByteArrayAsync();

            // create [IFormFile] which services can use to create images
            inputModel.PosterImageFile =
                new ApiImageFile(posterBytes, posterBytes.Length, "Poster", mediaJson.PosterPath);

            inputModel.MediaType = mediaType;

            return inputModel;
        }

        public async Task<int> GetIdFromTitle(string title, string mediaType)
        {
            string urlMediaType = string.Empty;
            if (mediaType == "Movie")
            {
                urlMediaType = "movie";
            }
            else if (mediaType == "Show")
            {
                urlMediaType = "tv";
            }

            var apiId = string.Empty;

            try
            {
                apiId = (string)JObject.Parse(
                        await this.client.GetStringAsync(
                            $"https://api.themoviedb.org/3/search/{urlMediaType}?api_key=238b937765146aa0e189640d869591e7&language=en-US&query={title}&page=1&include_adult=false"))
                    ["results"]?[0]["id"];
            }
            catch (Exception)
            {
                throw new Exception("Title not found");
            }

            if (int.TryParse(apiId, out int result))
            {
                return result;
            }
            else
            {
                throw new Exception("Title not found");
            }
        }

        public async Task<IEnumerable<int>> GetMoviesIds(int page)
        {
            var jsonResponse = await this.client
                .GetStringAsync($"https://api.themoviedb.org/3/discover/movie?api_key=238b937765146aa0e189640d869591e7&language=en-US&sort_by=vote_count.desc&include_adult=false&include_video=false&page={page}");

            var mediaResult = JsonConvert.DeserializeObject<MediaDiscoverApiDTO>(jsonResponse);

            return mediaResult.Results.Select(x => x.MediaApiId);
        }

        public async Task<IEnumerable<int>> GetShowsIds(int page)
        {
            var jsonResponse = await this.client
                .GetStringAsync($"https://api.themoviedb.org/3/discover/tv?api_key=238b937765146aa0e189640d869591e7&language=en-US&sort_by=vote_count.desc&page={page}&include_null_first_air_dates=false");

            var mediaResult = JsonConvert.DeserializeObject<MediaDiscoverApiDTO>(jsonResponse);

            return mediaResult.Results.Select(x => x.MediaApiId);
        }
    }
}
