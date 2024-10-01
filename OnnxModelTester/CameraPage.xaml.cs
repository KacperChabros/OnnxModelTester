using Android.Graphics;
using Android.OS;
using System.Timers;
using Microsoft.Maui.Controls;
using Android.Content;
using Android.Provider;
using System.IO;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using Microsoft.ML.OnnxRuntime.Tensors;
using Microsoft.ML.OnnxRuntime;
using AsyncAwaitBestPractices;

namespace OnnxModelTester
{
    public partial class CameraPage : ContentPage
    {
        private System.Timers.Timer _frameCaptureTimer;
        private bool _isCameraStarted = false;
        private bool _isCapturing = false;
        private int _fps = 2;

        public CameraPage()
        {
            InitializeComponent();
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

            _frameCaptureTimer = new System.Timers.Timer(double.Round(1000 / _fps));
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
                
            }
            else
            {
                await DisplayAlert("Error", "The ImageSource is not a StreamImageSource", "OK");
            }
            return imageSource;
        }
    }
}
