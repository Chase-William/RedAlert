using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace RedLearner
{
    public class RedLearner
    {        

        static readonly string _assetsPath = Path.Combine(Environment.CurrentDirectory, "assets");
        static readonly string _imagesFolder = Path.Combine(_assetsPath, "images_regression");
        static readonly string _trainTagsTsv = Path.Combine(_imagesFolder, "tags.tsv");
        //static readonly string _testTagsTsv = Path.Combine(_imagesFolder, "test-tags.tsv");
        static readonly string _inceptionTensorFlowModel = Path.Combine(_assetsPath, "inception", "tensorflow_inception_graph.pb");

        private MLContext mlContext;
        private ITransformer model;

        public RedLearner()
        {
            mlContext = new MLContext();
            model = GenerateModel(mlContext);            
        }

        private struct InceptionSettings
        {
            public const int ImageHeight = 224;
            public const int ImageWidth = 224;
            public const float Mean = 117;
            public const float Scale = 1;
            public const bool ChannelsLast = true;
        }

        /// <summary>
        /// Displays the results of the predictions
        /// </summary>
        /// <param name="imagePredictionData"></param>
        private void DisplayResults(IEnumerable<ImagePrediction> imagePredictionData)
        {
            foreach (ImagePrediction prediction in imagePredictionData)
            {
                Console.WriteLine($"Image: {Path.GetFileName(prediction.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
            }
        }

        /// <summary>
        /// Reads from tsv to get the <see cref="ImageData.Label" value></see>
        /// </summary>
        private IEnumerable<ImageData> ReadFromTsv(string file, string folder)
        {
            return File.ReadAllLines(file)
                .Select(line => line.Split('\t'))
                .Select(line => new ImageData()
                {
                    ImagePath = Path.Combine(folder, line[0])
                });

        }

        /// <summary>
        /// Classify a single image using the model that was created
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        //public string ClassifySingleImage(string path)
        //{
        //    var imageData = new ImageData()
        //    {
        //        ImagePath = path
        //    };

        //    try
        //    {
        //        // Make prediction function (input = ImageData, output = ImagePrediction)
        //        var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
        //        var prediction = predictor.Predict(imageData);
        //        Console.ForegroundColor = ConsoleColor.Green;
        //        Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
        //        Console.ForegroundColor = ConsoleColor.White;
        //        return prediction.PredictedLabelValue;
        //    }            
        //    catch (Exception ex)
        //    {
        //        throw new Exception(ex.Message);
        //    }
        //}

        public PredictionResult ClassifySingleImage(string path)
        {
            var imageData = new ImageData()
            {
                ImagePath = path
            };

            try
            {
                // Make prediction function (input = ImageData, output = ImagePrediction)
                var predictor = mlContext.Model.CreatePredictionEngine<ImageData, ImagePrediction>(model);
                var prediction = predictor.Predict(imageData);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"Image: {Path.GetFileName(imageData.ImagePath)} predicted as: {prediction.PredictedLabelValue} with score: {prediction.Score.Max()} ");
                Console.ForegroundColor = ConsoleColor.White;
                return new PredictionResult(prediction.PredictedLabelValue, prediction.Score.Max());
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        private ITransformer GenerateModel(MLContext mlContext)
        {
            // Initializing the pipeline
            IEstimator<ITransformer> pipeline = mlContext.Transforms.LoadImages(outputColumnName: "input", imageFolder: _imagesFolder, inputColumnName: nameof(ImageData.ImagePath))
                // The image transforms transform the images into the model's expected format.
                .Append(mlContext.Transforms.ResizeImages(outputColumnName: "input", imageWidth: InceptionSettings.ImageWidth, imageHeight: InceptionSettings.ImageHeight, inputColumnName: "input"))
                .Append(mlContext.Transforms.ExtractPixels(outputColumnName: "input", interleavePixelColors: InceptionSettings.ChannelsLast, offsetImage: InceptionSettings.Mean))
                .Append(mlContext.Model.LoadTensorFlowModel(_inceptionTensorFlowModel)
                .ScoreTensorFlowModel(outputColumnNames: new[] { "softmax2_pre_activation" }, inputColumnNames: new[] { "input" }, addBatchDimensionInput: true))
                .Append(mlContext.Transforms.Conversion.MapValueToKey(outputColumnName: "LabelKey", inputColumnName: "Label"))
                .Append(mlContext.MulticlassClassification.Trainers.LbfgsMaximumEntropy(labelColumnName: "LabelKey", featureColumnName: "softmax2_pre_activation"))
                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabelValue", "PredictedLabel"))
                .AppendCacheCheckpoint(mlContext);

            // loading data
            IDataView trainingData = mlContext.Data.LoadFromTextFile<ImageData>(path: _trainTagsTsv, hasHeader: false);
            // trains the model
            ITransformer model = pipeline.Fit(trainingData);

            // load and transform test data
            //IDataView testData = mlContext.Data.LoadFromTextFile<ImageData>(path: _testTagsTsv, hasHeader: false);
            //IDataView predictions = model.Transform(testData);

            //// Create an IEnumerable for the predictions for displaying results
            //IEnumerable<ImagePrediction> imagePredictionData = mlContext.Data.CreateEnumerable<ImagePrediction>(predictions, true);
            //DisplayResults(imagePredictionData);

            //// Evaluate()
            //// Assesses the model(compares the predicted values with the test dataset labels).
            //// Returns the model performance metrics.
            //MulticlassClassificationMetrics metrics = mlContext.MulticlassClassification.Evaluate(predictions,
            //    labelColumnName: "LabelKey",
            //    predictedLabelColumnName: "PredictedLabel");

            //Console.WriteLine($"LogLoss is: {metrics.LogLoss}");
            //Console.WriteLine($"PerClassLogLoss is: {String.Join(" , ", metrics.PerClassLogLoss.Select(c => c.ToString()))}");

            return model;
        }
    }

    internal class ImageData
    {
        /// <summary>
        /// contains the image file name.
        /// </summary>
        [LoadColumn(0)]
        public string ImagePath;

        /// <summary>
        /// contains a value for the image label.
        /// </summary>
        [LoadColumn(1)]
        public string Label;
    }

    /// <summary>
    /// used for prediction after the model has been trained.
    /// </summary>
    internal class ImagePrediction : ImageData
    {
        /// <summary>
        /// contains the confidence percentage for a given image classification.
        /// </summary>
        public float[] Score;

        /// <summary>
        /// contains a value for the predicted image classification label.
        /// </summary>
        public string PredictedLabelValue;
    }

    public readonly struct PredictionResult
    {
        public const string VAULT = "Vault";
        public const string VAULT_UI = "VaultUI";
        public const string FRIDGE = "Fridge";
        public const string FRIDGE_UI = "FridgeUI";
        public const string BED = "Bed";
        public const string BED_UI = "BedUI";

        public ImgClasses Class { get; }
        public float Confidence { get; }

        public PredictionResult(string _class, float _value)
        {
            switch (_class)
            {
                case VAULT:
                    Class = ImgClasses.Vault;
                    break;
                case VAULT_UI:
                    Class = ImgClasses.Vault_UI;
                    break;
                case FRIDGE:
                    Class = ImgClasses.Fridge;
                    break;
                case FRIDGE_UI:
                    Class = ImgClasses.Fridge_UI;
                    break;
                case BED:
                    Class = ImgClasses.Bed;
                    break;
                case BED_UI:
                    Class = ImgClasses.Bed_UI;
                    break;
                default:
                    Class = ImgClasses.Error;
                    break;
            }
            Confidence = _value;
        }
    }

    public enum ImgClasses
    {
        Error = 0,
        Vault,
        Vault_UI,
        Fridge,
        Fridge_UI,
        Bed,
        Bed_UI
    }
}