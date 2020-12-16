namespace CinemaHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using Microsoft.Extensions.DependencyInjection;

    public class MediaApiCrossService : IMediaApiCrossService
    {
        private readonly IServiceProvider serviceProvider;

        public MediaApiCrossService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task ScrapeShowsFromApi(int pages, string rootPath)
        {
            var mediaApiService = this.serviceProvider.GetRequiredService<IMovieAPIService>();
            var mediaService = this.serviceProvider.GetRequiredService<IMediaService>();

            for (int i = 1; i <= pages; i++)
            {
                var ids = await mediaApiService.GetShowsIds(i);

                foreach (var id in ids)
                {
                    var inputModel = await mediaApiService.GetDetailsFromApiAsync(id, "Show");
                    if (inputModel == null)
                    {
                        continue;
                    }

                    try
                    {
                        await mediaService.EditDetailsAsync(inputModel, string.Empty, rootPath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }

        public async Task ScrapeMoviesFromApi(int pages,  string rootPath)
        {
            var mediaApiService = this.serviceProvider.GetRequiredService<IMovieAPIService>();
            var mediaService = this.serviceProvider.GetRequiredService<IMediaService>();

            for (int i = 1; i <= pages; i++)
            {
                var ids = await mediaApiService.GetMoviesIds(i);

                foreach (var id in ids)
                {
                    var inputModel = await mediaApiService.GetDetailsFromApiAsync(id, "Movie");
                    if (inputModel == null)
                    {
                        continue;
                    }

                    try
                    {
                        await mediaService.EditDetailsAsync(inputModel, string.Empty, rootPath);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                }
            }
        }
    }
}
