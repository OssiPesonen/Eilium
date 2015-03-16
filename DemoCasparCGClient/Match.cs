using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eilium
{
    public class Match
    {
        public string MatchContestant1 { get; set; }
        public string MatchContestant2 { get; set; }
        public string MatchContestant1Score { get; set; }
        public string MatchContestant2Score { get; set; }

        public override string ToString()
        {
            return "Contestant 1: " + MatchContestant1 + "   Contestant2: " + MatchContestant2;
        }
    }
}
