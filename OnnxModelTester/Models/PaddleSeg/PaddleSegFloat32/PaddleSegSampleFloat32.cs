using Microsoft.ML.OnnxRuntime;
using Microsoft.ML.OnnxRuntime.Tensors;
using OnnxModelTester.Models.PaddleSeg.PaddleSegColorMaps;
using OnnxModelTester.PrePostProcessing;
using System.Diagnostics;

namespace OnnxModelTester.Models.PaddleSeg.PaddleSegFloat32
{
    public class PaddleSegSampleFloat32 : VisionSampleBase<PaddleSegImageProcessorFloat32>
    {
        public const string Identifier = "PaddleSegModel";
        public readonly string ModelFileName;
        private readonly IPaddleSegColorMap _colorMap;
        public List<long> Predictions;
        public PaddleSegSampleFloat32(string modelFilename, IPaddleSegColorMap colorMap, List<long> predictions)
            : base(Identifier, modelFilename)
        {
            ModelFileName = modelFilename;
            _colorMap = colorMap;
            Predictions = predictions;
        }

        protected override async Task<ImageProcessingResult> OnProcessImageAsync(byte[] image)
        {
            // do initial resize maintaining the aspect ratio so the smallest size is 800. this is arbitrary and 
            // chosen to be a good size to dispay to the user with the results
            using var sourceImage = await Task.Run(() => ImageProcessor.GetImageFromBytes(image, 800f))//check if necessary
                                            .ConfigureAwait(false);

            // do the preprocessing to resize the image to the 1024x512 with the model expects. 
            // NOTE: this does not maintain the aspect ratio but works well enough with this particular model.
            //       it may be better in other scenarios to resize and crop to convert the original image to a
            //       1024x512 image.
            using var preprocessedImage = await Task.Run(() => ImageProcessor.PreprocessSourceImage(image))
                                                    .ConfigureAwait(false);

            // Convert to Tensor of normalized float RGB data with NCHW ordering
            var tensor = await Task.Run(() => ImageProcessor.GetTensorForImage(preprocessedImage))
                                   .ConfigureAwait(false);


            // Run the model
            var predictions = await Task.Run(() => GetPredictions(tensor, sourceImage.Width, sourceImage.Height))
                                        .ConfigureAwait(false);

            // Draw the bounding box for the best prediction on the image from the first resize. 
            var outputImage = await Task.Run(() => ImageProcessor.ApplyPredictionsToImage(predictions, sourceImage))
                                        .ConfigureAwait(false);

            return new ImageProcessingResult(outputImage);
        }

        List<PaddleSegPrediction> GetPredictions(Tensor<float> input, int sourceImageWidth, int sourceImageHeight)
        {
            // Setup inputs. Names used must match the input names in the model. 
            var inputs = new List<NamedOnnxValue> { NamedOnnxValue.CreateFromTensor("x", input) };

            var stopwatch = new Stopwatch();
            stopwatch.Start();
            // Run inference
            using IDisposableReadOnlyCollection<DisposableNamedOnnxValue> results = Session.Run(inputs);
            stopwatch.Stop();
            var predictionTime = stopwatch.ElapsedMilliseconds;
            Predictions.Add(predictionTime);


            // Handle predictions
            var resultsArray = results.ToArray();
            var predictedValues = resultsArray[0].AsEnumerable<int>().ToArray();
            byte[] mask = predictedValues.Select(i => (byte)i).ToArray();
            var rgbaMask = ConvertGrayscaleToRgba(mask);

            return new List<PaddleSegPrediction>()
            {
                new PaddleSegPrediction
                {
                    Mask = rgbaMask
                }
            };
        }

        private byte[] ConvertGrayscaleToRgba(byte[] grayscalePixels)
        {
            // Make 4 times larger array 
            byte[] rgbaPixels = new byte[grayscalePixels.Length * 4];

            for (int i = 0; i < grayscalePixels.Length; i++)
            {
                byte grayscaleValue = grayscalePixels[i];
                byte[] rgbaValue = _colorMap.ColorMap[grayscaleValue];

                // assign RGBA value
                rgbaPixels[i * 4] = rgbaValue[0];     // R
                rgbaPixels[i * 4 + 1] = rgbaValue[1]; // G
                rgbaPixels[i * 4 + 2] = rgbaValue[2]; // B
                rgbaPixels[i * 4 + 3] = rgbaValue[3]; // A
            }

            return rgbaPixels;
        }
    }
}
