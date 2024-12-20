using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;


namespace FMK_1
{
	public sealed partial class MainPage : Page
	{
		public int player;
		//Color List
		private Color[] colors = new Color[]
		{
			Colors.Red,
			Colors.Green,
			Colors.Blue,
			Colors.Yellow
		};

		private SolidColorBrush PlayerOneColor;
		private SolidColorBrush PlayerTwoColor;
		private SolidColorBrush PlayerThreeColor;
		private SolidColorBrush PlayerFourColor;
		private int colorIndex = 0;  //Used to go through Color List.
		private Random random = new Random();
		private int PlayersNum = 0;
		private int playerTurn = 1;
		private int maxPlayers = 4;

		public MainPage()
		{
			this.InitializeComponent();
			colorIndex = 0;
			PlayerOneColor = new SolidColorBrush(colors[colorIndex]);
			PlayerTwoColor = new SolidColorBrush(colors[colorIndex + 1]);
			PlayerThreeColor = new SolidColorBrush(colors[colorIndex + 2]);
			PlayerFourColor = new SolidColorBrush(colors[colorIndex + 3]);

            DiceRollStoryboard.Completed += DiceRollStoryboard_Completed;
        }

		private Grid CreateGrid(string gridName, Thickness margin)
		{
			Grid grid = new Grid
			{
				Name = gridName,
				Background = new SolidColorBrush(Colors.Red),
				Height = 50,
				Width = 50,
				Visibility = Visibility.Visible,
				HorizontalAlignment = HorizontalAlignment.Left,
				Margin = margin,
				CanDrag = true
			};

			TextBlock textBlock = new TextBlock
			{
				Name = "Textblock_" + gridName,
				Text = "TEST",
				HorizontalAlignment = HorizontalAlignment.Center,
				VerticalAlignment = VerticalAlignment.Center
			};
			grid.Children.Add(textBlock);

			grid.DragStarting += Test1_DragStarting;

			return grid;
		}

		private void StartGameBtn_Click(object sender, RoutedEventArgs e)
		{
			player = 0;
			Start.Visibility = Visibility.Collapsed;

			Grid P1 = CreateGrid("P1", new Thickness(0, 150, 0, 0));
			Test.Children.Add(P1);

			// Create and add the second Grid (P2)
			Grid P2 = CreateGrid("P2", new Thickness(0, 0, 0, 150));
			Test.Children.Add(P2);

			ColorPieces();

			if (PlayersNum == 1 || PlayersNum == 2)
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
			else if (PlayersNum == 3)
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

			colorIndex = 0;
		}

        private void DiceButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the dots before starting the animation
            HideDots();

			// Stop the animation to reset it
            DiceRollStoryboard.Stop();

            // Start the animation
            DiceRollStoryboard.Begin();
        }

        private void DiceRollStoryboard_Completed(object sender, object e)
        {
            // Roll the dice after the animation completes
            int DiceValue = random.Next(1, 7);

            // Show the dots based on the rolled value
            ShowDots(DiceValue);

            // Display the result in the TextBox
            DiceResult.Text = $"Du slog {DiceValue}!";
        }

