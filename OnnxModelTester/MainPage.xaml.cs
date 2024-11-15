namespace OnnxModelTester
{
    public partial class MainPage : ContentPage
    {
        private string _selectedModel = string.Empty;
        private string _selectedColorMap = string.Empty;

        public MainPage()
        {
            InitializeComponent();
            var modelOptions = new List<string> 
            {
                "rtformer_alteredcityscapes_80k.onnx",
                "rtformer_alteredcityscapes_80k_no_crop.onnx",
                "rtformer_alteredcityscapes_400epoch.onnx",
                "rtformer_visionnavigatorset_1000epoch.onnx",
                "rtformer_visionnavigatorset_2000epoch.onnx",
                "pp_liteseg_visionnavigatorset_1900epoch.onnx",
                "pp_liteseg_vns_reduced_2300epoch.onnx"
            };
            ModelPicker.ItemsSource = modelOptions;

            var colorMapOptions = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("Full dataset - 13 classes", "full"),
                new KeyValuePair<string, string>("Reduced dataset - 11 classes", "reduced"),
            };
            ColorMapPicker.ItemsSource = colorMapOptions;
            ColorMapPicker.ItemDisplayBinding = new Binding("Key");
        }

        private async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPage(_selectedModel, _selectedColorMap));
        }

        private void OnModelPickerSelectionChanged(object sender, EventArgs e)
        {
            _selectedModel = ModelPicker.SelectedItem as string ?? string.Empty;
        }

        private void OnColorMapPickerSelectionChanged(object sender, EventArgs e)
        {
            if (ColorMapPicker.SelectedItem is KeyValuePair<string, string> selectedOption)
            {
                _selectedColorMap = selectedOption.Value ?? string.Empty;
            }
        }
    }

}
