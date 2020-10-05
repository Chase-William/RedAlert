using System.Drawing;

namespace RedLearner.YoloParser
{
    public class BoundingBoxDimensions : DimensionsBase { }

    public class YoloBoundingBox
    {
        public BoundingBoxDimensions Dimensions { get; set; }

        /// <summary>
        /// The class of object detected within the bounding box.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Confidence of the class detected.
        /// </summary>
        public float Confidence { get; set; }

        public RectangleF Rect
        {
            get { return new RectangleF(Dimensions.X, Dimensions.Y, Dimensions.Width, Dimensions.Height); }
        }

        /// <summary>
        /// Contains the color associated with the respective class used to draw on the image.
        /// </summary>
        public Color BoxColor { get; set; }
    }
}
