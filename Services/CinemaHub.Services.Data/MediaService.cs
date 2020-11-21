namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services;
    using CinemaHub.Services.Data.Models;
    using Microsoft.EntityFrameworkCore;

    public class MediaService : IMediaService
    {
        private readonly IRepository<Media> mediaRepository;
        private readonly IRepository<Genre> genreRepository;
        private readonly IMovieAPIService apiService;
        private IQueryable<Media> mediaQuery;

        public MediaService(IRepository<Media> mediaRepository, IRepository<Genre> genreRepository, IMovieAPIService apiService)
        {
            this.mediaRepository = mediaRepository;
            this.genreRepository = genreRepository;
            this.apiService = apiService;
            this.mediaQuery = this.mediaRepository.All();
        }

        public int ResultsFound { get; private set; }

        public int SearchMedia(string searchInput, MediaEnum mediaType)
        {
            if (mediaType.ToString() == nameof(Movie))
            {
                this.mediaQuery = this.mediaQuery.OfType<Movie>();
            }
            else if (mediaType.ToString() == nameof(Show))
            {
                this.mediaQuery = this.mediaQuery.OfType<Show>();
            }

            if (string.IsNullOrEmpty(searchInput))
            {
                searchInput = string.Empty;
            }

            // TODO: IMPROVE SEARCH IMPLEMENTATION
            this.mediaQuery = this.mediaQuery.Where(x => x.Title.Contains(searchInput)
                || x.Keywords.Any(x => x.Keyword.Name.Contains(searchInput)));

            int resultsCount = this.mediaQuery.Count();
            this.ResultsFound = resultsCount;
            return resultsCount;
        }

        public void SortBy(SortTypeEnum sortType)
        {
            switch (sortType)
            {
                case SortTypeEnum.ReleaseDate:
                    this.mediaQuery = this.mediaQuery.OrderBy(x => x.ReleaseDate);
                    break;
                case SortTypeEnum.Popularity:
                    this.mediaQuery = this.mediaQuery.OrderBy(x => x.Watchers.Count);
                    break;
            }
        }

        public void SortByDescending(SortTypeEnum sortType)
        {
            switch (sortType)
            {
                case SortTypeEnum.ReleaseDate:
                    this.mediaQuery = this.mediaQuery.OrderByDescending(x => x.ReleaseDate);
                    break;
                case SortTypeEnum.Popularity:
                    this.mediaQuery = this.mediaQuery.OrderByDescending(x => x.Watchers.Count);
                    break;
            }
        }

        public async Task<IEnumerable<MediaGridDTO>> GetPageElementsAsync(int page, int elementsPerPage)
        {
            int paginationCount = (page - 1) * elementsPerPage;

            var medias = await this.mediaQuery
                .Skip(paginationCount)
                .Take(elementsPerPage)
                .Select(x => new MediaGridDTO()
                {
                    Id = x.Id,
                    IsDetailFull = x.IsDetailFull,
                    MovieApiId = x.MovieApiId,
                    Overview = x.Overview,
                    Title = x.Title,
                    ImagePath = x.Images.FirstOrDefault(x => x.ImageType == ImageType.Poster).Path,
                    MediaType = x.GetType().Name,
                    ResultsFound = this.ResultsFound,
                })
                .ToListAsync();

            return medias;
        }

        public async Task<MediaDetailsDTO> GetMovieDetailsAsync(string id)
        {
            var media = await this.mediaRepository.All()
                .Include(x => x.Genres)
                .ThenInclude(x => x.Genre)
                .Include(x => x.Images)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (media is null)
            {
                return null;
            }

            if (media.IsDetailFull == false)
            {
                // TODO: PUT ALL DATA FROM API WHEN THERE IS NONE!!!!
            }

            var genres = media.Genres.Select(x => x.Genre.Name).ToList();
            var image = media.Images.FirstOrDefault(x => x.ImageType == ImageType.Poster).Path;

            return new MediaDetailsDTO()
            {
                Title = media.Title,
                Budget = media.Budget,
                Genres = genres,
                PosterPath = image,
                Overview = media.Overview,
                ReleaseDate = media.ReleaseDate,
                Id = media.Id,
                Language = media.Language,
                MediaType = media.GetType().Name,
            };
        }
    }
}
