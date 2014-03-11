using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace SłowotokCheat
{
    struct IntPoint
    {
        public IntPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public int X, Y;
    }
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<string> toCheck = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            var tablica = new char[4, 4] { 
                { 'Ł', 'R', 'Y', 'W' },
                { 'P', 'Z', 'C', 'Ź' },
                { 'R', 'R', 'I', 'Z' },
                { 'I', 'Ć', 'E', 'P' }
            };

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 4; y++)
                {
                    generateWords(tablica, tablica[x,y].ToString(), x, y, recursLvl: 10);
                }
            } 

            toCheck = toCheck.Distinct().ToList();
            toCheck.Sort();

            using (StreamReader file = new StreamReader("slowa.txt"))
            {
                string line;
                
                while ((line = await file.ReadLineAsync()) != null)
                {
                    foreach (var item in toCheck)
                    {
                        if (item.StartsWith(line))
                        {
                            rich.AppendText(line);
                            break;
                        }
                    }
                }
            }
        }

        private void generateWords(char[,] tablica, string slowo, int x, int y, int recursLvl)
        {
            char[,] _tablica = ((char[,])tablica.Clone()); // cloning the array
            _tablica[x, y] = '_';

            var possibilities = isMovePossible(_tablica, x, y);

            if (possibilities.Count == 0 || recursLvl == 1)
            {
                toCheck.Add(slowo);
            }
            else
            {
                foreach (var nextMove in possibilities)
                {
                    // the recursion
                    generateWords(_tablica, slowo + _tablica[nextMove.X, nextMove.Y], nextMove.X, nextMove.Y, recursLvl-1);
                }
            }
        }

        private List<IntPoint> isMovePossible(char[,] tablica, int x, int y)
        {
            var possibilities = new List<IntPoint>();

            for (int a = (x - 1); a <= (x + 1); a++)
            {
                for (int b = (y - 1); b <= (y + 1); b++)
                {
                    if (a >= 0 && a < 4 && b >= 0 && b < 4 && tablica[a,b] != '_') possibilities.Add(new IntPoint(a, b));
                }
            }

            return possibilities;
        }
    }
}
