namespace CinemaHub.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using CinemaHub.Common;
    using CinemaHub.Data.Models;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.DependencyInjection;

    public class UserSeeder : ISeeder
    {
        public async Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider, string rootPath)
        {
            var user = await this.CreateUser(serviceProvider, "CinemaAdmin", "admin1234"); // use secret manager for password

            if (user == null)
            {
                return;
            }

            await this.AddRole(serviceProvider, user, GlobalConstants.AdministratorRoleName);
        }

        private async Task<string> CreateUser(IServiceProvider serviceProvider, string username, string password)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    UserName = username,
                    EmailConfirmed = true,
                };
                await userManager.CreateAsync(user, password);
            }

            if (user == null)
            {
                throw new Exception("The password is probably not strong enough!");
            }

            return user.Id;
        }

        private async Task<IdentityResult> AddRole(IServiceProvider serviceProvider, string userId, string role)
        {
            IdentityResult result = null;
            var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            if (!await roleManager.RoleExistsAsync(role))
            {
                result = await roleManager.CreateAsync(new ApplicationRole(role));
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("The testUserPw password was probably not strong enough!");
            }

            result = await userManager.AddToRoleAsync(user, role);

            return result;
        }
    }
}
