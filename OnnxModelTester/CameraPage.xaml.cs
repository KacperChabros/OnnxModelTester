using AsyncAwaitBestPractices;
using OnnxModelTester.Models.RTFormer;
using System.Diagnostics;
using System.Timers;

namespace OnnxModelTester
{
    public partial class CameraPage : ContentPage
    {
        private System.Timers.Timer _frameCaptureTimer;
        private bool _isCameraStarted = false;
        private bool _isCapturing = false;
        private int _fps = 2;

        IVisionSample _rtformer;
        IVisionSample RTFormer => _rtformer ??= new RTFormerSample();

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

            _frameCaptureTimer = new System.Timers.Timer(double.Round(1000));
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
                
            }
            else
            {
                await DisplayAlert("Error", "The ImageSource is not a StreamImageSource", "OK");
            }
            return null;
        }
    }
}
