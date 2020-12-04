namespace CinemaHub.Web.ViewModels.Media
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AutoMapper;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class MediaPathViewModel : IMapFrom<Media>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string MediaType { get; set; }

        public string MediaPath => $"/Media/{this.MediaType}s/{this.Id}";

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Media, MediaPathViewModel>().ForMember(
                x => x.MediaType,
                opt => opt.MapFrom(x => x.GetType().Name));
        }
    }
}
