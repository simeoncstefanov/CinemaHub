namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Mapping;

    public class MediaMinimalViewModel : IMapFrom<Media>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public string ImagePath { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Media, MediaMinimalViewModel>()
                .ForMember(x => x.ImagePath, opt =>
                    opt.MapFrom(x => 
                        x.Images.FirstOrDefault(x => x.ImageType == ImageType.Poster).Path));
        }
    }
}
