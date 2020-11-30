namespace CinemaHub.Services.Data
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    using CinemaHub.Web.ViewModels.Discussions;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Routing;

    public interface IDiscussionsService
    {
        Task<IEnumerable<T>> GetDiscussions<T>(string mediaId, int page);

        Task CreateDiscussion(DiscussionInputModel inputModel, string userId);

        Task CreateComment(CommentInputModel inputModel, string userId, string discussionId);

        Task UpvoteComment(string commentId, string userId);

        Task DownvoteComment(string commentId, string userId);

        Task DeleteComment(string commentId);

        Task<bool> IsCommentUser(string commentId, string userId);

        Task<bool> IsDiscussionUser(string discussionId, string userId);

        Task DeleteDiscussion(string discussionId);
    }
}