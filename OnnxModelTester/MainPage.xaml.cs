namespace OnnxModelTester
{
    public partial class MainPage : ContentPage
    {
        private string _selectedModel = string.Empty;

        public MainPage()
        {
            InitializeComponent();
            var modelOptions = new List<string> 
            {
                "rtformer_alteredcityscapes_30k.onnx",
                "rtformer_alteredcityscapes_80k.onnx",
                "rtformer_alteredcityscapes_80k_no_crop.onnx",
            };
            ModelPicker.ItemsSource = modelOptions;
        }

        private async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPage(_selectedModel));
        }

        private void OnPickerSelectionChanged(object sender, EventArgs e)
        {
            _selectedModel = ModelPicker.SelectedItem as string ?? string.Empty;
        }
    }

}
