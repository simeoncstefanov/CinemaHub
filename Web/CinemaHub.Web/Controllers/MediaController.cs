namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Media;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json;

    public class MediaController : BaseController
    {
        private const int DefaultPerPage = 20;
        private readonly IMediaService mediaService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IWebHostEnvironment webHostEnvironment;

        public MediaController(IMediaService mediaService,
                               IWebHostEnvironment webHostEnvironment, 
                               UserManager<ApplicationUser> userManager)
        {
            this.mediaService = mediaService;
            this.userManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        // both controllers get the searched media with ajax
        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Movies()
        {
            return this.View(new MediaGridViewModel() { MediaType = "Movie" });
        }

        [Route("/[controller]/[action]")]
        public async Task<IActionResult> Shows()
        {
            return this.View(new MediaGridViewModel() { MediaType = "Show" });
        }

        public async Task<IActionResult> Movies(string id)
        {
            var viewModel = await this.mediaService.GetDetailsAsync<MediaDetailsViewModel>(id);
            if (viewModel.MediaType != "Movie" || viewModel is null)
            {
                // TODO: ADD ERROR VIEW;
                return this.NotFound();
            }

            return this.View("MediaDetails", viewModel);
        }

        public async Task<IActionResult> Shows(string id)
        {
            var viewModel = await this.mediaService.GetDetailsAsync<MediaDetailsViewModel>(id);
            if (viewModel.MediaType != "Show" || viewModel is null)
            {
                // TODO: ADD ERROR VIEW;
                return this.NotFound();
            }

            return this.View("MediaDetails", viewModel);
        }

        public async Task<IActionResult> All(string query)
        {
            return this.View("Error");
        }

        [Authorize]
        public async Task<IActionResult> Add()
        {
            return this.View(new MediaDetailsInputModel());
        }

        [Authorize]
        public async Task<IActionResult> Edit(string id)
        {
            var media = await this.mediaService.GetDetailsAsync<MediaDetailsInputModel>(id);
            return this.View(media);
        }

        // Edits or adds new movie if there is no id in the input model
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Edit(MediaDetailsInputModel edit)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            if (!this.ModelState.IsValid)
            {
                this.View(edit);
            }

            try
            {
                await this.mediaService.EditDetailsAsync(edit, user.Id, this.webHostEnvironment.WebRootPath);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
            }

            return this.Redirect("/");
        }
    }
}
