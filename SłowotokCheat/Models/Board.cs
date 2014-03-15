using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Models
{
    public class Board
    {
        public char[] Letters { get; set; }
        public List<string> Hashs { get; set; }
        public int Points { get; set; }
        public int TimeLimit { get; set; }
        public int Time { get; set; }
        public string Author { get; set; }
    }
}
