namespace OnnxModelTester
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnCameraButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new CameraPage());
        }
    }

}
