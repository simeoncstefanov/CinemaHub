namespace MoiteRecepti.Web.Areas.Administration.Controllers
{
    using CinemaHub.Common;
    using CinemaHub.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdminController : BaseController
    {
    }
}