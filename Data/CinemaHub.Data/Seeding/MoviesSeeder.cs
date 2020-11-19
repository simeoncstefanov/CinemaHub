namespace CinemaHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Runtime.CompilerServices;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;
    using CinemaHub.Data.Seeding.Models;
    using Microsoft.Extensions.Configuration;

    public class MoviesSeeder : ISeeder
    {
        private const int Pages = 3;
        private readonly HttpClient client;
        private readonly string rootPath;

        public MoviesSeeder(string rootPath)
        {
            this.client = new HttpClient();
            this.rootPath = rootPath;
        }

        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider, string rootPath)
        {
            if (dbContext.Movies.Any())
            {
                return;
            }

            for (int i = 1; i <= Pages; i++)
            {
                var result = await this.GetMediaJsonAsync(i);

                foreach (var movie in result.Movies)
                {
                    Movie dbMovie = new Movie()
                    {
                        Title = movie.Title,
                        Overview = movie.Overview,
                        MovieApiId = movie.MovieApiId,
                    };

                    if (!DateTime.TryParse(movie.ReleaseDate, out DateTime movieReleaseDate))
                    {
                        dbMovie = null;
                    }
                    else
                    {
                        dbMovie.ReleaseDate = movieReleaseDate;
                    }

                    var genres = dbContext.Genres.Where(x => movie.GenresIds.Contains(x.ApiId));

                    foreach (var genre in genres)
                    {
                        dbMovie.Genres.Add(new MediaGenre()
                        {
                            Genre = genre,
                            Media = dbMovie,
                        });
                    }

                    await dbContext.Movies.AddAsync(dbMovie);

                    var imagePath = this.DownloadImage("https://image.tmdb.org/t/p/w500/" + movie.PosterPath, dbMovie.Id).Result;

                    MediaImage image = new MediaImage()
                    {
                        Media = dbMovie,
                        ImageType = Data.Models.Enums.ImageType.Poster,
                        Path = imagePath,
                        Extension = System.IO.Path.GetExtension(imagePath),
                        Title = dbMovie.Title + " - Poster",
                    };

                    await dbContext.MediaImages.AddAsync(image);
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        public async Task<ResultDTO> GetMediaJsonAsync(int page)
        {
            var content = await this.client.GetStringAsync($"https://api.themoviedb.org/3/movie/popular?api_key=238b937765146aa0e189640d869591e7&language=en-US&page={page}");
            var result = Newtonsoft.Json.JsonConvert.DeserializeObject<ResultDTO>(content);
            return result;
        }

        private async Task<string> DownloadImage(string url, string movieGuid)
        {
            var response = await this.client.GetAsync(url);
            var imagePath = "\\images\\posters\\" +
                $"poster-{movieGuid}{System.IO.Path.GetExtension(url)}";

            using var fileStream = new FileStream(this.rootPath + imagePath, FileMode.CreateNew);
            await response.Content.CopyToAsync(fileStream);

            return imagePath;
        }
    }
}
