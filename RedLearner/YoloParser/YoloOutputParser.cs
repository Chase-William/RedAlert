using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace RedLearner.YoloParser
{
    public class YoloOutputParser
    {
        class CellDimensions : DimensionsBase { }

        /// <summary>
        /// The number of rows the target image is diveded into.
        /// </summary>
        public const int ROW_COUNT = 13;
        /// <summary>
        /// The number of columns the target image is diveded into.
        /// </summary>
        public const int COL_COUNT = 13;
        /// <summary>
        /// Total number of values stored in one cell of the grid.<br/>
        /// 125 == {<br/>
        ///     x - position of the bounding box center relative to the grid cell it's associated with<br/>
        ///     y - position of the bounding box center relative to the grid cell it's associated with<br/>
        ///     w - width of the bounding box<br/>
        ///     h - height of the bounding box<br/>
        ///     o - confidence value that an object exist within the bounding box, also known as objectness score<br/>
        ///     p1 - p20 - class probabilities for each of the 20 classes predicted by the model<br/>
        ///     <br/>
        ///     Therefore the 25 elements describing each of the 5 bounding boxes equates to 125 elements (5 * 25).<br/>
        /// }
        /// </summary>
        public const int CHANNEL_COUNT = 125;
        /// <summary>
        /// Number of bounding boxes per cell.
        /// </summary>
        public const int BOXES_PER_CELL = 5;
        /// <summary>
        /// Number of features contained within a box.<br/>
        /// Features - x, y, w, h, (p1 - p20)
        /// </summary>
        public const int BOX_INFO_FEATURE_COUNT = 5;
        /// <summary>
        /// Number of class predictions contained in each bounding box.
        /// </summary>
        public const int CLASS_COUNT = 20;
        /// <summary>
        /// Width of a cell in the main 13x13 image grid. (pixels)
        /// </summary>
        public const float CELL_WIDTH = 32;
        /// <summary>
        /// Height of a cell in the main 13x13 image grid. (pixels)
        /// </summary>
        public const float CELL_HEIGHT = 32;
        /// <summary>
        /// Starting position of the current cell in the grid.
        /// </summary>
        private int channelStride = ROW_COUNT * COL_COUNT;
        /// <summary>
        /// Pre-defined height and width ratios of bounding boxes.
        /// </summary>
        private float[] anchors = new float[] { 1.08F, 1.19F, 3.42F, 4.41F, 6.63F, 11.38F, 9.42F, 5.11F, 16.62F, 10.52F };
        /// <summary>
        /// Labels/classes that the model will predict, which is a subset of the totlal number of classes predicted by the original YOLOv2 model.
        /// </summary>
        private string[] labels = new string[]
        {
            "aeroplane", "bicycle", "bird", "boat", "bottle",
            "bus", "car", "cat", "chair", "cow",
            "diningtable", "dog", "horse", "motorbike", "person",
            "pottedplant", "sheep", "sofa", "train", "tvmonitor"
        };
        /// <summary>
        /// Colors that are associated with each label/class.
        /// </summary>
        private static Color[] classColors = new Color[]
        {
            Color.Khaki,
            Color.Fuchsia,
            Color.Silver,
            Color.RoyalBlue,
            Color.Green,
            Color.DarkOrange,
            Color.Purple,
            Color.Gold,
            Color.Red,
            Color.Aquamarine,
            Color.Lime,
            Color.AliceBlue,
            Color.Sienna,
            Color.Orchid,
            Color.Tan,
            Color.LightPink,
            Color.Yellow,
            Color.HotPink,
            Color.OliveDrab,
            Color.SandyBrown,
            Color.DarkTurquoise
        };

        /// <summary>
        /// Applies the sigmoid function that oututs a number between 0 and 1.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Value from o to 1</returns>
        private float Sigmoid(float value)
        {
            var k = (float)Math.Exp(value);
            return k / (1.0f + k);
        }

        /// <summary>
        /// Normalizes an input vector into a probability distribution.
        /// </summary>
        /// <param name="values"></param>
        /// <returns>Normalized probability solution</returns>
        private float[] Softmax(float[] values)
        {
            var maxVal = values.Max();
            var exp = values.Select(v => Math.Exp(v - maxVal));
            var sumExp = exp.Sum();

            return exp.Select(v => (float)(v / sumExp)).ToArray();
        }

        /// <summary>
        /// Maps elemeents in the one-dimensional model output to the corresponding position in a 125 x 13 x 13 tensor.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private int GetOffset(int x, int y, int channel)
        {
            // YOLO outputs a tensor that has a shape of 125x13x13, which 
            // WinML flattens into a 1D array.  To access a specific channel 
            // for a given (x,y) cell position, we need to calculate an offset
            // into the array
            return (channel * this.channelStride) + (y * COL_COUNT) + x;
        }

        /// <summary>
        /// Extracts the bounding box dimensions using the GetOffset method from the model output.
        /// </summary>
        /// <param name="modelOutput"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private BoundingBoxDimensions ExtractBoundingBoxDimensions(float[] modelOutput, int x, int y, int channel)
        {
            return new BoundingBoxDimensions
            {
                X = modelOutput[GetOffset(x, y, channel)],
                Y = modelOutput[GetOffset(x, y, channel + 1)],
                Width = modelOutput[GetOffset(x, y, channel + 2)],
                Height = modelOutput[GetOffset(x, y, channel + 3)]
            };
        }

        /// <summary>
        /// Extracts the confidence value and uses the sigmoid function to turn it into a percentage.
        /// </summary>
        /// <param name="modelOutput"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        private float GetConfidence(float[] modelOutput, int x, int y, int channel)
        {
            return Sigmoid(modelOutput[GetOffset(x, y, channel + 4)]);
        }

        /// <summary>
        /// Uses the bounding box dimensions and maps them onto its respective cell within the image.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="box"></param>
        /// <param name="boxDimensions"></param>
        /// <returns></returns>
        private CellDimensions MapBoundingBoxToCell(int x, int y, int box, BoundingBoxDimensions boxDimensions)
        {
            return new CellDimensions
            {
                X = ((float)x + Sigmoid(boxDimensions.X)) * CELL_WIDTH,
                Y = ((float)y + Sigmoid(boxDimensions.Y)) * CELL_HEIGHT,
                Width = (float)Math.Exp(boxDimensions.Width) * CELL_WIDTH * anchors[box * 2],
                Height = (float)Math.Exp(boxDimensions.Height) * CELL_HEIGHT * anchors[box * 2 + 1],
            };
        }

        /// <summary>
        /// Extracts the class predictions for the bounding box from the model output using the <see cref="GetOffset(int, int, int)"/>
        /// method and turns them into a probability distribution using Softmax method.
        /// </summary>
        /// <param name="modelOutput"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        public float[] ExtractClasses(float[] modelOutput, int x, int y, int channel)
        {
            float[] predictedClasses = new float[CLASS_COUNT];
            int predictedClassOffset = channel + BOX_INFO_FEATURE_COUNT;
            for (int predictedClass = 0; predictedClass < CLASS_COUNT; predictedClass++)
            {
                predictedClasses[predictedClass] = modelOutput[GetOffset(x, y, predictedClass + predictedClassOffset)];
            }
            return Softmax(predictedClasses);
        }

        /// <summary>
        /// Selects the clas from list of predicted classes with the highest probability.
        /// </summary>
        /// <param name="predictedClasses"></param>
        /// <returns></returns>
        private ValueTuple<int, float> GetTopResult(float[] predictedClasses)
        {
            return predictedClasses
                .Select((predictedClass, index) => (Index: index, Value: predictedClass))
                .OrderByDescending(result => result.Value)
                .First();
        }

        /// <summary>
        /// Filters overlapping bounding boxes with lower probabilities.
        /// </summary>
        /// <param name="boundingBoxA"></param>
        /// <param name="boundingBoxB"></param>
        /// <returns></returns>     
        private float IntersectionOverUnion(RectangleF boundingBoxA, RectangleF boundingBoxB)
        {
            var areaA = boundingBoxA.Width * boundingBoxA.Height;

            if (areaA <= 0)
                return 0;

            var areaB = boundingBoxB.Width * boundingBoxB.Height;

            if (areaB <= 0)
                return 0;

            var minX = Math.Max(boundingBoxA.Left, boundingBoxB.Left);
            var minY = Math.Max(boundingBoxA.Top, boundingBoxB.Top);
            var maxX = Math.Min(boundingBoxA.Right, boundingBoxB.Right);
            var maxY = Math.Min(boundingBoxA.Bottom, boundingBoxB.Bottom);

            var intersectionArea = Math.Max(maxY - minY, 0) * Math.Max(maxX - minX, 0);

            return intersectionArea / (areaA + areaB - intersectionArea);
        }

        /// <summary>
        /// Extracts all highly confident bounding boxes from the model output.<br/>
        /// Additional filtering needs to be done to remove overlapping images.
        /// </summary>
        /// <param name="yoloModelOutputs"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public IList<YoloBoundingBox> ParseOutputs(float[] yoloModelOutputs, float threshold = .3F)
        {
            var boxes = new List<YoloBoundingBox>();

            for (int row = 0; row < ROW_COUNT; row++)
            {
                for (int column = 0; column < COL_COUNT; column++)
                {
                    for (int box = 0; box < BOXES_PER_CELL; box++)
                    {
                        // Calculating the starting position within the one-dimensional model output.
                        var channel = (box * (CLASS_COUNT + BOX_INFO_FEATURE_COUNT));
                        // Get the dimensions of the current bounding box.
                        BoundingBoxDimensions boundingBoxDimensions = ExtractBoundingBoxDimensions(yoloModelOutputs, row, column, channel);
                        // Get the confidence for the current bounding box
                        float confidence = GetConfidence(yoloModelOutputs, row, column, channel);
                        // Mapping the current bouding box to the current cell being processed
                        CellDimensions mappedBoundingBox = MapBoundingBoxToCell(row, column, box, boundingBoxDimensions);

                        // If the confidence is below the threshold, process the next bounding box
                        if (confidence < threshold)
                            continue;

                        // Get the probability distribution of the predicted classes for the current bounding box
                        float[] predictedClasses = ExtractClasses(yoloModelOutputs, row, column, channel);
                        // Get the value and index of the class with the highest probability for the current box and compute score
                        var (topResultIndex, topResultScore) = GetTopResult(predictedClasses);
                        var topScore = topResultScore * confidence;

                        // Keep only the bounding boxes that are above the current threshold
                        if (topScore < threshold)
                            continue;

                        // If the current bounding box exceeds the threshold, add it to the boxes list
                        boxes.Add(new YoloBoundingBox()
                        {
                            Dimensions = new BoundingBoxDimensions
                            {
                                X = (mappedBoundingBox.X - mappedBoundingBox.Width / 2),
                                Y = (mappedBoundingBox.Y - mappedBoundingBox.Height / 2),
                                Width = mappedBoundingBox.Width,
                                Height = mappedBoundingBox.Height,
                            },
                            Confidence = topScore,
                            Label = labels[topResultIndex],
                            BoxColor = classColors[topResultIndex]
                        });                        
                    }
                }
            }
            // Return all highly confident bounding boxes
            return boxes;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="boxes"></param>
        /// <param name="limit"></param>
        /// <param name="threshold"></param>
        /// <returns></returns>
        public IList<YoloBoundingBox> FilterBoundingBoxes(IList<YoloBoundingBox> boxes, int limit, float threshold)
        {         
            // Active == true means ready for processing
            var activeCount = boxes.Count;
            // Stores whether or not the bounding box is active
            var isActiveBoxes = new bool[boxes.Count];

            // Set all bounding boxes to true by default
            for (int i = 0; i < isActiveBoxes.Length; i++)
                isActiveBoxes[i] = true;

            // Sort the list of bounding boxes in descending order based on their confidence
            var sortedBoxes = boxes.Select((b, i) => new { Box = b, Index = i })
                    .OrderByDescending(b => b.Box.Confidence)
                    .ToList();

            // Holds filtered results
            var results = new List<YoloBoundingBox>();

            // Proccess the bounding boxes
            for (int i = 0; i < boxes.Count; i++)
            {
                // Check whether the current bounding box can be processed
                if (isActiveBoxes[i])
                {
                    // If so add it to the list of results
                    var boxA = sortedBoxes[i].Box;
                    results.Add(boxA);

                    if (results.Count >= limit)
                        break;

                    // Look at the adjacent bounding boxes
                    for (var j = i + 1; j < boxes.Count; j++)
                    {
                        // Check if the box is active
                        if (isActiveBoxes[j])
                        {
                            var boxB = sortedBoxes[j].Box;

                            // Check to see if the first box and second exceed the threshold
                            if (IntersectionOverUnion(boxA.Rect, boxB.Rect) > threshold)
                            {
                                isActiveBoxes[j] = false;
                                activeCount--;

                                if (activeCount <= 0)
                                    break;
                            }
                        }                        
                    }
                }

                // If no more bounding boxes to process, break out
                if (activeCount <= 0)
                    break;
            }

            return results;
        }
    }
}
