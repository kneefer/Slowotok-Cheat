using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SłowotokCheat.Models
{
    public class Answer
    {
        public string Word { get; set; }
        public bool Found { get; set; }

        public override string ToString()
        {
            return Word + ": " + (Found ? "Y" : "N");
        }
    }

    public class AnswersResponse
    {
        public List<Answer> Answers { get; set; }
        public List<object> Badges { get; set; }
        public List<object> NewBadges { get; set; }
        public bool NewLevel { get; set; }
        public int Time { get; set; }
    }
}
