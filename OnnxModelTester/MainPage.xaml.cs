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
                "pp_liteseg_vnsreduced_corrected_800epoch.onnx",
                "pp_liteseg_vnsreduced_corrected_800epoch_float16.onnx",
                "pp_liteseg_visionnavigatorset_1500epoch.onnx",
                "pp_liteseg_vnsReducedxAlteredScapes_1500epoch.onnx",
                "mobileseg_vnsReducedxAlteredScapes_1500epoch.onnx",
                "pp_liteseg_vnsreduced_1500epoch.onnx"
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
            if (string.IsNullOrWhiteSpace(_selectedModel))
            {
                await DisplayAlert("No model selected", "A model needs to be selected to proceed", "OK");
                return;
            }
            if (string.IsNullOrWhiteSpace(_selectedColorMap))
            {
                await DisplayAlert("No color map selected", "A color map needs to be selected to proceed", "OK");
                return;
            }
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
