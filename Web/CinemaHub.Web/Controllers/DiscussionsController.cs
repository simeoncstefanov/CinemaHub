namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Web.Filters.Action.InputModelTransfer;
    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels.Discussions;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class DiscussionsController : BaseController
    {
        private const int DiscussionsPerPage = 1;

        private readonly IDiscussionsService discussionsService;
        private readonly UserManager<ApplicationUser> userManager;

        public DiscussionsController(IDiscussionsService discussionsService, UserManager<ApplicationUser> userManager)
        {
            this.discussionsService = discussionsService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> All(string mediaId, int page = 1)
        {
            try
            {
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
                                 };

            return this.View("~/Views/Discussions/Comments.cshtml", inputModel);
        }

        [Authorize]
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

        [Authorize]
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
        public async Task<IActionResult> Comment(CommentInputModel inputModel)
        {
            return this.View();
        }

        [HttpDelete]

        public async Task<IActionResult> Delete(string discussionId)
        {
            return this.View();
        }
    }
}
