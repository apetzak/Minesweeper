using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace Minesweeper
{
    public class DataController
    {
        public SQLiteConnection dbConnection = new SQLiteConnection("Data Source=MyDatabase.sqlite;Version=3;");
        public SQLiteCommand command = new SQLiteCommand();
        public SQLiteDataReader reader;
        public string sql = ""; // used for all queries

        public DataController()
        {
            command = new SQLiteCommand(sql, dbConnection);
        }

        // opens dbConnection and executes query
        // used for all inserting queries
        public void ExecuteNonQuery()
        {
            try
            {
                dbConnection.Open();
                command = new SQLiteCommand(sql, dbConnection);
                command.ExecuteNonQuery();
                dbConnection.Close();
            }
            catch (Exception e)
            {
                System.Windows.MessageBox.Show(e.ToString());
            }
        }

        // creates database tables, only used once
        public void CreateTables()
        {
            sql = "create table scores (score INT, type INT)";
            ExecuteNonQuery();
            sql = "create table settings (difficulty INT, mines INT, rows INT, columns INT, width INT, height INT, maximized INT, gridColor VARCHAR(20), backgroundColor VARCHAR(20))";
            ExecuteNonQuery();
        }

        // called when a non-custom game is won
        public void InsertScore(int score, int type)
        {
            if (type == 4) // return if game is custom difficulty
                return;
            sql = String.Format("insert into scores (score, type) values('{0}', '{1}')", score, type);
            ExecuteNonQuery();
        }

        // returns a list of scores for a certain difficulty
        public List<int> LoadScore(int type, List<int> list)
        {
            sql = String.Format("select score from scores where type='{0}' order by score asc", type);

            dbConnection.Open();
            command = new SQLiteCommand(sql, dbConnection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                list.Add(reader.GetInt32(0));
            }
            dbConnection.Close();
            list.Sort();
            return list;
        }

        // deletes and replaces the settings in the database
        public void SaveSettings(int difficulty, int mines, int rows, int columns, int width, int height, int maximized, string gridColor, string backgroundColor)
        {
            sql = "delete from settings";
            ExecuteNonQuery();
            sql = String.Format("insert into settings (difficulty, mines, rows, columns, width, height, maximized, gridColor, backgroundColor) " +
                                "values('{0}', '{1}', '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}')", difficulty, mines, rows, columns, width, height, maximized, gridColor, backgroundColor);
            ExecuteNonQuery();
        }

        // loads settings and sets the values on the mainwindow
        public void LoadSettings(MainWindow window)
        {
            sql = "select * from settings";
            dbConnection.Open();
            command = new SQLiteCommand(sql, dbConnection);
            reader = command.ExecuteReader();

            while (reader.Read())
            {
                window.DifficultyMode = reader.GetInt32(0);
                window.MineCount = reader.GetInt32(1);
                window.RowCount = reader.GetInt32(2);
                window.ColumnCount = reader.GetInt32(3);
                window.Width = reader.GetInt32(4);
                window.Height = reader.GetInt32(5);
                if (reader.GetInt32(6) == 1)
                    window.WindowState = System.Windows.WindowState.Maximized;
                window.gridColor = reader.GetString(7);
                window.SetFlag(reader.GetString(7));
                window.backColor = reader.GetString(8);
            }
            dbConnection.Close();

            // create an options window just to get the colors for the mainwindow (lazy method)
            OptionsWindow ow = new OptionsWindow();
            window.Background = ow.GetColor(window.backColor);
            window.GridColor = ow.GetColor(window.gridColor);
            ow.Close();
        }
    }
}
