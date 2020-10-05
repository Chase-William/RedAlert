using Microsoft.ML.Data;

namespace RedLearner.DataStructures
{
    public class ImageNetPrediction
    {
        /// <summary>
        /// Contains the dimensions, objectness score, and class probabilities for each bounding box detected in an image.
        /// </summary>
        [ColumnName("grid")]
        public float[] PredictedLabels;
    }
}
