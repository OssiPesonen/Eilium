using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using DemoCasparCGClient;

namespace Eilium
{
    public partial class Hotkeys : Form
    {
       private Form1 m_form = null;

        public Keys toggleInformationTemp;       
        public Keys toggleCountdownTemp;
        public Keys toggleClockTemp;
        public Keys toggleCasterTemp;
        public Keys toggleTwitterTemp;
        public Keys toggleScheduleTemp;
        public Keys toggleSponsorTemp;
        public Keys toggleGroupsTemp;
        public Keys togglePlayoffsTemp;
        public Keys togglePlayerTeamTemp;
        public Keys togglePresentationTemp;
        public Keys toggleEndCreditsTemp;
        public Keys toggleVsPlayersTemp;
        public Keys toggleVsTeamsTemp;
        public Keys toggleInterviewTemp;
        public Keys hideAllTemp;
        public Keys togglePanelistTemp;
        public Keys toggleGuestsTemp;
        public Keys toggleHSIngameTemp;
        public Keys switchHSIngameTemp;

        public Keys toggleInformation { get; set; }
        public Keys toggleCountdown { get; set; }
        public Keys toggleClock { get; set; }
        public Keys toggleCasters { get; set; }
        public Keys toggleTwitter { get; set; }
        public Keys toggleSchedule { get; set; }
        public Keys toggleSponsors { get; set; }
        public Keys toggleGroups { get; set; }
        public Keys togglePlayoffs { get; set; }
        public Keys togglePlayerTeam { get; set; }
        public Keys togglePresentation { get; set; }
        public Keys toggleEndCredits { get; set; }
        public Keys toggleVsPlayers { get; set; }
        public Keys toggleVsTeams { get; set; }
        public Keys toggleInterview { get; set; }
        public Keys hideAll { get; set; }
        public Keys togglePanelist { get; set; }
        public Keys toggleGuests { get; set; }
        public Keys toggleHSIngame { get; set; }
        public Keys switchHSIngame { get; set; }

        public string casparCGTemplateFolderPath { get; set; }

        private class Data
        {
            public TextBox box { get; set; }
        }

        public Hotkeys(Form1 f)
        {
            InitializeComponent();

            loadHotkeyData();

            m_form = f;

            if (toggleInformation != Keys.None) {
                informationKey.Text = GetDescription(toggleInformation);
                toggleInformationTemp = toggleInformation;
            }

            if (toggleCountdown != Keys.None)
            {
                countdownKey.Text = GetDescription(toggleCountdown);
                toggleCountdownTemp = toggleCountdown;
            }

            if (toggleClock != Keys.None)
            {
                clockKey.Text = GetDescription(toggleClock);
                toggleClockTemp = toggleClock;
            }

            if (toggleCasters != Keys.None)
            {
                casterKey.Text = GetDescription(toggleCasters);
                toggleCasterTemp = toggleCasters;
            }

            if (toggleTwitter != Keys.None)
            {
                twitterKey.Text = GetDescription(toggleTwitter);
                toggleTwitterTemp = toggleTwitter;
            }

            if (toggleSchedule != Keys.None)
            {
                scheduleKey.Text = GetDescription(toggleSchedule);
                toggleScheduleTemp = toggleSchedule;
            }

            if (toggleSponsors != Keys.None)
            {
                sponsorKey.Text = GetDescription(toggleSponsors);
                toggleSponsorTemp = toggleSponsors;
             }

            if (toggleGroups != Keys.None)
            {
                groupKey.Text = GetDescription(toggleGroups);
                toggleGroupsTemp = toggleGroups;
             }

            if (togglePlayoffs != Keys.None)
            {
                playoffsKey.Text = GetDescription(togglePlayoffs);
                togglePlayoffsTemp = togglePlayoffs;
             }

            if (togglePlayerTeam != Keys.None)
            {
                vsPlayerKey.Text = GetDescription(togglePlayerTeam);
                togglePlayerTeamTemp = togglePlayerTeam;
            }

            if (togglePresentation != Keys.None)
            {
                presentationKey.Text = GetDescription(togglePresentation);
                togglePresentationTemp = togglePresentation;
            }

            if (toggleEndCredits != Keys.None)
            {
                endCreditsKey.Text = GetDescription(toggleEndCredits);
                toggleEndCreditsTemp = toggleEndCredits;
             }

            if (toggleVsPlayers != Keys.None)
            {
                vsPlayerKey.Text = GetDescription(toggleVsPlayers);
                toggleVsPlayersTemp = toggleVsPlayers;      
            }

            if (toggleVsTeams != Keys.None)
            {
                vsTeamKey.Text = GetDescription(toggleVsTeams);
                toggleVsTeamsTemp = toggleVsTeams;
            }

            if (toggleInterview != Keys.None)
            {
                interviewKey.Text = GetDescription(toggleInterview);
                toggleInterviewTemp = toggleInterview;
            }
            if (hideAll != Keys.None)
            {
                hideAllKey.Text = GetDescription(hideAll);
                hideAllTemp = hideAll;
            }

            if (togglePanelist != Keys.None)
            {
                panelistKey.Text = GetDescription(togglePanelist);
                togglePanelistTemp = togglePanelist;
             }

            if (toggleGuests != Keys.None)
            {
                guestKey.Text = GetDescription(toggleGuests);
                toggleGuestsTemp = toggleGuests;
            }

            if (toggleHSIngame != Keys.None)
            {
                hsIngameKey.Text = GetDescription(toggleHSIngame);
                toggleHSIngameTemp = toggleHSIngame;
            }
            if (switchHSIngame != Keys.None)
            {
                switcHSIngameKey.Text = GetDescription(switchHSIngame);
                toggleHSIngameTemp = switchHSIngame;
            }

            casparCGTemplatesFolder.Text = casparCGTemplateFolderPath;
        }

