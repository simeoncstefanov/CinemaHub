namespace CinemaHub.Web.ViewModels.Discussions
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Reflection;
    using System.Text;

    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class DiscussionViewModel : IMapFrom<Discussion>, IHaveCustomMappings
    {
        public string Id { get; set; }

        public string Title { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime LastReply { get; set; }

        public string CreatorId { get; set; }

        public string Username { get; set; }

        public string AvatarImagePath { get; set; }

        public int CommentCount { get; set; }

        public bool isLocked { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Discussion, DiscussionViewModel>()
                .ForMember(
                    x => x.Username,
                    opt => opt.MapFrom(x => x.Creator.UserName))
                .ForMember(
                    x => x.AvatarImagePath,
                    opt => opt.MapFrom(x => x.Creator.AvatarImage.Path))
                .ForMember(
                    x => x.CommentCount,
                    opt => opt.MapFrom(x => x.Comments.Count))
                .ForMember(
                    x => x.LastReply,
                    opt => opt.MapFrom(x => x.Comments.Select(x => x.CreatedOn).OrderByDescending(x => x).FirstOrDefault()));
        }
    }
}
