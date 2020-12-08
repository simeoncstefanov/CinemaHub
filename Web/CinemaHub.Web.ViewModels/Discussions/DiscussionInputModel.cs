namespace CinemaHub.Web.ViewModels.Discussions
{
    using System.ComponentModel.DataAnnotations;

    public class DiscussionInputModel
    {
        [Required]
        public string MediaId { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
