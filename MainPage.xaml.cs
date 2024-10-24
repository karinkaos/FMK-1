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
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using Windows.UI.Popups;
using Windows.Foundation;

namespace FMK_1
{
    public class GameState
    {
        public int NumberOfPlayers { get; set; }
        public List<Player> Players { get; set; }

        public GameState()
        {
            Players = new List<Player>();
        }
    }

    public class Player
    {
        public string Color { get; set; }
        public List<int> PiecePositions { get; set; }

        public Player()
        {
            PiecePositions = new List<int>(new int[4]); // Assuming 4 pieces per player
        }
    }

    public class Piece
    {
        public int Position { get; set; } // Position of the piece on the board
    }

    public sealed partial class MainPage : Page
    {
        List<Grid> createdGrids = new List<Grid>();

        public int Win_Points;
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
		private int colorIndex = 0;  //Used to go through Color List
		private Random random = new Random();
		private int PlayersNum = 0;
		private int playerTurn = 1;
		private int maxPlayers = 4;
		private bool isLoadMainMenu = false;
		private bool allowTurnSwitch = true;

		public MainPage()
        {
            this.InitializeComponent();
            colorIndex = 0;
            PlayerOneColor = new SolidColorBrush(colors[colorIndex]);
            PlayerTwoColor = new SolidColorBrush(colors[colorIndex + 1]);
            PlayerThreeColor = new SolidColorBrush(colors[colorIndex + 2]);
            PlayerFourColor = new SolidColorBrush(colors[colorIndex + 3]);
			PlaySound("ms-appx:///Assets/startup.wav");
		}

        private void StartGameBtn_Click(object sender, RoutedEventArgs e)
        {
            gameStart();
        }

        private void gameStart()
        {
            //Går genom alla skapade grids i "createdGrids" och tar bort dom, Det är om man startar ett nytt spel så försvinner dom.
            foreach (Grid grid in createdGrids)
            {
                if (VisualTreeHelper.GetParent(grid) is Panel parentPanel)
                {
                    parentPanel.Children.Remove(grid);
                }
            }
            //Win_points checka så fort när 4 pjäser hamnar i mål avslutas spelet, Man borde göra så den checkar om EN spelare har targit bort sina
            //Skapar pjäser för varje spelar, Borde vara en function som gör det bara änligt hur många spelare det är
            //TODO:Skapa function för spelare + bättre namn på grupperna typ första spelare, andra spelare och inte färger
            Win_Points = 0;
            Start.Visibility = Visibility.Collapsed;
            CreatePieces("Red", new SolidColorBrush(Colors.Red), "0,0");
            CreatePieces("Green", new SolidColorBrush(Colors.Green), "1,0");
            CreatePieces("Pink", new SolidColorBrush(Colors.Pink), "2,0");
            CreatePieces("Blue", new SolidColorBrush(Colors.Blue), "3,0");

            //ColorPieces();
        }

        private string GetColorName(SolidColorBrush brush)
        {
            //Kanske Onödigt, behövde någon som fick mig färger på enkel sätt
            if (brush.Color == Colors.Red)
                return "Red";
            if (brush.Color == Colors.Green)
                return "Green";
            if (brush.Color == Colors.Pink)
                return "Pink";
            if (brush.Color == Colors.Blue)
                return "Blue";

            return "UnknownColor";
        }

        public class CustomGrid : Grid
        {
            public string SpotName { get; set; } //function för customgrid för att spara stringen
        }

        private void CreatePieces(string Name, SolidColorBrush color, string Tag)
        {
            for (int i = 0; i < 4; i++)
            {
                string colorName = GetColorName(color);
                string spotName = $"{colorName}Spot{i}";
                var spotGrid = FindName(spotName) as Grid;
                CustomGrid P1 = CreateGrid($"{Name}{i}", color, Tag, spotName);

                spotGrid.Children.Add(P1);
                createdGrids.Add(P1);
                //Borde vara relativt förståbart, lite kronligt hur jag har gjort men funkar, skapar 4 pjäser med vad kan skrev in
            }
        }

