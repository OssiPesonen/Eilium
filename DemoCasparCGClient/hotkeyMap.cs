using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoCasparCGClient
{
    public class hotkeyMap
    {
        public Keys informationHotkey { get; set; }
        public Keys countdownHotkey { get; set; }
        public Keys clockHotkey { get; set; }
        public Keys casterHotkey { get; set; }
        public Keys twitterHotkey { get; set; }
        public Keys scheduleHotkey { get; set; }
        public Keys sponsorsKeys { get; set; }
        public Keys groupsHotkey { get; set; }
        public Keys playoffsHotkey { get; set; }
        public Keys presentationHotkey { get; set; }
        public Keys endcreditHotkey { get; set; }
        public Keys interviewHotkey { get; set; }
        public Keys vsPlayersHotkey { get; set; }
        public Keys vsTeamsHotkey { get; set; }
        public Keys hideAll { get; set; }
        public Keys guestHotkey { get; set; }
        public Keys panelistHotkey { get; set; }
        public Keys hsIngameHotkey { get; set; }
        public Keys switchHSIngameHotkey { get; set; }

        public string casparCGTemplateFolder { get; set; }
    }
}
