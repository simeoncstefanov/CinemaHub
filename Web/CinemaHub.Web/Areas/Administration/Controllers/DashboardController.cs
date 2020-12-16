namespace MoiteRecepti.Web.Areas.Administration.Controllers
{
    using CinemaHub.Common;
    using CinemaHub.Services.Data;
    using CinemaHub.Web.Controllers;
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.MediaEdits;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class DashboardController : BaseController
    {
        private const int DefaultPerPage = 5;
        private readonly IMediaEditService mediaEditService;
        private readonly IMediaService mediaService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public DashboardController(IMediaEditService mediaEditService, IMediaService mediaService, IWebHostEnvironment webHostEnvironment)
        {
            this.mediaEditService = mediaEditService;
            this.mediaService = mediaService;
            this.webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {

            return this.View();
        }

        public async Task<IActionResult> EditsApproval(int page = 1)
        {
            var editsForApproval = await this.mediaEditService.GetEditsForApproval<MediaDetailsInputModel>(page, DefaultPerPage);

            var viewModel = new MediaEditApprovePageViewModel()
            {
                CurrentPage = page,
                TotalResults = await this.mediaEditService.GetEditsForApprovalCount(),
                ElementsPerPage = DefaultPerPage,
                ComparisonEdited = new List<Tuple<MediaDetailsInputModel, MediaDetailsInputModel>>(),
            };

            foreach (var edit in editsForApproval)
            {
                var realMedia = await this.mediaService.GetDetailsAsync<MediaDetailsInputModel>(edit.Id);

                var tuple = new Tuple<MediaDetailsInputModel, MediaDetailsInputModel>(edit, realMedia);
                viewModel.ComparisonEdited.Add(tuple);
            }

            return this.View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveEdit(string editId, string isApproved, string keywordsList)
        {
            if (isApproved == "Approve")
            {
                var edit = await this.mediaEditService.GetAndApproveEdit<MediaDetailsInputModel>(editId);

                if (edit.PosterPath != null)
                {
                    using (var fileStream = new FileStream(this.webHostEnvironment.WebRootPath + edit.PosterPath, FileMode.Open))
                    {
                        var formFile = new FormFile(fileStream, 0, fileStream.Length, "Poster", fileStream.Name);
                        edit.PosterImageFile = formFile;
                        edit.Keywords = keywordsList;
                        await this.mediaService.EditDetailsAsync(edit, string.Empty, this.webHostEnvironment.WebRootPath);
                    }
                }
                else
                {
                    edit.Keywords = keywordsList;
                    await this.mediaService.EditDetailsAsync(edit, string.Empty, this.webHostEnvironment.WebRootPath);
                }
            }
            else if (isApproved == "Reject")
            {
                await this.mediaEditService.RejectEdit(editId);
            }

            return this.RedirectToAction("EditsApproval");
        }

    }
}