        private CustomGrid CreateGrid(string Name, SolidColorBrush color, string Tag, string spotName)
        {
            CustomGrid grid = new CustomGrid
            {
                Name = Name,
                Background = new SolidColorBrush(Windows.UI.Colors.Transparent),
                Height = 50,
                Width = 50,
                Visibility = Visibility.Visible,
                CanDrag = true,
                Tag = Tag
                //Tag är indelat i 2 nummer delat med ett "," för att skilja på dom
                //Det är för första nummeret "[0]" visar vilken färg den tillhör, just nu är 0 röd, 1 grön, 2 rosa, 3 blå, och det tillhör Tag som ligger på stegen (själva spelplanen)
                //det andra nummret är vilken plats är på, den börjar på 0 för första steget är 1 och sista är 44 eller 45 med målet (Men den kan aldrig stå på målet för den försvinner)
                //Så om en pjäs har tags "2,30" så vet man att det tillhör rosa boet eller spelare 3 (Lite problem med färger och vilken spelare som är vem)
                //Och den är på steg 30,

                //Medans stegen har 4 olika nummer, 1 för varje färg som kan ställa sig på den
                //så säg tillex första röda steget, Tag="1,31,21,11" har den,  Men det är första steget för red, 31 för grön, 21 för rosa, 11 för blå
                //(Lite roligt att den har alltid samma sista nummer)
            };
            grid.SpotName = spotName;

            const double scaleFactor = 0.1; // Adjust this scale as needed
            double offsetX = 10; // Adjust offset if needed
            double offsetY = 0;

            //Detta är hur jag fick pjäserna att få sin form, osynlig grid med dessa på som fyller färger
            //Hade ingen anning hur jag skulle ändra pjäsern jag gjorde på figma till detta, försökte göra Path men fixade inte det
            Polygon canvas = new Polygon
            {
                Visibility = Visibility.Visible,
                Fill = color,
                Stroke = new SolidColorBrush(Windows.UI.Colors.Black),
                StrokeThickness = 1,
                Points = new PointCollection
                {
                    new Point(109 * scaleFactor + offsetX, 172 * scaleFactor + offsetY),
                    new Point(87 * scaleFactor + offsetX, 159 * scaleFactor + offsetY),
                    new Point(65 * scaleFactor + offsetX, 136 * scaleFactor + offsetY),
                    new Point(53 * scaleFactor + offsetX, 106 * scaleFactor + offsetY),
                    new Point(53 * scaleFactor + offsetX, 76 * scaleFactor + offsetY),
                    new Point(64 * scaleFactor + offsetX, 48 * scaleFactor + offsetY),
                    new Point(76 * scaleFactor + offsetX, 34 * scaleFactor + offsetY),
                    new Point(91 * scaleFactor + offsetX, 21 * scaleFactor + offsetY),
                    new Point(105 * scaleFactor + offsetX, 11 * scaleFactor + offsetY),
                    new Point(124 * scaleFactor + offsetX, 3 * scaleFactor + offsetY),
                    new Point(157 * scaleFactor + offsetX, 0 * scaleFactor + offsetY),
                    new Point(176 * scaleFactor + offsetX, 3 * scaleFactor + offsetY),
                    new Point(196 * scaleFactor + offsetX, 12 * scaleFactor + offsetY),
                    new Point(212 * scaleFactor + offsetX, 23 * scaleFactor + offsetY),
                    new Point(225 * scaleFactor + offsetX, 38 * scaleFactor + offsetY),
                    new Point(236 * scaleFactor + offsetX, 56 * scaleFactor + offsetY),
                    new Point(240 * scaleFactor + offsetX, 81 * scaleFactor + offsetY),
                    new Point(240 * scaleFactor + offsetX, 103 * scaleFactor + offsetY),
                    new Point(234 * scaleFactor + offsetX, 120 * scaleFactor + offsetY),
                    new Point(228 * scaleFactor + offsetX, 137 * scaleFactor + offsetY),
                    new Point(218 * scaleFactor + offsetX, 146 * scaleFactor + offsetY),
                    new Point(209 * scaleFactor + offsetX, 155 * scaleFactor + offsetY),
                    new Point(200 * scaleFactor + offsetX, 161 * scaleFactor + offsetY),
                    new Point(191 * scaleFactor + offsetX, 167 * scaleFactor + offsetY),
                    new Point(186 * scaleFactor + offsetX, 172 * scaleFactor + offsetY),
                    new Point(288 * scaleFactor + offsetX, 432 * scaleFactor + offsetY),
                    new Point(286 * scaleFactor + offsetX, 441 * scaleFactor + offsetY),
                    new Point(280 * scaleFactor + offsetX, 446 * scaleFactor + offsetY),
                    new Point(274 * scaleFactor + offsetX, 451 * scaleFactor + offsetY),
                    new Point(261 * scaleFactor + offsetX, 456 * scaleFactor + offsetY),
                    new Point(239 * scaleFactor + offsetX, 462 * scaleFactor + offsetY),
                    new Point(207 * scaleFactor + offsetX, 468 * scaleFactor + offsetY),
                    new Point(175 * scaleFactor + offsetX, 471 * scaleFactor + offsetY),
                    new Point(145 * scaleFactor + offsetX, 471 * scaleFactor + offsetY),
                    new Point(111 * scaleFactor + offsetX, 471 * scaleFactor + offsetY),
                    new Point(81 * scaleFactor + offsetX, 467 * scaleFactor + offsetY),
                    new Point(64 * scaleFactor + offsetX, 465 * scaleFactor + offsetY),
                    new Point(40 * scaleFactor + offsetX, 460 * scaleFactor + offsetY),
                    new Point(18 * scaleFactor + offsetX, 451 * scaleFactor + offsetY),
                    new Point(8 * scaleFactor + offsetX, 439 * scaleFactor + offsetY),
                    new Point(3 * scaleFactor + offsetX, 435 * scaleFactor + offsetY),
                    // Closing back to the first point (optional if you want a closed shape)
                    new Point(109 * scaleFactor + offsetX, 172 * scaleFactor + offsetY)
                }
            };

            grid.Children.Add(canvas);

            grid.DragStarting += PieceDrag;

            return grid;

        }

