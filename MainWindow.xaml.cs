using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

namespace Minesweeper
{
    public partial class MainWindow : Window
    {
        // global variables
        public List<Square> squaresToFlip = new List<Square>();  // used to uncover surrounding squares when an empty (no number) square is clicked
        public DispatcherTimer timer = new DispatcherTimer();    // tracks game time
        public DataController controller = new DataController(); // loads/saves data for scores and settings
        public bool GameIsActive = false;
        public int time = 0;       // timer count     
        public int RowCount;       // grid height
        public int ColumnCount;    // grid width
        public int MineCount;      // number of mines to be set on the grid
        public int DifficultyMode; // 1 = easy, 2 = medium, 3 = hard, 4 = custom
        public Brush GridColor = Brushes.Blue;        // 
        public string gridColor = "Blue";             // 
        public string backColor = "White";            // default colors
        public string Flag = "flag_blue.png";         // 
        public string Question = "question_blue.png"; // 

        // set file name for flag and question image
        public void SetFlag(string s)
        {
            Flag = String.Format("flag_{0}.png", s);
            Question = String.Format("question_{0}.png", s);
        }

        public MainWindow()
        {          
            InitializeComponent();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += TimerTick;
            //controller.CreateTables();   // create score and settings tables
            controller.LoadSettings(this); // load settings (colors, grid size, mine count, window size, etc.)
            lblMines.Content = MineCount;  // set mine count on label
            PopulateGrid();
        }

        public void PopulateGrid()
        {
            // create grid
            for (int i = 0; i < RowCount; i++)
                grid.RowDefinitions.Add(new RowDefinition());
            for (int i = 0; i < ColumnCount; i++)
                grid.ColumnDefinitions.Add(new ColumnDefinition());       

            // add squares to grid
            for (int i = 0; i < RowCount; i++)
                for (int j = 0; j < ColumnCount; j++)
                {                   
                    Square s = new Square(i, j, 0, GridColor);
                    s.Click += Left_Click;
                    s.MouseRightButtonDown += Right_Click;
                    Grid.SetRow(s, i);
                    Grid.SetColumn(s, j);
                    grid.Children.Add(s);
                }

            // lay mines
            Random rand = new Random();
            int mines = 0;
            while (mines < MineCount) // keep adding until MineCount is reached
            {
                int randomIndex = rand.Next(0, grid.Children.Count - 1); // get random number
                if (!(grid.Children[randomIndex] as Square).IsMine)      // only add if its not already a mine
                {
                    (grid.Children[randomIndex] as Square).IsMine = true;
                    mines++; // increment mines if added
                }
            }
       
            // get number of surrounding mines for each square
            foreach (Square sq in grid.Children)
            {
                mines = 0;           
                int x = sq.X;
                int y = sq.Y;

                foreach (Square s in grid.Children)
                {
                    if (!s.IsMine)
                        continue;

                    // check surrounding squares and increment mines
                    if (s.X == x - 1 && s.Y == y)
                        mines++;
                    else if (s.X == x - 1 && s.Y == y - 1)
                        mines++;
                    else if (s.X == x - 1 && s.Y == y + 1)
                        mines++;
                    else if (s.X == x + 1 && s.Y == y)
                        mines++;
                    else if (s.X == x + 1 && s.Y == y - 1)
                        mines++;
                    else if (s.X == x + 1 && s.Y == y + 1)
                        mines++;
                    else if (s.X == x && s.Y == y - 1)
                        mines++;
                    else if (s.X == x && s.Y == y + 1)
                        mines++;
                }
                sq.SurroundingMines = mines;
                sq.SetFontColor(mines);
            }
        }

        private void TimerTick(object sender, EventArgs e)
        {
            time++;
            lblTime.Content = time; // increment label time

            if (time == 999) // maxiumum time allowed
                timer.Stop();
        }

        public void StartTimer()
        {
            if (GameIsActive == false)
            {
                timer.Start();
                GameIsActive = true;
            }
        }