        private void loadHotkeyData()
        {
            if (File.Exists("hotkeys.xml"))
            {
                hotkeyMap map;
                XmlSerializer xs = new XmlSerializer(typeof(hotkeyMap));

                using (FileStream fs = new FileStream("hotkeys.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    map = xs.Deserialize(fs) as hotkeyMap;
                }
                if (map != null)
                {

                    toggleInformation = map.informationHotkey;
                    toggleCountdown = map.countdownHotkey;
                    toggleClock = map.clockHotkey;
                    toggleCasters = map.casterHotkey;
                    toggleTwitter = map.twitterHotkey;
                    toggleSchedule = map.scheduleHotkey;
                    toggleSponsors = map.sponsorsKeys;
                    toggleGroups = map.groupsHotkey;
                    togglePlayoffs = map.playoffsHotkey;
                    togglePresentation = map.presentationHotkey;
                    toggleEndCredits = map.endcreditHotkey;
                    toggleVsPlayers = map.vsPlayersHotkey;
                    toggleVsTeams = map.vsTeamsHotkey;
                    toggleInterview = map.interviewHotkey;
                    hideAll = map.hideAll;
                    toggleGuests = map.guestHotkey;
                    togglePanelist = map.panelistHotkey;
                    toggleHSIngame = map.hsIngameHotkey;
                    switchHSIngame = map.switchHSIngameHotkey;
                    casparCGTemplateFolderPath = map.casparCGTemplateFolder;
                }
            }
        }

        private void saveHotkeyData()
        {
            string message = "Are you sure you want to save Hotkeys?";
            string caption = "Save";

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    hotkeyMap map = new hotkeyMap();

                    map.informationHotkey = toggleInformation;
                    map.countdownHotkey = toggleCountdown;
                    map.clockHotkey = toggleClock;
                    map.casterHotkey = toggleCasters;
                    map.twitterHotkey = toggleTwitter;
                    map.scheduleHotkey = toggleSchedule;
                    map.sponsorsKeys = toggleSponsors;
                    map.groupsHotkey = toggleGroups;
                    map.playoffsHotkey = togglePlayoffs;
                    map.presentationHotkey = togglePresentation;
                    map.endcreditHotkey = toggleEndCredits;
                    map.vsPlayersHotkey = toggleVsPlayers;
                    map.vsTeamsHotkey = toggleVsTeams;
                    map.interviewHotkey = toggleInterview;
                    map.hideAll = hideAll;
                    map.guestHotkey = toggleGuests;
                    map.panelistHotkey = togglePanelist;
                    map.hsIngameHotkey = toggleHSIngame;
                    map.switchHSIngameHotkey = switchHSIngame;
                    map.casparCGTemplateFolder = casparCGTemplateFolderPath;

                    SaveXML.SaveData(map, "hotkeys.xml");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }        

        public void loopThrough(TextBox box)
        {
            foreach (TextBox c in this.Controls.OfType<TextBox>())
            {
                if (c.Name != box.Name)
                {
                    if (c.Text == box.Text)
                        MessageBox.Show("Error. Hotkey already defined");
                }
            }
        }

        public static string GetDescription(Keys key)
        {
            switch (key)
            {
                //letters
                case Keys.A:
                case Keys.B:
                case Keys.C:
                case Keys.D:
                case Keys.E:
                case Keys.F:
                case Keys.G:
                case Keys.H:
                case Keys.I:
                case Keys.J:
                case Keys.K:
                case Keys.L:
                case Keys.M:
                case Keys.N:
                case Keys.O:
                case Keys.P:
                case Keys.Q:
                case Keys.R:
                case Keys.S:
                case Keys.T:
                case Keys.U:
                case Keys.V:
                case Keys.W:
                case Keys.X:
                case Keys.Y:
                case Keys.Z:
                    return Enum.GetName(typeof(Keys), key);

                //digits
                case Keys.D0:
                    return "0";
                case Keys.NumPad0:
                    return "Number Pad 0";
                case Keys.D1:
                    return "1";
                case Keys.NumPad1:
                    return "Number Pad 1";
                case Keys.D2:
                    return "2";
                case Keys.NumPad2:
                    return "Number Pad 2";
                case Keys.D3:
                    return "3";
                case Keys.NumPad3:
                    return "Number Pad 3";
                case Keys.D4:
                    return "4";
                case Keys.NumPad4:
                    return "Number Pad 4";
                case Keys.D5:
                    return "5";
                case Keys.NumPad5:
                    return "Number Pad 5";
                case Keys.D6:
                    return "6";
                case Keys.NumPad6:
                    return "Number Pad 6";
                case Keys.D7:
                    return "7";
                case Keys.NumPad7:
                    return "Number Pad 7";
                case Keys.D8:
                    return "8";
                case Keys.NumPad8:
                    return "Number Pad 8";
                case Keys.D9:
                    return "9";
                case Keys.NumPad9:
                    return "Number Pad 9";

                //punctuation
                case Keys.Add:
                    return "Number Pad +";
                case Keys.Subtract:
                    return "Number Pad -";
                case Keys.Divide:
                    return "Number Pad /";
                case Keys.Multiply:
                    return "Number Pad *";
                case Keys.Space:
                    return "Spacebar";
                case Keys.Decimal:
                    return "Number Pad .";

                //function
                case Keys.F1:
                case Keys.F2:
                case Keys.F3:
                case Keys.F4:
                case Keys.F5:
                case Keys.F6:
                case Keys.F7:
                case Keys.F8:
                case Keys.F9:
                case Keys.F10:
                case Keys.F11:
                case Keys.F12:
                case Keys.F13:
                case Keys.F14:
                case Keys.F15:
                case Keys.F16:
                case Keys.F17:
                case Keys.F18:
                case Keys.F19:
                case Keys.F20:
                case Keys.F21:
                case Keys.F22:
                case Keys.F23:
                case Keys.F24:
                    return Enum.GetName(typeof(Keys), key);

                //navigation
                case Keys.Up:
                    return "Up Arrow";
                case Keys.Down:
                    return "Down Arrow";
                case Keys.Left:
                    return "Left Arrow";
                case Keys.Right:
                    return "Right Arrow";
                case Keys.Prior:
                    return "Page Up";
                case Keys.Next:
                    return "Page Down";
                case Keys.Home:
                    return "Home";
                case Keys.End:
                    return "End";

                //control keys
                case Keys.Back:
                    return "Backspace";
                case Keys.Tab:
                    return "Tab";
                case Keys.Escape:
                    return "Escape";
                case Keys.Enter:
                    return "Enter";
                case Keys.Shift:
                case Keys.ShiftKey:
                    return "Shift";
                case Keys.LShiftKey:
                    return "Shift (Left)";
                case Keys.RShiftKey:
                    return "Shift (Right)";
                case Keys.Control:
                case Keys.ControlKey:
                    return "Control";
                case Keys.LControlKey:
                    return "Control (Left)";
                case Keys.RControlKey:
                    return "Control (Right)";
                case Keys.Menu:
                case Keys.Alt:
                    return "Alt";
                case Keys.LMenu:
                    return "Alt (Left)";
                case Keys.RMenu:
                    return "Alt (Right)";
                case Keys.Pause:
                    return "Pause";
                case Keys.CapsLock:
                    return "Caps Lock";
                case Keys.NumLock:
                    return "Num Lock";
                case Keys.Scroll:
                    return "Scroll Lock";
                case Keys.PrintScreen:
                    return "Print Screen";
                case Keys.Insert:
                    return "Insert";
                case Keys.Delete:
                    return "Delete";
                case Keys.Help:
                    return "Help";
                case Keys.LWin:
                    return "Windows (Left)";
                case Keys.RWin:
                    return "Windows (Right)";
                case Keys.Apps:
                    return "Context Menu";

                //browser keys
                case Keys.BrowserBack:
                    return "Browser Back";
                case Keys.BrowserFavorites:
                    return "Browser Favorites";
                case Keys.BrowserForward:
                    return "Browser Forward";
                case Keys.BrowserHome:
                    return "Browser Home";
                case Keys.BrowserRefresh:
                    return "Browser Refresh";
                case Keys.BrowserSearch:
                    return "Browser Search";
                case Keys.BrowserStop:
                    return "Browser Stop";

                //media keys
                case Keys.VolumeDown:
                    return "Volume Down";
                case Keys.VolumeMute:
                    return "Volume Mute";
                case Keys.VolumeUp:
                    return "Volume Up";
                case Keys.MediaNextTrack:
                    return "Next Track";
                case Keys.Play:
                case Keys.MediaPlayPause:
                    return "Play";
                case Keys.MediaPreviousTrack:
                    return "Previous Track";
                case Keys.MediaStop:
                    return "Stop";
                case Keys.SelectMedia:
                    return "Select Media";

                //IME keys
                case Keys.HanjaMode:
                case Keys.JunjaMode:
                case Keys.HangulMode:
                case Keys.FinalMode:    //duplicate values: Hanguel, Kana, Kanji  
                case Keys.IMEAccept:
                case Keys.IMEConvert:   //duplicate: IMEAceept
                case Keys.IMEModeChange:
                case Keys.IMENonconvert:
                    return null;

                //special keys
                case Keys.LaunchMail:
                    return "Launch Mail";
                case Keys.LaunchApplication1:
                    return "Launch Favorite Application 1";
                case Keys.LaunchApplication2:
                    return "Launch Favorite Application 2";
                case Keys.Zoom:
                    return "Zoom";

                //oem keys 
                case Keys.OemSemicolon: //oem1
                    return ";";
                case Keys.OemQuestion:  //oem2
                    return "?";
                case Keys.Oemtilde:     //oem3
                    return "~";
                case Keys.OemOpenBrackets:  //oem4
                    return "[";
                case Keys.OemPipe:  //oem5
                    return "|";
                case Keys.OemCloseBrackets:    //oem6
                    return "]";
                case Keys.OemQuotes:        //oem7
                    return "'";
                case Keys.OemBackslash: //oem102
                    return "/";
                case Keys.Oemplus:
                    return "+";
                case Keys.OemMinus:
                    return "-";
                case Keys.Oemcomma:
                    return ",";
                case Keys.OemPeriod:
                    return ".";

                //unsupported oem keys
                case Keys.Oem8:
                case Keys.OemClear:
                    return null;

                //unsupported other keys
                case Keys.None:
                case Keys.LButton:
                case Keys.RButton:
                case Keys.MButton:
                case Keys.XButton1:
                case Keys.XButton2:
                case Keys.Clear:
                case Keys.Sleep:
                case Keys.Cancel:
                case Keys.LineFeed:
                case Keys.Select:
                case Keys.Print:
                case Keys.Execute:
                case Keys.Separator:
                case Keys.ProcessKey:
                case Keys.Packet:
                case Keys.Attn:
                case Keys.Crsel:
                case Keys.Exsel:
                case Keys.EraseEof:
                case Keys.NoName:
                case Keys.Pa1:
                case Keys.KeyCode:
                case Keys.Modifiers:
                    return null;

                default:
                    throw new NotSupportedException(Enum.GetName(typeof(Keys), key));
            }
        }

        private void btnSaveForm_Click(object sender, EventArgs e)
        {
                toggleInformation = toggleInformationTemp;
                toggleCountdown = toggleCountdownTemp;
                toggleClock = toggleClockTemp;
                toggleCasters = toggleCasterTemp;
                toggleTwitter = toggleTwitterTemp;
                toggleSchedule = toggleScheduleTemp;
                toggleSponsors = toggleSponsorTemp;
                toggleGroups = toggleGroupsTemp;
                togglePlayoffs = togglePlayoffsTemp;
                togglePlayerTeam = togglePlayerTeamTemp;
                togglePresentation = togglePresentationTemp;
                toggleEndCredits = toggleEndCreditsTemp;
                toggleVsPlayers = toggleVsPlayersTemp;
                toggleVsTeams = toggleVsTeamsTemp;
                toggleInterview = toggleInterviewTemp;
                hideAll = hideAllTemp;
                toggleGuests = toggleGuestsTemp;
                togglePanelist = togglePanelistTemp;
                toggleHSIngame = toggleHSIngameTemp;
                switchHSIngame = switchHSIngameTemp;

           casparCGTemplateFolderPath = casparCGTemplatesFolder.Text;

           saveHotkeyData();

           MessageBox.Show(this, "Options saved", "Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);

           m_form.loadHotkeys();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            base.OnClosing(e);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            this.Visible = false;
            base.OnFormClosing(e);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Visible = false;
        }

// 
// All KeyDown events
//

        private void clockKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleClockTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            clockKey.Text = keyvalue.ToUpper();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            toggleInformationTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            informationKey.Text = keyvalue.ToUpper();
        }

        private void casterKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleCasterTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            casterKey.Text = keyvalue.ToUpper();
        }

