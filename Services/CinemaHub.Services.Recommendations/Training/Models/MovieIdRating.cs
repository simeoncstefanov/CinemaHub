namespace CinemaHub.Services.Recommendations.Training.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    public class MovieIdRating
    {
        public string MediaId { get; set; }

        public float Score { get; set; }
    }
}
