namespace CinemaHub.Web.ViewComponents
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using CinemaHub.Services.Data;
    using CinemaHub.Web.ViewModels.Media;
    using Microsoft.AspNetCore.Mvc;

    public class MediaGridViewComponent : ViewComponent
    {
        public MediaGridViewComponent()
        {
        }

        public async Task<IViewComponentResult> InvokeAsync(MediaGridViewModel model)
        {
            return this.View(model);
        }
    }
}