        private void twitterKey_KeyDown_1(object sender, KeyEventArgs e)
        {
            toggleTwitterTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            twitterKey.Text = keyvalue.ToUpper();
        }

        private void countdownKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleCountdownTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            countdownKey.Text = keyvalue.ToUpper();
        }

        private void scheduleKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleScheduleTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            scheduleKey.Text = keyvalue.ToUpper();
        }

        private void sponsorKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleSponsorTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            sponsorKey.Text = keyvalue.ToUpper();
        }

        private void groupKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleGroupsTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            groupKey.Text = keyvalue.ToUpper();
        }

        private void playoffsKey_KeyDown(object sender, KeyEventArgs e)
        {
            togglePlayoffsTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            playoffsKey.Text = keyvalue.ToUpper();
        }

        private void playerteamKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleVsPlayersTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            vsPlayerKey.Text = keyvalue.ToUpper();
        }

        private void presentationKey_KeyDown(object sender, KeyEventArgs e)
        {
            togglePresentationTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            presentationKey.Text = keyvalue.ToUpper();
        }

        private void endCredits_KeyDown(object sender, KeyEventArgs e)
        {
            toggleEndCreditsTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            endCreditsKey.Text = keyvalue.ToUpper();
        }

        private void vsTeamKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleVsTeamsTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            vsTeamKey.Text = keyvalue.ToUpper();
        }

        private void interviewKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleInterviewTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            interviewKey.Text = keyvalue.ToUpper();
        }

        // Prevent Keystrokes

        private void textBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void hideAllKey_KeyDown(object sender, KeyEventArgs e)
        {
            hideAll = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            hideAllKey.Text = keyvalue.ToUpper();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                casparCGTemplatesFolder.Text = folderBrowserDialog1.SelectedPath;
            }
        }

        private void button17_Click(object sender, EventArgs e)
        {
            hideAllKey.Text = "";
        }

        private void button16_Click(object sender, EventArgs e)
        {
            endCreditsKey.Text = "";
        }

        private void button15_Click(object sender, EventArgs e)
        {
            presentationKey.Text = "";
        }

        private void button9_Click(object sender, EventArgs e)
        {
            vsTeamKey.Text = "";
        }

        private void button10_Click(object sender, EventArgs e)
        {
            vsPlayerKey.Text = "";
        }

        private void button11_Click(object sender, EventArgs e)
        {
            playoffsKey.Text = "";
        }

        private void button12_Click(object sender, EventArgs e)
        {
            groupKey.Text = "";
        }

        private void button13_Click(object sender, EventArgs e)
        {
            sponsorKey.Text = "";
        }

        private void button14_Click(object sender, EventArgs e)
        {
            scheduleKey.Text = "";
        }

        private void button8_Click(object sender, EventArgs e)
        {
            interviewKey.Text = "";
        }

        private void button7_Click(object sender, EventArgs e)
        {
            twitterKey.Text = "";
        }

        private void button6_Click(object sender, EventArgs e)
        {
            casterKey.Text = "";
        }

        private void button5_Click(object sender, EventArgs e)
        {
            countdownKey.Text = "";
        }

        private void button4_Click(object sender, EventArgs e)
        {
            clockKey.Text = "";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            informationKey.Text = "";
        }

        private void guestKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleGuestsTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            guestKey.Text = keyvalue.ToUpper();
        }

        private void panelistKey_KeyDown(object sender, KeyEventArgs e)
        {
            togglePanelistTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            panelistKey.Text = keyvalue.ToUpper();

        }

        private void hsIngameKey_KeyDown(object sender, KeyEventArgs e)
        {
            toggleHSIngameTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            hsIngameKey.Text = keyvalue.ToUpper();
        }

        private void switcHSIngameKey_KeyDown(object sender, KeyEventArgs e)
        {
            switchHSIngameTemp = e.KeyCode;
            string keyvalue = GetDescription(e.KeyCode);
            switcHSIngameKey.Text = keyvalue.ToUpper();
        }
    }
}
