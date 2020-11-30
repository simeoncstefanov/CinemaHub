namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Security.Cryptography.X509Certificates;

    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Mapping;

    public class MediaDetailsViewModel : IMapFrom<Media>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string Overview { get; set; }

        public string Language { get; set; }

        public DateTime ReleaseDate { get; set; }

        public int Runtime { get; set; }

        public int Budget { get; set; }

        public bool IsDetailFull { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public string PosterPath { get; set; }

        public string BackdropPath { get; set; }

        public string MediaType { get; set; }

        public List<string> GenresList { get; set; }

        public IEnumerable<KeywordViewModel> Keywords { get; set; }

        public double AverageRating { get; set; }

        public int RatingCount { get; set; }

        public int CurrentUserRating { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Media, MediaDetailsViewModel>()
                .ForMember(x => x.MediaType, opt => opt.MapFrom(x => x.GetType().Name))
                .ForMember(
                    x => x.PosterPath,
                    opt => opt.MapFrom(x => x.Images.Where(x => x.ImageType == ImageType.Poster).FirstOrDefault().Path))
                .ForMember(x => x.GenresList, opt => opt.MapFrom(x => x.Genres.Select(x => x.Genre.Name).ToList()))
                .ForMember(
                    x => x.Keywords,
                    opt => opt.MapFrom(
                        x => x.Keywords.Select(
                            x => new KeywordViewModel() { Id = x.Keyword.Id, Value = x.Keyword.Name, })));
        }
    }
}
