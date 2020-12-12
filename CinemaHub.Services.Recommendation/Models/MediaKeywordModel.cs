namespace CinemaHub.Services.Recommendation.Models
{
    using Microsoft.ML.Data;

    public class MediaKeywordModel
    {
        public string UserId { get; set; }

        public string Keywords { get; set; }

        public bool IsLiked { get; set; }
    }
}
  