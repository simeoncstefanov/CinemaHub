namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Web.ViewModels.Watchlist;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    [Route("api/watchlist")]
    [ApiController]
    public class WatchlistAPIController : ControllerBase
    {
        private readonly IUserService userService;

        public WatchlistAPIController(IUserService userService)
        {
            this.userService = userService;
        }

        public async Task<ActionResult> GetUserMediaWatchlist(string mediaId)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            return this.StatusCode(200);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<WatchlistViewModel>> AddToWatchlist(string mediaId, string watchType, bool delete = false)
        {
            var userId = this.User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (delete)
            {
                await this.userService.DeleteWatchlistAsync(userId, mediaId);
                return new WatchlistViewModel() { IsAdded = false, WatchType = string.Empty };
            }
            bool isWatchTypeValid = Enum.TryParse<WatchType>(watchType, true, out WatchType watchEnum);
            if (!isWatchTypeValid)
            {
                return this.BadRequest();
            }

            await this.userService.AddToUserWatchlistAsync(userId, mediaId, watchEnum);
            return new WatchlistViewModel() { IsAdded = true, WatchType = watchType };
        }
    }
}