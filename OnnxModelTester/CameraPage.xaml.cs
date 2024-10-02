//using Android.Graphics;
//using Android.OS;
using System.Timers;
//using Microsoft.Maui.Controls;
//using Android.Content;
//using Android.Provider;
//using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using AsyncAwaitBestPractices;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Android.Graphics;
using System.IO;
using SkiaSharp;
using AndroidX.ConstraintLayout.Core.Motion.Utils;
using Android.Renderscripts;
using OnnxModelTester.Models.RTFormer;

namespace OnnxModelTester
{
    public partial class CameraPage : ContentPage
    {
        private System.Timers.Timer _frameCaptureTimer;
        private bool _isCameraStarted = false;
        private bool _isCapturing = false;
        private int _fps = 2;
        //private byte[] _model;
        IVisionSample _rtformer;
        IVisionSample RTFormer => _rtformer ??= new RTFormerSample();

        public CameraPage()
        {
            InitializeComponent();
            //InitializeOnnxModel();
        }

        private void cameraView_CamerasLoaded(object sender, EventArgs e)
        {
            cameraView.Camera = cameraView.Cameras.First();

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await cameraView.StopCameraAsync();
                await cameraView.StartCameraAsync();
                _isCameraStarted = true;
            });
        }

        //private void InitializeOnnxModel()
        //{
        //    var assembly = GetType().Assembly;
        //    using var modelStream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.model_rtformer1.onnx");

        //    if (modelStream == null)
        //    {
        //        throw new FileNotFoundException("Model could not be found");
        //    }

        //    using var modelMemoryStream = new MemoryStream();
        //    modelStream.CopyTo(modelMemoryStream);
        //    _model = modelMemoryStream.ToArray();
        //}


        private void StartCapturing(object sender, EventArgs e)
        {
            if (!_isCameraStarted)
            {
                DisplayAlert("Missing Camera Permission", "The app needs a permission to the camera", "OK");
                return;
            }
            if (_isCapturing)
                return;

            _isCapturing = true;

            _frameCaptureTimer = new System.Timers.Timer(double.Round(5000));
            _frameCaptureTimer.Elapsed += CaptureFrame;
            _frameCaptureTimer.Start();
        }

        private void StopCapturing(object sender, EventArgs e)
        {
            _isCapturing = false;
            _frameCaptureTimer?.Stop();
            _frameCaptureTimer?.Dispose();
        }

        private async void CaptureFrame(object sender, ElapsedEventArgs e)
        {
            if (_isCapturing)
            {
                UpdateImage().SafeFireAndForget();
            }
        }
        private async Task UpdateImage()
        {
            if (!_isCapturing)
            {
                return;
            }

            var imageSource = await GetImageAsync();
            MainThread.BeginInvokeOnMainThread(() =>
            {
                myImage.Source = imageSource;
            });
        }

        private async Task<ImageSource> GetImageAsync()
        {
            var imageSource = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.JPEG);
            if (imageSource is StreamImageSource streamImageSource)
            {
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();

                var stream = await streamImageSource.Stream(CancellationToken.None);
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                var imageBytes = ms.ToArray();
                var orientedImageBytes = Utils.HandleOrientation(imageBytes);


                IVisionSample sample = RTFormer;
                var result = await sample.ProcessImageAsync(orientedImageBytes);

                stopwatch.Stop();
                Console.WriteLine($"----------------------ELAPSED TIME: {stopwatch.ElapsedMilliseconds} ms--------------------------------");
                return ImageSource.FromStream(() => new MemoryStream(result.Image));
                //using var runOptions = new RunOptions();
                //using var session = new InferenceSession(_model);

                //var stream = await streamImageSource.Stream(CancellationToken.None);
                //Console.WriteLine("-----------------------------------------------");
                //using Image<Rgb24> image = SixLabors.ImageSharp.Image.Load<Rgb24>(stream);
                //Console.WriteLine("------------------------AAAAAAAAAAAAAAAAAAAAAAAAAAAAA-----------------------");
                //int width = image.Width;
                //int height = image.Height;

                
                //Console.WriteLine($"----------Image: W:{width} H:{height} loaded-------------------");
                
            }
            else
            {
                await DisplayAlert("Error", "The ImageSource is not a StreamImageSource", "OK");
            }
            return null;
            //return imageSource;
        }
    }
}