        private void Bts_click(object sender, RoutedEventArgs e)
        {
            Start.Visibility = Visibility.Visible;
            End.Visibility = Visibility.Collapsed;

            colorIndex = 0;
        }
        //Gör en int utanför så kan hämta den för flytta pjäser
        public int DiceValue;

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
			DiceValue = random.Next(1, 7);

			// Show dots based on rolled value
			ShowDots(DiceValue);

			if (DiceValue == 6)
			{
				DiceResult.Text = $"You rolled a {DiceValue}! Roll again!";
				allowTurnSwitch = false;  // Disable turn switch when 6 is rolled
				// <--- Player movement
			}
			else
			{
				DiceResult.Text = $"You rolled a {DiceValue}!";
				// <--- Player movement

				// Only allow turnswitch if player didn't roll a 6 before
				if (!allowTurnSwitch)
				{
					allowTurnSwitch = true;  // Enable turn switch again
				}
				else // Rolled 1-5:
				{					
					TurnSwitch();					
				}
			}
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

		private void ChooseClrSqr_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            Rectangle clickedRectangle = sender as Rectangle;

            clickedRectangle.Fill = new SolidColorBrush(colors[colorIndex]);

            //Trök ihop dessa till en function som borde funka, känns dock att man kan fixa "if" sattsena till något enklare
            if(clickedRectangle.Name == "ChooseClrSqr1") { PlayerOneColor = new SolidColorBrush(colors[colorIndex]); }
            if(clickedRectangle.Name == "ChooseClrSqr2") { PlayerTwoColor = new SolidColorBrush(colors[colorIndex]); }
            if(clickedRectangle.Name == "ChooseClrSqr3") { PlayerThreeColor = new SolidColorBrush(colors[colorIndex]); }
            if(clickedRectangle.Name == "ChooseClrSqr4") { PlayerFourColor = new SolidColorBrush(colors[colorIndex]); }

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
            PlayerBox3.Fill = PlayerThreeColor;
            PlayerBox4.Fill = PlayerFourColor;
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
            //Detta är när man dra något dra bart över något dropbart, tog bort lite Default UI saker som dåligt ut
            //TODO: ta bort att det är ett X när man drar över något man inte kan släppa på
            e.AcceptedOperation = DataPackageOperation.Move;
            e.DragUIOverride.IsGlyphVisible = false;
            e.DragUIOverride.IsCaptionVisible = false;
            e.DragUIOverride.IsContentVisible = true;
        }