        private void Left_Click(object sender, RoutedEventArgs e)
        {
            Square square = (sender as Square);
            StartTimer(); // start game if !GameIsActive

            if (square.IsFlagged || square.IsQuestioned)
                return;

            square.Transform(false, Brushes.White); // disable square and set color to white

            if (square.IsMine)
            { 
                // game over
                timer.Stop();
                GameIsActive = false;

                foreach (Square s in grid.Children)
                {
                    s.Click -= Left_Click; // remove events
                    s.MouseRightButtonDown -= Right_Click;

                    if (s.IsMine)
                    {
                        s.IsEnabled = false;

                        // reveal mine (set image)
                        if (s.IsFlagged)
                            s.Content = GetImage(Flag);
                        else
                            s.Content = GetImage("mine.png");
                    }
                }
                square.Content = GetImage("crossmine.jpg"); // set cross image on mine that was clicked
            }
            else
            {
                // flip square
                square.Flipped = true;
                if (square.SurroundingMines != 0)
                {
                    square.Content = square.SurroundingMines; // set number on square
                }             
                else
                {
                    // uncover surrounding squares
                    squaresToFlip.Add(new Square(square.X, square.Y)); // add copy of square to squaresToFlip

                    bool AddSquares = true;
                    while (AddSquares)
                    {
                        int count = squaresToFlip.Count;
                        AddSquaresToFlip(); // loops through squaresToFlip and adds surrounding mines

                        if (squaresToFlip.Count == count) // exit loop if no new squares were added
                            AddSquares = false;
                    }

                    // loop through both collections of squares and find match, then uncover the grid square
                    foreach (Square s in squaresToFlip)
                    {
                        foreach (Square _s in grid.Children)
                        {
                            if (_s.X == s.X && _s.Y == s.Y)
                            {
                                _s.Transform(false, Brushes.White);
                                if (_s.SurroundingMines != 0)
                                    _s.Content = _s.SurroundingMines;
                                break;
                            }
                        }
                    }
                    squaresToFlip = new List<Square>();
                }
            }

            // check if game is finished
            bool finished = true;
            foreach (Square s in grid.Children)
            {
                if (!s.Flipped && !s.IsMine)
                {
                    finished = false;
                    break; // exit loop if any non-mine is not flipped
                }
            }
              
            if (finished) // game has been completed
            {          
                foreach (Square s in grid.Children)
                    s.Transform(false, null); // disable square and remove color

                timer.Stop(); // stop game
                controller.InsertScore(time, DifficultyMode); // add score
            }
        }

        // adds squares to squaresToFlip
        // gets called until no more surrounding squares are found
        public void AddSquaresToFlip()
        {
            List<Square> squaresToAdd = new List<Square>();
            foreach (Square _s in squaresToFlip)
            {
                if (_s.SurroundingMines != 0) // ignore squares with a number
                    continue;

                foreach (Square s in grid.Children)
                {
                    if (s.IsMine || s.Flipped) // ignore mines and flipped squares
                        continue;

                    // check 8 surrounding squares and mark flipped if match is found
                    if (s.X == _s.X - 1 && s.Y == _s.Y)
                        s.Flipped = true;
                    else if (s.X == _s.X - 1 && s.Y == _s.Y - 1)
                        s.Flipped = true;
                    else if (s.X == _s.X - 1 && s.Y == _s.Y + 1)
                        s.Flipped = true;
                    else if (s.X == _s.X + 1 && s.Y == _s.Y)
                        s.Flipped = true;
                    else if (s.X == _s.X + 1 && s.Y == _s.Y - 1)
                        s.Flipped = true;
                    else if (s.X == _s.X + 1 && s.Y == _s.Y + 1)
                        s.Flipped = true;
                    else if (s.X == _s.X && s.Y == _s.Y - 1)
                        s.Flipped = true;
                    else if (s.X == _s.X && s.Y == _s.Y + 1)
                        s.Flipped = true;

                    if (s.Flipped == true) // add if it was flipped
                        squaresToAdd.Add(s);
                }
            }
            foreach (Square s in squaresToAdd)
                squaresToFlip.Add(s);
        }

