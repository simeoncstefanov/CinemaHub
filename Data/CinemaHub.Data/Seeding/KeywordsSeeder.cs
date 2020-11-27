namespace CinemaHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using CinemaHub.Data.Models;

    public class KeywordsSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider, string rootPath)
        {
            if (dbContext.Keywords.Any())
            {
                return;
            }

            dbContext.Keywords.Add(new Keyword() { Name = "time travel" });
            dbContext.Keywords.Add(new Keyword() { Name = "patriotic" });
            dbContext.Keywords.Add(new Keyword() { Name = "anime" });
            dbContext.Keywords.Add(new Keyword() { Name = "friends with benefits" });
        }
    }
}
