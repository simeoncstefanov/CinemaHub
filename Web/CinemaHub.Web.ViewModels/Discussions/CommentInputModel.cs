namespace CinemaHub.Web.ViewModels.Discussions
{
    using System.ComponentModel.DataAnnotations;

    public class CommentInputModel
    {
        [Required]
        public string Content { get; set; }
    }
}
