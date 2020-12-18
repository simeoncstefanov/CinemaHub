using System;
using System.Collections.Generic;
using System.Text;

namespace CinemaHub.Services.Recommendations.Training.Models
{
    public class MovieRating
    {
        public string userId { get; set; }

        public string movieId { get; set; }

        public float Label { get; set; }
    }
}