        private void PieceDrag(UIElement sender, DragStartingEventArgs args)
        {
            //Hur saker sparars när man börjar dra saker
            if (sender is FrameworkElement element)
            {
                string name = element.Name;
                string tag = element.Tag?.ToString();
                string spotName = (element as CustomGrid)?.SpotName;

                args.Data.Properties.Add("Name", name);
                args.Data.Properties.Add("Tag", tag);
                args.Data.Properties.Add("SpotName", spotName);

                args.Data.SetText(name); // Default text format
                args.Data.SetData("CustomTagFormat", tag); // Custom format for the tag
                args.Data.SetData("CustomSpotFormat", spotName);
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
                if (sender is Panel dropZone)
                {
                    var parent = VisualTreeHelper.GetParent(draggedElement) as Panel;
                    string[] PieceTags = PieceTag.Split(',');                //Det ska vara Typ "Path 1, Steg 2"
                    int Path = int.Parse(PieceTags[0].Trim());               //Visar om det är för första eller andra eller...
                    int CurrentPieceSpot = int.Parse(PieceTags[1].Trim());   //Vilket steg det är i från början till slutet

                    var SpotTags = (string)dropZone.Tag;
                    string[] SpotTag = SpotTags.Split(",");
                    int SpotPath = int.Parse(SpotTag[Path].Trim());         //Delar bara tags på steget som man har dragit till

					if (CurrentPieceSpot + DiceValue == SpotPath && dropZone.Children.Count == 1)
                    {
                        //OM PjäsPlats + Tärningskast ÄR Steget OCH Steget har EXAKT 1 Barn

                        //Letar efter barnet, hämtar dens StartZon i hemmet och Flytter ditt
						Panel child = (Grid)VisualTreeHelper.GetChild(dropZone, 0);
						var ChildTags = (string)child.Tag;
                        string[] ChildTag = ChildTags.Split(',');
                        int ChildPath = int.Parse(ChildTag[0].Trim());

						if(Path != ChildPath)
						{
                            if (child.Parent is Panel parentPanel)
                            {
                                parentPanel.Children.Remove(child);
                                //Lägg tillbacka den i hemmet och sätter dens steg till 0
                                //Detta känns som mycket kod för vad jag gör men kom inte på något annat sätt
                                //Kanske man kunde lägga dom i en array? eller något med att göra som alla "Spots" gör sina barn till 0? vet inte Men Funkar

                                string spotName = ((CustomGrid)child).SpotName;
                                var targetPanel = FindName(spotName) as Panel;
                                targetPanel.Children.Add(child);
                                if(targetPanel.Name == "RedSpot0" || targetPanel.Name == "RedSpot1" || targetPanel.Name == "RedSpot2" || targetPanel.Name == "RedSpot3")
                                {
                                    child.Tag = "0,0";
                                }
                                else if (targetPanel.Name == "GreenSpot0" || targetPanel.Name == "GreenSpot1" || targetPanel.Name == "GreenSpot2" || targetPanel.Name == "GreenSpot3")
                                {
                                    child.Tag = "1,0";
                                }
                                else if (targetPanel.Name == "PinkSpot0" || targetPanel.Name == "PinkSpot1" || targetPanel.Name == "PinkSpot2" || targetPanel.Name == "PinkSpot3")
                                {
                                    child.Tag = "2,0";
                                }
                                else if (targetPanel.Name == "BlueSpot0" || targetPanel.Name == "BlueSpot1" || targetPanel.Name == "BlueSpot2" || targetPanel.Name == "BlueSpot3")
                                {
                                    child.Tag = "3,0";
                                }
                            }
                        }
                    }

                    if (CurrentPieceSpot + DiceValue == SpotPath && dropZone.Children.Count == 0)
                    {
                        //OM PjäsPlats + Tärningskast ÄR Steget OCH Steget har EXAKT 0 Barn

                        //Lägg in den du drog, ta bort dens gamla hem och uppdatera tagen så den säger rätt steg den är på
                        //Och spela ett litet ljud
                        draggedElement.Tag = $"{Path},{SpotPath}";
                        parent?.Children.Remove(draggedElement);
                        dropZone.Children.Add(draggedElement);
                        await PlaySound("ms-appx:///Assets/Goal.mp3");
                    }
                    if (sender is FrameworkElement element && element.Name == "Goal" && CurrentPieceSpot + DiceValue >= SpotPath)
                    {
                        //OM PjäsPlats + Tärningskast ÄR Steget OCH Namnet är EXAKT "Goal"
                        
                        //När man går in i mål, ta bort pjäsern, lägg till ett vinn poäng (Som sakt borde fixas) Och ett litet ljud klipp
                        parent?.Children.Remove(draggedElement);
                        draggedElement.Visibility = Visibility.Collapsed;
                        Win_Points++;

                        if (Win_Points == 4)
                        {
                            //när spelat har slutat
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
                }
            }
        }
        //TODO: Ändra så bara 6 gör som man kan lämna Boet
        //TODO: Ska kan kunna vinna på ett nummer över målet eller ska man ha exakt (Just nu över)
        private static async Task PlaySound(string sound)
        {
            //Spelar ljud som har i "sound"
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
            Menu.Visibility = Visibility.Collapsed;
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
            AboutGrid.Visibility = Visibility.Collapsed;
            AboutBtn.Visibility = Visibility.Visible;
        }
        private void CloseBtn_Click(object sender, RoutedEventArgs e)
        {
            AboutAndRules.Visibility = Visibility.Collapsed;
            AboutText.Visibility = Visibility.Collapsed;
            AboutGrid.Visibility = Visibility.Collapsed;
            RulesGrid.Visibility = Visibility.Collapsed;
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

        //Save Load Function
        private async void SaveBtnS_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSlotIndex != -1) // Ensure a slot is selected
            {
                GameState gameState = new GameState
                {
                    NumberOfPlayers = PlayersNum, // Set your game logic to determine number of players
                    Players = new List<Player>()
                };

                // Populate Players list with colors and positions based on your game logic
                for (int i = 0; i < PlayersNum; i++)
                {
                    var player = new Player
                    {
                        Color = GetPlayerColor(i), // Method to get player color
                        PiecePositions = new List<int>() // Initialize a list for piece positions
                    };

                    // Iterate over each piece index (0 to 3 for 4 pieces)
                    for (int pieceIndex = 0; pieceIndex < 4; pieceIndex++)
                    {
                        int position = GetPiecePosition(i, pieceIndex); // Get position for each piece
                        player.PiecePositions.Add(position); // Add the position to the player's list
                    }

                    gameState.Players.Add(player); // Add the player to the game state
                }

                // Save the game state
                await SaveGameAsync(gameState, selectedSlotIndex);
            }
        }

        private int GetPiecePosition(int playerIndex, int pieceIndex)
        {
            // Ensure the playerIndex and pieceIndex are valid
            if (playerIndex < 0 || playerIndex >= 4) // Assuming 4 players
            {
                Debug.WriteLine($"Invalid playerIndex: {playerIndex}");
                return -1; // Or handle the error as needed
            }

            // Ensure pieceIndex is valid for the player's pieces
            int playerPieceCount = 4; // Assuming each player has 4 pieces
            if (pieceIndex < 0 || pieceIndex >= playerPieceCount)
            {
                Debug.WriteLine($"Invalid pieceIndex: {pieceIndex} for Player {playerIndex}");
                return -1; // Or handle the error as needed
            }

            // Calculate the index of the piece in the createdGrids list
            int pieceGridIndex = playerIndex * playerPieceCount + pieceIndex;

            // Retrieve the piece based on the calculated index
            if (pieceGridIndex < createdGrids.Count)
            {
                Grid piece = createdGrids[pieceGridIndex];

                // Log the Tag for debugging purposes
                Debug.WriteLine($"Piece {pieceGridIndex} Tag: {piece.Tag}");

                // Assuming Tag represents the position in the format "Path,Spot"
                if (piece.Tag is string tag)
                {
                    string[] tags = tag.Split(',');

                    // Ensure there are exactly 2 tags for Path and Spot
                    if (tags.Length == 2)
                    {
                        // Use the Spot as the position you want to return
                        if (int.TryParse(tags[1].Trim(), out int position)) // Parse the second element as the position
                        {
                            Debug.WriteLine($"Player {playerIndex} Piece {pieceIndex} Position: {position}");
                            return position; // Return the integer position
                        }
                        else
                        {
                            Debug.WriteLine($"Failed to parse position from Tag for Piece {pieceGridIndex}. Tag: {tag}");
                        }
                    }
                    else
                    {
                        Debug.WriteLine($"Invalid tag format for Piece {pieceGridIndex}. Expected 2 tags, got {tags.Length}.");
                    }
                }

                Debug.WriteLine($"Player {playerIndex} Piece {pieceIndex} Position could not be determined.");
            }
            else
            {
                Debug.WriteLine($"Piece index {pieceGridIndex} is out of bounds for createdGrids.");
            }

            return -1; // Return -1 if the position cannot be determined
        }

        private async Task SaveGameAsync(GameState gameState, int slotIndex)
        {
            try
            {
                // Get the local folder of your application
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // Create or get the "Saved" folder
                StorageFolder savedFolder = await localFolder.CreateFolderAsync("Saved", CreationCollisionOption.OpenIfExists);

                // Define the file name based on the slot index
                string fileName = $"slot_{slotIndex}.json";

                // Create or replace the file in the Saved folder
                StorageFile file = await savedFolder.CreateFileAsync(fileName, CreationCollisionOption.ReplaceExisting);
                var savedFolder2 = await localFolder.CreateFolderAsync("Saved", CreationCollisionOption.OpenIfExists);
                Debug.WriteLine($"Saved folder path: {savedFolder2.Path}");

                // Serialize the game state to JSON
                string json = JsonConvert.SerializeObject(gameState); // Ensure you have Newtonsoft.Json

                // Write the JSON to the file
                await FileIO.WriteTextAsync(file, json);

                // Show a message box indicating success
                var successDialog = new MessageDialog("Game saved successfully!");
                await successDialog.ShowAsync();

                await CheckSavedGamesAsync();
            }
            catch (Exception ex)
            {
                // Show a message box indicating an error
                var errorDialog = new MessageDialog($"Failed to save game: {ex.Message}");
                await errorDialog.ShowAsync();
            }
        }

        private async void LoadBtnS_Click(object sender, RoutedEventArgs e)
        {
            if (selectedSlotIndex != -1) // Ensure a slot is selected
            {
                GameState loadedGame = await LoadGameAsync(selectedSlotIndex);
                if (loadedGame != null)
                {
                    // Load the game state into your game logic
                    LoadGameState(loadedGame);
                }
                else
                {
                    // Handle the case when the slot is empty (optional)
                    await ShowMessage("Selected slot is empty. Please choose a different slot.");
                }
            }
        }

        private async Task<GameState> LoadGameAsync(int slotIndex)
        {
            GameState gameState = null; // Initialize the game state to null
            string fileName = $"slot_{slotIndex}.json";

            try
            {
                // Get the local folder of your application
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // Access the "Saved" folder
                StorageFolder savedFolder = await localFolder.GetFolderAsync("Saved");

                // Define the full path to the file
                var file = await savedFolder.GetFileAsync(fileName);

                // Read the JSON from the file
                string json = await FileIO.ReadTextAsync(file);

                // Deserialize the JSON to a GameState object
                gameState = JsonConvert.DeserializeObject<GameState>(json);

                // Show a message box indicating success
                var successDialog = new MessageDialog("Game loaded successfully!");
                await successDialog.ShowAsync();
            }
            catch (FileNotFoundException)
            {
                // Show a message box indicating that the file was not found
                var errorDialog = new MessageDialog("Slot is empty or file not found.");
                await errorDialog.ShowAsync();
            }
            catch (Exception ex)
            {
                // Log the error message and show a message box indicating an error
                Debug.WriteLine($"Error loading game: {ex.Message}");
                var errorDialog = new MessageDialog($"Failed to load game: {ex.Message}");
                await errorDialog.ShowAsync();
            }

            return gameState; // Return the loaded game state (or null if loading failed)
        }

        private void LoadGameState(GameState gameState)
        {
            // Check if gameState is null to avoid NullReferenceException
            if (gameState == null)
            {
                Debug.WriteLine("No game state to load.");
                return;
            }

            // Output the number of players
            Debug.WriteLine($"Number of Players: {gameState.NumberOfPlayers}");
            PlayersNum = gameState.NumberOfPlayers;

            // Output each player's data
            for (int i = 0; i < gameState.Players.Count; i++)
            {
                var player = gameState.Players[i];

                // Output player color
                Debug.WriteLine($"Player {i + 1}: Color = {player.Color}");

                // Manually parse the ARGB hex string
                string hexColor = player.Color.TrimStart('#'); // Remove the '#' at the beginning

                // Convert hex string to ARGB values
                byte a = Convert.ToByte(hexColor.Substring(0, 2), 16); // Alpha
                byte r = Convert.ToByte(hexColor.Substring(2, 2), 16); // Red
                byte g = Convert.ToByte(hexColor.Substring(4, 2), 16); // Green
                byte b = Convert.ToByte(hexColor.Substring(6, 2), 16); // Blue

                // Create a Color object
                Color color = Color.FromArgb(a, r, g, b);
                SolidColorBrush playerColorBrush = new SolidColorBrush(color);

                if (i + 1 == 1)
                {
                    PlayerOneColor = playerColorBrush;
                }
                else if (i + 1 == 2)
                {
                    PlayerTwoColor = playerColorBrush; // Ensure PlayerTwoColor is defined
                }
                else if (i + 1 == 3)
                {
                    PlayerThreeColor = playerColorBrush; // Ensure PlayerThreeColor is defined
                }
                else if (i + 1 == 4)
                {
                    PlayerFourColor = playerColorBrush; // Ensure PlayerFourColor is defined
                }

                // Output each piece's position (only up to 4 pieces)
                for (int j = 0; j < player.PiecePositions.Count && j < 4; j++)
                {
                    Debug.WriteLine($"  Piece {j + 1} Position: {player.PiecePositions[j]}");
                }
            }
            gameStart();
        }

        private void InitializePlayers(List<Player> players)
        {
            foreach (var player in players)
            {
                // Logic to initialize each player based on the loaded data
                // For instance, create player pieces and set their positions
                for (int i = 0; i < player.PiecePositions.Count; i++)
                {
                    // Logic to place player pieces on the board
                    // Assuming each piece corresponds to its index in PiecePositions
                    int position = player.PiecePositions[i];
                    PlacePiece(player.Color, position); // Update this method to handle positioning
                }
            }
        }

        private int selectedSlotIndex;
        private void ToggleButton_Checked(object sender, RoutedEventArgs e)
        {
            var selectedButton = sender as ToggleButton;
            selectedSlotIndex = int.Parse(selectedButton.Tag.ToString()); // Assuming Tag holds the index

            // Uncheck other ToggleButtons and reset their background
            if (selectedButton?.Parent is Panel parentPanel)
            {
                foreach (var child in parentPanel.Children)
                {
                    if (child is ToggleButton toggleButton && toggleButton != selectedButton)
                    {
                        toggleButton.IsChecked = false;
                        toggleButton.Background = new SolidColorBrush(Colors.White);
                    }
                }
            }

            // Change the background color of the selected button
            selectedButton.Background = new SolidColorBrush(Colors.LightGray);
        }

        private void PlacePiece(string color, int position)
        {
            // Logic to visually place the piece on the game board based on color and position
        }

        private async Task ShowMessage(string message)
        {
            var dialog = new MessageDialog(message);
            await dialog.ShowAsync();
        }

        private string GetPlayerColor(int playerIndex)
        {
            SolidColorBrush brush;
            switch (playerIndex)
            {
                case 0:
                    brush = PlayerOneColor;
                    break;
                case 1:
                    brush = PlayerTwoColor;
                    break;
                case 2:
                    brush = PlayerThreeColor;
                    break;
                case 3:
                    brush = PlayerFourColor;
                    break;
                default:
                    return "#000000"; // Fallback color as a string
            }
            // Return the color as a hex string (ARGB format)
            return $"#{brush.Color.A:X2}{brush.Color.R:X2}{brush.Color.G:X2}{brush.Color.B:X2}";
        }

        private async Task CheckSavedGamesAsync()
        {
            try
            {
                // Get the local folder of your application
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;

                // Access the "Saved" folder
                StorageFolder savedFolder = await localFolder.GetFolderAsync("Saved");

                // Get all JSON files in the Saved folder
                var files = await savedFolder.GetFilesAsync();

                // Loop through the files to check for saved games
                foreach (var file in files)
                {
                    // Extract the slot index from the file name (e.g., "slot_1.json" -> 1)
                    string fileName = file.Name;
                    if (fileName.StartsWith("slot_") && fileName.EndsWith(".json"))
                    {
                        // Extract the slot number
                        string slotNumberStr = fileName.Substring(5, fileName.Length - 10); // Extracting number part
                        if (int.TryParse(slotNumberStr, out int slotNumber))
                        {
                            // Update the content of the corresponding toggle button
                            switch (slotNumber)
                            {
                                case 1:
                                    SaveSlot1.Content = "Saved 1";
                                    break;
                                case 2:
                                    SaveSlot2.Content = "Saved 2";
                                    break;
                                case 3:
                                    SaveSlot3.Content = "Saved 3";
                                    break;
                                case 4:
                                    SaveSlot4.Content = "Saved 4";
                                    break;
                                case 5:
                                    SaveSlot5.Content = "Saved 5";
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error checking saved games: {ex.Message}");
            }
        }

        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await CheckSavedGamesAsync();
        }
    }
}
