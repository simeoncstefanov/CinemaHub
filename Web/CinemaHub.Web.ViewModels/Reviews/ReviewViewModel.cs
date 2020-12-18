namespace CinemaHub.Web.ViewModels.Reviews
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class ReviewViewModel : IMapFrom<Review>, IHaveCustomMappings
    {
        public string RatingId { get; set; }

        public byte Rating { get; set; }

        public string Title { get; set; }

        public string ReviewText { get; set; }

        public string AvatarImage { get; set; }

        public DateTime CreatedOn { get; set; }

        public string Creator { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Review, ReviewViewModel>()
                .ForMember(x => x.Creator, opt => opt.MapFrom(x => x.Rating.Creator.UserName))
                .ForMember(x => x.Rating, opt => opt.MapFrom(x => x.Rating.Score))
                .ForMember(
                    x => x.AvatarImage,
                    opt => opt.MapFrom(x => x.Creator.AvatarImage.Path))
                .ForMember(
                    x => x.RatingId,
                    opt => opt.MapFrom(x => x.Rating.Id));
        }
    }
}
