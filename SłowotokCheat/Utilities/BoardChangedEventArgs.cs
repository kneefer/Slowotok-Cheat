using SłowotokCheat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Utilities
{
    public delegate void BoardChangedEventHandler(object sender, BoardChangedEventArgs e);
    public class BoardChangedEventArgs : EventArgs
    {
        public BoardChangedEventArgs(Board _board)
        {
            NewBoard = _board;
        }
        public Board NewBoard { get; set; }
    }
}
