namespace CinemaHub.Web.ViewModels.Reviews
{
    using System.ComponentModel.DataAnnotations;

    public class RatingInputModel
    {
        public string MediaId { get; set; }

        [Range(0, 10)]
        public byte Value { get; set; }
    }
}
