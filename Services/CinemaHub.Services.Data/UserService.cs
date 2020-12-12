namespace CinemaHub.Services.Data
{
    using CinemaHub.Data.Models.Enums;
    using Microsoft.AspNetCore.Http;

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Common;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Mapping;

    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Migrations;

    public class UserService : IUserService
    {
        private const int MediaPerPage = 20;
        private readonly IRepository<MediaWatcher> watchRepository;
        private readonly IRepository<AvatarImage> avatarRepo;

        public UserService(
            IRepository<MediaWatcher> watchRepository, 
            IRepository<AvatarImage> avatarImageRepo,
            IRepository<ApplicationUser> userRepo)
        {
            this.watchRepository = watchRepository;
            this.avatarRepo = avatarImageRepo;
        }

        public async Task AddToUserWatchlistAsync(string userId, string mediaId, WatchType watchType)
        {
            var watcher = this.watchRepository.All()
                .FirstOrDefault(x => x.UserId == userId && x.MediaId == mediaId);

            if (watcher == null)
            {
                watcher = new MediaWatcher()
                              {
                                  UserId = userId, 
                                  MediaId = mediaId,
                              };

                await this.watchRepository.AddAsync(watcher);
            }

            watcher.WatchType = watchType;
            await this.watchRepository.SaveChangesAsync();
        }

        public async Task ChangeAvatarAsync(IFormFile image, string rootPath, string userId)
        {
            var avatarImage = await this.avatarRepo.All().FirstOrDefaultAsync(x => x.UserId == userId);

            var avatarPath = "\\images\\avatars\\";
            var path = rootPath + avatarPath;

            var name = "avatar-" + avatarImage.UserId;
            var imageExtension = await FileDownloader.DownloadImage(image, path, name);

            avatarImage.Path = avatarPath + $"{name}.{imageExtension}";
            avatarImage.Extension = imageExtension;

            this.avatarRepo.Update(avatarImage);
            await this.avatarRepo.SaveChangesAsync();
        }

        public async Task DeleteWatchlistAsync(string userId, string mediaId)
        {
            var watcher = await this.watchRepository.All()
                              .FirstOrDefaultAsync(x => x.UserId == userId && x.MediaId == mediaId);

            if (watcher == null)
            {
                return;
            }

            this.watchRepository.Delete(watcher);
            await this.watchRepository.SaveChangesAsync();
        }

        public async Task<string> GetAvatarPath(string userId)
        {
            var avatarImage = await this.avatarRepo.AllAsNoTracking().FirstOrDefaultAsync(x => x.UserId == userId);

            if (avatarImage != null)
            {
                return avatarImage.Path;
            }

            return GlobalConstants.DefaultAvatarImagePath;
        }

        public async Task<IEnumerable<T>> GetUserWatchListAsync<T>(string userId, int page, WatchType watchType)
        {
            int pagination = (page - 1) * MediaPerPage;
            var watches = await this.watchRepository.AllAsNoTracking()
                              .Where(x => x.UserId == userId && x.WatchType == watchType)
                              .Skip(pagination)
                              .Take(MediaPerPage)
                              .To<T>().ToListAsync();

            return watches;
        }

        public async Task<string> GetWatchtypeUserAsync(string userId, string mediaId)
        {
            var watcher = await this.watchRepository.AllAsNoTracking()
                              .FirstOrDefaultAsync(x => x.UserId == userId && x.MediaId == mediaId);

            if (watcher == null)
            {
                return string.Empty;
            }
            return watcher.WatchType.ToString();
        }
    }
}