        private void ShowDots(int DiceValue)
        {
            // Reset visibility of all dots before showing the current ones
            HideDots();

            switch (DiceValue)
            {
                case 1:
                    Dot2.Visibility = Visibility.Visible;
                    Dot21.Visibility = Visibility.Visible;
                    break;
                case 2:
                    Dot1.Visibility = Visibility.Visible;
                    Dot6.Visibility = Visibility.Visible;
                    Dot11.Visibility = Visibility.Visible;
                    Dot61.Visibility = Visibility.Visible;
                    break;
                case 3:
                    Dot1.Visibility = Visibility.Visible;
                    Dot3.Visibility = Visibility.Visible;
                    Dot5.Visibility = Visibility.Visible;
                    Dot11.Visibility = Visibility.Visible;
                    Dot31.Visibility = Visibility.Visible;
                    Dot51.Visibility = Visibility.Visible;
                    break;
                case 4:
                    Dot1.Visibility = Visibility.Visible;
                    Dot3.Visibility = Visibility.Visible;
                    Dot4.Visibility = Visibility.Visible;
                    Dot6.Visibility = Visibility.Visible;
                    Dot11.Visibility = Visibility.Visible;
                    Dot31.Visibility = Visibility.Visible;
                    Dot41.Visibility = Visibility.Visible;
                    Dot61.Visibility = Visibility.Visible;
                    break;
                case 5:
                    Dot1.Visibility = Visibility.Visible;
                    Dot2.Visibility = Visibility.Visible;
                    Dot3.Visibility = Visibility.Visible;
                    Dot4.Visibility = Visibility.Visible;
                    Dot6.Visibility = Visibility.Visible;
                    Dot11.Visibility = Visibility.Visible;
                    Dot21.Visibility = Visibility.Visible;
                    Dot31.Visibility = Visibility.Visible;
                    Dot41.Visibility = Visibility.Visible;
                    Dot61.Visibility = Visibility.Visible;
                    break;
                case 6:
                    Dot1.Visibility = Visibility.Visible;
                    Dot2.Visibility = Visibility.Visible;
                    Dot3.Visibility = Visibility.Visible;
                    Dot4.Visibility = Visibility.Visible;
                    Dot5.Visibility = Visibility.Visible;
                    Dot6.Visibility = Visibility.Visible;
                    Dot11.Visibility = Visibility.Visible;
                    Dot21.Visibility = Visibility.Visible;
                    Dot31.Visibility = Visibility.Visible;
                    Dot41.Visibility = Visibility.Visible;
                    Dot51.Visibility = Visibility.Visible;
                    Dot61.Visibility = Visibility.Visible;
                    break;
            }
        }

        private void HideDots()
		{
			Dot1.Visibility = Visibility.Collapsed;
			Dot2.Visibility = Visibility.Collapsed;
			Dot3.Visibility = Visibility.Collapsed;
			Dot4.Visibility = Visibility.Collapsed;
			Dot5.Visibility = Visibility.Collapsed;
			Dot6.Visibility = Visibility.Collapsed;
			Dot11.Visibility = Visibility.Collapsed;
			Dot21.Visibility = Visibility.Collapsed;
			Dot31.Visibility = Visibility.Collapsed;
			Dot41.Visibility = Visibility.Collapsed;
			Dot51.Visibility = Visibility.Collapsed;
			Dot61.Visibility = Visibility.Collapsed;
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
		private void DicePlaceOnBoard(int turn)
		{
			switch (turn)
			{
				case 1:
					// Position for turn 1
					Dice2.SetValue(Grid.ColumnProperty, 0); // Column 0
					Dice2.SetValue(Grid.RowProperty, 1);    // Row 1
					Dice2.VerticalAlignment = VerticalAlignment.Top;
					Dice2.Margin = new Thickness(20, 20, 0, 0);
					break;

				case 2:
					// Position for turn 2
					Dice2.SetValue(Grid.ColumnProperty, 3); // Column 3
					Dice2.SetValue(Grid.RowProperty, 1);    // Row 1
					Dice2.VerticalAlignment = VerticalAlignment.Top;
					Dice2.Margin = new Thickness(0, 20, 20, 0);
					break;

				case 3:
					// Position for turn 3
					Dice2.SetValue(Grid.ColumnProperty, 3); // Column 3
					Dice2.SetValue(Grid.RowProperty, 1);    // Row 1
					Dice2.VerticalAlignment = VerticalAlignment.Bottom;
					Dice2.Margin = new Thickness(0, 0, 20, 20);
					break;

				case 4:
					// Position for turn 4
					Dice2.SetValue(Grid.ColumnProperty, 0); // Column 0
					Dice2.SetValue(Grid.RowProperty, 1);    // Row 1
					Dice2.VerticalAlignment = VerticalAlignment.Bottom;
					Dice2.Margin = new Thickness(20, 0, 0, 20);
					break;

				default:
					// Reset or do nothing
					break;
			}
		}

		private void TurnBtn_Click(object sender, RoutedEventArgs e)
		{
			// Increment the turn
			playerTurn++;

			// If playerTurn exceeds the maximum number of players, reset to 1
			if (playerTurn > maxPlayers)
			{
				playerTurn = 1;
			}

			// Call the DicePlaceOnBoard function to move the dice based on the player's turn
			DicePlaceOnBoard(playerTurn);
		}

		private void ChooseClrSqr1_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			Rectangle clickedRectangle = sender as Rectangle;

			clickedRectangle.Fill = new SolidColorBrush(colors[colorIndex]);

			PlayerOneColor = new SolidColorBrush(colors[colorIndex]);

			colorIndex++;

			if (colorIndex >= colors.Length)
			{
				colorIndex = 0;
			}

		}

