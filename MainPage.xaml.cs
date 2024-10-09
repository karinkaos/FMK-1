using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;


namespace FMK_1
{
    public sealed partial class MainPage : Page
    {
        //Color List
		private Color[] colors = new Color[]
		{
			Colors.Red,    
            Colors.Green, 
            Colors.Blue,   
            Colors.Yellow 
        };

		private int colorIndex = 0;  //Used to go through Color List.
        private Random random = new Random();
        private int PlayersNum = 0;

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            Start.Visibility = Visibility.Collapsed;
            if(PlayersNum == 1 || PlayersNum ==2)
            {
				RedPiece1.Visibility = Visibility.Visible;
				RedPiece2.Visibility = Visibility.Visible;
				RedPiece3.Visibility = Visibility.Visible;
				RedPiece4.Visibility = Visibility.Visible;

				GreenPiece1.Visibility = Visibility.Visible;
				GreenPiece2.Visibility = Visibility.Visible;
				GreenPiece3.Visibility = Visibility.Visible;
				GreenPiece4.Visibility = Visibility.Visible;
			}
            else if(PlayersNum == 3)
            {
				RedPiece1.Visibility = Visibility.Visible;
				RedPiece2.Visibility = Visibility.Visible;
				RedPiece3.Visibility = Visibility.Visible;
				RedPiece4.Visibility = Visibility.Visible;

				GreenPiece1.Visibility = Visibility.Visible;
				GreenPiece2.Visibility = Visibility.Visible;
				GreenPiece3.Visibility = Visibility.Visible;
				GreenPiece4.Visibility = Visibility.Visible;

				BluePiece1.Visibility = Visibility.Visible;
				BluePiece2.Visibility = Visibility.Visible;
				BluePiece3.Visibility = Visibility.Visible;
				BluePiece4.Visibility = Visibility.Visible;
			}
            else
            {
				RedPiece1.Visibility = Visibility.Visible;
				RedPiece2.Visibility = Visibility.Visible;
				RedPiece3.Visibility = Visibility.Visible;
				RedPiece4.Visibility = Visibility.Visible;

				GreenPiece1.Visibility = Visibility.Visible;
				GreenPiece2.Visibility = Visibility.Visible;
				GreenPiece3.Visibility = Visibility.Visible;
				GreenPiece4.Visibility = Visibility.Visible;

				BluePiece1.Visibility = Visibility.Visible;
				BluePiece2.Visibility = Visibility.Visible;
				BluePiece3.Visibility = Visibility.Visible;
				BluePiece4.Visibility = Visibility.Visible;

				YellowPiece1.Visibility = Visibility.Visible;
                YellowPiece2.Visibility = Visibility.Visible;
                YellowPiece3.Visibility = Visibility.Visible;
                YellowPiece4.Visibility = Visibility.Visible;
            }
        }
        private void Bts_click(object sender, RoutedEventArgs e)
        {
            Start.Visibility = Visibility.Visible;
            End.Visibility = Visibility.Collapsed;
			
            RedPiece1.Visibility = Visibility.Collapsed;
			RedPiece2.Visibility = Visibility.Collapsed;
			RedPiece3.Visibility = Visibility.Collapsed;
			RedPiece4.Visibility = Visibility.Collapsed;

			GreenPiece1.Visibility = Visibility.Collapsed;
			GreenPiece2.Visibility = Visibility.Collapsed;
			GreenPiece3.Visibility = Visibility.Collapsed;
			GreenPiece4.Visibility = Visibility.Collapsed;

			BluePiece1.Visibility = Visibility.Collapsed;
			BluePiece2.Visibility = Visibility.Collapsed;
			BluePiece3.Visibility = Visibility.Collapsed;
			BluePiece4.Visibility = Visibility.Collapsed;
			
            YellowPiece1.Visibility = Visibility.Collapsed;
			YellowPiece2.Visibility = Visibility.Collapsed;
			YellowPiece3.Visibility = Visibility.Collapsed;
			YellowPiece4.Visibility = Visibility.Collapsed;
		}

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

		private void ChooseClrSqr1_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			Rectangle clickedRectangle = sender as Rectangle ;

			clickedRectangle.Fill = new SolidColorBrush(colors[colorIndex]);

			colorIndex++;

			if (colorIndex >= colors.Length)
			{
				colorIndex = 0;
			}
		}

		private void PlayerSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
		{
			if (e.NewValue == 1)
			{
				PlayersNum = 2;
                BlockClrSqr3.Visibility = Visibility.Visible;
				BlockClrSqr4.Visibility = Visibility.Visible;
			}
			else if (e.NewValue == 2)
			{
				PlayersNum = 2;
				BlockClrSqr3.Visibility = Visibility.Visible;
				BlockClrSqr4.Visibility = Visibility.Visible;
			}
			else if (e.NewValue == 3)
			{
				PlayersNum = 3;
				BlockClrSqr3.Visibility = Visibility.Collapsed;
				BlockClrSqr4.Visibility = Visibility.Visible;
			}
			else if (e.NewValue == 4)
			{
				PlayersNum = 4;
				BlockClrSqr3.Visibility = Visibility.Collapsed;
				BlockClrSqr4.Visibility = Visibility.Collapsed;
			}
		}

		private void DiceButton_Click_1(object sender, RoutedEventArgs e)
		{
			End.Visibility = Visibility.Visible;
		}
	}
}
