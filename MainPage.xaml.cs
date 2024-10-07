using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace FMK_1
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Start.Visibility = Visibility.Collapsed;
        }
    }
}
