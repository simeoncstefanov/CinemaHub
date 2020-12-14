namespace CinemaHub.Web
{
    using System;
    using System.Reflection;
    using System.Threading.Tasks;

    using AutoMapper;
    using CinemaHub.Common;
    using CinemaHub.Data;
    using CinemaHub.Data.Common;
    using CinemaHub.Data.Common.Repositories;
    using CinemaHub.Data.Models;
    using CinemaHub.Data.Repositories;
    using CinemaHub.Data.Seeding;
    using CinemaHub.Services;
    using CinemaHub.Services.Data;
    using CinemaHub.Services.Data.Models;
    using CinemaHub.Services.Mapping;
    using CinemaHub.Services.Messaging;
    using CinemaHub.Services.Recommendations;
    using CinemaHub.Web.Authorization;
    using CinemaHub.Web.ViewModels;
    using CinemaHub.Web.ViewModels.Media;

    using ContactManager.Authorization;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Authorization;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(this.configuration.GetConnectionString("DefaultConnection")));

            services.AddDefaultIdentity<ApplicationUser>(IdentityOptionsProvider.GetIdentityOptions)
                .AddRoles<ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();

            services.Configure<CookiePolicyOptions>(
                options =>
                    {
                        options.CheckConsentNeeded = context => true;
                        options.MinimumSameSitePolicy = SameSiteMode.None;
                    });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
                {
                    options.Cookie.Name = "CinemaHub.Session";
                    options.IdleTimeout = TimeSpan.FromSeconds(10);
                    options.Cookie.IsEssential = true;
                });

            services.AddControllersWithViews(
                options =>
                    {
                        options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                        var policy = new AuthorizationPolicyBuilder()
                            .RequireAuthenticatedUser()
                            .Build();
                        options.Filters.Add(new AuthorizeFilter(policy));
                    })
                .AddRazorRuntimeCompilation()
                .AddSessionStateTempDataProvider();

            services.AddAntiforgery(options =>
                {
                    options.HeaderName = "X-CSRF-TOKEN";
                });

            services.AddRazorPages()
                .AddSessionStateTempDataProvider();

            services.AddSingleton(this.configuration);

            // Data repositories
            services.AddScoped(typeof(IDeletableEntityRepository<>), typeof(EfDeletableEntityRepository<>));
            services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));
            services.AddScoped<IDbQueryRunner, DbQueryRunner>();

            // Authorization Handlers
            services.AddScoped<IAuthorizationHandler, EntityIsCreatedByAuthorizationHandler>();

            services.AddSingleton<IAuthorizationHandler, EntityAdministratorAuthorizationHandler>();

            // Application services
            services.AddTransient<IMediaService, MediaService>();
            services.AddTransient<IMovieAPIService, MovieAPIService>();
            services.AddTransient<IEmailSender, SendGridEmailSender>(provider => new SendGridEmailSender(this.configuration.GetSection("SendGridAPIKey").Value));
            services.AddTransient<IKeywordService, KeywordService>();
            services.AddTransient<IReviewsService, ReviewsService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IDiscussionsService, DiscussionsService>();
            services.AddTransient<IRecommendService, RecommendService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            AutoMapperConfig.RegisterMappings(typeof(ErrorViewModel).GetTypeInfo().Assembly);
            AutoMapperConfig.RegisterMappings(typeof(MediaDetailsViewModel).GetTypeInfo().Assembly);

            // Seed data on application startup
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                dbContext.Database.Migrate();
                new ApplicationDbContextSeeder().SeedAsync(dbContext, serviceScope.ServiceProvider, env.WebRootPath).GetAwaiter().GetResult();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseEndpoints(
                endpoints =>
                    {
                        endpoints.MapAreaControllerRoute(
                            "Identity",
                            "Identity",
                            "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("areaRoute", "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
                        endpoints.MapRazorPages();
                    });
        }
    }
}
