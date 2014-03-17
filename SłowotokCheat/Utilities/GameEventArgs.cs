using SłowotokCheat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Utilities
{
    public delegate void BoardChangedEventHandler(object sender, BoardEventArgs e);
    public delegate void SendAnswerGotPossibleEventHandler(object sender, EventArgs e);
    public delegate void ConnectionErrorEventHandler(object sender, EventArgs e);

    public class BoardEventArgs : EventArgs
    {
        public BoardEventArgs(Board _board)
        {
            NewBoard = _board;
        }
        public Board NewBoard { get; set; }
    }
}
