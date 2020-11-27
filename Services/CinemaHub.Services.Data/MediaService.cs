﻿namespace CinemaHub.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;

    using AutoMapper;
    using CinemaHub.Data;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Models.Enums;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Services;
    using CinemaHub.Services.Data.Models;
    using Microsoft.EntityFrameworkCore;
    using CinemaHub.Web.ViewModels.Media;

    public class MediaService : IMediaService
    {
        private readonly IRepository<Media> mediaRepository;
        private readonly IRepository<Genre> genreRepository;
        private readonly IRepository<Keyword> keywordRepository;
        private IQueryable<Media> mediaQuery;

        public MediaService(
            IRepository<Media> mediaRepository,
            IRepository<Genre> genreRepository,
            IRepository<Keyword> keywordRepository)
        {
            this.mediaRepository = mediaRepository;
            this.genreRepository = genreRepository;
            this.keywordRepository = keywordRepository;
            this.mediaQuery = this.mediaRepository.All();
        }

        public int ResultsFound { get; set; }

        public async Task<MediaResultDTO> GetPageAsync(int page, int elementsPerPage)
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
                })
                .ToListAsync();

            return new MediaResultDTO()
            {
                ResultCount = this.ResultsFound,
                Results = medias,
                ResultsPerPage = elementsPerPage,
                CurrentPage = page,
            };

        }

        public async Task<T> GetDetailsAsync<T>(string id)
        {
            var media = this.mediaRepository.All()
                .Include(x => x.Genres)
                .ThenInclude(x => x.Genre)
                .Include(x => x.Images)
                .Include(x => x.Keywords)
                .ThenInclude(x => x.Keyword)
                .Where(x => x.Id == id)
                .To<T>()
                .FirstOrDefault();

            if (media is null)
            {
                throw new InvalidOperationException($"Media with id {id} not found");
            }

            return media;
        }

        public async Task EditDetailsAsync(MediaDetailsInputModel inputModel, string userId, string rootPath)
        {
            var media = this.mediaRepository.All()
                .Include(x => x.Genres)
                .ThenInclude(x => x.Genre)
                .FirstOrDefault(x => x.Id == inputModel.Id);

            if (media is null)
            {
                string qualifiedName = $"CinemaHub.Data.Models.{inputModel.MediaType}, CinemaHub.Data.Models, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
                var type = Type.GetType(qualifiedName);
                media = (Media)Activator.CreateInstance(type);
            }

            media.Title = inputModel.Title;
            media.Overview = inputModel.Overview;
            media.IsDetailFull = true;
            media.Budget = inputModel.Budget;
            media.Language = inputModel.Language;
            media.Runtime = inputModel.Runtime;
            media.YoutubeTrailerUrl = inputModel.YoutubeTrailerUrl;
            media.ReleaseDate = inputModel.ReleaseDate;

            var genres = inputModel.Genres.Split(", ").ToList();
            var genresDb = media.Genres.ToList();

            // Remove all genres which are not in the inputModel's genres and ignore all which are
            foreach (var genre in genresDb)
            {
                if (!genres.Contains(genre.Genre.Name))
                {
                    media.Genres.Remove(genre);
                    continue;
                }
                genres.Remove(genre.Genre.Name);
            }

            // Add all the remaining genres
            foreach (var genre in genres)
            {
                genresDb.Add(new MediaGenre()
                {
                    Media = media,
                    Genre = this.genreRepository.All().Where(x => x.Name == genre).FirstOrDefault(),
                });
            }

            var keywords = Newtonsoft.Json.JsonConvert.DeserializeObject<List<KeywordDTO>>(inputModel.Keywords);
            var dbKeywordsIds = media.Keywords.Select(x => x.Id).ToList();

            // Create all keywords which do not exist then add every keyword from the inputModel
            foreach (var keyword in keywords)
            {
                var keywordDb = this.keywordRepository.All().FirstOrDefault(x => x.Id == keyword.Id);

                if (keywordDb is null)
                {
                    keywordDb = new Keyword() { Name = keyword.Value };
                    await this.keywordRepository.AddAsync(keywordDb);
                }

                if (!dbKeywordsIds.Contains(keyword.Id))
                {
                    media.Keywords.Add(new MediaKeyword()
                    {
                        Keyword = keywordDb,
                        Media = media,
                    });
                }

                dbKeywordsIds.Remove(keyword.Id);
            }

            // Delete all the keywords which are not in the inputModel
            foreach (var id in dbKeywordsIds)
            {
                var keyword = media.Keywords.FirstOrDefault(x => x.KeywordId == id);
                media.Keywords.Remove(keyword);
            }

            await this.mediaRepository.SaveChangesAsync();
        }

        public async Task ApplyQueryAsync(MediaQueryDTO query)
        {
            if (query.MediaType == nameof(Movie))
            {
                this.mediaQuery = this.mediaQuery.OfType<Movie>();
            }
            else if (query.MediaType == nameof(Show))
            {
                this.mediaQuery = this.mediaQuery.OfType<Show>();
            }

            this.mediaQuery = this.mediaQuery.Where(x => x.Title.Contains(query.SearchQuery));
            this.ResultsFound = this.mediaQuery.Count();
        }

        private void SortBy(SortTypeEnum sortType)
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

        private void SortByDescending(SortTypeEnum sortType)
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

    }
}
