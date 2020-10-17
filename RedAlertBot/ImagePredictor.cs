using RedLearner;
using System;
using System.Collections.Generic;
using System.Text;

namespace RedAlertBot
{
    /// <summary>
    /// Manages the interaction between the <see cref="ImageRecorder"/> and <see cref="RedLearner.RedLearner"/>.
    /// </summary>
    public class ImagePredictor
    {   
        public event EventHandler<UpdateTickEventArgs> UpdateTick;

        /// <summary>
        /// Learner that classifies images.
        /// </summary>
        private static RedLearner.RedLearner Learner { get; set; } = new RedLearner.RedLearner();

        private PredictionResult lastPrediction;
        public PredictionResult LastPrediction
        {
            get => lastPrediction;
            set
            {                                
                lastPrediction = value;
                UpdateTick?.Invoke(this, new UpdateTickEventArgs(value));
            }
        }

        public void PredictImage(string path) => LastPrediction = Learner.ClassifySingleImage(path);
    }

    public class UpdateTickEventArgs : EventArgs
    {
        public PredictionResult Value { get; }

        private UpdateTickEventArgs() { }
        public UpdateTickEventArgs(PredictionResult _value)
        {
            Value = _value;
        }
    }      
}
