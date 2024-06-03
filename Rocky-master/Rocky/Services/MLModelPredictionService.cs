using Microsoft.Extensions.ML;
using Rocky_Models.Models;

namespace Rocky.Services
{
    public class MLModelPredictionService
    {
        private readonly PredictionEnginePool<MLModelInput, MLModelOutput> _predictionEnginePool;

        public MLModelPredictionService(PredictionEnginePool<MLModelInput, MLModelOutput> predictionEnginePool)
        {
            _predictionEnginePool = predictionEnginePool;
        }

        public MLModelOutput Predict(MLModelInput input)
        {
            return _predictionEnginePool.Predict(input);
        }
    }
}
