using Microsoft.ML.Data;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace RedLearner.DataStructures
{
    public class ImageNetData
    {
        /// <summary>
        /// Location of file.
        /// </summary>
        [LoadColumn(0)]
        public string ImagePath;

        /// <summary>
        /// Name of file.
        /// </summary>
        [LoadColumn(1)]
        public string Label;

        /// <summary>
        /// Loads mulitple image files stored in the target directory.<br/>
        /// Returns - A collection of all the images.
        /// </summary>
        /// <param name="imageFolder">Target directory</param>
        /// <returns>Image collection in memory</returns>
        public static IEnumerable<ImageNetData> ReadFromFile(string imageFolder)
        {
            return Directory
                .GetFiles(imageFolder)
                .Where(filePath => Path.GetExtension(filePath) != ".md")
                .Select(filePath => new ImageNetData { ImagePath = filePath, Label = Path.GetFileName(filePath) });
        }
    }
}
