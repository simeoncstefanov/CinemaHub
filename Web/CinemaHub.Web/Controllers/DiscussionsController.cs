namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Web.Authorization;
    using CinemaHub.Web.Filters.Action.InputModelTransfer;
    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels.Discussions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class DiscussionsController : BaseController
    {
        private const int DiscussionsPerPage = 10;

        private readonly IDiscussionsService discussionsService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IAuthorizationService authorizationService;

        public DiscussionsController(
            IDiscussionsService discussionsService,
            IAuthorizationService authorizationService,
            UserManager<ApplicationUser> userManager)
        {
            this.authorizationService = authorizationService;
            this.discussionsService = discussionsService;
            this.userManager = userManager;
        }

        [AllowAnonymous]
        public async Task<IActionResult> All(string mediaId = null, int page = 1)
        {
            try
            {
                if (mediaId == null)
                {
                    return this.Redirect("/");
                }

                var discussions = await this.discussionsService.GetDiscussions<DiscussionViewModel>(mediaId, page, DiscussionsPerPage);
                var totalResults = await this.discussionsService.GetTotalDiscussions(mediaId);

                var inputModel = new DiscussionPageViewModel()
                                     {
                                         TotalResults = totalResults,
                                         TotalPages = (int)Math.Ceiling(decimal.Divide(totalResults, DiscussionsPerPage)),
                                         CurrentPage = page,
                                         Discussions = discussions,
                                         MediaId = mediaId,
                                     };

                return this.View("~/Views/Discussions/All.cshtml", inputModel);
            }
            catch (Exception e)
            {
                return this.NotFound();
            }
        }

        [ImportModelState]
        [AllowAnonymous]
        [Route("[controller]/{discussionId}")]
        public async Task<IActionResult> MediaDiscussion(string discussionId, int page = 1)
        {
            var comments = await this.discussionsService.GetComments<CommentViewModel>(discussionId, page, DiscussionsPerPage);
            var totalComments = await this.discussionsService.GetTotalComments(discussionId);
            var discussionTitle = await this.discussionsService.GetDiscussionTitle(discussionId);
            var discussionMedia = await this.discussionsService.GetDiscussionMedia(discussionId);

            var inputModel = new CommentPageViewModel()
                                 {
                                     TotalResults = totalComments,
                                     TotalPages = (int)Math.Ceiling(decimal.Divide(totalComments, DiscussionsPerPage)),
                                     CurrentPage = page,
                                     Comments = comments,
                                     DiscussionTitle = discussionTitle,
                                     MediaId = discussionMedia,
                                     DiscussionId = discussionId,
                                 };

            return this.View("~/Views/Discussions/Comments.cshtml", inputModel);
        }

        [ImportInputModel(ClassName = nameof(DiscussionInputModel))]
        [ImportModelState]
        public async Task<IActionResult> Create(string mediaId)
        {
            if (mediaId == null)
            {
                return this.Redirect("/");
            }

            return this.View("~/Views/Discussions/Create.cshtml", new DiscussionInputModel() { MediaId = mediaId });
        }

        [HttpPost]
        [ExportModelState]
        [ExportInputModel]
        public async Task<IActionResult> Create(DiscussionInputModel inputModel)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("Create", "Discussions", new { mediaId = inputModel.MediaId });
            }

            try
            {
                await this.discussionsService.CreateDiscussion(inputModel, user.Id);
            }
            catch (Exception e)
            {
                this.ModelState.AddModelError(string.Empty, e.Message);
                return this.RedirectToAction("Create", "Discussions", new { mediaId = inputModel.MediaId });
            }

            return this.RedirectToAction("All", "Discussions", new { mediaId = inputModel.MediaId });
        }

        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> CreateComment(CommentInputModel inputModel)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                return this.RedirectToAction("MediaDiscussion", "Discussions", new { discussionId = inputModel.DiscussionId });
            }

            await this.discussionsService.CreateComment(inputModel.Content, user.Id, inputModel.DiscussionId);
            return this.RedirectToAction("MediaDiscussion", "Discussions", new { discussionId = inputModel.DiscussionId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(string commentId)
        {
            var comment = await this.discussionsService.GetCommentInfo<CommentViewModel>(commentId);

            if (comment == null)
            {
                return this.NotFound();
            }

            var isAuthorized = await this.authorizationService.AuthorizeAsync(this.User, comment, AuthorizedOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return this.Forbid();
            }

            await this.discussionsService.DeleteComment(commentId);

            return this.RedirectToAction("MediaDiscussion", "Discussions", new { discussionId = comment.DiscussionId });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteDiscussion(string discussionId)
        {
            var discussion = await this.discussionsService.GetDiscussionInfo<DiscussionViewModel>(discussionId);

            if (discussion == null)
            {
                return this.NotFound();
            }

            var isAuthorized = await this.authorizationService.AuthorizeAsync(this.User, discussion, AuthorizedOperations.Delete);
            if (!isAuthorized.Succeeded)
            {
                return this.Forbid();
            }

            await this.discussionsService.DeleteDiscussion(discussionId);

            return this.RedirectToAction("All", "Discussions", new { mediaId = discussion.MediaId });
        }
    }
}
