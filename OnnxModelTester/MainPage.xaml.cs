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
                "rtformer_alteredcityscapes_80k.onnx",
                "rtformer_alteredcityscapes_80k_no_crop.onnx",
                "rtformer_alteredcityscapes_400epoch.onnx",
                "rtformer_visionnavigatorset_1000epoch.onnx",
                "rtformer_visionnavigatorset_2000epoch.onnx",
                "pp_liteseg_visionnavigatorset_1900epoch.onnx"
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
