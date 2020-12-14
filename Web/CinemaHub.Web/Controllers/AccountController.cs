namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using CinemaHub.Data.Models;
    using CinemaHub.Services.Data;
    using CinemaHub.Web.Filters.Action;
    using CinemaHub.Web.Filters.Action.ModelStateTransfer;
    using CinemaHub.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;

    using Newtonsoft.Json;

    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;
        private readonly IUserService userService;
        private readonly IWebHostEnvironment webHostEnvironment;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager,
            IUserService userService,
            IWebHostEnvironment webHostEnvironment)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
            this.userService = userService;
            this.webHostEnvironment = webHostEnvironment;
        }

        [AllowAnonymous]
        [Route("/login")]
        [HttpGet]
        [ImportModelState]
        public IActionResult Login(string returnUrl = null)
        {
            return this.View("~/Views/Home/Index.cshtml");
        }

        [Route("/login")]
        [AllowAnonymous]
        [HttpPost]
        [ExportModelState]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var returnUrl = model.ReturnUrl ?? this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                var result = await this.signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");
                    return this.LocalRedirect(returnUrl);
                }

                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("Identity", "Account/LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");
                    return this.Redirect("/Identity/Account/Lockout");
                }
                else
                {
                    this.ModelState.AddModelError("InvalidLogin", "Invalid login attempt.");
                    this.ModelState.Remove("RememberMe");
                    return this.RedirectToAction("Login", "Account");
                }
            }

            return this.RedirectToAction("Login", "Account");
        }

        [HttpPost]
        public async Task<IActionResult> UploadImage(AvatarUploadInputModel inputModel)
        {
            var userId = this.userManager.GetUserId(this.User);

            try
            {
                await this.userService.ChangeAvatarAsync(inputModel.Image, this.webHostEnvironment.WebRootPath, userId);
            }
            catch (Exception ex)
            {
                this.ModelState.AddModelError(string.Empty, ex.Message);
            }

            return this.Redirect("/Identity/Account/Manage");
        }
    }
}
