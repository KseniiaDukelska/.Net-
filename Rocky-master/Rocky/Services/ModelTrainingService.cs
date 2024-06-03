using Microsoft.ML;
using Microsoft.ML.Data;
using Rocky_Models.Models;
using System.IO;

namespace Rocky.Services
{
    public class ModelTrainingService
    {
        private readonly MLContext _mlContext;

        public ModelTrainingService()
        {
            _mlContext = new MLContext();
        }

        public void TrainModel(string trainingDataPath, string modelPath)
        {
            var data = _mlContext.Data.LoadFromTextFile<UserInteraction>(
                path: trainingDataPath,
                hasHeader: true,
                separatorChar: ',');

            var pipeline = _mlContext.Transforms.Conversion.MapValueToKey(nameof(UserInteraction.UserId))
                .Append(_mlContext.Transforms.Conversion.MapValueToKey(nameof(UserInteraction.ProductId)))
                .Append(_mlContext.Recommendation().Trainers.MatrixFactorization(
                    nameof(MLModelOutput.Score),
                    nameof(UserInteraction.UserId),
                    nameof(UserInteraction.ProductId)));

            var model = pipeline.Fit(data);

            using (var fileStream = new FileStream(modelPath, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                _mlContext.Model.Save(model, data.Schema, fileStream);
            }
        }
    }
}
