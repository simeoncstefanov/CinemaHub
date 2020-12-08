namespace CinemaHub.Web.Areas.Identity.Pages.Account.Manage
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;

    public class WatchlistModel : PageModel
    {
        private readonly Dictionary<string, string> watchTypeFormatting = new Dictionary<string, string>()
                                                                          {
                                                                              {"onwatchlist", "Want to watch"},
                                                                              {"completed", "Completed"},
                                                                              {"dropped", "Dropped"},
                                                                              {"currentlywatched", "Currently Watched"}
                                                                          };

        private readonly IMediaService mediaService;

        private readonly UserManager<ApplicationUser> userManager;

        public WatchlistModel(
            IMediaService mediaService,
            UserManager<ApplicationUser> userManager)
        {
            this.mediaService = mediaService;
            this.userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;

        [BindProperty(SupportsGet = true)]
        public string WatchType { get; set; } = "onwatchlist";

        [BindProperty(SupportsGet = true)]
        public string SortType { get; set; } = "a-z";

        public int ElementsPerPage { get; set; } = 20;

        public string Username { get; set; }

        public string WatchTypeFormatted => this.watchTypeFormatting[this.WatchType];

        public int ResultCount { get; set; }

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(this.ResultCount, this.ElementsPerPage));

        public IEnumerable<MediaGridDTO> MediaSet { get; set; }

        [Authorize]
        public async Task OnGetAsync()
        {
            var user = await this.userManager.GetUserAsync(this.User);
            this.Username = user.UserName;

            var query = new MediaQueryDTO()
            {
                WatchType = this.WatchType,
                SortType = this.SortType,
                UserId = user.Id,
                Page = this.CurrentPage,
                ElementsPerPage = this.ElementsPerPage,
            };

            var media = await this.mediaService.GetPageAsync(query);
            this.ResultCount = media.ResultCount;
            this.MediaSet = media.Results;
            
            this.Page();
        }
    }
}
