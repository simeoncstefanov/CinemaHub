namespace CinemaHub.Web.ViewModels.Recommendations
{
    using System.Collections.Generic;

    public class RecommendationResponseViewModel
    {
        public IEnumerable<MediaRecommendationViewModel> Recommendations { get; set; }

        public int TotalRecommendations { get; set; }
    }
}
