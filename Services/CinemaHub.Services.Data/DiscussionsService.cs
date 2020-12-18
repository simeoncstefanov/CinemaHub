namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    using CinemaHub.Common;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Web.ViewModels.Discussions;

    using Microsoft.EntityFrameworkCore;

    public class DiscussionsService : IDiscussionsService
    {
        private IDeletableEntityRepository<Discussion> discussionRepo;
        private IDeletableEntityRepository<Comment> commentRepo;
        private IRepository<CommentVote> voteRepo;
        private IRepository<Media> mediaRepo;

        public DiscussionsService(
            IDeletableEntityRepository<Discussion> discussionRepo,
            IDeletableEntityRepository<Comment> commentRepo,
            IRepository<CommentVote> voteRepo,
            IRepository<Media> mediaRepo)
        {
            this.discussionRepo = discussionRepo;
            this.commentRepo = commentRepo;
            this.voteRepo = voteRepo;
            this.mediaRepo = mediaRepo;
        }

        public async Task CreateComment(string content, string userId, string discussionId)
        {
            var comment = new Comment()
                              {
                                  Content = content,
                                  CreatorId = userId,
                                  DiscussionId = discussionId,
                              };

            await this.commentRepo.AddAsync(comment);
            await this.commentRepo.SaveChangesAsync();
        }

        public async Task CreateDiscussion(DiscussionInputModel inputModel, string userId)
        {
            bool doesMediaExist = await this.mediaRepo.AllAsNoTracking().AnyAsync(x => x.Id == inputModel.MediaId);

            if (!doesMediaExist)
            {
                throw new Exception(string.Format(GlobalExceptions.MediaDoesNotExist, inputModel.MediaId));
            }

            var discussion = new Discussion()

                                 {
                                     Title = inputModel.Title,
                                     CreatorId = userId,
                                     MediaId = inputModel.MediaId,
                                 };

            await this.discussionRepo.AddAsync(discussion);
            await this.CreateComment(inputModel.Content, userId, discussion.Id);
            await this.discussionRepo.SaveChangesAsync();
        }

        public async Task DeleteComment(string commentId)
        {
            var comment = await this.commentRepo.All().FirstOrDefaultAsync(x => x.Id == commentId);
            this.commentRepo.Delete(comment);
            await this.commentRepo.SaveChangesAsync();
        }

        public async Task DeleteDiscussion(string discussionId)
        {
            var discussion = await this.discussionRepo.All().Include(x => x.Comments).FirstOrDefaultAsync(x => x.Id == discussionId);
            this.discussionRepo.Delete(discussion);
            await this.discussionRepo.SaveChangesAsync();
        }

        public async Task VoteComment(string commentId, string userId, bool isUpvote)
        {
            var vote = await this.voteRepo.AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.CommentId == commentId && x.UserId == userId);

            if (vote != null)
            {
                this.voteRepo.Delete(vote);
            }
            else
            {
                vote = new CommentVote() { CommentId = commentId, UserId = userId, IsUpvote = isUpvote, };
                await this.voteRepo.AddAsync(vote);
            }

            await this.voteRepo.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetDiscussions<T>(string mediaId, int page, int count)
        {
            int pagination = (page - 1) * count;
            var discussions = await this.discussionRepo.AllAsNoTracking().Where(x => x.MediaId == mediaId)
                .OrderByDescending(x => x.CreatedOn)
                .Skip(pagination)
                .Take(count)
                .To<T>()
                .ToListAsync();

            return discussions;
        }

        public async Task<T> GetDiscussionInfo<T>(string discussionId)
        {
            var discussion = await this.discussionRepo.AllAsNoTracking()
                              .Where(x => x.Id == discussionId)
                              .To<T>()
                              .FirstOrDefaultAsync();

            return discussion;
        }

        public async Task<T> GetCommentInfo<T>(string commentId)
        {
            var comment = await this.commentRepo.AllAsNoTracking()
                              .Where(x => x.Id == commentId)
                              .To<T>()
                              .FirstOrDefaultAsync();

            return comment;
        }

        public async Task<IEnumerable<T>> GetComments<T>(string discussionId, int page, int count)
        {
            int pagination = (page - 1) * count;
            var comments = await this.commentRepo.AllAsNoTracking().Where(x => x.DiscussionId == discussionId)
                                  .OrderBy(x => x.CreatedOn)
                                  .Skip(pagination)
                                  .Take(count)
                                  .To<T>()
                                  .ToListAsync();

            return comments;
        }

        public async Task<int> GetTotalDiscussions(string mediaId)
        {
            return await this.discussionRepo.AllAsNoTracking().CountAsync(x => x.MediaId == mediaId);
        }

        public async Task<int> GetTotalComments(string discussionId)
        {
            return await this.commentRepo.AllAsNoTracking().CountAsync(x => x.DiscussionId == discussionId);
        }

        public async Task<string> GetDiscussionTitle(string discussionId)
        {
            var title = await this.discussionRepo.AllAsNoTracking().Where(x => x.Id == discussionId).Select(x => x.Title).FirstOrDefaultAsync();
            return title;
        }

        public async Task<string> GetDiscussionMedia(string discussionId)
        {
            return await this.discussionRepo.AllAsNoTracking().Where(x => x.Id == discussionId).Select(x => x.MediaId).FirstOrDefaultAsync();
        }
    }
}