        // change image on right click
        // assume that Right_Click event handler has been removed from all uncovered squares
        private void Right_Click(object sender, RoutedEventArgs e)
        {
            Square square = (sender as Square);
            StartTimer(); // start game if !GameIsActive

            if (square.IsBlank && Convert.ToInt32(lblMines.Content) > 0)
                square.ChangeContent(false, true, false, GetImage(Flag), lblMines, -1);

            else if (square.IsFlagged)
                square.ChangeContent(false, false, true, GetImage(Question), lblMines, 1);    
                    
            else if (square.IsQuestioned)
                square.ChangeContent(true, false, false, null, lblMines, 0);                                        
        }

        // takes image name as string and returns image from directory folder
        public Image GetImage(string name)
        {
            string path = System.IO.Path.Combine(Environment.CurrentDirectory, @"Images\" + name);
            Image image = new Image();
            image.Source = new BitmapImage(new Uri(path));
            return image;
        }

        private void BtnOptions_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow w = new OptionsWindow(this);
            w.Show();          
        }

        private void BtnHiscores_Click(object sender, RoutedEventArgs e)
        {
            ScoresWindow w = new ScoresWindow();
            w.Show();
        }

        // restart game
        private void BtnRestart_Click(object sender, RoutedEventArgs e)
        {           
            if (!GameIsActive) // only enable events if game is not active
                foreach (Square s in grid.Children)
                {
                    s.Click += Left_Click;
                    s.MouseRightButtonDown += Right_Click;
                }              

            foreach (Square s in grid.Children)
            {
                s.Flipped = false;
                s.Transform(true, GridColor); // enable and set color
                s.Content = ""; // clear content
            }

            ResetGame();
        }

        private void BtnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame();
        }

        private void ResetGame()
        {
            timer.Stop();
            time = 0;
            GameIsActive = false;
            lblMines.Content = MineCount;
            lblTime.Content = "0";                  
        }

        public void NewGame()
        {
            ResetGame();
            grid.Children.Clear();
            grid.RowDefinitions.Clear();
            grid.ColumnDefinitions.Clear();
            PopulateGrid();
            SetFontSize();
        }

        // called when options are changed from the OptionsWindow
        public void ChangeOptions(int difficulty, Brush gridColor, Brush backgroundColor, int height = 0, int width = 0, int mines = 0)
        {
            SetOptions(difficulty, 1, 9, 9, 10);
            SetOptions(difficulty, 2, 16, 16, 40);
            SetOptions(difficulty, 3, 16, 30, 99);
            SetOptions(difficulty, 4, height, width, mines);
            SizeChanged -= Window_SizeChanged; // temporarily disable event so SetGridSize() isn't called twice
            SetGridSize();
            SizeChanged += Window_SizeChanged;
            GridColor = gridColor;
            Background = backgroundColor;          
            NewGame(); // start new game
        }

        public void SetOptions(int difficulty, int dif, int rows, int columns, int mines)
        {
            if (difficulty != dif) // return if difficulty doesn't match
                return;
            DifficultyMode = dif;
            RowCount = rows;
            ColumnCount = columns;
            MineCount = mines;
        }

        // sets width of MainWindow and grid then calls SetFontSize()
        public void SetGridSize()
        {
            grid.Width = grid.ActualHeight * Convert.ToDouble(ColumnCount) / Convert.ToDouble(RowCount); // set grid dimensions
            this.Width = this.WindowState != WindowState.Maximized ? this.Width = grid.Width + 35 : this.Width;
            SetFontSize();
        }      

        // calculates and sets font size on all squares
        // called during SetGridSize() and NewGame()
        public void SetFontSize()
        {
            double fontSize = grid.Width / ColumnCount / 1.5; // get font size
            foreach (Square s in grid.Children)
                s.FontSize = fontSize;
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            SetGridSize();
        }

        private void mainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            int maximized = this.WindowState == WindowState.Maximized ? 1 : 0; // used to save maximized setting
            controller.SaveSettings(DifficultyMode, MineCount, RowCount, ColumnCount, Convert.ToInt32(this.Width), Convert.ToInt32(this.Height), maximized, gridColor, backColor);
            Environment.Exit(1); // force close application in case the score or options window was opened
        }
    }
}