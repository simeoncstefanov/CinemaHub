namespace CinemaHub.Services.Recommendation.Trainers
{
    using System;
    using System.Data.SqlClient;

    using CinemaHub.Services.Recommendation.Models;

    using Microsoft.ML;
    using Microsoft.ML.Data;
    using Microsoft.ML.Trainers;

    public class KeywordContentTrainer
    {
        private readonly MLContext mlContext;
        private readonly PredictionEngine<MediaKeywordModel, MediaKeywordPredictionModel> predEngine;
        private readonly ITransformer trainedModel;
        private readonly IDataView trainingDataView;

        public KeywordContentTrainer()
        {
            this.mlContext = new MLContext(seed: 0);
        }

        public void LoadDataFromDatabase()
        {
            DatabaseLoader loader = this.mlContext.Data.CreateDatabaseLoader<MediaKeywordModel>();

            string connectionString = "Server=.;Database=CinemaHub;Trusted_Connection=True;MultipleActiveResultSets=true";
            string queryForData = "SELECT r.CreatorId AS UserId, IIF(r.Score > 6, CAST(1 AS BIT), CAST(0 AS BIT)) AS IsLiked, string_agg(k.Name, ', ') AS Keywords FROM Rating AS r INNER JOIN MediaKeywords AS mk ON mk.MediaId = r.MediaId INNER JOIN Keywords AS k ON k.Id = mk.KeywordId GROUP BY r.MediaId, r.CreatorId, r.Score";
            DatabaseSource dbSource = new DatabaseSource(SqlClientFactory.Instance, connectionString, queryForData);

            IDataView data = loader.Load(dbSource);

            DataOperationsCatalog.TrainTestData trainTestSplit = mlContext.Data.TrainTestSplit(data, testFraction: 0.2);
            IDataView trainingData = trainTestSplit.TrainSet;
            IDataView testData = trainTestSplit.TestSet;

            var preview1 = trainingData.Preview();
            var preview = testData.Preview();

            var model = this.BuildAndTrainModel(this.mlContext, trainingData);

            this.Evaluate(this.mlContext, model, testData);

            var sampleData = new MediaKeywordModel()
                                 {
                                     Keywords = "wegfwegweg",
                                     UserId = "p[",
                                 };

            var sampleData1 = new MediaKeywordModel()
                                 {
                                     Keywords = "sequel, aftercreditsstinger, superhero",
                                     UserId = "eb28dc2e-182c-4040-9797-295d21c00681",
                                 };

            this.UseModelWithSingleItem(this.mlContext, model, sampleData);
            this.UseModelWithSingleItem(this.mlContext, model, sampleData1);
        }

        public ITransformer BuildAndTrainModel(MLContext mlContext, IDataView trainingData)
        {
            var estimator = mlContext.Transforms.Text
                .FeaturizeText(outputColumnName: "Content_k", inputColumnName: nameof(MediaKeywordModel.Keywords))
                .Append(mlContext.Transforms.CopyColumns("KeywordsFeaturized", "Content_k"))
                .Append(mlContext.Transforms.NormalizeMinMax("KeywordsFeaturized", "KeywordsFeaturized"))
                .Append(
                    mlContext.Transforms.Text.FeaturizeText(
                        inputColumnName: nameof(MediaKeywordModel.UserId),
                        outputColumnName: "UserFeaturized"))
                .Append(
                    mlContext.Transforms.Concatenate("Features", "UserFeaturized", "KeywordsFeaturized"));



            var trainer = mlContext.BinaryClassification.Trainers.SdcaLogisticRegression(
                new SdcaLogisticRegressionBinaryTrainer.Options()
                    {
                        LabelColumnName = nameof(MediaKeywordModel.IsLiked),
                        FeatureColumnName = "Features",

                    });

            var trainingPipeline = estimator.Append(trainer);

            Console.WriteLine("=============== Create and Train the Model ===============");
            var model = trainingPipeline.Fit(trainingData);
            Console.WriteLine("=============== End of training ===============");
            return model;
        }

        public void Evaluate(MLContext mlContext, ITransformer model, IDataView splitTestSet)
        {
            Console.WriteLine("=============== Evaluating Model accuracy with Test data===============");
            IDataView predictions = model.Transform(splitTestSet);
            CalibratedBinaryClassificationMetrics metrics = mlContext.BinaryClassification.Evaluate(data: predictions, labelColumnName: nameof(MediaKeywordModel.IsLiked), scoreColumnName: "Score");

            Console.WriteLine();
            Console.WriteLine("Model quality metrics evaluation");
            Console.WriteLine("--------------------------------");
            Console.WriteLine($"Accuracy: {metrics.Accuracy:P2}");
            Console.WriteLine($"Auc: {metrics.AreaUnderRocCurve:P2}");
            Console.WriteLine($"F1Score: {metrics.F1Score:P2}");
            Console.WriteLine("=============== End of model evaluation ===============");
        }

        private void UseModelWithSingleItem(MLContext mlContext, ITransformer model, MediaKeywordModel sampleStatement)
        {
            PredictionEngine<MediaKeywordModel, MediaKeywordPredictionModel> predictionFunction = mlContext.Model.CreatePredictionEngine<MediaKeywordModel, MediaKeywordPredictionModel>(model);

            var resultPrediction = predictionFunction.Predict(sampleStatement);

            Console.WriteLine();
            Console.WriteLine("=============== Prediction Test of model with a single sample and test dataset ===============");

            Console.WriteLine();
            Console.WriteLine($"Sentiment: {resultPrediction.Keywords} | Prediction: {(Convert.ToBoolean(resultPrediction.Prediction) ? "Positive" : "Negative")}");

            Console.WriteLine("=============== End of Predictions ===============");
            Console.WriteLine();
        }
    }
}
