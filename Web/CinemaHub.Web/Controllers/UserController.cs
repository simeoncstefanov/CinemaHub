namespace CinemaHub.Web.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    public class UserController : BaseController
    {
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Index(IFormFile imageFile)
        {
            return this.BadRequest();
        }
    }
}
