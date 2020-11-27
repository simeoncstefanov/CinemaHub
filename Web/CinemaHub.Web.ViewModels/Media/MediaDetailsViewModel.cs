namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;

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

        public string ReleaseDateString { get; set; }

        public int Runtime { get; set; }

        public int Budget { get; set; }

        public bool IsDetailFull { get; set; }

        public string YoutubeTrailerUrl { get; set; }

        public string PosterPath { get; set; }

        public string MediaType { get; set; }

        public List<string> GenresList { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Media, MediaDetailsViewModel>()
                .ForMember(x => x.MediaType, opt =>
                    opt.MapFrom(x => x.GetType().Name))
                .ForMember(x => x.PosterPath, opt =>
                    opt.MapFrom(x =>
                        x.Images.Where(x => x.ImageType == ImageType.Poster)
                        .FirstOrDefault().Path))
                .ForMember(x => x.GenresList, opt =>
                    opt.MapFrom(x =>
                        x.Genres.Select(x => x.Genre.Name)
                        .ToList()))
                .ForMember(x => x.ReleaseDateString, opt =>
                    opt.MapFrom(x =>
                        x.ReleaseDate.HasValue ? x.ReleaseDate.Value.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) :
                        null));
        }
    }
}
