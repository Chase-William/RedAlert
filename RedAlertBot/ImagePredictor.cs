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
        /// <summary>
        /// Learner that classifies images.
        /// </summary>
        private static RedLearner.RedLearner Learner { get; set; } = new RedLearner.RedLearner();
       
        public void PredictImage(string path)
        {
            Console.WriteLine(Learner.ClassifySingleImage(path));
        }
    }
}
