using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Minesweeper
{
    public partial class OptionsWindow : Window
    {
        public static MainWindow main;

        public static int Difficulty = 0;
        public static string Height;
        public static string Width;
        public static string Mines;
        public string GridColor;
        public string BackColor;

        public OptionsWindow()
        {

        }

        public OptionsWindow(MainWindow w)
        {
            InitializeComponent();
            PopulateColors();
            comboGridColor.SelectedItem = w.gridColor; // load color from mainwindow
            comboBackColor.SelectedItem = w.backColor; // load color from mainwindow
            main = w;

            // load difficulty from mainwindow
            if (w.DifficultyMode == 1)
                rbEasy.IsChecked = true;
            if (w.DifficultyMode == 2)
                rbMedium.IsChecked = true;
            if (w.DifficultyMode == 3)
                rbHard.IsChecked = true;
            if (w.DifficultyMode == 4)
            {
                rbCustom.IsChecked = true;
                txtHeight.Text = Convert.ToString(w.RowCount);
                txtWidth.Text = Convert.ToString(w.ColumnCount);
                txtMines.Text = Convert.ToString(w.MineCount);
            }                
        }

        private void rbEasy_Checked(object sender, RoutedEventArgs e)
        {
            Checked(false, 1);
        }

        private void rbMedium_Checked(object sender, RoutedEventArgs e)
        {
            Checked(false, 2);
        }

        private void rbHard_Checked(object sender, RoutedEventArgs e)
        {
            Checked(false, 3);
        }

        private void rbCustom_Checked(object sender, RoutedEventArgs e)
        {
            Checked(true, 4);
        }

        // disables custom difficulty textboxes if custom is not selected, sets difficulty
        public void Checked(bool check, int dif)
        {
            txtHeight.IsEnabled = check;
            txtWidth.IsEnabled = check;
            txtMines.IsEnabled = check;
            Difficulty = dif;
        }

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            if (rbCustom.IsChecked == true)
            {
                // make sure all required fields have valid values
                if (txtHeight.Text == "" || txtHeight.Text == "0" || txtWidth.Text == "" || txtWidth.Text == "0" || txtMines.Text == "" || txtMines.Text == "0"
                  || Convert.ToInt32(txtWidth.Text) < 3 || Convert.ToInt32(txtWidth.Text) < Convert.ToInt32(txtHeight.Text) || Convert.ToDouble(txtHeight.Text) < Convert.ToDouble(txtWidth.Text) / 1.875)
                    return;
                else
                    main.ChangeOptions(Difficulty, GetGridColor(), GetBackgroundColor(), Convert.ToInt32(txtHeight.Text), Convert.ToInt32(txtWidth.Text), Convert.ToInt32(txtMines.Text));
            } 
            else               
                main.ChangeOptions(Difficulty, GetGridColor(), GetBackgroundColor());

            main.SetFlag(comboGridColor.Text.ToLower());
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void txtHeight_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateText(txtHeight, Height);
        }

        private void txtWidth_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateText(txtWidth, Width);
        }

        private void txtMines_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            ValidateText(txtMines, Mines);
        }

        // validates custom difficulty textboxes (prevents non-numeric entry)
        // called during the Text_Changed events
        // returns old value if new value fails to convert to int
        private void ValidateText(TextBox txtBox, string s)
        {
            if (txtBox.Text == "")
            {
                s = "0";
                return;
            }
            try
            {
                Convert.ToInt32(txtBox.Text);
            }
            catch
            {
                txtBox.Text = s;
                return;
            }
            s = txtBox.Text;
        }

        // adds colors to comboboxes
        public void PopulateColors()
        {
            foreach (Control c in grid.Children)
                if (c.GetType() == typeof(ComboBox))
                {
                    (c as ComboBox).Items.Add("Blue");
                    (c as ComboBox).Items.Add("Green");
                    (c as ComboBox).Items.Add("Orange");
                    (c as ComboBox).Items.Add("Yellow");
                    (c as ComboBox).Items.Add("Purple");
                    (c as ComboBox).Items.Add("Gray");
                    (c as ComboBox).Items.Add("Black");
                }
            comboBackColor.Items.Add("White");
        }

        // gets a string and returns the corresponding color
        public Brush GetColor(string s)
        {
            Brush b = Brushes.Black;
            if (s == "White")
                return Brushes.White;
            if (s == "Blue")
                return Brushes.Blue;
            if (s == "Green")
                return Brushes.Green;
            if (s == "Orange")
                return Brushes.Orange;
            if (s == "Yellow")
                return Brushes.Yellow;
            if (s == "Purple")
                return Brushes.Purple;
            if (s == "Gray")
                return Brushes.Gray;
            return b;
        }

        public Brush GetGridColor()
        {
            Brush b = GetColor(comboGridColor.SelectedItem.ToString());
            main.gridColor = comboGridColor.SelectedItem.ToString();
            return b;
        }

        public Brush GetBackgroundColor()
        {
            Brush b = GetColor(comboBackColor.SelectedItem.ToString());
            main.backColor = comboBackColor.SelectedItem.ToString();
            return b;
        }
    }
}
