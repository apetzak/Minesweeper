using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper
{
    public class Square : Button
    {
        public int X; // horizontal coordinate in grid
        public int Y; // vertical coordinate in grid
        public bool IsMine = false;
        public bool Flipped = false; // true = has been left clicked (uncovered)
        public int SurroundingMines; // number of mines in the 8 surrounding spots
        public bool IsBlank = true;       // 
        public bool IsFlagged = false;    // only 1 of these is true at a time
        public bool IsQuestioned = false; //
        
        public Square(int x, int y, double fontSize = 0, Brush color = null)
        {
            X = x;
            Y = y;
            if (fontSize != 0)
                FontSize = fontSize;
            Background = color;
            FontWeight = FontWeights.Bold;
            Focusable = false;
            IsEnabled = true;
        }

        // changes various attributes
        public void Transform(bool isEnabled, Brush color, Image content = null, double fontSize = 0)
        {
            if (color != null)
                Background = color;
            if (content != null)
                Content = content;
            if (fontSize != 0)
                FontSize = fontSize;
            IsEnabled = isEnabled;
        }

        // sets image on square and increments/decrements the mines label
        public void ChangeContent(bool blank, bool flagged, bool questioned, Image content, Label lbl, int i)
        {
            IsBlank = blank;
            IsFlagged = flagged;
            IsQuestioned = questioned;
            Content = content;
            lbl.Content = (Convert.ToInt32(Convert.ToString(lbl.Content)) + i);
        }

        public void SetFontColor(int mines)
        {
            GetColor(mines, 1, Brushes.DodgerBlue);
            GetColor(mines, 2, Brushes.MediumSeaGreen);
            GetColor(mines, 3, Brushes.IndianRed);
            GetColor(mines, 4, Brushes.Navy);
            GetColor(mines, 5, Brushes.DarkOrange);
            GetColor(mines, 6, Brushes.Aqua);
            GetColor(mines, 7, Brushes.Brown);
            GetColor(mines, 8, Brushes.Black);
        }

        // sets color if mine count matches
        private void GetColor(int mines, int number, Brush color)
        {
            if (mines == number)
                Foreground = color;
        }
    }
}
