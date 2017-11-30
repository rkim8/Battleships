using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace rk
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        BoatGenerator boatGen = new BoatGenerator();

        List<Point[]> boatLocations = null;
        Button[,] CPUButtons = new Button[10,10];

        Button[,] PlayerButtons = new Button[10, 10];

        int PlayerBoatsSunk = 0;
        int CPUBoatsSunk = 0;

        int[] BoatLength = new int[] { 1,2,3,4,5 };
        Direction boatDirection = Direction.North;
        int boatCount = 0;

        bool gameOver = false;

        Brush baseColor = new SolidColorBrush(Colors.Black);
        Brush baseWritingColor = new SolidColorBrush(Colors.Black); 
        Brush clickedColor = new SolidColorBrush(Color.FromRgb(255, 182, 193));
        Brush vicinityColor = new SolidColorBrush(Colors.White);
        Brush subColor = new SolidColorBrush(Colors.Red);

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            

           
            boatLocations = boatGen.CalculateBoats();
            CreateCPUButtons();
            CreatePlayerButtons();
            WritePoints();
            //PrintPoints();
        }

        public MainWindow()
        {
            InitializeComponent();
        }

        public void PrintPoints() 
        {
            string conc = "";
            foreach (Point[] pArr in boatLocations)
            {
                foreach (Point p in pArr)
                {
                    conc += p.X + " " + p.Y + "  ";
                }
                conc += "    ";
            }
            MessageBox.Show(conc);
        }

        private void CPUButtons_Click(object sender, RoutedEventArgs e)
        {
            if (boatCount != BoatLength.Length)
            {
                //MessageBox.Show("Place ALL your boats!");
                playerStatus.Content = "Place ALL your boats!";
                return;
            }
            if (gameOver)
            {
                return;
            }
            Button tempButton = (Button)sender; //tempbutton takkinn sem ýtt er á
            Cell c = (Cell)tempButton.Tag;

            if (c.BoatType == Boat.None)
            {
                //MessageBox.Show("Miss");
                playerStatus.Content = "You Missed!";
                tempButton.Background = Brushes.Blue;
            }
            else 
            {
                //MessageBox.Show("Hit");
                playerStatus.Content = "You Hit!";
                c.BoatState = State.Sunk;
                tempButton.Background = Brushes.Red;
                if (CheckSunk(CPUButtons,tempButton))
                {
                    //MessageBox.Show("You sunk boat: " + c.BoatType.ToString());
                    playerStatus.Content = "You sunk the " + EnumHelper.ToString(c.BoatType);
                    CPUBoatsSunk++;
                    if (CheckWins(Player.Player))
                    {
                        //MessageBox.Show("Nice, you won");
                        playerStatus.Content = "Nice, you won";
                        GameOver(Player.Player);
                    }

                }
            }

            CPUMove();


        }

        private void CPUMove() 
        {
            Point p = BoatFinder.GetPoint();
            Button tempButton = PlayerButtons[p.X, p.Y];
            Cell c = (Cell)tempButton.Tag;
            if (c.BoatType != Boat.None)
            {
                CPUStatus.Content = "CPU Hit!";
                tempButton.Background = Brushes.Red;
                BoatFinder.BoatFound(p, c);
                c.BoatState = State.Sunk;
                if (CheckSunk(PlayerButtons, tempButton))
                {
                    //MessageBox.Show("You lost your: " + c.BoatType.ToString());
                    CPUStatus.Content = "You lost your " + EnumHelper.ToString(c.BoatType);
                    BoatFinder.BoatSunk(c);
                    PlayerBoatsSunk++;
                    if (CheckWins(Player.CPU))
                    {
                        //MessageBox.Show("The compuuter won!");
                        CPUStatus.Content = "You lost";
                        GameOver(Player.CPU);
                    }
                }
            }
            else
            {
                CPUStatus.Content = "CPU Missed!";
                tempButton.Background = Brushes.Blue;
                BoatFinder.BoatNotFound(c);
            }
        }

        private bool CheckWins(Player player)
        {
            switch (player)
            {
                case Player.Player:
                    return CPUBoatsSunk >= this.BoatLength.Length;
                case Player.CPU:
                    return PlayerBoatsSunk >= this.BoatLength.Length;
                default:
                    return false;
            }
        }

        private void GameOver(Player winner) 
        {
            gameOver = true;
            if (winner == Player.Player)
            {
                MessageBox.Show("The winner is: you! :)");
            }
            else if (winner == Player.CPU)
            {
                MessageBox.Show("The winner is: The Computer :(");
            }
        }

        private bool CheckSunk(Button[,] haystack, Button tempButton) 
        {
            Cell c = (Cell)tempButton.Tag;
            if (c.BoatType == Boat.PatrolBoat) return true;

            Point coords = CoordinatesOfButton(haystack, tempButton);
            List<Direction> directions = new List<Direction>();
            for (int i = 0; i < 4; i++)
            {
                Direction dx = (Direction)i;
                Point p = Point.CreatePointFrom(coords, 1, dx);
                if (Point.ValidatePoint(p) && ((Cell)haystack[p.X, p.Y].Tag).BoatType == c.BoatType) directions.Add(dx);
            }
            int sunkCount = 1;
            foreach (Direction dx in directions)
            {
                Cell boatCell = c;
                for (int i = 1; boatCell.BoatType == c.BoatType; i++)
                {
                    Point p = Point.CreatePointFrom(coords, i, dx);
                    if (!Point.ValidatePoint(p)) break;
                    boatCell = (Cell)haystack[p.X, p.Y].Tag;
                    if (boatCell.BoatState == State.Sunk) sunkCount++;
                }
            }
            if(sunkCount == BoatLength[(int)c.BoatType]) return true;
            return false;
            
        }

        Random colorGen = new Random();
        private void WritePoints() 
        {
            int boatCount = 0;
            foreach (Point[] pArr in boatLocations)
            {
                foreach (Point p in pArr)
                {

                    //CPUButtons[p.X, p.Y].Background = Brushes.White;
                    CPUButtons[p.X, p.Y].Tag = new Cell(Player.CPU, p ,(Boat) boatCount, State.Floating);
                }
                boatCount++;
            }
        }

        private void CreateCPUButtons() {
            int y = 0;
            int x = 0;
            int width = 35;
            int height = 35;
            int xMargin = 3;
            int yMargin = 3;

            CPUGrid.Children.Clear();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {

                    Button tempButton = new Button();
                    tempButton.Height = width;
                    tempButton.Width = height;
                    tempButton.Margin = new Thickness(x, y, 0, 0);
             
                    tempButton.VerticalAlignment = VerticalAlignment.Top;
                   
                    tempButton.HorizontalAlignment = HorizontalAlignment.Left;
                  
                    tempButton.Background = baseColor;
                    tempButton.Foreground = baseWritingColor;
                    tempButton.Tag = new Cell(Player.CPU, new Point(i,j));
                    tempButton.Style = Resources["MouseOverButtonStyle"] as Style;
                    

                    tempButton.Focusable = false;
                    tempButton.Name = "CPUButton" + i + "" + j;
                    tempButton.Click += new RoutedEventHandler(CPUButtons_Click);

                  
                    CPUGrid.Children.Add(tempButton);
                    CPUButtons[i, j] = tempButton;

                    x = x + width + xMargin;
                }
                y = y + height + yMargin;
                x = 0;
            }
        }

        private void CreatePlayerButtons()
        {
            int y = 0;
            int x = 0;
            int width = 35;
            int height = 35;
            int xMargin = 3;
            int yMargin = 3;

            playerGrid.Children.Clear();
            for (int i = 0; i < 10; i++)
            {
                for (int j = 0; j < 10; j++)
                {

                    Button tempButton = new Button();
                    tempButton.Height = width;
                    tempButton.Width = height;
                    tempButton.Margin = new Thickness(x, y, 0, 0);
                    tempButton.VerticalAlignment = VerticalAlignment.Top;
                    tempButton.HorizontalAlignment = HorizontalAlignment.Left;
                    tempButton.Background = baseColor;
                    tempButton.Foreground = baseWritingColor;
                    tempButton.Tag = new Cell(Player.Player, new Point(i,j));
                    tempButton.Focusable = false;
                    tempButton.Name = "PlayerButton" + i + "" + j;
                    tempButton.Style = Resources["MouseOverButtonStyle"] as Style;
                    tempButton.MouseEnter += new MouseEventHandler(playerButtons_MouseEnter);
                    tempButton.MouseLeave += new MouseEventHandler(playerButtons_MouseLeave);
                    tempButton.Click += new RoutedEventHandler(playerButtons_Click);
                    tempButton.MouseRightButtonUp += new MouseButtonEventHandler(playerButtons_MouseRightButtonUp);

                    PlayerButtons[i, j] = tempButton;
                    playerGrid.Children.Add(tempButton);

                    x = x + width + xMargin;
                }
                y = y + height + yMargin;
                x = 0;
            }
        }

        private void playerButtons_MouseRightButtonUp(object sender, MouseEventArgs e)
        {
            int nextDirection = ((int)boatDirection) + 1;
            if (nextDirection > 3) nextDirection = 0;
            playerButtons_MouseLeave(sender, null);
            boatDirection = EnumHelper.NumToEnum<Direction>(nextDirection);
            playerButtons_MouseEnter(sender, null);
        }

        private void playerButtons_Click(object sender, RoutedEventArgs e)
        {
            if (boatCount > BoatLength.Length - 1) return;
            Point coords = CoordinatesOfButton(PlayerButtons, (Button)sender);
            Brush brushToUse = Brushes.White;
            bool error;
            Point[] points = CreatePlayerPoints(coords, BoatLength[boatCount], out error);
            
            if (points.Length != BoatLength[boatCount] || error) return;
            foreach (Point p in points)
            {
                PlayerButtons[p.X, p.Y].Background = brushToUse;
                Cell c = (Cell)PlayerButtons[p.X, p.Y].Tag;
                // = new Cell(Player.CPU,(Boat) boatCount,State.Floating);
                c.BoatState = State.Floating;
                c.BoatType = (Boat) boatCount;
            }
            boatCount++;
            
        }

        private void playerButtons_MouseLeave(object sender, MouseEventArgs e)
        {
            if (boatCount > BoatLength.Length - 1) return;
            Point coords = CoordinatesOfButton(PlayerButtons,(Button)sender);
            Brush brushToUse = Brushes.Black;
            bool error;
            Point[] points = CreatePlayerPoints(coords, BoatLength[boatCount], out error);
            foreach (Point p in points)
            {
                PlayerButtons[p.X, p.Y].Background = brushToUse;
            }
            
        }

        private void playerButtons_MouseEnter(object sender, MouseEventArgs e) 
        {
            if (boatCount > BoatLength.Length - 1) return;
            Point coords = CoordinatesOfButton(PlayerButtons, (Button) sender);
            Color c = (Color)ColorConverter.ConvertFromString("#808080");
            Brush brushToUse = new SolidColorBrush(c);
            bool error;
            Point[] points = CreatePlayerPoints(coords, BoatLength[boatCount], out error);
            if (points.Length != BoatLength[boatCount] || error) brushToUse = Brushes.Red;
            foreach (Point p in points)
	        {
                PlayerButtons[p.X, p.Y].Background = brushToUse;
	        }
            
        }

        private Point CoordinatesOfButton(Button[,] haystack,Button find) 
        {
            int w = haystack.GetLength(0); // width
            int h = haystack.GetLength(1); // height

            for (int x = 0; x < w; ++x)
            {
                for (int y = 0; y < h; ++y)
                {
                    if (haystack[x, y].Equals(find))
                        return new Point(x, y);
                }
            }

            return new Point();
        }

        private Point[] CreatePlayerPoints(Point initialPoint,int length, out bool error) 
        {
            error = false;
            List<Point> points = new List<Point>();
            if (ValidatePlayerPoint(initialPoint)) points.Add(initialPoint);
            if (!CheckSurroundings(initialPoint)) error = true;
            if (length == 0)
            {
                error = true;
                return points.ToArray();
            }
            for (int i = 1; i < length; i++)
            {
                Point tempPoint = Point.CreatePointFrom(initialPoint, i, boatDirection);
                if (!ValidatePlayerPoint(tempPoint))
                {
                    error = true;
                    return points.ToArray();
                }
                if (!CheckSurroundings(tempPoint)) error = true;
                points.Add(tempPoint);
            }
            return points.ToArray();
        }

        private bool CheckSurroundings(Point p)
        {
            Point[] surroundings = { new Point(p.X - 1, p.Y), new Point(p.X, p.Y - 1), new Point(p.X, p.Y + 1), new Point(p.X + 1, p.Y) };
            foreach (Point check in surroundings)
            {
                if (!Point.ValidatePoint(check)) continue;
                if (!ValidatePlayerPoint(check)) return false;
            }
            return true;
        }

        private bool ValidatePlayerPoint(Point p) 
        {
            if (!Point.ValidatePoint(p)) return false;
            if (((Cell)PlayerButtons[p.X, p.Y].Tag).BoatType != Boat.None) return false;
            return true;
        }

        private void newGame_Click(object sender, RoutedEventArgs e)
        {
            boatGen = new BoatGenerator();
            boatLocations = null;
            CPUButtons = new Button[10, 10];
            PlayerButtons = new Button[10, 10];
            PlayerBoatsSunk = 0;
            CPUBoatsSunk = 0;
            boatDirection = Direction.North;
            boatCount = 0;
            gameOver = false;
            boatLocations = boatGen.CalculateBoats();
            CPUGrid.Children.Clear();
            CreateCPUButtons();
            playerGrid.Children.Clear();
            CreatePlayerButtons();
            WritePoints();
        }

        private void exitGame_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown(0);
        }
    }
}
