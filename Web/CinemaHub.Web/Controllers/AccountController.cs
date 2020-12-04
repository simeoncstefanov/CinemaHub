namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Web;

    using CinemaHub.Data.Models;
    using CinemaHub.Web.ViewModels.Account;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    using Microsoft.Extensions.Logging;

    public class AccountController : BaseController
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly ILogger<AccountController> logger;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            ILogger<AccountController> logger,
            UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.logger = logger;
        }

        [Route("/login")]
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return this.View("~/Views/Home/Index.cshtml");
        }

        [Route("/login")]
        [HttpPost]
        public async Task<IActionResult> Login(LoginInputModel model)
        {
            var returnUrl = model.ReturnUrl ?? this.Url.Content("~/");

            if (this.ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await this.signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded)
                {
                    this.logger.LogInformation("User logged in.");
                    return this.Redirect(this.Request.Headers["Referer"].ToString());
                }

                if (result.RequiresTwoFactor)
                {
                    return this.RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = model.RememberMe });
                }

                if (result.IsLockedOut)
                {
                    this.logger.LogWarning("User account locked out.");
                    return this.RedirectToPage("./Lockout");
                }
                else
                {
                    this.ModelState.AddModelError("InvalidLogin", "Invalid login attempt.");
                    return this.RedirectToAction("Login", "Account");
                }
            }

            // If we got this far, something failed, redisplay form
            return this.Redirect(this.Url.RouteUrl("login") + returnUrl);
        }
    }
}
