namespace CinemaHub.Services.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using AutoMapper;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels.Media;

    using Microsoft.AspNetCore.Http;

    using Newtonsoft.Json;

    public class MovieApiDTO : IMapTo<MediaDetailsInputModel>, IHaveCustomMappings
    {
        [JsonIgnore]
        public string Id { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("overview")]
        public string Overview { get; set; }

        [JsonProperty("original_language")]
        public string Language { get; set; }

        [JsonProperty("release_date")]
        public string ReleaseDate { get; set; }

        [JsonProperty("id")]
        public int MovieApiId { get; set; }

        [JsonProperty("backdrop_path")]
        public string BackdropPath { get; set; }

        [JsonProperty("poster_path")]
        public string PosterPath { get; set; }

        [JsonProperty("genres")]
        public List<GenreApiDTO> Genres { get; set; }

        [JsonProperty("budget")]
        public int Budget { get; set; }

        [JsonProperty("runtime")]
        public int Runtime { get; set; }

        [JsonProperty("keywords")]
        public KeywordListApiDTO KeywordsListApi { get; set; }

        public IEnumerable<KeywordViewModel> KeywordsList { get; set; }

        public IFormFile PosterImageFile { get; set; }

        public string MediaType { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<MovieApiDTO, MediaDetailsInputModel>().ForMember(
                    x => x.ReleaseDate,
                    opt => opt.MapFrom(
                        x => DateTime.ParseExact(x.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture)))
                .ForMember(x => x.Genres, opt => opt.MapFrom(x => string.Join(", ", x.Genres.Select(x => x.Name))));
        }
    }
}
