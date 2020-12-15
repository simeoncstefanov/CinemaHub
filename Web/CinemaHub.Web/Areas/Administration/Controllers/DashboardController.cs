namespace MoiteRecepti.Web.Areas.Administration.Controllers
{
    using CinemaHub.Common;
    using CinemaHub.Services.Data;
    using CinemaHub.Web.Controllers;
    using CinemaHub.Web.ViewModels.Media;
    using CinemaHub.Web.ViewModels.MediaEdits;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class DashboardController : BaseController
    {
        private const int DefaultPerPage = 5;
        private readonly IMediaEditService mediaEditService;
        private readonly IMediaService mediaService;

        public DashboardController(IMediaEditService mediaEditService, IMediaService mediaService)
        {
            this.mediaEditService = mediaEditService;
            this.mediaService = mediaService;
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
                CurrentPage = 1,
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
    }
}