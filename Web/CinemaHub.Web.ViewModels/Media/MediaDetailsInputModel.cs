namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.Linq;

    using AutoMapper;

    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Mapping;

    using Microsoft.AspNetCore.Http;

    using Newtonsoft.Json;

    public class MediaDetailsInputModel : IMapFrom<Media>, IMapTo<Media>, IMapFrom<MediaEdit>, IHaveCustomMappings
    {
        public string Id { get; set; }

        [Required]
        [MinLength(3, ErrorMessage = "Title should be at least 3 characters long")]
        public string Title { get; set; }

        [Required]
        [MinLength(20, ErrorMessage = "Overview should be at least 20 characters long")]
        public string Overview { get; set; }

        public string Language { get; set; }

        [Required]
        public DateTime ReleaseDate { get; set; }

        public string ReleaseDateString => this.ReleaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        [Range(1, int.MaxValue, ErrorMessage = "Runtime should be more than 0")]
        public int Runtime { get; set; }

        public int Budget { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        [JsonIgnore]
        public IFormFile PosterImageFile { get; set; }

        public string PosterPath { get; set; }

        [Required]
        public string MediaType { get; set; }

        [Required]
        public string Genres { get; set; }

        public string Keywords { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Media, MediaDetailsInputModel>()
                .ForMember(x => x.MediaType, opt => opt.MapFrom(x => x.GetType().Name))
                .ForMember(
                    x => x.PosterPath,
                    opt => opt.MapFrom(x => x.Images.Where(x => x.ImageType == ImageType.Poster).FirstOrDefault().Path))
                .ForMember(
                    x => x.Genres,
                    opt => opt.MapFrom(x => string.Join(", ", x.Genres.Select(x => x.Genre.Name)))).ForMember(
                    x => x.Keywords,
                    opt => opt.MapFrom(
                        x => JsonConvert.SerializeObject(
                            x.Keywords.Select(
                                x => new KeywordViewModel() { Id = x.Keyword.Id, Value = x.Keyword.Name, }))));

            configuration.CreateMap<MediaEdit, MediaDetailsInputModel>()
                .ForMember(x => x.Id, opt => opt.MapFrom(x => x.MediaId))
                .ForMember(x => x.Keywords, opt => opt.MapFrom(x => x.KeywordsJson));
        }
    }
}
