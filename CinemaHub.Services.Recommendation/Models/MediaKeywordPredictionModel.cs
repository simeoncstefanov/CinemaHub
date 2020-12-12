namespace CinemaHub.Services.Recommendation.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.ML.Data;

    public class MediaKeywordPredictionModel : MediaKeywordModel
    {
        [ColumnName("PredictedLabel")]
        public bool Prediction { get; set; }
    }
}
