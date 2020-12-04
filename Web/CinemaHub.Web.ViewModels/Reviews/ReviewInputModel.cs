namespace CinemaHub.Web.ViewModels.Reviews
{
    using System.ComponentModel.DataAnnotations;

    public class ReviewInputModel
    {
        [Required]
        public string Title { get; set; }

        [Required]
        public string MediaId { get; set; }

        [Required]
        public string ReviewText { get; set; }

        public int CurrentUserRating { get; set; }
    }
}
