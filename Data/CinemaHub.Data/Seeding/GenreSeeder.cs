namespace CinemaHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Common;
    using CinemaHub.Data.Models;

    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;

    internal class GenreSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider, string rootPath)
        {
            if (dbContext.Genres.Any())
            {
                return;
            }

            dbContext.Genres.Add(new Genre() { Name = "Action", ApiId = 28 });
            dbContext.Genres.Add(new Genre() { Name = "Adventure", ApiId = 12});
            dbContext.Genres.Add(new Genre() { Name = "Animation", ApiId = 16});
            dbContext.Genres.Add(new Genre() { Name = "Comedy", ApiId = 35 });
            dbContext.Genres.Add(new Genre() { Name = "Crime", ApiId = 80 });
            dbContext.Genres.Add(new Genre() { Name = "Documentary", ApiId = 99 });
            dbContext.Genres.Add(new Genre() { Name = "Drama", ApiId = 18 });
            dbContext.Genres.Add(new Genre() { Name = "Family", ApiId = 10751 });
            dbContext.Genres.Add(new Genre() { Name = "Fantasy", ApiId = 14 });
            dbContext.Genres.Add(new Genre() { Name = "History", ApiId = 36 });
            dbContext.Genres.Add(new Genre() { Name = "Horror", ApiId = 27 });
            dbContext.Genres.Add(new Genre() { Name = "Music", ApiId = 10402 });
            dbContext.Genres.Add(new Genre() { Name = "Mystery", ApiId = 9648 });
            dbContext.Genres.Add(new Genre() { Name = "Romance", ApiId = 10749 });
            dbContext.Genres.Add(new Genre() { Name = "Science Fiction", ApiId = 878 });
            dbContext.Genres.Add(new Genre() { Name = "TV Movie", ApiId = 10770 });
            dbContext.Genres.Add(new Genre() { Name = "Thriller", ApiId = 53 });
            dbContext.Genres.Add(new Genre() { Name = "War", ApiId = 10752 });
            dbContext.Genres.Add(new Genre() { Name = "Western", ApiId = 37 });
        }
    }
}
