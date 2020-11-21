namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data.Models;

    public interface IMediaService
    {
        Task<IEnumerable<MediaGridDTO>> GetPageElementsAsync(int page, int elementsPerPage);

        int SearchMedia(string searchInput, MediaEnum mediaType);

        void SortBy(SortTypeEnum sortType);

        void SortByDescending(SortTypeEnum sortType);

        Task<MediaDetailsDTO> GetMovieDetailsAsync(string id);
    }
}
