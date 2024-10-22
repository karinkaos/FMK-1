using System;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.ApplicationModel.Activation;
using Windows.Media.Core;
using Windows.UI.Xaml.Controls.Primitives;
using System.Xml.Linq;
using System.Collections.Generic;
using Windows.ApplicationModel.Core;


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

		private string[] players = new string[]
		{
			"Player 1",
			"Player 2",
			"Player 3",
			"Player 4"
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
		private bool isLoadMainMenu = false;

		public MainPage()
		{
			this.InitializeComponent();
			colorIndex = 0;
			PlayerOneColor = new SolidColorBrush(colors[colorIndex]);
			PlayerTwoColor = new SolidColorBrush(colors[colorIndex + 1]);
			PlayerThreeColor = new SolidColorBrush(colors[colorIndex + 2]);
			PlayerFourColor = new SolidColorBrush(colors[colorIndex + 3]);
		}

        private Grid CreateGrid(string gridName)
        {
            Grid grid = new Grid
            {
                Name = gridName,
                Background = new SolidColorBrush(Colors.Red),
                Height = 50,
                Width = 50,
                Visibility = Visibility.Visible,
                CanDrag = true,
                Tag = "0, -5"   //Path och steg
            };

            TextBlock textBlock = new TextBlock
            {
                Name = "Textblock_" + gridName,
                Text = "TEST",
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(textBlock);

            grid.DragStarting += PieceDrag;

            return grid;
        }
        List<Grid> createdGrids = new List<Grid>();

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
		{
            foreach (Grid grid in createdGrids)
            {
                if (VisualTreeHelper.GetParent(grid) is Panel parentPanel)
                {
                    parentPanel.Children.Remove(grid);
                }
            }
            player = 0;
			Start.Visibility = Visibility.Collapsed;

            for (int i = 0; i < 4; i++)
            {
                string name = $"p{i}";
                Grid P1 = CreateGrid(name);

                if (i == 0) RedSpot1.Children.Add(P1);
                else if (i == 1) RedSpot2.Children.Add(P1);
                else if (i == 2) RedSpot3.Children.Add(P1);
                else if (i == 3) RedSpot4.Children.Add(P1);

                createdGrids.Add(P1);
            }

            ColorPieces();

			if (PlayersNum == 1 || PlayersNum == 2)
			{
				

				GreenPiece1.Visibility = Visibility.Visible;
				GreenPiece2.Visibility = Visibility.Visible;
				GreenPiece3.Visibility = Visibility.Visible;
				GreenPiece4.Visibility = Visibility.Visible;
			}
			else if (PlayersNum == 3)
			{
				

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

        public int DiceValue;

        private async void DiceButton_Click(object sender, RoutedEventArgs e)
        {
            // Hide the dots before starting the animation
            HideDots();

            // Stop the animation to reset it
            DiceRollStoryboard.Stop();

            // Start the animation
            DiceRollStoryboard.Begin();

			await Task.Delay(2150);
            DiceBox.Visibility = Visibility.Collapsed;
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

            if (DiceValue == 6)
            {
                DiceResult.Text = $"Du slog {DiceValue}! Slå igen!";
                //<------Player movement
            }
            else
            {
                DiceResult.Text = $"Du slog {DiceValue}!";
                //<------Player movement
                TurnSwitch();
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

		///private void DiceButton_Click_1(object sender, RoutedEventArgs e)
		    ///{
			/// End.Visibility = Visibility.Visible;
		    ///}
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
		/* Clickable Button for switch turns.
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
		*/
		private void TurnSwitch()
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
			
			PlayerBox1.Fill = PlayerOneColor;


			PlayerBox2.Fill = PlayerTwoColor;
			GreenPiece1.Fill = PlayerTwoColor;
			GreenPiece2.Fill = PlayerTwoColor;
			GreenPiece3.Fill = PlayerTwoColor;
			GreenPiece4.Fill = PlayerTwoColor;

			PlayerBox3.Fill = PlayerThreeColor;
			BluePiece1.Fill = PlayerThreeColor;
			BluePiece2.Fill = PlayerThreeColor;
			BluePiece3.Fill = PlayerThreeColor;
			BluePiece4.Fill = PlayerThreeColor;

			PlayerBox4.Fill = PlayerFourColor;
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


        private void PieceDragOver(object sender, DragEventArgs e)
        {
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsContentVisible = true;
        }

        private void PieceDrag(UIElement sender, DragStartingEventArgs args)
        {
            if (sender is FrameworkElement element)
            {
                string name = element.Name;

                string tag = element.Tag?.ToString();

                args.Data.Properties.Add("Name", name);
                args.Data.Properties.Add("Tag", tag);

                args.Data.SetText(name); // Default text format
                args.Data.SetData("CustomTagFormat", tag); // Custom format for the tag
            }
        }



        public int PieceNumber;
        private async void PieceDrop(object sender, DragEventArgs e)
        {
            var PieceName = e.DataView.Properties["Name"] as string;
            var PieceTag = e.DataView.Properties["Tag"] as string;

            var draggedElement = (Grid)this.FindName(PieceName);

            if (draggedElement != null)
            {
                var parent = VisualTreeHelper.GetParent(draggedElement) as Panel;

                if (sender is FrameworkElement element && element.Name == "Goal")
                {
                    parent?.Children.Remove(draggedElement);
                    draggedElement.Visibility = Visibility.Collapsed;
                    player++;

                    if (player == 4)
                    {
                        foreach (Grid grid in createdGrids)
                        {
                            if (VisualTreeHelper.GetParent(grid) is Panel parentPanel)
                            {
                                parentPanel.Children.Remove(grid);
                            }
                        }
                        End.Visibility = Visibility.Visible;
                        await PlaySound("ms-appx:///Assets/Win.mp3");
                    }
                }

                else if (sender is Panel dropZone)
                {
                    //
                    string[] PieceTags = PieceTag.Split(',');                //Det ska vara Typ "Path 1, Steg 2"
                    int Path = int.Parse(PieceTags[0].Trim());               //Visar om det är för första eller andra eller...
                    int CurrentPieceSpot = int.Parse(PieceTags[1].Trim());   //Vilket steg det är i från böjran till slutet,
                    //


                    //
                    var SpotTags = (string)dropZone.Tag;
                    string[] SpotTag = SpotTags.Split(",");
                    int SpotPath = int.Parse(SpotTag[Path].Trim());
                    //
                    DiceValue = 6;

                    if (CurrentPieceSpot + DiceValue == SpotPath && dropZone.Children.Count == 1)
                    {
                        await PlaySound("ms-appx:///Assets/Win.mp3");	//Upptagen Plats
                    }

                    //if (CurrentPieceSpot + DiceValue == SpotPath && dropZone.Children.Count == 1)
                    //{
                    //    Push Function
                    //    Checka så om Path [0] är Olika och gör det lagligt att gå på och  parent?.Children.Remove() på något sätt
                    //}

                    if (CurrentPieceSpot + DiceValue == SpotPath && dropZone.Children.Count == 0)       //CurrentPieceSpot + DiceRoll = SpotPath
                    {
                        draggedElement.Tag = $"{Path},{SpotPath}";

                        parent?.Children.Remove(draggedElement);
                        dropZone.Children.Add(draggedElement);
                        await PlaySound("ms-appx:///Assets/Goal.mp3");
                    }
                }
            }
        }

        private static async Task PlaySound(string sound)
        {
            MediaElement SoundPlayer = new MediaElement();
            var soundFile = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri(sound));
            var stream = await soundFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
            SoundPlayer.SetSource(stream, soundFile.ContentType);
            SoundPlayer.Play();
        }
        // --- Rules And About ---
        private void RulesBtn_Click(object sender, RoutedEventArgs e)
		{
			Storyboard RulesAnimationKey = (Storyboard)this.Resources["RulesAnimationKey"];
			RulesAnimationKey.Begin();

			AboutAndRules.Visibility = Visibility.Visible;
			RulesGrid.Visibility = Visibility.Visible;
			AboutBtn.Visibility = Visibility.Visible;
		}
		private void AboutBtn_Click(object sender, RoutedEventArgs e)
		{
			RulesGrid.Visibility = Visibility.Collapsed;
			AboutGrid.Visibility = Visibility.Visible;
			AboutBtn.Visibility = Visibility.Collapsed;
		}
		private void BackToRulesBtn_Click(object sender, RoutedEventArgs e)
		{
			RulesGrid.Visibility = Visibility.Visible;
			AboutGrid.Visibility= Visibility.Collapsed;
			AboutBtn.Visibility = Visibility.Visible;
		}
		private void CloseBtn_Click(object sender, RoutedEventArgs e)
		{
			AboutAndRules.Visibility = Visibility.Collapsed;
			AboutText.Visibility= Visibility.Collapsed;
			AboutGrid.Visibility = Visibility.Collapsed;
			RulesGrid.Visibility= Visibility.Collapsed;
		}
        // ------

        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            // Get the currently selected ToggleButton
            var selectedButton = sender as ToggleButton;

            // Ensure that the parent is a Panel (e.g., StackPanel)
            if (selectedButton?.Parent is Panel parentPanel)
            {
                // Loop through all children of the parent panel
                foreach (var child in parentPanel.Children)
                {
                    // Check if the child is a ToggleButton
                    if (child is ToggleButton toggleButton && toggleButton != selectedButton)
                    {
                        // Uncheck other ToggleButtons
                        toggleButton.IsChecked = false;
                        // Reset background color of unselected buttons
                        toggleButton.Background = new SolidColorBrush(Colors.White);
                    }
                }
            }

            // Change the background color of the selected button
            selectedButton.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void ToggleButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var toggleButton = sender as ToggleButton;

            // Reset background color when unchecked
            toggleButton.Background = new SolidColorBrush(Colors.White);
        }

        private void LoadGameMBtn_Click(object sender, RoutedEventArgs e)
        {
            if (Start.Visibility == Visibility.Visible)
            {
                Start.Visibility = Visibility.Collapsed;
            }
            isLoadMainMenu = true;
            Save.Visibility = Visibility.Visible;
            Savepanel.Visibility = Visibility.Collapsed;
            Loadpanel.Visibility = Visibility.Visible;
        }

        private void Menubtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Visible;
        }

        private void SaveBtnM_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
            Save.Visibility = Visibility.Visible;
            Savepanel.Visibility = Visibility.Visible;
            Loadpanel.Visibility = Visibility.Collapsed;
        }

        private void BackBtnM_Click(object sender, RoutedEventArgs e)
        {
            Start.Visibility = Visibility.Visible;
            Menu.Visibility = Visibility.Collapsed;
        }

        private void LoadGameBtn_Click(object sender, RoutedEventArgs e)
        {
            isLoadMainMenu = false;
            Save.Visibility = Visibility.Visible;
            Menu.Visibility = Visibility.Collapsed;
            Savepanel.Visibility = Visibility.Collapsed;
            Loadpanel.Visibility = Visibility.Visible;
        }

        private void LBackbtn_Click(object sender, RoutedEventArgs e)
        {
            if (isLoadMainMenu)
            {
                Save.Visibility = Visibility.Collapsed;
                Start.Visibility = Visibility.Visible;
                isLoadMainMenu = false;
                // if saved the game isloadmainmenu will be always true so need to fix it
            }
            else
            {
                Save.Visibility = Visibility.Collapsed;
                Menu.Visibility = Visibility.Visible;
            }
        }

        private void BackToMenu_Click(object sender, RoutedEventArgs e)
        {
            Save.Visibility = Visibility.Collapsed;
            if (Menu.Visibility == Visibility.Collapsed)
            {
                Menu.Visibility = Visibility.Visible;
            }
        }

        private void Resumebtn_Click(object sender, RoutedEventArgs e)
        {
            Menu.Visibility = Visibility.Collapsed;
        }

        private void ExitBtn_Click(Object sender, RoutedEventArgs e)
        {
            CoreApplication.Exit();
        }



	}
}