		private void ChooseClrSqr2_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			Rectangle clickedRectangle = sender as Rectangle;

			clickedRectangle.Fill = new SolidColorBrush(colors[colorIndex]);

			PlayerTwoColor = new SolidColorBrush(colors[colorIndex]);

			colorIndex++;

			if (colorIndex >= colors.Length)
			{
				colorIndex = 0;
			}
		}

		private void ChooseClrSqr3_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			Rectangle clickedRectangle = sender as Rectangle;

			clickedRectangle.Fill = new SolidColorBrush(colors[colorIndex]);

			PlayerThreeColor = new SolidColorBrush(colors[colorIndex]);

			colorIndex++;

			if (colorIndex >= colors.Length)
			{
				colorIndex = 0;
			}

		}

		private void ChooseClrSqr4_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
		{
			Rectangle clickedRectangle = sender as Rectangle;

			clickedRectangle.Fill = new SolidColorBrush(colors[colorIndex]);

			PlayerFourColor = new SolidColorBrush(colors[colorIndex]);

			colorIndex++;

			if (colorIndex >= colors.Length)
			{
				colorIndex = 0;
			}

		}

		private void ColorPieces()
		{
			RedPiece1.Fill = PlayerOneColor;
			RedPiece2.Fill = PlayerOneColor;
			RedPiece3.Fill = PlayerOneColor;
			RedPiece4.Fill = PlayerOneColor;

			GreenPiece1.Fill = PlayerTwoColor;
			GreenPiece2.Fill = PlayerTwoColor;
			GreenPiece3.Fill = PlayerTwoColor;
			GreenPiece4.Fill = PlayerTwoColor;

			BluePiece1.Fill = PlayerThreeColor;
			BluePiece2.Fill = PlayerThreeColor;
			BluePiece3.Fill = PlayerThreeColor;
			BluePiece4.Fill = PlayerThreeColor;

			YellowPiece1.Fill = PlayerFourColor;
			YellowPiece2.Fill = PlayerFourColor;
			YellowPiece3.Fill = PlayerFourColor;
			YellowPiece4.Fill = PlayerFourColor;
		}

		private async void Push_Click(object sender, RoutedEventArgs e)
		{
			PushSound.Play();

			Push.Begin();
			await Task.Delay(700);
			ExplosionImage1.Visibility = Visibility.Visible;

			await Task.Delay(500);
			ExplosionImage1.Visibility = Visibility.Collapsed;

			ExplosionImage2.Visibility = Visibility.Visible;
		}


		//TODO: Byt så när man lyfter så ändras lyft iconen till något passande
		private void Test_DragOver(object sender, DragEventArgs e)
		{
			e.AcceptedOperation = DataPackageOperation.Move;
			e.DragUIOverride.Caption = "";
			e.DragUIOverride.IsGlyphVisible = true;
		}

		private void Test1_DragStarting(UIElement sender, DragStartingEventArgs args)
		{
			if (sender is FrameworkElement element)
			{
				string name = element.Name;

				args.Data.Properties.Add("Name", name);

				args.Data.SetText(name);

				args.DragUI.SetContentFromDataPackage();
			}
		}



		private async void Test_Drop(object sender, DragEventArgs e)
		{
			if (e.DataView.Properties.ContainsKey("Name"))
			{
				var name = e.DataView.Properties["Name"] as string;

				var draggedElement = (UIElement)this.FindName(name);

				if (draggedElement != null)
				{
					draggedElement.Visibility = Visibility.Collapsed;
					player++;
				}
				if (player == 2)
				{
					End.Visibility = Visibility.Visible;
					MediaElement SoundPlayer = new MediaElement();
					var soundFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Win.mp3"));
					var stream = await soundFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
					SoundPlayer.SetSource(stream, soundFile.ContentType);
					SoundPlayer.Play();
				}
				else
				{
					MediaElement SoundPlayer = new MediaElement();
					var soundFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Assets/Goal.mp3"));
					var stream = await soundFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
					SoundPlayer.SetSource(stream, soundFile.ContentType);
					SoundPlayer.Play();
				}
			}
		}
    }
}
