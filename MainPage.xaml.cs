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
        private Random random = new Random();

		private void DiceButton_Click(object sender, RoutedEventArgs e)
		{
            int DiceValue = random.Next(1, 7);
            HideDots();

            switch (DiceValue)
            {
                case 1:
					Dot2.Visibility = Visibility.Visible;
                    break;
                case 2:
					Dot1.Visibility = Visibility.Visible;
					Dot6.Visibility = Visibility.Visible;
					break;
				case 3:
					Dot1.Visibility = Visibility.Visible;
					Dot3.Visibility = Visibility.Visible;
					Dot5.Visibility = Visibility.Visible;
					break;
				case 4:
					Dot1.Visibility = Visibility.Visible;
					Dot3.Visibility = Visibility.Visible;
					Dot4.Visibility = Visibility.Visible;
					Dot6.Visibility = Visibility.Visible;
					break;
				case 5:
					Dot1.Visibility = Visibility.Visible;
					Dot2.Visibility = Visibility.Visible;
					Dot3.Visibility = Visibility.Visible;
					Dot4.Visibility = Visibility.Visible;
					Dot6.Visibility = Visibility.Visible;
					break;
				case 6:
					Dot1.Visibility = Visibility.Visible;
					Dot2.Visibility = Visibility.Visible;
					Dot3.Visibility = Visibility.Visible;
					Dot4.Visibility = Visibility.Visible;
					Dot5.Visibility = Visibility.Visible;
					Dot6.Visibility = Visibility.Visible;
					break;
			}

			DiceResult.Text = $"Du slog {DiceValue}!";

		}

        private void HideDots()
        {
            Dot1.Visibility = Visibility.Collapsed;
            Dot2.Visibility = Visibility.Collapsed;
			Dot3.Visibility = Visibility.Collapsed;
			Dot4.Visibility = Visibility.Collapsed;
			Dot5.Visibility = Visibility.Collapsed;
			Dot6.Visibility = Visibility.Collapsed;

		}
	}
}
