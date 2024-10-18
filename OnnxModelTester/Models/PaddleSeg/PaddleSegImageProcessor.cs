using Microsoft.ML.OnnxRuntime.Tensors;
using OnnxModelTester.PrePostProcessing;
using SkiaSharp;

namespace OnnxModelTester.Models.PaddleSeg
{
    public class PaddleSegImageProcessor : SkiaSharpImageProcessor<PaddleSegPrediction, float>
    {
        const int RequiredWidth = 512;
        const int RequiredHeight = 1024;

        protected override SKBitmap OnPreprocessSourceImage(SKBitmap sourceImage)
            => sourceImage.Resize(new SKImageInfo(RequiredWidth, RequiredHeight), SKFilterQuality.Medium);

        protected override Tensor<float> OnGetTensorForImage(SKBitmap image)
        {
            var bytes = image.GetPixelSpan();

            // For Ultraface, the expected input would be 320 x 240 x 4 (in RGBA format)
            var expectedInputLength = RequiredWidth * RequiredHeight * 4;

            // For the Tensor, we need 3 channels so 320 x 240 x 3 (in RGB format)
            var expectedOutputLength = RequiredWidth * RequiredHeight * 3;

            if (bytes.Length != expectedInputLength)
            {
                throw new Exception($"The parameter {nameof(image)} is an unexpected length. " +
                                    $"Expected length is {expectedInputLength}");
            }

            // The channelData array is expected to be in RGB order with a mean applied i.e. (pixel - 127.0f) / 128.0f
            float[] channelData = new float[expectedOutputLength];

            // Extract only the desired channel data (don't want the alpha)
            var expectedChannelLength = expectedOutputLength / 3;
            var redOffset = expectedChannelLength * 0;
            var greenOffset = expectedChannelLength * 1;
            var blueOffset = expectedChannelLength * 2;

            for (int i = 0, i2 = 0; i < bytes.Length; i += 4, i2++)
            {
                var r = Convert.ToSingle(bytes[i]);
                var g = Convert.ToSingle(bytes[i + 1]);
                var b = Convert.ToSingle(bytes[i + 2]);
                channelData[i2 + redOffset] = (r - 127.0f) / 128.0f;
                channelData[i2 + greenOffset] = (g - 127.0f) / 128.0f;
                channelData[i2 + blueOffset] = (b - 127.0f) / 128.0f;
            }

            return new DenseTensor<float>(new Memory<float>(channelData),
                                          new[] { 1, 3, RequiredHeight, RequiredWidth });
        }

        protected override void OnApplyMaskPrediction(PaddleSegPrediction prediction, SKCanvas canvas, int orgImgWidth, int orgImgHeight)
        {
            // Create RGBA bitmap in original size
            var rgbaPixels = prediction.Mask;

            //int[] byteFrequency = new int[256];

            //foreach (byte b in rgbaPixels)
            //{
            //    byteFrequency[b]++;
            //}

            SKBitmap bitmap = new SKBitmap(512, 1024, SKColorType.Rgba8888, SKAlphaType.Premul);
            IntPtr pixelPointer = bitmap.GetPixels();
            System.Runtime.InteropServices.Marshal.Copy(rgbaPixels, 0, pixelPointer, rgbaPixels.Length);
            SKRect destRect = new SKRect(0, 0, orgImgWidth, orgImgHeight);

            canvas.DrawBitmap(bitmap, destRect);
        }
    }
}
