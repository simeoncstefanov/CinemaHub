namespace CinemaHub.Services
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    public interface IMediaApiCrossService
    {
        Task ScrapeShowsFromApi(int pages, string rootPath);

        Task ScrapeMoviesFromApi(int pages, string rootPath);
    }
}
