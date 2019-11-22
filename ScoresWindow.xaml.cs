using System.Collections.Generic;
using System.Windows;

namespace Minesweeper
{
    public partial class ScoresWindow : Window
    {
        public static DataController controller = new DataController();

        // load lists of each score type
        List<int> easy = controller.LoadScore(1, new List<int>());
        List<int> medium = controller.LoadScore(2, new List<int>());
        List<int> hard = controller.LoadScore(3, new List<int>());

        public ScoresWindow()
        {
            InitializeComponent();
            SetScores();
        }      
        
        // adds scores to listbox
        public void SetScores()
        {
            bool valid = true;
            int i = 0;
            while (valid) // loop until all scores have been added
            {
                string s = (i + 1) + ".      ";
                if (i >= 9)
                    s = (i + 1) + ".    ";

                if (easy.Count > i)
                    s = s + GetString(easy[i]);
                else
                    s = s + "           ";
                if (medium.Count > i)
                    s = s + GetString(medium[i]) + " ";
                else
                    s = s + "            ";
                if (hard.Count > i)
                    s = s + hard[i];

                listBox.Items.Add(s);
                i++;
                if (easy.Count < i && medium.Count < i && hard.Count < i) // no scores left to add
                {
                    valid = false;
                    listBox.Items.RemoveAt(listBox.Items.Count - 1);
                }                
            }
        }

        // adds a certain amount of spaces to a number depending on number length
        public string GetString(int i)
        {
            if (i >= 100)
                return i.ToString() + "        ";
            else if (i >= 10)
                return i.ToString() + "          ";
            else
                return i.ToString() + "            ";
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            listBox.Height = this.ActualHeight - 90;
        }
    }
}