namespace CinemaHub.Services.Recommendation
{
    using System;

    using CinemaHub.Services.Recommendation.Models;

    using Microsoft.ML;
    using Microsoft.ML.Data;

    class Program
    {
        static void Main(string[] args)
        {
            MLContext mlContext = new MLContext();

            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<MediaKeywordModel>();

            string connectionString = "Server=.;Database=CinemaHub;Trusted_Connection=True;";

            string queryForData = "";
        }
    }
}
