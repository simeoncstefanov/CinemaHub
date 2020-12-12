namespace CinemaHub.Web.ViewModels.Discussions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using AutoMapper;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    public class CommentViewModel : IMapFrom<Comment>, IHaveCustomMappings
    {
        public string CreatorId { get; set; }

        public string Content { get; set; }

        public string Username { get; set; }

        public DateTime CreatedOn { get; set; }

        public string AvatarImagePath { get; set; }

        public int Upvotes { get; set; }

        public int Downvotes { get; set; }

        public void CreateMappings(IProfileExpression configuration)
        {
            configuration.CreateMap<Comment, CommentViewModel>()
                .ForMember(
                    x => x.Username, 
                    opt => opt.MapFrom(x => x.Creator.UserName))
                .ForMember(
                    x => x.Upvotes,
                    opt => opt.MapFrom(x => x.CommentVotes.Where(x => x.IsUpvote).Count()))
                .ForMember(
                    x => x.Upvotes, 
                    opt => opt.MapFrom(x => x.CommentVotes.Where(x => !x.IsUpvote).Count()))
                .ForMember(
                    x => x.AvatarImagePath, 
                    opt => opt.MapFrom(x => x.Creator.AvatarImage.Path));
        }
    }
}
