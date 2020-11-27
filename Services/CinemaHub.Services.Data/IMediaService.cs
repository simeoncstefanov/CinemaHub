namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Web.ViewModels.Media;

    public interface IMediaService
    {
        Task<MediaResultDTO> GetPageAsync(int page, int elementsPerPage);

        Task ApplyQueryAsync(MediaQueryDTO query);

        Task<T> GetDetailsAsync<T>(string id);

        Task EditDetailsAsync(MediaDetailsInputModel mediaDetails, string userId, string rootPath);
    }
}
