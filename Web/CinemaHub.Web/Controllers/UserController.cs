namespace CinemaHub.Web.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class UserController : BaseController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
