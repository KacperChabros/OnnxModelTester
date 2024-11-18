using Android.Content;
using Android.Provider;
using AsyncAwaitBestPractices;
using OnnxModelTester.Models.PaddleSeg;
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
        private string _selectedModel;
        private IPaddleSegColorMap _colorMap;
        public List<long> Predictions = new List<long>();

        IVisionSample _paddleSegModel;
        IVisionSample PaddleSegModel => _paddleSegModel ??= new PaddleSegSample(_selectedModel, _colorMap, Predictions);

        public CameraPage(string selectedModel, string selectedColorMap)
        {
            InitializeComponent();
            _selectedModel = selectedModel;
            _colorMap = ColorMapFactory.CreateColorMap(selectedColorMap);
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

        private async void StartCapturing(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_selectedModel))
            {
                await DisplayAlert("No model selected", "A model needs to be selected in the previous page", "OK");
                return;
            }

            if (!_isCameraStarted)
            {
                await DisplayAlert("Missing Camera Permission", "The app needs a permission to the camera", "OK");
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
            SaveResults();
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
                var stream = await streamImageSource.Stream(CancellationToken.None);
                using MemoryStream ms = new MemoryStream();
                stream.CopyTo(ms);
                var imageBytes = ms.ToArray();
                var orientedImageBytes = Utils.HandleOrientation(imageBytes);


                IVisionSample sample = PaddleSegModel;
                var result = await sample.ProcessImageAsync(orientedImageBytes);

                return ImageSource.FromStream(() => new MemoryStream(result.Image));
                
            }
            else
            {
                await DisplayAlert("Error", "The ImageSource is not a StreamImageSource", "OK");
            }
            return null;
        }

        private void SaveResults()
        {
            var dateAndTime = $"{DateTime.Now:yyyy-MM-dd_HH-mm-ss}";
            var fileName = $"{_selectedModel.Split('.')[0]}_{dateAndTime}.txt";
            var context = Android.App.Application.Context;
            ContentValues values = new ContentValues();
            values.Put(MediaStore.Downloads.InterfaceConsts.DisplayName, fileName);
            values.Put(MediaStore.Downloads.InterfaceConsts.MimeType, "text/plain");
            values.Put(MediaStore.Downloads.InterfaceConsts.RelativePath, "Download/");
            var uri = context.ContentResolver.Insert(MediaStore.Downloads.ExternalContentUri, values);
            if (uri != null)
            {
                using (var outputStream = context.ContentResolver.OpenOutputStream(uri))
                using (var writer = new StreamWriter(outputStream))
                {
                    writer.WriteLine($"Prediction time measurements for {_selectedModel} created on {dateAndTime}");
                    foreach (var prediction in Predictions)
                    {
                        writer.WriteLine(prediction.ToString());
                    }
                }
            }
        }
    }
}
