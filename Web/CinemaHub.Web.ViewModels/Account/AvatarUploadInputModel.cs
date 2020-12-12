namespace CinemaHub.Web.ViewModels.Account
{
    using Microsoft.AspNetCore.Http;

    public class AvatarUploadInputModel
    {
        public IFormFile Image { get; set; }
    }
}
