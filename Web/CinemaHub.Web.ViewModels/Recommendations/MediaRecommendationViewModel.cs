namespace CinemaHub.Web.ViewModels.Recommendations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels.Media;

    public class MediaRecommendationViewModel : IMapFrom<Media>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime ReleaseDate { get; set; }

        public string PosterPath { get; set; }

        public string MediaType { get; set; }

        public IEnumerable<string> Genres { get; set; }

        public double AverageRating { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Media, MediaRecommendationViewModel>()
                .ForMember(x => x.MediaType, opt => opt.MapFrom(x => x.GetType().Name)).ForMember(
                    x => x.PosterPath,
                    opt => opt.MapFrom(x => x.Images.Where(x => x.ImageType == ImageType.Poster).FirstOrDefault().Path))
                .ForMember(x => x.Genres, opt => opt.MapFrom(x => x.Genres.Select(x => x.Genre.Name).ToList()))
                .ForMember(
                    x => x.AverageRating,
                    opt => opt.MapFrom(x => x.Ratings.Select(x => (double)x.Score).Average()));
        }
    }
}