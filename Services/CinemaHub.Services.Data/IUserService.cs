namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models.Enums;

    using Microsoft.AspNetCore.Http;

    public interface IUserService
    {
        Task AddToUserWatchlistAsync(string userId, string mediaId, WatchType watchType);

        Task DeleteWatchlistAsync(string userId, string mediaId);

        Task<IEnumerable<T>> GetUserWatchListAsync<T>(string userId, int page, WatchType watchType);

        Task ChangeAvatarAsync(IFormFile image, string rootPath, string userId);

        Task<string> GetAvatarPath(string userId);

        Task<string> GetWatchtypeUserAsync(string userId, string mediaId);
    }
}
