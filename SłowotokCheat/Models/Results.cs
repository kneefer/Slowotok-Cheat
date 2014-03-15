using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Models
{
    public class Result
    {
        public string User { get; set; }
        public bool Pro { get; set; }
        public string UserId { get; set; }
        public object FbId { get; set; }
        public string Avatar { get; set; }
        public int Points { get; set; }
        public string Longest { get; set; }
        public int Level { get; set; }
    }

    public class GameSummary
    {
        public List<Result> Results { get; set; }
        public List<object> Badges { get; set; }
        public List<object> NewBadges { get; set; }
        public string UserId { get; set; }
        public int FbId { get; set; }
        public bool IsFbApp { get; set; }
        public int Place { get; set; }
        public int Time { get; set; }
    }
}
