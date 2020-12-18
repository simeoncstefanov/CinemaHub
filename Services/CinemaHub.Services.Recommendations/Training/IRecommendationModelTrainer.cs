using Microsoft.ML;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace CinemaHub.Services.Recommendations.Training
{
    public interface IRecommendationModelTrainer
    {
        Task<(IDataView training, IDataView testing)> LoadData();

        Task<ITransformer> BuildAndTrainModel(IDataView trainingDataView);

        Task EvaluateModel(IDataView testDataView, ITransformer model);

        Task SaveModel(IDataView dataView, ITransformer model, string path);
    }
}
