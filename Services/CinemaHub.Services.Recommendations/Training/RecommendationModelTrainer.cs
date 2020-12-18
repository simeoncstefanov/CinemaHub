namespace CinemaHub.Services.Recommendations.Training
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    using Microsoft.ML;
    using Microsoft.Data.SqlClient;
    using Microsoft.Extensions.Configuration;
    using Microsoft.ML.Data;
    using CinemaHub.Services.Recommendations.Training.Models;
    using Microsoft.ML.Trainers;

    public class RecommendationModelTrainer : IRecommendationModelTrainer
    {
        private readonly MLContext mlContext;
        private readonly IConfiguration configuration;
        public RecommendationModelTrainer(IConfiguration configuration)
        {
            this.configuration = configuration;
            this.mlContext = new MLContext();
        }

        public async Task<(IDataView training, IDataView testing)> LoadData()
        {
            DatabaseLoader loader = mlContext.Data.CreateDatabaseLoader<MovieRating>();

            string connectionString = this.configuration.GetConnectionString("DefaultConnection") + ";MultipleActiveResultSets=true";

            string sqlCommand = "SELECT [r].[MediaId] AS movieId, [r].[CreatorId] AS userId, CAST([r].[Score] AS REAL) AS Label FROM Rating AS r";

            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, sqlCommand);

            IDataView data = loader.Load(dbSource);

            var preview = data.Preview();
            DataOperationsCatalog.TrainTestData dataSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            return (dataSplit.TrainSet, dataSplit.TestSet);
        }

        public async Task<ITransformer> BuildAndTrainModel(IDataView trainingDataView)
        {
            IEstimator<ITransformer> estimator = mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "userIdEncoded", inputColumnName: "userId")
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "movieIdEncoded", inputColumnName: "movieId"));

            var options = new MatrixFactorizationTrainer.Options
            {
                MatrixColumnIndexColumnName = "userIdEncoded",
                MatrixRowIndexColumnName = "movieIdEncoded",
                LabelColumnName = "Label",
                NumberOfIterations = 20,
                ApproximationRank = 100
            };

            var trainerEstimator = estimator.Append(mlContext.Recommendation().Trainers.MatrixFactorization(options));

            Console.WriteLine("=============== Training the model ===============");
            ITransformer model = trainerEstimator.Fit(trainingDataView);

            return model;
        }

        public async Task EvaluateModel(IDataView testDataView, ITransformer model)
        {
            Console.WriteLine("=============== Evaluating the model ===============");
            var prediction = model.Transform(testDataView);

            var metrics = mlContext.Regression.Evaluate(prediction, labelColumnName: "Label", scoreColumnName: "Score");
            Console.WriteLine("Root Mean Squared Error : " + metrics.RootMeanSquaredError.ToString());
            Console.WriteLine("RSquared: " + metrics.RSquared.ToString());
        }

        public async Task SaveModel(IDataView dataView, ITransformer model, string path)
        {
            mlContext.Model.Save(model, dataView.Schema, path + "recommendation_model.zip");
        }
    }
}
