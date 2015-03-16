using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlServerCe;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using System.Security.Principal;
using System.IO;
using Svt.Caspar;
using System.Threading;
using System.Xml.Serialization;
using System.Xml.Linq;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Globalization;
using Eilium;
using System.Data.SqlClient;

namespace DemoCasparCGClient
{
    public partial class Form1 : Form
    {
        // Database commands
        SqlCeConnection conn = new SqlCeConnection(@"Data Source=" + AppDomain.CurrentDomain.BaseDirectory + "Players.sdf");
        SqlCeCommand cmd = new SqlCeCommand();
        SqlCeDataReader dr;             

        //Main Caspar Device
        public CasparDevice casparDevice = new CasparDevice();
        CasparCGDataCollection cgData = new CasparCGDataCollection();

        public string graphicsFolder = "graphics";
        public string playerPictureFolder = "pictures";

        public string casparCGTemplatesFolderPath = "";

        public string sponsorView = "Sponsors_Camera";
        public string sponsorGame = "Dota";

        public string raceLeft = "empty";
        public string raceRight = "empty";

        public string interviewTemplateSelection = "Interview";

        public int casparChannel = System.Convert.ToInt16(ConfigurationManager.AppSettings["Layer"]);
        public int casparLayer = System.Convert.ToInt16(ConfigurationManager.AppSettings["Channel"]);

        //AMCP/OSC Connection Bits
        public string caspar_server_IP { get; set; }
        public int caspar_server_OSC_port { get; set; }
        public int caspar_server_AMCP_port { get; set; }

        /*
         * Values for templates checking i they're active on the screen or not
        */
        public Boolean isUpcomingActive = false;
        public Boolean isRightNowActive = false;
        public Boolean isCastersActive = false;
        public Boolean isCasterLeftActive = false;
        public Boolean isCasterRightActive = false;
        public Boolean isClockActive = false;
        public Boolean isCountdownActive = false;
        public Boolean isScheduleRightActive = false;
        public Boolean isScheduleLeftActive = false;
        public Boolean isSponsorsActive = false;
        public Boolean isSC2LantrekLogoActive = false;
        public Boolean isTwitterActive = false;
        public Boolean isGroupsActive = false;
        public Boolean isPlayoffsActive = false;
        public Boolean isPlayersActive = false;
        public Boolean isTeamsActive = false;
        public Boolean isTeamLeftActive = false;
        public Boolean isTeamRightActive = false;
        public Boolean isInfoActive = false;
        public Boolean isEndCreditsActive = false;
        public Boolean isDotaPickingActive = false;
        public Boolean isScoreboardActive = false;
        public Boolean isPresentationActive = false;
        public Boolean isInterviewActive = false;
        public Boolean isGuestsActive = false;
        public Boolean isPanelistsActive = false;

        bool glowCover = false;

        /* 
         * Formatting HOTKEYS       
        */
        public Keys toggleInformation = Keys.None;
        public Keys toggleCountdown = Keys.None;
        public Keys toggleClock = Keys.None;
        public Keys toggleCasters = Keys.None;
        public Keys toggleTwitter = Keys.None;
        public Keys toggleSchedule = Keys.None;
        public Keys toggleSponsors = Keys.None;
        public Keys toggleGroups = Keys.None;
        public Keys togglePlayoffs = Keys.None;
        public Keys togglePlayerTeam = Keys.None;
        public Keys togglePresentation = Keys.None;
        public Keys toggleEndCredits = Keys.None;
        public Keys toggleVsPlayers = Keys.None;
        public Keys toggleVsTeams = Keys.None;
        public Keys toggleInterview = Keys.None;
        public Keys hideAll = Keys.None;
        public Keys guestsHotkey = Keys.None;
        public Keys panelistHotkey = Keys.None;
        public Keys toggleHSIngame = Keys.None;
        public Keys switchHSIngame = Keys.None;

        string errorFile = "Error.txt";

        public string serverAddress = "localhost";
        public int OCSPort = 5253;
        public int AMCPPort = 5250;
        public string templateFolder = "";        

        public Panel activePanel;
        public Panel activePresentationPanel;

        List<string> panelItems = new List<string>();

        public Form1()
        {
            InitializeComponent();

            loadData();
            loadHotkeys();
            
            if(!File.Exists(errorFile))
            {
                File.Create(errorFile);
            }

            activePanel = infoPanel01;
            activePresentationPanel = presentationPanel01;

            KeyPreview = true;

            if (textboxServerAddress.Text != "")
                caspar_server_IP = textboxServerAddress.Text;
            else
                caspar_server_IP = serverAddress;

            if (textboxOSCPort.Text != "")
                caspar_server_OSC_port = System.Convert.ToInt32(textboxOSCPort.Text);
            else
                caspar_server_OSC_port = OCSPort;

            if (textboxAMCPPort.Text != "")
                caspar_server_AMCP_port = System.Convert.ToInt32(textboxAMCPPort.Text);
            else
                caspar_server_AMCP_port = AMCPPort;

            dtCountDownTime.ShowUpDown = true;
            dtCountDownTime.Format = DateTimePickerFormat.Custom;

            templateFolder = textboxTemplateFolder.Text;

            if (CBonlyShowRo8.Checked == true)
            {
                ro16score1.Enabled = false; ro16score2.Enabled = false; ro16score3.Enabled = false; ro16score4.Enabled = false; ro16score5.Enabled = false; ro16score6.Enabled = false; ro16score7.Enabled = false; ro16score8.Enabled = false; ro16score9.Enabled = false; ro16score10.Enabled = false; ro16score11.Enabled = false; ro16score12.Enabled = false; ro16score13.Enabled = false; ro16score14.Enabled = false; ro16score15.Enabled = false; ro16score16.Enabled = false; ro16team1.Enabled = false; ro16team2.Enabled = false; ro16team3.Enabled = false; ro16team4.Enabled = false; ro16team5.Enabled = false; ro16team6.Enabled = false; ro16team7.Enabled = false; ro16team8.Enabled = false; ro16team9.Enabled = false; ro16team10.Enabled = false; ro16team11.Enabled = false; ro16team12.Enabled = false; ro16team13.Enabled = false; ro16team14.Enabled = false; ro16team15.Enabled = false; ro16team16.Enabled = false;
            }
            else
            {
                ro16team1.Enabled = true; ro16team2.Enabled = true; ro16team3.Enabled = true; ro16team4.Enabled = true; ro16team5.Enabled = true; ro16team6.Enabled = true; ro16team7.Enabled = true; ro16team8.Enabled = true; ro16team9.Enabled = true; ro16team10.Enabled = true; ro16team11.Enabled = true; ro16team12.Enabled = true; ro16team13.Enabled = true; ro16team14.Enabled = true; ro16team15.Enabled = true; ro16team16.Enabled = true;
            }

            disableServerSettings();           

            loadPlayersToComboboxes();
            loadTeamsToComboboxes();
            loadPlayerPictures();
            loadTeamLogos();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            Thread t = new Thread(new ThreadStart(SplashScreen));
            t.Start();
            Thread.Sleep(1000);
            t.Abort();
        }

        // Confirm saving when exiting
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string message = "Would you like to save before closing?";
            string caption = "Close";

            MessageBoxButtons buttons = MessageBoxButtons.YesNoCancel;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
                saveData();

            else if (result == DialogResult.Cancel)
                e.Cancel = true;
        }

        public void SplashScreen()
        {
            Application.Run(new SplashScreen());
        }

        System.Timers.Timer MyTimer;

        private void timerEvent(int interval)
        {
            MyTimer = new System.Timers.Timer();
            MyTimer.Interval = interval;
            MyTimer.Elapsed += timer1_Tick;
            MyTimer.Start();
            MyTimer.Dispose();
        }

        #region Save and Load all data from input fields to an XML file
        // Save all inputted data into an XML file
        private void saveData()
        {
            string message = "Are you sure you want to save?";
            string caption = "Save";

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                try
                {
                    Information info = new Information();

                    #region Config
                    info.TemplateFolder = textboxTemplateFolder.Text;
                    info.ServerAddress = textboxServerAddress.Text;
                    info.OSCPort = textboxOSCPort.Text;
                    info.AMCPPort = textboxAMCPPort.Text;
                    #endregion
                    #region Information

                    info.infoPanel1Title = infoPanel01Title.Text;
                    info.infoPanel1Row1 = infoPanel01Text01.Text;
                    info.infoPanel1Row2 = infoPanel01Text02.Text;
                    info.infoPanel1Row3 = infoPanel01Text03.Text;

                    info.infoPanel2Title = infoPanel02Title.Text;
                    info.infoPanel2Row1 = infoPanel02Text01.Text;
                    info.infoPanel2Row2 = infoPanel02Text02.Text;
                    info.infoPanel2Row3 = infoPanel02Text03.Text;

                    info.infoPanel3Title = infoPanel03Title.Text;
                    info.infoPanel3Row1 = infoPanel03Text01.Text;
                    info.infoPanel3Row2 = infoPanel03Text02.Text;
                    info.infoPanel3Row3 = infoPanel03Text03.Text;

                    info.infoPanel4Title = infoPanel04Title.Text;
                    info.infoPanel4Row1 = infoPanel04Text01.Text;
                    info.infoPanel4Row2 = infoPanel04Text02.Text;
                    info.infoPanel4Row3 = infoPanel04Text03.Text;

                    info.infoPanel5Title = infoPanel05Title.Text;
                    info.infoPanel5Row1 = infoPanel05Text01.Text;
                    info.infoPanel5Row2 = infoPanel05Text02.Text;
                    info.infoPanel5Row3 = infoPanel05Text03.Text;

                    info.infoPanel6Title = infoPanel06Title.Text;
                    info.infoPanel6Row1 = infoPanel06Text01.Text;
                    info.infoPanel6Row2 = infoPanel06Text02.Text;
                    info.infoPanel6Row3 = infoPanel06Text03.Text;

                    info.infoPanel7Title = infoPanel07Title.Text;
                    info.infoPanel7Row1 = infoPanel07Text01.Text;
                    info.infoPanel7Row2 = infoPanel07Text02.Text;
                    info.infoPanel7Row3 = infoPanel07Text03.Text;

                    info.infoPanel8Title = infoPanel08Title.Text;
                    info.infoPanel8Row1 = infoPanel08Text01.Text;
                    info.infoPanel8Row2 = infoPanel08Text02.Text;
                    info.infoPanel8Row3 = infoPanel08Text03.Text;

                    info.infoPanel9Title = infoPanel09Title.Text;
                    info.infoPanel9Row1 = infoPanel09Text01.Text;
                    info.infoPanel9Row2 = infoPanel09Text02.Text;
                    info.infoPanel9Row3 = infoPanel09Text03.Text;                   
                    #endregion
                    #region Casters
                    info.CasterNameLeft = textboxCasterNameLeft.Text;
                    info.CasterNicknameLeft = textboxCasterNicknameLeft.Text;
                    info.CasterTwitterLeft = textboxCasterAltLeft.Text;

                    info.CasterNameRight = textboxCasterNameRight.Text;
                    info.CasterNicknameRight = textboxCasterNicknameRight.Text;
                    info.CasterTwitterRight = textboxCasterAltRight.Text;
                    #endregion
                    #region Twitter
                    info.Twitter1 = twitterHashtagTextbox.Text;
                    info.Twitter2 = twitterURLTextbox.Text;
                    #endregion
                    #region Schedule

                    info.BoxLeftTitle = textboxBoxLeftTitle.Text;
                    info.BoxLeftC1 = tbScheduleC1.Text;
                    info.BoxLeftC2 = tbScheduleC2.Text;
                    info.BoxLeftC3 = tbScheduleC3.Text;
                    info.BoxLeftC4 = tbScheduleC4.Text;
                    info.BoxLeftC5 = tbScheduleC5.Text;
                    info.BoxLeftC6 = tbScheduleC6.Text;
                    info.BoxLeftC7 = tbScheduleC7.Text;
                    info.BoxLeftC8 = tbScheduleC8.Text;

                    info.BoxLeftT1 = textboxBoxLeftT1.Text;
                    info.BoxLeftT2 = textboxBoxLeftT2.Text;
                    info.BoxLeftT3 = textboxBoxLeftT3.Text;
                    info.BoxLeftT4 = textboxBoxLeftT4.Text;
                    info.BoxLeftT5 = textboxBoxLeftT5.Text;
                    info.BoxLeftT6 = textboxBoxLeftT6.Text;
                    info.BoxLeftT7 = textboxBoxLeftT7.Text;
                    info.BoxLeftT8 = textboxBoxLeftT8.Text;
                    #endregion
                    #region Groups
                    info.GroupJSONUrl = textGroupsJSONFileURL.Text;
                    info.GroupTitle = boxGroupTitle.Text;

                    info.Group1Title = group1title.Text;
                    info.Group2Title = group2title.Text;
                    info.Group3Title = group3title.Text;
                    info.Group4Title = group4title.Text;
                    #endregion
                    #region Groups - Group A
                    info.Group1Name1 = group1name1.Text;
                    info.Group1Name2 = group1name2.Text;
                    info.Group1Name3 = group1name3.Text;
                    info.Group1Name4 = group1name4.Text;

                    info.Group1Wins1 = group1wins1.Text;
                    info.Group1Wins2 = group1wins2.Text;
                    info.Group1Wins3 = group1wins3.Text;
                    info.Group1Wins4 = group1wins4.Text;
                    #endregion
                    #region Groups - Group B
                    info.Group2Name1 = group2name1.Text;
                    info.Group2Name2 = group2name2.Text;
                    info.Group2Name3 = group2name3.Text;
                    info.Group2Name4 = group2name4.Text;

                    info.Group2Wins1 = group2wins1.Text;
                    info.Group2Wins2 = group2wins2.Text;
                    info.Group2Wins3 = group2wins3.Text;
                    info.Group2Wins4 = group2wins4.Text;
                    #endregion
                    #region Groups - Group C
                    info.Group3Name1 = group3name1.Text;
                    info.Group3Name2 = group3name2.Text;
                    info.Group3Name3 = group3name3.Text;
                    info.Group3Name4 = group3name4.Text;

                    info.Group3Wins1 = group3wins1.Text;
                    info.Group3Wins2 = group3wins2.Text;
                    info.Group3Wins3 = group3wins3.Text;
                    info.Group3Wins4 = group3wins4.Text;
                    #endregion
                    #region Groups - Group D
                    info.Group4Name1 = group4name1.Text;
                    info.Group4Name2 = group4name2.Text;
                    info.Group4Name3 = group4name3.Text;
                    info.Group4Name4 = group4name4.Text;

                    info.Group4Wins1 = group4wins1.Text;
                    info.Group4Wins2 = group4wins2.Text;
                    info.Group4Wins3 = group4wins3.Text;
                    info.Group4Wins4 = group4wins4.Text;
                    #endregion
                    #region End Credits
                    info.EndCredits = rtbCreditsText.Text;
                    info.EndCreditsLength = tbCreditsLength.Text;
                    info.EndCreditsStartPos = tbCreditsStartPos.Text;
                    #endregion
                    #region Playoffs
                    info.PlayoffsJSONUrl = textPlayoffsJSONFileURL.Text;
                    info.PlayoffsTitle = boxPlayoffsTitle.Text;

                    info.Ro16Team1 = ro16team1.Text;
                    info.Ro16Team2 = ro16team2.Text;
                    info.Ro16Team3 = ro16team3.Text;
                    info.Ro16Team4 = ro16team4.Text;
                    info.Ro16Team5 = ro16team5.Text;
                    info.Ro16Team6 = ro16team6.Text;
                    info.Ro16Team7 = ro16team7.Text;
                    info.Ro16Team8 = ro16team8.Text;
                    info.Ro16Team9 = ro16team9.Text;
                    info.Ro16Team10 = ro16team10.Text;
                    info.Ro16Team11 = ro16team11.Text;
                    info.Ro16Team12 = ro16team12.Text;
                    info.Ro16Team13 = ro16team13.Text;
                    info.Ro16Team14 = ro16team14.Text;
                    info.Ro16Team15 = ro16team15.Text;
                    info.Ro16Team16 = ro16team16.Text;
                    info.Ro16Score1 = ro16score1.Text;
                    info.Ro16Score2 = ro16score2.Text;
                    info.Ro16Score3 = ro16score3.Text;
                    info.Ro16Score4 = ro16score4.Text;
                    info.Ro16Score5 = ro16score5.Text;
                    info.Ro16Score6 = ro16score6.Text;
                    info.Ro16Score7 = ro16score7.Text;
                    info.Ro16Score8 = ro16score8.Text;
                    info.Ro16Score9 = ro16score9.Text;
                    info.Ro16Score10 = ro16score10.Text;
                    info.Ro16Score11 = ro16score11.Text;
                    info.Ro16Score12 = ro16score12.Text;
                    info.Ro16Score13 = ro16score13.Text;
                    info.Ro16Score14 = ro16score14.Text;
                    info.Ro16Score15 = ro16score15.Text;
                    info.Ro16Score16 = ro16score16.Text;

                    info.Ro8Team1 = ro8team1.Text;
                    info.Ro8Team2 = ro8team2.Text;
                    info.Ro8Team3 = ro8team3.Text;
                    info.Ro8Team4 = ro8team4.Text;
                    info.Ro8Team5 = ro8team5.Text;
                    info.Ro8Team6 = ro8team6.Text;
                    info.Ro8Team7 = ro8team7.Text;
                    info.Ro8Team8 = ro8team8.Text;
                    info.Ro8Score1 = ro8score1.Text;
                    info.Ro8Score2 = ro8score2.Text;
                    info.Ro8Score3 = ro8score3.Text;
                    info.Ro8Score4 = ro8score4.Text;
                    info.Ro8Score5 = ro8score5.Text;
                    info.Ro8Score6 = ro8score6.Text;
                    info.Ro8Score7 = ro8score7.Text;
                    info.Ro8Score8 = ro8score8.Text;

                    info.Ro4Team1 = ro4team1.Text;
                    info.Ro4Team2 = ro4team2.Text;
                    info.Ro4Team3 = ro4team3.Text;
                    info.Ro4Team4 = ro4team4.Text;
                    info.Ro4Score1 = ro4score1.Text;
                    info.Ro4Score2 = ro4score2.Text;
                    info.Ro4Score3 = ro4score3.Text;
                    info.Ro4Score4 = ro4score4.Text;

                    info.Ro2Team1 = ro2team1.Text;
                    info.Ro2Team2 = ro2team2.Text;
                    info.Ro2Score1 = ro2score1.Text;
                    info.Ro2Score2 = ro2score2.Text;
                    #endregion
                    #region scoreboard 
                    info.scoreBoardName1 = hearthstoneScoreboardName1Textbox.Text;
                    info.scoreBoardName2 = hearthstoneScoreboardName2Textbox.Text;
                    info.scoreBoardScore1 = hearthstoneScoreboardScore1Numeric.Text;
                    info.scoreBoardScore2 = hearthstoneScoreboardScore2Numeric.Text;
                    #endregion
                    #region clock & countdown
                    info.clockTitle = clockTitleTextbox.Text;
                    info.countdownTitle = countdownTitleTextbox.Text;
                    info.vsPlayersRoundof = vsPlayersRoundOf.Text;
                    info.vsTeamsRoundOf = vsTeamsRoundOfTextbox.Text;
                    #endregion
                    SaveXML.SaveData(info, "data.xml");
                    setInfoColors("Kenttien tiedot tallennettu");

                    System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
                    MyTimer.Interval = (5000);
                    MyTimer.Tick += new EventHandler(timer1_Tick);
                    MyTimer.Start();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }


        // Load all inputted data from an XML file
        private void loadData()
        {
            if (File.Exists("data.xml"))
            {
                Information info;
                XmlSerializer xs = new XmlSerializer(typeof(Information));

                using (FileStream fs = new FileStream("data.xml", FileMode.Open, FileAccess.Read, FileShare.Read))
                {
                    info = xs.Deserialize(fs) as Information;
                }
                if (info != null)
                {
                    #region Config
                    textboxServerAddress.Text = info.ServerAddress;
                    textboxAMCPPort.Text = info.AMCPPort;
                    textboxOSCPort.Text = info.OSCPort;
                    textboxTemplateFolder.Text = info.TemplateFolder;
                    #endregion
                    #region Information
                    infoPanel01Title.Text = info.infoPanel1Title;
                    infoPanel01Text01.Text = info.infoPanel1Row1;
                    infoPanel01Text02.Text = info.infoPanel1Row2;
                    infoPanel01Text03.Text = info.infoPanel1Row3;

                    infoPanel02Title.Text = info.infoPanel2Title;
                    infoPanel02Text01.Text = info.infoPanel2Row1;
                    infoPanel02Text02.Text = info.infoPanel2Row2;
                    infoPanel02Text03.Text = info.infoPanel2Row3;

                    infoPanel03Title.Text = info.infoPanel3Title;
                    infoPanel03Text01.Text = info.infoPanel3Row1;
                    infoPanel03Text02.Text = info.infoPanel3Row2;
                    infoPanel03Text03.Text = info.infoPanel3Row3;

                    infoPanel04Title.Text = info.infoPanel4Title;
                    infoPanel04Text01.Text = info.infoPanel4Row1;
                    infoPanel04Text02.Text = info.infoPanel4Row2;
                    infoPanel04Text03.Text = info.infoPanel4Row3;

                    infoPanel05Title.Text = info.infoPanel5Title;
                    infoPanel05Text01.Text = info.infoPanel5Row1;
                    infoPanel05Text02.Text = info.infoPanel5Row2;
                    infoPanel05Text03.Text = info.infoPanel5Row3;

                    infoPanel06Title.Text = info.infoPanel6Title;
                    infoPanel06Text01.Text = info.infoPanel6Row1;
                    infoPanel06Text02.Text = info.infoPanel6Row2;
                    infoPanel06Text03.Text = info.infoPanel6Row3;

                    infoPanel07Title.Text = info.infoPanel7Title;
                    infoPanel07Text01.Text = info.infoPanel7Row1;
                    infoPanel07Text02.Text = info.infoPanel7Row2;
                    infoPanel07Text03.Text = info.infoPanel7Row3;

                    infoPanel08Title.Text = info.infoPanel8Title;
                    infoPanel08Text01.Text = info.infoPanel8Row1;
                    infoPanel08Text02.Text = info.infoPanel8Row2;
                    infoPanel08Text03.Text = info.infoPanel8Row3;

                    infoPanel09Title.Text = info.infoPanel9Title;
                    infoPanel09Text01.Text = info.infoPanel9Row1;
                    infoPanel09Text02.Text = info.infoPanel9Row2;
                    infoPanel09Text03.Text = info.infoPanel9Row3;
                    #endregion
                    #region Casters
                    textboxCasterNameLeft.Text = info.CasterNameLeft;
                    textboxCasterNicknameLeft.Text = info.CasterNicknameLeft;
                    textboxCasterAltLeft.Text = info.CasterTwitterLeft;

                    textboxCasterNameRight.Text = info.CasterNameRight;
                    textboxCasterNicknameRight.Text = info.CasterNicknameRight;
                    textboxCasterAltRight.Text = info.CasterTwitterRight;
                    #endregion
                    #region Schedule
                    textboxBoxLeftTitle.Text = info.BoxLeftTitle;
                    tbScheduleC1.Text = info.BoxLeftC1;
                    tbScheduleC2.Text = info.BoxLeftC2;
                    tbScheduleC3.Text = info.BoxLeftC3;
                    tbScheduleC4.Text = info.BoxLeftC4;
                    tbScheduleC5.Text = info.BoxLeftC5;
                    tbScheduleC6.Text = info.BoxLeftC6;
                    tbScheduleC7.Text = info.BoxLeftC7;
                    tbScheduleC8.Text = info.BoxLeftC8;

                    textboxBoxLeftT1.Text = info.BoxLeftT1;
                    textboxBoxLeftT2.Text = info.BoxLeftT2;
                    textboxBoxLeftT3.Text = info.BoxLeftT3;
                    textboxBoxLeftT4.Text = info.BoxLeftT4;
                    textboxBoxLeftT5.Text = info.BoxLeftT5;
                    textboxBoxLeftT6.Text = info.BoxLeftT6;
                    textboxBoxLeftT7.Text = info.BoxLeftT7;
                    textboxBoxLeftT8.Text = info.BoxLeftT8;

                    #endregion
                    #region Groups
                    boxGroupTitle.Text = info.GroupTitle;
                    textGroupsJSONFileURL.Text = info.GroupJSONUrl;
                    group1title.Text = info.Group1Title;
                    group2title.Text = info.Group2Title;
                    group3title.Text = info.Group3Title;
                    group4title.Text = info.Group4Title;
                    #endregion                 
                    #region Groups - Group A
                    group1name1.Text = info.Group1Name1;
                    group1name2.Text = info.Group1Name2;
                    group1name3.Text = info.Group1Name3;
                    group1name4.Text = info.Group1Name4;

                    group1wins1.Text = info.Group1Wins1;
                    group1wins2.Text = info.Group1Wins2;
                    group1wins3.Text = info.Group1Wins3;
                    group1wins4.Text = info.Group1Wins4;
                    #endregion
                    #region Groups - Group B
                    group2name1.Text = info.Group2Name1;
                    group2name2.Text = info.Group2Name2;
                    group2name3.Text = info.Group2Name3;
                    group2name4.Text = info.Group2Name4;

                    group2wins1.Text = info.Group2Wins1;
                    group2wins2.Text = info.Group2Wins2;
                    group2wins3.Text = info.Group2Wins3;
                    group2wins4.Text = info.Group2Wins4;
                    #endregion
                    #region Groups - Group C
                    group3name1.Text = info.Group3Name1;
                    group3name2.Text = info.Group3Name2;
                    group3name3.Text = info.Group3Name3;
                    group3name4.Text = info.Group3Name4;

                    group3wins1.Text = info.Group3Wins1;
                    group3wins2.Text = info.Group3Wins2;
                    group3wins3.Text = info.Group3Wins3;
                    group3wins4.Text = info.Group3Wins4;
                    #endregion
                    #region Groups - Group D
                    group4name1.Text = info.Group4Name1;
                    group4name2.Text = info.Group4Name2;
                    group4name3.Text = info.Group4Name3;
                    group4name4.Text = info.Group4Name4;

                    group4wins1.Text = info.Group4Wins1;
                    group4wins2.Text = info.Group4Wins2;
                    group4wins3.Text = info.Group4Wins3;
                    group4wins4.Text = info.Group4Wins4;
                    #endregion
                    #region End Credits
                    rtbCreditsText.Text = info.EndCredits;
                    tbCreditsLength.Text = info.EndCreditsLength;
                    tbCreditsStartPos.Text = info.EndCreditsStartPos;
                    #endregion
                    #region Twitter 
                    twitterHashtagTextbox.Text = info.Twitter1;
                    twitterURLTextbox.Text = info.Twitter2;
                    #endregion
                    #region Playoffs

                    textPlayoffsJSONFileURL.Text = info.PlayoffsJSONUrl;

                    boxPlayoffsTitle.Text = info.PlayoffsTitle;

                    ro16team1.Text = info.Ro16Team1;
                    ro16team2.Text = info.Ro16Team2;
                    ro16team3.Text = info.Ro16Team3;
                    ro16team4.Text = info.Ro16Team4;
                    ro16team5.Text = info.Ro16Team5;
                    ro16team6.Text = info.Ro16Team6;
                    ro16team7.Text = info.Ro16Team7;
                    ro16team8.Text = info.Ro16Team8;
                    ro16team9.Text = info.Ro16Team9;
                    ro16team10.Text = info.Ro16Team10;
                    ro16team11.Text = info.Ro16Team11;
                    ro16team12.Text = info.Ro16Team12;
                    ro16team13.Text = info.Ro16Team13;
                    ro16team14.Text = info.Ro16Team14;
                    ro16team15.Text = info.Ro16Team15;
                    ro16team16.Text = info.Ro16Team16;

                    ro16score1.Text = info.Ro16Score1;
                    ro16score2.Text = info.Ro16Score2;
                    ro16score3.Text = info.Ro16Score3;
                    ro16score4.Text = info.Ro16Score4;
                    ro16score5.Text = info.Ro16Score5;
                    ro16score6.Text = info.Ro16Score6;
                    ro16score7.Text = info.Ro16Score7;
                    ro16score8.Text = info.Ro16Score8;
                    ro16score9.Text = info.Ro16Score9;
                    ro16score10.Text = info.Ro16Score10;
                    ro16score11.Text = info.Ro16Score11;
                    ro16score12.Text = info.Ro16Score12;
                    ro16score13.Text = info.Ro16Score13;
                    ro16score14.Text = info.Ro16Score14;
                    ro16score15.Text = info.Ro16Score15;
                    ro16score16.Text = info.Ro16Score16;

                    ro8team1.Text = info.Ro8Team1;
                    ro8team2.Text = info.Ro8Team2;
                    ro8team3.Text = info.Ro8Team3;
                    ro8team4.Text = info.Ro8Team4;
                    ro8team5.Text = info.Ro8Team5;
                    ro8team6.Text = info.Ro8Team6;
                    ro8team7.Text = info.Ro8Team7;
                    ro8team8.Text = info.Ro8Team8;

                    ro8score1.Text = info.Ro8Score1;
                    ro8score2.Text = info.Ro8Score2;
                    ro8score3.Text = info.Ro8Score3;
                    ro8score4.Text = info.Ro8Score4;
                    ro8score5.Text = info.Ro8Score5;
                    ro8score6.Text = info.Ro8Score6;
                    ro8score7.Text = info.Ro8Score7;
                    ro8score8.Text = info.Ro8Score8;

                    ro4team1.Text = info.Ro4Team1;
                    ro4team2.Text = info.Ro4Team2;
                    ro4team3.Text = info.Ro4Team3;
                    ro4team4.Text = info.Ro4Team4;

                    ro4score1.Text = info.Ro4Score1;
                    ro4score2.Text = info.Ro4Score2;
                    ro4score3.Text = info.Ro4Score3;
                    ro4score4.Text = info.Ro4Score4;

                    ro2team1.Text = info.Ro2Team1;
                    ro2team2.Text = info.Ro2Team2;

                    ro2score1.Text = info.Ro2Score1;
                    ro2score2.Text = info.Ro2Score2;
                    #endregion
                    #region scoreboard
                    hearthstoneScoreboardName1Textbox.Text = info.scoreBoardName1;
                    hearthstoneScoreboardName2Textbox.Text = info.scoreBoardName2;
                    hearthstoneScoreboardScore1Numeric.Text =  info.scoreBoardScore1;
                    hearthstoneScoreboardScore2Numeric.Text = info.scoreBoardScore2;
                    #endregion
                    #region clock & countdown
                    clockTitleTextbox.Text = info.clockTitle;
                    countdownTitleTextbox.Text = info.countdownTitle;
                    vsPlayersRoundOf.Text = info.vsPlayersRoundof;
                    vsTeamsRoundOfTextbox.Text = info.vsTeamsRoundOf;
                    #endregion
                    setInfoColors("Data loaded");

                    System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
                    MyTimer.Interval = (5000);
                    MyTimer.Tick += new EventHandler(timer1_Tick);
                    MyTimer.Start();
                }
            }
        }

        public void loadHotkeys()
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
                    casparCGTemplatesFolderPath = map.casparCGTemplateFolder;
                    panelistHotkey = map.panelistHotkey;
                    guestsHotkey = map.guestHotkey;
                    toggleHSIngame = map.hsIngameHotkey;
                    switchHSIngame = map.switchHSIngameHotkey;
                    
                    setInfoColors("Hotkeys loaded");

                    System.Windows.Forms.Timer MyTimer = new System.Windows.Forms.Timer();
                    MyTimer.Interval = (5000);
                    MyTimer.Tick += new EventHandler(timer1_Tick);
                    MyTimer.Start();
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            setOkayColors();
        }

        private void confirmSaveData()
        {
            string message = "Do you want to save settings before closing?";
            string caption = "Close";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons);

            if (result == DialogResult.Yes)
            {
                saveData();
                Close();
            }
            else if (result == DialogResult.No)
            {
                Close();
            }
        }

        private void confirmLoadData()
        {
            string message = "Do you want to load previous data from the data.xml file?";
            string caption = "Load data";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                loadData();
            }
            else if (result == DialogResult.No)
            {

            }
        }
        #endregion

        #region CasparCG settings and functions

        private void disableServerSettings()
        {
            textboxServerAddress.Enabled = false;
            textboxOSCPort.Enabled = false;
            textboxAMCPPort.Enabled = false;
            textboxTemplateFolder.Enabled = false;
        }

        private void disconnectCasparCGDevice()
        {
            casparDevice.Disconnect();
            btnConnectServer.Enabled = true;
            btnDisconnectServer.Enabled = false;
        }

        private void SetupCasparCGDevice()
        {
            casparDevice.Settings.Hostname = caspar_server_IP;
            casparDevice.Settings.Port = caspar_server_AMCP_port;
            casparDevice.Settings.AutoConnect = false;
            casparDevice.Connected += new EventHandler<Svt.Network.NetworkEventArgs>(caspar_AMCP_Connected);
            casparDevice.Disconnected += new EventHandler<Svt.Network.NetworkEventArgs>(caspar_AMCP_Disconnected);
            casparDevice.FailedConnect += new EventHandler<Svt.Network.NetworkEventArgs>(casparDevice_AMCPFailed_Connect);
            casparDevice.Connect();

        }

        void caspar_AMCP_Connected(object sender, Svt.Network.NetworkEventArgs e)
        {
            Invoke(new MethodInvoker(delegate()
            {
                tsConnection.BackColor = Color.LightSeaGreen;
                tsConnection.ForeColor = Color.White;
                tsConnection.Text = "Connected to server";
                disableServerSettings();
                btnConnectServer.Enabled = false;
                btnDisconnectServer.Enabled = true;
            }));
        }

        void caspar_AMCP_Disconnected(object sender, Svt.Network.NetworkEventArgs e)
        {
            casparDevice_AMCPFailed_Connect(sender, e);
            btnConnectServer.Enabled = true;
            btnDisconnectServer.Enabled = false;
        }

        void casparDevice_AMCPFailed_Connect(object sender, Svt.Network.NetworkEventArgs e)
        {
            Invoke(new MethodInvoker(delegate()
           {
               tsConnection.BackColor = Color.OrangeRed;
               tsConnection.ForeColor = Color.White;
               tsConnection.Text = "Connected to server";
               btnConnectServer.Enabled = true;
               btnDisconnectServer.Enabled = false;
           }));
        }

        // Caspar Commands, Update & Invoke
        private void casparUpdateInvoke(int channel, int layer, string functionName)
        {
            string templateFolder = textboxTemplateFolder.Text;
            casparDevice.SendString("CG " + channel + "-" + layer + " UPDATE 1 \"" + cgData.ToAMCPEscapedXml() + "\"");
            casparDevice.SendString("CG " + channel + "-" + layer + " INVOKE 1 \"" + templateFolder + "/" + functionName + "\"");
        }

        // Caspar Commands, Invoke
        private void casparInvoke(int channel, int layer, string functionName)
        {
            casparDevice.Channels[channel].CG.Invoke(layer, functionName);
            System.Diagnostics.Debug.WriteLine("Invoke");
            System.Diagnostics.Debug.WriteLine(functionName);
        }
        // Caspar Commands, Add
        private void casparAdd(int channel, int layer, string template)
        {
            string templateFolder = textboxTemplateFolder.Text;
            
           //  casparDevice.Channels[channel].CG.Add( layer, templateFolder + "/" + template, true, cgData);

            string xmlData = cgData.ToXml();

            casparDevice.SendString("CG " + channel + "-" + layer + " ADD 1 \""  + templateFolder + "/" + template.ToUpper() + "\" 1 \"" + cgData.ToAMCPEscapedXml() + "\"");
          //  casparDevice.SendString("CG " + channel + "-" + layer 
        }

        private void casparLoadBG(int channel, int layer, string filename, bool loop, string transition, uint duration, string easing )
        {
            string templateFolder = textboxTemplateFolder.Text;

            string looping;
            if (loop == true)
                looping = "LOOP";
            else
                looping = "";

            casparDevice.SendString("LOADBG " + channel + "-" + layer + " \"" + filename + "\" " + transition + " " + duration + " " + easing + " " + looping);
        }

        // Caspar Commands, Stop
        private void casparStop(int channel, int layer)
        {
            casparDevice.SendString("CG " + channel + "-" + layer + " STOP 1");
            System.Diagnostics.Debug.WriteLine("Stop");
            System.Diagnostics.Debug.WriteLine(layer);
        }

        private void setDefaultSettings()
        {
            textboxAMCPPort.Text = System.Convert.ToString(AMCPPort);
            textboxServerAddress.Text = serverAddress;
            textboxOSCPort.Text = System.Convert.ToString(OCSPort);
            textboxTemplateFolder.Text = templateFolder;
        }

        private void setWarningColors(string text)
        {
            tsVaroitus.BackColor = Color.OrangeRed;
            tsVaroitus.ForeColor = Color.White;
            tsVaroitus.Text = text;
        }

        private void setOkayColors()
        {
            tsVaroitus.BackColor = Color.Transparent;
            tsVaroitus.ForeColor = Color.Transparent;
            tsVaroitus.Text = "";
        }

        private void setInfoColors(string text)
        {
            tsVaroitus.BackColor = Color.LightSeaGreen;
            tsVaroitus.ForeColor = Color.White;
            tsVaroitus.Text = text;
        }

        private void setEmptyInfoColor(string text)
        {
            tsVaroitus.BackColor = Color.YellowGreen;
            tsVaroitus.ForeColor = Color.White;
            tsVaroitus.Text = text;
        }

        #endregion
        #region Button clicks
        private void menuItemLataa_Click(object sender, EventArgs e)
        {
            DateTime lastWriteData = System.IO.File.GetLastWriteTime("data.xml");

            string message = "Do you want to load previous data? Last save done " + lastWriteData.ToString("dd.mm HH:mm:ss");
            string caption = "Load";

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                loadData();
            }
        }
        private void menuItemTallenna_Click(object sender, EventArgs e)
        {
            DateTime lastWriteData = System.IO.File.GetLastWriteTime("data.xml");

            string message = "Do you want to overwrite data set at " + lastWriteData.ToString("dd.mm HH:mm:ss") + "";
            string caption = "Save";

            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                saveData();
            }
        }
        private void lopetaToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void tallennaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveData();
        }
        private void lataaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            confirmLoadData();
        }
        private void lopetaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void cbSettingsLock_CheckedChanged(object sender, EventArgs e)
        {
            if (cbLockSettings.Checked)
            {
                textboxServerAddress.Enabled = false;
                textboxOSCPort.Enabled = false;
                textboxAMCPPort.Enabled = false;
                textboxTemplateFolder.Enabled = false;
                templateFolder = textboxTemplateFolder.Text;
                serverAddress = textboxServerAddress.Text;
                AMCPPort = System.Convert.ToInt32(textboxAMCPPort.Text);
                OCSPort =  System.Convert.ToInt32(textboxOSCPort.Text);
            }
            else
            {
                textboxServerAddress.Enabled = true;
                textboxOSCPort.Enabled = true;
                textboxAMCPPort.Enabled = true;
                textboxTemplateFolder.Enabled = true;
            }
        }
        private void CBonlyShowRo8_CheckedChanged(object sender, EventArgs e)
        {
            if (CBonlyShowRo8.Checked == true)
            {
                ro16score1.Enabled = false; ro16score2.Enabled = false; ro16score3.Enabled = false; ro16score4.Enabled = false; ro16score5.Enabled = false; ro16score6.Enabled = false; ro16score7.Enabled = false; ro16score8.Enabled = false; ro16score9.Enabled = false; ro16score10.Enabled = false; ro16score11.Enabled = false; ro16score12.Enabled = false; ro16score13.Enabled = false; ro16score14.Enabled = false; ro16score15.Enabled = false; ro16score16.Enabled = false; ro16team1.Enabled = false; ro16team2.Enabled = false; ro16team3.Enabled = false; ro16team4.Enabled = false; ro16team5.Enabled = false; ro16team6.Enabled = false; ro16team7.Enabled = false; ro16team8.Enabled = false; ro16team9.Enabled = false; ro16team10.Enabled = false; ro16team11.Enabled = false; ro16team12.Enabled = false; ro16team13.Enabled = false; ro16team14.Enabled = false; ro16team15.Enabled = false; ro16team16.Enabled = false;
            }
            else
            {
                ro16score1.Enabled = true; ro16score2.Enabled = true; ro16score3.Enabled = true; ro16score4.Enabled = true; ro16score5.Enabled = true; ro16score6.Enabled = true; ro16score7.Enabled = true; ro16score8.Enabled = true; ro16score9.Enabled = true; ro16score10.Enabled = true; ro16score11.Enabled = true; ro16score12.Enabled = true; ro16score13.Enabled = true; ro16score14.Enabled = true; ro16score15.Enabled = true; ro16score16.Enabled = true; ro16team1.Enabled = true; ro16team2.Enabled = true; ro16team3.Enabled = true; ro16team4.Enabled = true; ro16team5.Enabled = true; ro16team6.Enabled = true; ro16team7.Enabled = true; ro16team8.Enabled = true; ro16team9.Enabled = true; ro16team10.Enabled = true; ro16team11.Enabled = true; ro16team12.Enabled = true; ro16team13.Enabled = true; ro16team14.Enabled = true; ro16team15.Enabled = true; ro16team16.Enabled = true;
            }
        }

        private void buttonShowGroups_Click(object sender, EventArgs e)
        {
            showGroups();
        }
        private void btnShowEndCredits_Click(object sender, EventArgs e)
        {
            showEndCredits();
        }
        private void btnHideEndCredits_Click(object sender, EventArgs e)
        {
            hideEndCredits();
        }
        private void buttonHideGroups_Click(object sender, EventArgs e)
        {
            hideGroups();
        }
        private void btnShowPlayerLeft_Click(object sender, EventArgs e)
        {
            showPlayerLeft();
        }
        private void btnHidePlayers_Click(object sender, EventArgs e)
        {
            hidePlayers();
        }
        private void btnShowPlayerRight_Click(object sender, EventArgs e)
        {
        }
        private void btnShowSponsors_Click(object sender, EventArgs e)
        {
            showSponsors();
        }
        private void btnHideSponsors_Click(object sender, EventArgs e)
        {
            hideSponsors();
        }
        private void btnShowCountdown_Click(object sender, EventArgs e)
        {
            showCountdown();
        }
        private void btnHideCountdown_Click(object sender, EventArgs e)
        {
            hideCountdown();
        }
        private void btnShowScheduleLeft_Click(object sender, EventArgs e)
        {
            showScheduleLeft();
        }
        private void btnHideSchedules_Click(object sender, EventArgs e)
        {
            hideSchedule();
        }
        private void btnShowCasterLeft_Click(object sender, EventArgs e)
        {
            showCasterLeft();
        }
        private void btnShowCasterRight_Click(object sender, EventArgs e)
        {
            showCasterRight();
        }
        private void btnShowCasters_Click(object sender, EventArgs e)
        {
            showCasters();
        }
        private void btnHideCasters_Click(object sender, EventArgs e)
        {
            hideCasters();
        }
        private void btnShowClock_Click(object sender, EventArgs e)
        {
            showClock();
        }
        private void btnHideClock_Click(object sender, EventArgs e)
        {
            hideClock();
        }
        private void btnShowGroups_Click(object sender, EventArgs e)
        {
            showGroups();
        }
        private void btnHideGroups_Click(object sender, EventArgs e)
        {
            hideGroups();
        }
        private void btnHideGraphics_Click(object sender, EventArgs e)
        {
            stopGraphics();
        }
        private void btnRestoreDefaultServerSettings_Click(object sender, EventArgs e)
        {
            setDefaultSettings();
        }
        private void btnDisconnectServer_Click(object sender, EventArgs e)
        {
            disconnectCasparCGDevice();
        }
        private void btnConnectServer_Click(object sender, EventArgs e)
        {
            SetupCasparCGDevice();
        }
        private void infoPanel02_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel02);
        }
        private void infoPanel03_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel03);
        }
        private void infoPanel04_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel04);
        }
        private void infoPanel05_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel05);
        }
        private void infoPanel06_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel06);
        }
        private void infoPanel07_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel07);
        }

        private void infoPanel08_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel08);
        }
        private void infoPanel09_DoubleClick(object sender, EventArgs e)
        {
            togglePanel(infoPanel09);
        }
        private void button11_Click(object sender, EventArgs e)
        {
            showInfo();
        }
        private void btnHideInfo_Click(object sender, EventArgs e)
        {
            hideInfo();
        }
        private void btnShowTwitter_Click(object sender, EventArgs e)
        {
            showTwitter();
        }
        private void btnHideTwitter_Click(object sender, EventArgs e)
        {
            hideTwitter();
        }
        private void btnShowGroups_Click_1(object sender, EventArgs e)
        {
            showGroups();
        }
        private void btnUpdateInfo_Click(object sender, EventArgs e)
        {
            showInfo(true);
        }
        private void btnUpdateTwitter_Click(object sender, EventArgs e)
        {
            showTwitter(true);
        }
        private void btnShowPlayoffs_Click_1(object sender, EventArgs e)
        {
            showPlayoffs();
        }
        private void button1_Click(object sender, EventArgs e)
        {
            fillPlayoffs();
        }
        private void button2_Click(object sender, EventArgs e)
        {
            showPlayoffs(true);
        }
        private void btnShowPlayers_Click(object sender, EventArgs e)
        {
            showPlayerLeft();
        }
        private void panel3_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePanel(infoPanel01);
        }
        private void btnUpdateCasters_Click(object sender, EventArgs e)
        {
            showCasters(true);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            showScheduleLeft(true);
        }
        private void btnUpdateCountdown_Click(object sender, EventArgs e)
        {
            showCountdown(true);
        }
        private void btnShowPlayoffs_Click_2(object sender, EventArgs e)
        {
            showPlayoffs();
        }
        private void button2_Click_1(object sender, EventArgs e)
        {
            showPlayoffs(true);
        }
        private void btnHidePlayoffs_Click_2(object sender, EventArgs e)
        {
            hidePlayoffs();
        }
        private void btnUpdatePlayoffJSON_Click(object sender, EventArgs e)
        {
            fillPlayoffs();
        }
        private void button3_Click(object sender, EventArgs e)
        {
            fillGroups();
        }
        private void btnHideGroups_Click_1(object sender, EventArgs e)
        {
            hideGroups();
        }
        private void btnShowEndCredits_Click_1(object sender, EventArgs e)
        {
            showEndCredits();
        }
        private void btnHideEndCredits_Click_1(object sender, EventArgs e)
        {
            hideEndCredits();
        }
        private void btnUpdateGroups_Click(object sender, EventArgs e)
        {
            showGroups(true);
        }
        private void togglePanel(Panel panel)
        {
            if (activePanel != null)
            {
                foreach (Control c in activePanel.Controls)
                {
                    c.Enabled = false;
                }
                activePanel.BackColor = Color.FromArgb(216, 234, 238);
                activePanel = null;
            }

            foreach (Control c in panel.Controls)
            {
                if (c.Enabled == false)
                {
                    panel.BackColor = Color.White;
                    c.Enabled = true;
                    activePanel = panel;
                }
                else
                {
                    panel.BackColor = Color.FromArgb(216, 234, 238);
                    c.Enabled = false;
                }
            }
        }

        public void togglePresentationPanel(Panel panel)
        {
            if (activePresentationPanel != null)
            {
                foreach (Control c in activePresentationPanel.Controls)
                {
                    c.Enabled = false;
                }
                activePresentationPanel.BackColor = Color.FromArgb(216, 234, 238);
                activePresentationPanel = null;
            }

            foreach (Control c in panel.Controls)
            {
                if (c.Enabled == false)
                {
                    panel.BackColor = Color.White;
                    c.Enabled = true;
                    activePresentationPanel = panel;
                }
                else
                {
                    panel.BackColor = Color.FromArgb(216, 234, 238);
                    c.Enabled = false;
                }
            }
        }

        private void interviewTemplate(object sender, EventArgs e)
        {
            RadioButton btn = sender as RadioButton;
            if (btn != null && btn.Checked)
            {
                switch (btn.Name)
                {
                    case "rbInterviewTemplateNormal":
                        interviewTemplateSelection = "Interview";
                        break;
                    case "rbInterviewTemplateThird":
                        interviewTemplateSelection = "Interview_lower_third";
                        break;
                }
            }
        }
        private void btnShowPlayoffs_Click(object sender, EventArgs e)
        {
            showPlayoffs();
        }
        private void btnHidePlayoffs_Click(object sender, EventArgs e)
        {
            hidePlayoffs();
        }
        #endregion
        #region CasparCG Layer numbers
        // ALL FUNCTIONS
        // LAYERS
        // COUNTDOWN: 1
        // CLOCK: 2
        // CASTERS: 3
        // INFOPANEL LEFT: 4
        // INFOPANEL RIGHT: 5
        // GROUPS: 6
        // PLAYOFFS: 7
        // Twitter: 8 
        #endregion
        // All functions are named as follows
        // showTemplate
        // hideTemplate
        //
        // ie. showUpcoming, showGroups etc.
        // Same goes for 
        // hideTemplate
        // ie. hideUpcoming, hideGroups etc.
        //
        // For the UpdateInvoke there is a boolean set in some function's parameters bool update = false;
        // By changing this to 'true' it changes the 'Add' to 'Update'

        #region Functions for stopping all gfx and checking the bigger screen area
        // Function: Stops all graphics in CasparCG screen by going through each hide function and executing it
        private void stopGraphics()
        {
            if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
            {
                System.Diagnostics.Debug.WriteLine("Clear on: " + 0);

                try
                {
                    if (isClockActive)
                        hideClock();

                    if (isCountdownActive)
                        hideCountdown();

                    if (isCastersActive || isCasterLeftActive || isCasterRightActive)
                        hideCasters();

                    if (isScheduleLeftActive)
                        hideSchedule();

                    if (isGroupsActive)
                        hideGroups();

                    if (isPlayoffsActive)
                        hidePlayoffs();

                    if (isPlayersActive)
                        hidePlayers();

                    if (isSponsorsActive)
                        hideSponsors();

                    if (isEndCreditsActive)
                        hideEndCredits();

                    if (isInfoActive)
                        hideInfo();

                    if (isTwitterActive)
                        hideTwitter();

                    if (isTeamsActive)
                        hideTeams();

                    if (isInterviewActive)
                        hideInterview();

                    Thread.Sleep(1000);

                    casparDevice.Channels[0].CG.Clear();
                    casparDevice.Channels[0].Clear();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(ex.ToString());
                    throw;
                }
                finally
                {
                    casparDevice.Channels[0].CG.Clear();
                    casparDevice.Channels[0].Clear();
                }
            }
        }
        #endregion


        #region Layers 9 Schedule
        private void showScheduleLeft(bool update = false)
        {
            try
            {
                cgData.Clear();

                cgData.SetData("scheduleTitle", textboxBoxLeftTitle.Text);

                cgData.SetData("scheduleC1", tbScheduleC1.Text);
                cgData.SetData("scheduleC2", tbScheduleC2.Text);
                cgData.SetData("scheduleC3", tbScheduleC3.Text);
                cgData.SetData("scheduleC4", tbScheduleC4.Text);
                cgData.SetData("scheduleC5", tbScheduleC5.Text);
                cgData.SetData("scheduleC6", tbScheduleC6.Text);
                cgData.SetData("scheduleC7", tbScheduleC7.Text);
                cgData.SetData("scheduleC8", tbScheduleC8.Text);

                cgData.SetData("scheduleT1", textboxBoxLeftT1.Text);
                cgData.SetData("scheduleT2", textboxBoxLeftT2.Text);
                cgData.SetData("scheduleT3", textboxBoxLeftT3.Text);
                cgData.SetData("scheduleT4", textboxBoxLeftT4.Text);
                cgData.SetData("scheduleT5", textboxBoxLeftT5.Text);
                cgData.SetData("scheduleT6", textboxBoxLeftT6.Text);
                cgData.SetData("scheduleT7", textboxBoxLeftT7.Text);
                cgData.SetData("scheduleT8", textboxBoxLeftT8.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isCastersActive)
                        hideCasters();

                    if (update == true)
                    {
                        casparUpdateInvoke(1, 9, "Schedule");
                    }
                    else
                    {
                        casparAdd(1, 9, "Schedule");
                        isScheduleLeftActive = true;

                        btnShowScheduleLeft.Enabled = false;
                        btnHideSchedules.Enabled = true;
                        btnUpdateSchedule.Enabled = true;
                    }
                }
            }
        }


        private void hideSchedule()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparStop(1, 9);
                    isScheduleLeftActive = false;

                    btnShowScheduleLeft.Enabled = true;
                    btnHideSchedules.Enabled = false;
                    btnUpdateSchedule.Enabled = false;
                }
            }
        }
        #endregion

        #region Layer 10 Information
        private void showInfo(bool update = false)
        {
            string panel = Convert.ToString(activePanel.Name);

            panelItems.Clear();

            foreach (Control c in activePanel.Controls)
            {
                if (c is TextBox)
                {
                    panelItems.Add(c.Text);
                }
            }

            try
            {
                cgData.Clear();
                cgData.SetData("infoTextTitle", panelItems[0]);
                cgData.SetData("infoText01", panelItems[1]);
                cgData.SetData("infoText02", panelItems[2]);
                cgData.SetData("infoText03", panelItems[3]);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 10, "Info");
                    }
                    else
                    {
                        casparAdd(1, 10, "Info");
                        isInfoActive = true;

                        btnShowInfo.Enabled = false;
                        btnHideInfo.Enabled = true;
                        btnUpdateInfo.Enabled = true;

                    }
                }
            }
        }

        private void hideInfo()
        {
            try  { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparStop(1, 10);
                    isInfoActive = false;

                    btnShowInfo.Enabled = true;
                    btnHideInfo.Enabled = false;
                    btnUpdateInfo.Enabled = false;

                    if (isClockActive)
                        hideClock();

                }
            }
        }
        #endregion

        #region Layer 11 Clock
        private void showClock()
        {
            try {
                cgData.Clear();
                cgData.SetData("tDescription", clockTitleTextbox.Text);            
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparAdd(1, 11, "Clock");
                    isClockActive = true;

                    btnShowClock.Enabled = false;
                    btnHideClock.Enabled = true;    
                }
            }
        }
        private void hideClock()
        {
            try
            {
            
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isCountdownActive)
                        hideCountdown();

                    casparStop(1,11);
                    isClockActive = false;

                    btnHideClock.Enabled = false;
                    btnShowClock.Enabled = true;
                }
            }
        }
        #endregion

        #region Layer 12 Countdown
        private void showCountdown(bool update = false)
        {
            setOkayColors();

            if (isInfoActive)
            {
                try
                {
                    cgData.Clear();
                    cgData.SetData("f0", dtCountDownTime.Value.Year.ToString());
                    cgData.SetData("f1", dtCountDownTime.Value.Month.ToString());
                    cgData.SetData("f2", dtCountDownTime.Value.Day.ToString());
                    cgData.SetData("f3", dtCountDownTime.Value.Hour.ToString());
                    cgData.SetData("f4", dtCountDownTime.Value.Minute.ToString());
                    cgData.SetData("f5", "0");
                    cgData.SetData("tDescription", countdownTitleTextbox.Text);
                }
                catch { }
                finally
                {
                    if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                    {
                        if (isClockActive == false)
                            showClock();

                        if (update == true)
                        {
                            casparUpdateInvoke(1, 12, "Countdown");
                        }
                        else
                        {
                            casparAdd(1, 12, "Countdown");
                            isCountdownActive = true;

                            btnShowCountdown.Enabled = false;
                            btnHideCountdown.Enabled = true;
                            btnUpdateCountdown.Enabled = true;
                        }
                    }
                }
            }
            else
            {
                setWarningColors("Activate Information-panel first");
                timerEvent(100000);
            }
        }

        private void hideCountdown()
        {
            try
            {
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparStop(1, 12);
                    isCountdownActive = false;

                    btnShowCountdown.Enabled = true;
                    btnHideCountdown.Enabled = false;
                    btnUpdateCountdown.Enabled = false;
                }
            }
        }
        #endregion

        #region Layers 13-14 Casters
        private void showCasterLeft(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText", textboxCasterNicknameLeft.Text);
                cgData.SetData("realnameText", textboxCasterNameLeft.Text);
                cgData.SetData("altText", textboxCasterAltLeft.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 13, "CasterLeft");
                    }
                    else
                    {
                        casparAdd(1, 13, "CasterLeft");
                        isCasterLeftActive = true;

                        btnShowCasters.Enabled = false;
                        btnShowCasterLeft.Enabled = false;

                        btnHideCasters.Enabled = true;
                        btnUpdateCasters.Enabled = true;
                    }
                }
            }
        }

        private void showCasterRight(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText2", textboxCasterNicknameRight.Text);
                cgData.SetData("realnameText2", textboxCasterNameRight.Text);
                cgData.SetData("altText2", textboxCasterAltRight.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 14, "CasterRight");
                    }
                    else
                    {
                        casparAdd(1, 14, "CasterRight");
                        isCasterRightActive = true;

                        btnShowCasters.Enabled = false;

                        btnShowCasterRight.Enabled = false;
                        btnHideCasters.Enabled = true;
                        btnUpdateCasters.Enabled = true;
                    }
                }
            }
        }

        private void showCasters(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText", textboxCasterNicknameLeft.Text);
                cgData.SetData("realnameText", textboxCasterNameLeft.Text);
                cgData.SetData("altText", textboxCasterAltLeft.Text);
                cgData.SetData("nicknameText2", textboxCasterNicknameRight.Text);
                cgData.SetData("realnameText2", textboxCasterNameRight.Text);
                cgData.SetData("altText2", textboxCasterAltRight.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 13, "Casters");
                    }
                    else
                    {
                        casparAdd(1, 13, "Casters");
                        isCastersActive = true;

                        btnShowCasters.Enabled = false;
                        btnShowCasterLeft.Enabled = false;

                        btnHideCasters.Enabled = true;
                        btnUpdateCasters.Enabled = true;
                    }
                }
            }
        }

        private void hideCasters()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isCasterLeftActive)
                    {
                        casparStop(1, 13);
                        isCasterLeftActive = false;
                    }

                    if (isCasterRightActive)
                    {
                        casparStop(1, 14);
                        isCasterRightActive = false;
                    }
                    if (isCastersActive)
                    {
                        casparStop(1, 13);
                        isCastersActive = true;
                    }

                    btnShowCasters.Enabled = true;
                    btnShowCasterLeft.Enabled = true;
                    btnShowCasterRight.Enabled = true;
                    btnHideCasters.Enabled = false;
                    btnUpdateCasters.Enabled = false;
                }
            }
        }
        #endregion

        #region Layer 16, Guests
        private void showGuestsLeft(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText", guestsLeft01Textbox.Text);
                cgData.SetData("realnameText", guestsLeft02Textbox.Text);
                cgData.SetData("altText", guestsLeft03Textbox.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 13, "CasterLeft");
                    }
                    else
                    {
                        casparAdd(1, 13, "CasterLeft");
                        isGuestsActive = true;

                        guestsShowButton.Enabled = false;
                        guestsHideButton.Enabled = true;
                        guestsUpdateButton.Enabled = true;
                    }
                }
            }
        }

        private void showGuestsRight(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText2", guestsRight01Textbox.Text);
                cgData.SetData("realnameText2", guestsRight02Textbox.Text);
                cgData.SetData("altText2", guestsRight03Textbox.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 14, "CasterRight");
                    }
                    else
                    {
                        casparAdd(1, 14, "CasterRight");
                    }
                }
            }
        }

        private void hideGuests()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isGuestsActive)
                    {
                        casparStop(1, 13);
                        casparStop(1, 14);
                        isGuestsActive = false;
                    }

                    guestsShowButton.Enabled = true;
                    guestsHideButton.Enabled = false;
                    guestsUpdateButton.Enabled = false;
                }
            }
        }

        private void showPanelist(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText", panelistNicknameTextbox.Text);
                cgData.SetData("realnameText", panelistRealNameTextbox.Text);
                cgData.SetData("altText", panelistTwitterTextbox.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 13, "CasterLeft");
                    }
                    else
                    {
                        casparAdd(1, 13, "CasterLeft");
                        isPanelistsActive = true;

                        panelistShowButton.Enabled = false;
                        panelistHideButton.Enabled = true;
                        panelistUpdateButton.Enabled = true;
                    }
                }
            }
        }

        private void hidePanelist()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isPanelistsActive)
                    {
                        casparStop(1, 13);
                        isPanelistsActive = false;
                    }

                    panelistShowButton.Enabled = true;
                    panelistHideButton.Enabled = false;
                    panelistUpdateButton.Enabled = false;
                }
            }
        }
        #endregion

        #region Layer 15 Twitter
        private void showTwitter(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("tTwitterTitle", twitterTitleTextbox.Text);
                cgData.SetData("tTwitterHashtag", twitterHashtagTextbox.Text);
                cgData.SetData("tTwitterURL", twitterURLTextbox.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 15, "Twitter");
                    }
                    else
                    {
                        casparAdd(1, 15, "Twitter");
                        isTwitterActive = true;

                        btnShowTwitter.Enabled = false;
                        btnHideTwitter.Enabled = true;
                        btnUpdateTwitter.Enabled = true;
                    }
                }
            }
        }

        private void hideTwitter()
        {
            try
            {
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparStop(1, 15);
                    isTwitterActive = false;

                    btnShowTwitter.Enabled = true;
                    btnHideTwitter.Enabled = false;
                    btnUpdateTwitter.Enabled = false;
                }
            }
        }
        #endregion

        #region Layer 35 Sponsors
        private void showSponsors()
        {
            try
            {
                cgData.Clear();
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparAdd(1, 35, sponsorView);

                    isSponsorsActive = true;

                    btnShowSponsors.Enabled = false;
                    btnHideSponsors.Enabled = true;
                }
            }
        }

        private void hideSponsors()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparStop(1,35);

                    isSponsorsActive = false;

                    btnShowSponsors.Enabled = true;
                    btnHideSponsors.Enabled = false;
                }
            }
        }
        #endregion

        #region Layers 17 Interview
        private void showInterview(bool update = false)
        {
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText", interviewText01Textbox.Text);
                cgData.SetData("realnameText", interviewText02Textbox.Text);
                cgData.SetData("altText", interviewText03Textbox.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 17, "Interview");
                    }
                    else
                    {
                        casparAdd(1, 17, "Interview");
                        isInterviewActive = true;

                        interviewShowButton.Enabled = false;
                        interviewHideButton.Enabled = true;
                        interviewUpdateButton.Enabled = true;
                    }
                }
            }
        }
        private void hideInterview()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isInterviewActive)
                    {
                        casparStop(1, 17);
                        isInterviewActive = false;
                    }

                    interviewShowButton.Enabled = true;
                    interviewHideButton.Enabled = false;
                    interviewUpdateButton.Enabled = false;
                }
            }
        }
        #endregion

        #region Layer 18 Presentation
        public void showPresentation(bool update = false)
        {
            string panel = Convert.ToString(activePresentationPanel.Name);

            panelItems.Clear();

            foreach (Control c in activePresentationPanel.Controls)
            {
                if (c is TextBox)
                {
                    panelItems.Add(c.Text);
                }
            }

            try
            {
                cgData.Clear();
                cgData.SetData("tNickname", panelItems[0]);
                cgData.SetData("tName", panelItems[1]);
                cgData.SetData("tNationality", panelItems[2]);
                cgData.SetData("logo", "logos/" + templateFolder + "/" + panelItems[3]);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 18, "Presentation");
                    }
                    else
                    {
                        casparAdd(1, 18, "Presentation");
                        isPresentationActive = true;

                        presentationShowButton.Enabled = false;
                        presentationHideButton.Enabled = true;
                        presentationUpdateButton.Enabled = true;
                    }
                }
            }
        }

        public void hidePresentation()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparStop(1, 18);

                    isPresentationActive = false;

                    presentationShowButton.Enabled = true;
                    presentationHideButton.Enabled = false;
                    presentationUpdateButton.Enabled = false;
                }
            }
        }
        #endregion

        #region Layer 30 Scoreboard 

        private void showScoreboard(bool update = false, string clear = "")
        {
           
            try
            {
                cgData.Clear();
                cgData.SetData("nicknameText01", hearthstoneScoreboardName1Textbox.Text);
                cgData.SetData("nicknameText02", hearthstoneScoreboardName2Textbox.Text);
                cgData.SetData("scoreText01", hearthstoneScoreboardScore1Numeric.Value.ToString());
                cgData.SetData("scoreText02", hearthstoneScoreboardScore2Numeric.Value.ToString());

                cgData.SetData("nicknameTextLeft", hearthstoneScoreboardName1Textbox.Text);
                cgData.SetData("nicknameTextRight", hearthstoneScoreboardName2Textbox.Text);

                if (templateFolder == "hearthstone")
                {
                    cgData.SetData("playerImageLeft", playerPictureFolder + "/" + templateFolder +"_versus" + "/" + hearthstoneScoreboardName1Textbox.Text + ".png");
                    cgData.SetData("playerImageRight", playerPictureFolder + "/" + templateFolder + "_versus" + "/" + hearthstoneScoreboardName2Textbox.Text + ".png");
                }
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        // glowCover = true -> Left player active. False -> Right player active
                        if (clear == "")
                        {
                            cgData.SetData("borders", glowCover.ToString());
                            casparUpdateInvoke(1, 30, "Scoreboard");

                            if (glowCover == false)
                                glowCover = true;
                            else
                                glowCover = false;
                        }
                        else if (clear == "Clear")
                        {
                            cgData.SetData("borders", "Clear");

                            casparUpdateInvoke(1, 30, "Scoreboard");
                            glowCover = false;
                        }
                    }
                    else
                    {
                        /* Remember to add PLAY <CHANNEL>-<LAYER> DECKLINK <NUMBER>.
                        * <Number> coomes from textbox
                         * Old mixer commands for GoPro cameras connected to Decklink                        

                        casparDevice.SendString("MIXER 1-26 MIPMAP 0");
                        casparDevice.SendString("MIXER 1-26 FILL -0.188281 0 0.555469 0.555556 1 LINEAR");

                        casparDevice.SendString("MIXER 1-25 MIPMAP 0");
                        casparDevice.SendString("MIXER 1-25 FILL 0.648438 0 0.55569 0.55556 1 LINEAR");
                        casparDevice.SendString("MIXER 1-25 CROP 0.371094 0 0.628906 1 1 LINEAR");
                        casparDevice.SendString("MIXER 1-26 CROP 0.34375 0 0.59375 1 1 LINEAR");
                        
                       casparDevice.SendString("PLAY 1-25 DECKLINK DEVICE " + scoreboardDecklink1Textbox.Text + " 720P5000 MIX 25");
                        casparDevice.SendString("PLAY 1-26 DECKLINK DEVICE " + scoreboardDecklink2Textbox.Text + " 720P5000 MIX 25");
                        casparDevice.SendString("MIXER 1-25 OPACITY 1.0 25 LINEAR");
                        casparDevice.SendString("MIXER 1-26 OPACITY 1.0 25 LINEAR");
                        */
                        casparAdd(1, 30, "Scoreboard");
                      //  casparAdd(1, 31, "Sponsors_Ingame");
                        isScoreboardActive = true;
                        
                        showScoreboardButton.Enabled = false;
                        hideScoreboardButton.Enabled = true;
                        updateScoreboardButton.Enabled = true;
                    }
                }
            }
        }

        private void hideScoreboard()
        {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    /*
                     casparDevice.SendString("LOADBG 1-25 EMPTY MIX 25 AUTO");
                     casparDevice.SendString("PLAY 1-25");
                     casparDevice.SendString("LOADBG 1-26 EMPTY MIX 25 AUTO");
                     casparDevice.SendString("PLAY 1-26");
                    
                    casparDevice.SendString("MIXER 1-25 OPACITY 0.0 25 LINEAR");
                    casparDevice.SendString("MIXER 1-26 OPACITY 0.0 25 LINEAR");
                    */
                    casparStop(1, 30);
                   // casparStop(1, 31);
                    isScoreboardActive = false;

                   showScoreboardButton.Enabled = true;
                   hideScoreboardButton.Enabled = false;
                   updateScoreboardButton.Enabled = false;
                }
        }

        #endregion  
      
        // Background video runs on layer 20
        #region Layer 21 Groups

        private void fillGroups()
        {
            if (cbGroupsUseJSON.Checked)
            {
               try {

                using (var webClient = new System.Net.WebClient())
                {
                    string remoteURL = textGroupsJSONFileURL.Text;

                        var jsonURL = webClient.DownloadString(remoteURL);

                        JObject o = JObject.Parse(jsonURL);

                        setInfoColors("Data retrieval successfull");

                        JArray groups = (JArray)o["Groups"];

                        MessageBox.Show(o.ToString());
                        
                            List<Group> group_a = new List<Group>();
                            List<Group> group_b = new List<Group>();
                            List<Group> group_c = new List<Group>();
                            List<Group> group_d = new List<Group>();

                            foreach (JObject group in groups.Children<JObject>())
                            {
                                if (cbGroupsJSONRetrieveNumral.Checked == true)
                                {
                                    if (group["name"].ToString() == "Group 1")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_a.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (group["name"].ToString() == "Group A")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_a.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }

                                if (cbGroupsJSONRetrieveNumral.Checked == true)
                                {
                                    if (group["name"].ToString() == "Group 2")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_b.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (group["name"].ToString() == "Group B")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_b.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }

                                if (cbGroupsJSONRetrieveNumral.Checked == true)
                                {
                                    if (group["name"].ToString() == "Group 3")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_c.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (group["name"].ToString() == "Group C")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_c.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }

                                if (cbGroupsJSONRetrieveNumral.Checked == true)
                                {
                                    if (group["name"].ToString() == "Group 4")
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_d.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }
                                else
                                {
                                    if (group["name"].ToString() == "Group D" )
                                    {
                                        foreach (JObject groupResult in group["GroupResult"].Children<JObject>())
                                        {
                                            group_d.Add(new Group()
                                            {
                                                Team = groupResult["Contestant"]["name"].ToString(),
                                                TeamScore = groupResult["points"].ToString()
                                            });
                                        }
                                    }
                                }
                            }

                            /*
                             * Checking if the list group_x contains any values.
                             * After that going through each cell (max 4) and setting them
                             * in their right fields
                             */
                             
                            /* GROUP A */
                            if (group_a.Any())
                            {
                                group1name1.Text = group_a[0].Team.ToString();
                                group1wins1.Text = group_a[0].TeamScore.ToString();

                                group1name2.Text = group_a[1].Team.ToString();
                                group1wins2.Text = group_a[1].TeamScore.ToString();

                                group1name3.Text = group_a[2].Team.ToString();
                                group1wins3.Text = group_a[2].TeamScore.ToString();

                                group1name4.Text = group_a[3].Team.ToString();
                                group1wins4.Text = group_a[3].TeamScore.ToString();
                            }

                            /* GROUP B */
                            if (group_b.Any())
                            {
                                group2name1.Text = group_b[0].Team.ToString();
                                group2wins1.Text = group_b[0].TeamScore.ToString();

                                group2name2.Text = group_b[1].Team.ToString();
                                group2wins2.Text = group_b[1].TeamScore.ToString();

                                group2name3.Text = group_b[2].Team.ToString();
                                group2wins3.Text = group_b[2].TeamScore.ToString();

                                group2name4.Text = group_b[3].Team.ToString();
                                group2wins4.Text = group_b[3].TeamScore.ToString();
                            }

                            /* GROUP C */
                            if (group_c.Any())
                            {
                                group3name1.Text = group_c[0].Team.ToString();
                                group3wins1.Text = group_c[0].TeamScore.ToString();

                                group3name2.Text = group_c[1].Team.ToString();
                                group3wins2.Text = group_c[1].TeamScore.ToString();

                                group3name3.Text = group_c[2].Team.ToString();
                                group3wins3.Text = group_c[2].TeamScore.ToString();

                                group3name4.Text = group_c[3].Team.ToString();
                                group3wins4.Text = group_c[3].TeamScore.ToString();
                            }

                            /* GROUP D */
                            if (group_d.Any())
                            {
                                group4name1.Text = group_d[0].Team.ToString();
                                group4wins1.Text = group_d[0].TeamScore.ToString();

                                group4name2.Text = group_d[1].Team.ToString();
                                group4wins2.Text = group_d[1].TeamScore.ToString();

                                group4name3.Text = group_d[2].Team.ToString();
                                group4wins3.Text = group_d[2].TeamScore.ToString();

                                group4name4.Text = group_d[3].Team.ToString();
                                group4wins4.Text = group_d[3].TeamScore.ToString();
                            }
                        }
                    }

                    catch(Exception e) {
                        MessageBox.Show("Something went wrong. You might have an invalid JSON url.\nCheck the Error log for more information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        if (File.Exists(errorFile))
                        {
                            using (StreamWriter sw = File.AppendText(errorFile)) {
                            sw.WriteLine(DateTime.Now);
                            sw.WriteLine(e.ToString());
                            sw.WriteLine(" ");
                         }
                    }
                }
            }
        }

        private void showGroups(bool update = false)
        {
            try
            {
                cgData.Clear();

                    cgData.SetData("boxGroupTitle", boxGroupTitle.Text);

                    cgData.SetData("group1title", group1title.Text);
                    cgData.SetData("group2title", group2title.Text);
                    cgData.SetData("group3title", group3title.Text);
                    cgData.SetData("group4title", group4title.Text);

                    cgData.SetData("group1name1", group1name1.Text); cgData.SetData("group1wins1", group1wins1.Text); 
                    cgData.SetData("group1name2", group1name2.Text); cgData.SetData("group1wins2", group1wins2.Text); 
                    cgData.SetData("group1name3", group1name3.Text); cgData.SetData("group1wins3", group1wins3.Text); 
                    cgData.SetData("group1name4", group1name4.Text); cgData.SetData("group1wins4", group1wins4.Text); 

                    cgData.SetData("group2name1", group2name1.Text); cgData.SetData("group2wins1", group2wins1.Text);
                    cgData.SetData("group2name2", group2name2.Text); cgData.SetData("group2wins2", group2wins2.Text);
                    cgData.SetData("group2name3", group2name3.Text); cgData.SetData("group2wins3", group2wins3.Text); 
                    cgData.SetData("group2name4", group2name4.Text); cgData.SetData("group2wins4", group2wins4.Text); 

                    cgData.SetData("group3name1", group3name1.Text); cgData.SetData("group3wins1", group3wins1.Text);
                    cgData.SetData("group3name2", group3name2.Text); cgData.SetData("group3wins2", group3wins2.Text);
                    cgData.SetData("group3name3", group3name3.Text); cgData.SetData("group3wins3", group3wins3.Text);
                    cgData.SetData("group3name4", group3name4.Text); cgData.SetData("group3wins4", group3wins4.Text);


                    cgData.SetData("group4name1", group4name1.Text); cgData.SetData("group4wins1", group4wins1.Text);
                    cgData.SetData("group4name2", group4name2.Text); cgData.SetData("group4wins2", group4wins2.Text);
                    cgData.SetData("group4name3", group4name3.Text); cgData.SetData("group4wins3", group4wins3.Text);
                    cgData.SetData("group4name4", group4name4.Text); cgData.SetData("group4wins4", group4wins4.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {

                    if (update == true)
                    {
                        casparUpdateInvoke(1, 21, "Groups");
                    }
                    else
                    {
                        casparDevice.SendString("LOADBG 1-20 " + templateFolder.ToUpper() + "_BACKGROUND MIX 50 LOOP");
                        casparDevice.SendString("PLAY 1-20");
                        casparAdd(1, 21, "Groups");
                    }
                    
                    isGroupsActive = true;

                    btnHideGroups.Enabled = true;
                    btnShowGroups.Enabled = false;
                    btnUpdateGroups.Enabled = true;
                }
            }
        }

        private void hideGroups()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparDevice.SendString("LOADBG 1-20 EMPTY MIX 50 AUTO");
                    casparDevice.SendString("PLAY 1-20");
                    casparStop(1, 21);

                    isGroupsActive = false;

                    btnHideGroups.Enabled = false;
                    btnUpdateGroups.Enabled = false;
                    btnShowGroups.Enabled = true;
                }
            }
        }

        #endregion

        #region Layer 21 Playoffs

        private void fillPlayoffs()
        {
            if (cbPlayoffsUseJSON.Checked)
            {
                using (var webClient = new System.Net.WebClient())
                {                   
                    string remoteURL = textPlayoffsJSONFileURL.Text;
                    if (remoteURL == null)
                    {
                        setWarningColors("The URL field is empty");
                    }
                    else
                    {                        
                        try
                        {
                            var jsonURL = webClient.DownloadString(remoteURL);

                            JObject o = JObject.Parse(jsonURL);
      
                            setInfoColors("Data retrieval successfull");

                                JArray matches = (JArray)o["Matches"][0];

                                List<Match> ro16_contestants = new List<Match>();
                                List<Match> ro8_contestants = new List<Match>();
                                List<Match> ro4_contestants = new List<Match>();
                                List<Match> ro2_contestants = new List<Match>();

                                foreach (JObject match in matches.Children<JObject>())
                                {

                                    if (match["Match"]["round_of"].ToString() == "16")
                                    {
                                        ro16_contestants.Add(new Match()
                                        {
                                            MatchContestant1 = match["Contestant1"]["name"].ToString(),
                                            MatchContestant2 = match["Contestant2"]["name"].ToString(),
                                            MatchContestant1Score = match["MatchContestant1"]["score"].ToString(),
                                            MatchContestant2Score = match["MatchContestant2"]["score"].ToString()
                                        });
                                    }

                                    if (match["Match"]["round_of"].ToString() == "8")
                                    {
                                        ro8_contestants.Add(new Match()
                                        {
                                            MatchContestant1 = match["Contestant1"]["name"].ToString(),
                                            MatchContestant2 = match["Contestant2"]["name"].ToString(),
                                            MatchContestant1Score = match["MatchContestant1"]["score"].ToString(),
                                            MatchContestant2Score = match["MatchContestant2"]["score"].ToString()
                                        });
                                    }

                                    if (match["Match"]["round_of"].ToString() == "4")
                                    {
                                        ro4_contestants.Add(new Match()
                                        {
                                            MatchContestant1 = match["Contestant1"]["name"].ToString(),
                                            MatchContestant2 = match["Contestant2"]["name"].ToString(),
                                            MatchContestant1Score = match["MatchContestant1"]["score"].ToString(),
                                            MatchContestant2Score = match["MatchContestant2"]["score"].ToString()
                                        });
                                    }

                                    if (match["Match"]["round_of"].ToString() == "2")
                                    {
                                        ro2_contestants.Add(new Match()
                                        {
                                            MatchContestant1 = match["Contestant1"]["name"].ToString(),
                                            MatchContestant2 = match["Contestant2"]["name"].ToString(),
                                            MatchContestant1Score = match["MatchContestant1"]["score"].ToString(),
                                            MatchContestant2Score = match["MatchContestant2"]["score"].ToString()
                                        });
                                    }
                                }

                                if (ro16_contestants.Any())
                                {
                                    // Round of 8
                                    // Match 1
                                    ro16team1.Text = ro16_contestants[0].MatchContestant1.ToString();
                                    ro16score1.Text = ro16_contestants[0].MatchContestant1Score.ToString();
                                    ro16team2.Text = ro16_contestants[0].MatchContestant2.ToString();
                                    ro16score2.Text = ro16_contestants[0].MatchContestant2Score.ToString();

                                    // Match 2
                                    ro16team3.Text = ro16_contestants[1].MatchContestant1.ToString();
                                    ro16score3.Text = ro16_contestants[1].MatchContestant1Score.ToString();
                                    ro16team4.Text = ro16_contestants[1].MatchContestant2.ToString();
                                    ro16score4.Text = ro16_contestants[1].MatchContestant2Score.ToString();

                                    // Match 3
                                    ro16team5.Text = ro16_contestants[2].MatchContestant1.ToString();
                                    ro16score5.Text = ro16_contestants[2].MatchContestant1Score.ToString();
                                    ro16team6.Text = ro16_contestants[2].MatchContestant2.ToString();
                                    ro16score6.Text = ro16_contestants[2].MatchContestant2Score.ToString();

                                    // Match 4
                                    ro16team7.Text = ro16_contestants[3].MatchContestant1.ToString();
                                    ro16score7.Text = ro16_contestants[3].MatchContestant1Score.ToString();
                                    ro16team8.Text = ro16_contestants[3].MatchContestant2.ToString();
                                    ro16score8.Text = ro16_contestants[3].MatchContestant2Score.ToString();

                                    // Match 5
                                    ro16team9.Text = ro16_contestants[4].MatchContestant1.ToString();
                                    ro16score9.Text = ro16_contestants[4].MatchContestant1Score.ToString();
                                    ro16team10.Text = ro16_contestants[4].MatchContestant2.ToString();
                                    ro16score10.Text = ro16_contestants[4].MatchContestant2Score.ToString();

                                    // Match 6
                                    ro16team11.Text = ro16_contestants[5].MatchContestant1.ToString();
                                    ro16score11.Text = ro16_contestants[5].MatchContestant1Score.ToString();
                                    ro16team12.Text = ro16_contestants[5].MatchContestant2.ToString();
                                    ro16score12.Text = ro16_contestants[5].MatchContestant2Score.ToString();

                                    // Match 7
                                    ro16team13.Text = ro16_contestants[6].MatchContestant1.ToString();
                                    ro16score13.Text = ro16_contestants[6].MatchContestant1Score.ToString();
                                    ro16team14.Text = ro16_contestants[6].MatchContestant2.ToString();
                                    ro16score14.Text = ro16_contestants[6].MatchContestant2Score.ToString();

                                    // Match 8
                                    ro16team15.Text = ro16_contestants[7].MatchContestant1.ToString();
                                    ro16score15.Text = ro16_contestants[7].MatchContestant1Score.ToString();
                                    ro16team16.Text = ro16_contestants[7].MatchContestant2.ToString();
                                    ro16score16.Text = ro16_contestants[7].MatchContestant2Score.ToString();
                                }

                                if (ro8_contestants.Any())
                                {
                                    // Round of 8
                                    // Match 1
                                    ro8team1.Text = ro8_contestants[0].MatchContestant1.ToString();
                                    ro8score1.Text = ro8_contestants[0].MatchContestant1Score.ToString();
                                    ro8team2.Text = ro8_contestants[0].MatchContestant2.ToString();
                                    ro8score2.Text = ro8_contestants[0].MatchContestant2Score.ToString();

                                    // Match 2
                                    ro8team3.Text = ro8_contestants[1].MatchContestant1.ToString();
                                    ro8score3.Text = ro8_contestants[1].MatchContestant1Score.ToString();
                                    ro8team4.Text = ro8_contestants[1].MatchContestant2.ToString();
                                    ro8score4.Text = ro8_contestants[1].MatchContestant2Score.ToString();

                                    // Match 3
                                    ro8team5.Text = ro8_contestants[2].MatchContestant1.ToString();
                                    ro8score5.Text = ro8_contestants[2].MatchContestant1Score.ToString();
                                    ro8team6.Text = ro8_contestants[2].MatchContestant2.ToString();
                                    ro8score6.Text = ro8_contestants[2].MatchContestant2Score.ToString();

                                    // Match 4
                                    ro8team7.Text = ro8_contestants[3].MatchContestant1.ToString();
                                    ro8score7.Text = ro8_contestants[3].MatchContestant1Score.ToString();
                                    ro8team8.Text = ro8_contestants[3].MatchContestant2.ToString();
                                    ro8score8.Text = ro8_contestants[3].MatchContestant2Score.ToString();
                                }

                                if (ro4_contestants.Any())
                                {
                                    // Round of 4
                                    // Match 1
                                    ro4team1.Text = ro4_contestants[0].MatchContestant1.ToString();
                                    ro4score1.Text = ro4_contestants[0].MatchContestant1Score.ToString();
                                    ro4team2.Text = ro4_contestants[0].MatchContestant2.ToString();
                                    ro4score2.Text = ro4_contestants[0].MatchContestant2Score.ToString();

                                    // Match 2
                                    ro4team3.Text = ro4_contestants[1].MatchContestant1.ToString();
                                    ro4score3.Text = ro4_contestants[1].MatchContestant1Score.ToString();
                                    ro4team4.Text = ro4_contestants[1].MatchContestant2.ToString();
                                    ro4score4.Text = ro4_contestants[1].MatchContestant2Score.ToString();
                                }

                                if (ro2_contestants.Any())
                                {
                                    // Round of 24
                                    // Match 1
                                    ro2team1.Text = ro2_contestants[0].MatchContestant1.ToString();
                                    ro2score1.Text = ro2_contestants[0].MatchContestant1Score.ToString();
                                    ro2team2.Text = ro2_contestants[0].MatchContestant2.ToString();
                                    ro2score2.Text = ro2_contestants[0].MatchContestant2Score.ToString();
                                }
                        }
                        catch(Exception e) {
                            MessageBox.Show("Something went wrong. You might have an invalid JSON url.\nCheck the Error log for more information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            if (File.Exists(errorFile))
                            {
                                using (StreamWriter sw = File.AppendText(errorFile)) {
                                    sw.WriteLine(DateTime.Now);
                                    sw.WriteLine(e.ToString());
                                    sw.WriteLine(" ");
                                }
                            }
                        }
                    }
                }

            }
        }
        private void showPlayoffs(bool update = false)
        {
            try
            {
                cgData.Clear();

                cgData.SetData("boxPlayoffsTitle", boxPlayoffsTitle.Text);

                cgData.SetData("ro16team1", ro16team1.Text); cgData.SetData("ro16score1", ro16score1.Text);
                cgData.SetData("ro16team2", ro16team2.Text); cgData.SetData("ro16score2", ro16score2.Text);
                cgData.SetData("ro16team3", ro16team3.Text); cgData.SetData("ro16score3", ro16score3.Text);
                cgData.SetData("ro16team4", ro16team4.Text); cgData.SetData("ro16score4", ro16score4.Text);
                cgData.SetData("ro16team5", ro16team5.Text); cgData.SetData("ro16score5", ro16score5.Text);
                cgData.SetData("ro16team6", ro16team6.Text); cgData.SetData("ro16score6", ro16score6.Text);
                cgData.SetData("ro16team7", ro16team7.Text); cgData.SetData("ro16score7", ro16score7.Text);
                cgData.SetData("ro16team8", ro16team8.Text); cgData.SetData("ro16score8", ro16score8.Text);
                cgData.SetData("ro16team9", ro16team9.Text); cgData.SetData("ro16score9", ro16score9.Text);
                cgData.SetData("ro16team10", ro16team10.Text); cgData.SetData("ro16score10", ro16score10.Text);
                cgData.SetData("ro16team11", ro16team11.Text); cgData.SetData("ro16score11", ro16score11.Text);
                cgData.SetData("ro16team12", ro16team12.Text); cgData.SetData("ro16score12", ro16score12.Text);
                cgData.SetData("ro16team13", ro16team13.Text); cgData.SetData("ro16score13", ro16score13.Text);
                cgData.SetData("ro16team14", ro16team14.Text); cgData.SetData("ro16score14", ro16score14.Text);
                cgData.SetData("ro16team15", ro16team15.Text); cgData.SetData("ro16score15", ro16score15.Text);
                cgData.SetData("ro16team16", ro16team16.Text); cgData.SetData("ro16score16", ro16score16.Text);

                cgData.SetData("ro8team1", ro8team1.Text); cgData.SetData("ro8score1", ro8score1.Text);
                cgData.SetData("ro8team2", ro8team2.Text); cgData.SetData("ro8score2", ro8score2.Text);
                cgData.SetData("ro8team3", ro8team3.Text); cgData.SetData("ro8score3", ro8score3.Text);
                cgData.SetData("ro8team4", ro8team4.Text); cgData.SetData("ro8score4", ro8score4.Text);
                cgData.SetData("ro8team5", ro8team5.Text); cgData.SetData("ro8score5", ro8score5.Text);
                cgData.SetData("ro8team6", ro8team6.Text); cgData.SetData("ro8score6", ro8score6.Text);
                cgData.SetData("ro8team7", ro8team7.Text); cgData.SetData("ro8score7", ro8score7.Text);
                cgData.SetData("ro8team8", ro8team8.Text); cgData.SetData("ro8score8", ro8score8.Text);

                cgData.SetData("ro4team1", ro4team1.Text); cgData.SetData("ro4score1", ro4score1.Text);
                cgData.SetData("ro4team2", ro4team2.Text); cgData.SetData("ro4score2", ro4score2.Text);
                cgData.SetData("ro4team3", ro4team3.Text); cgData.SetData("ro4score3", ro4score3.Text);
                cgData.SetData("ro4team4", ro4team4.Text); cgData.SetData("ro4score4", ro4score4.Text);

                cgData.SetData("ro2team1", ro2team1.Text); cgData.SetData("ro2score1", ro2score1.Text);
                cgData.SetData("ro2team2", ro2team2.Text); cgData.SetData("ro2score2", ro2score2.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (CBonlyShowRo8.Checked)
                    {
                        if (update == true)
                        {
                            casparUpdateInvoke(1, 21, "Playoffs_RO8");
                        }
                        else
                        {
                            casparDevice.SendString("LOADBG 1-20 " + templateFolder.ToUpper() + "_BACKGROUND MIX 50 LOOP");
                            casparDevice.SendString("PLAY 1-20");
                            casparAdd(1, 21, "Playoffs_RO8");
                        }
                    }
                    else
                    {
                        if (update == true)
                        {
                            casparUpdateInvoke(1, 21, "Playoffs");
                        }
                        else
                        {
                            casparDevice.SendString("LOADBG 1-20 " + templateFolder.ToUpper() + "_BACKGROUND MIX 50 LOOP");
                            casparDevice.SendString("PLAY 1-20");
                            casparAdd(1, 21, "Playoffs");
                        }
                    }

                    isPlayoffsActive = true;

                    btnShowPlayoffs.Enabled = false;
                    btnUpdatePlayoffs.Enabled = true;
                    btnHidePlayoffs.Enabled = true;

                }
            }
        }

        private void hidePlayoffs()
        {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparDevice.SendString("LOADBG 1-20 EMPTY MIX 50 AUTO");
                    casparDevice.SendString("PLAY 1-20");
                    casparStop(1, 21);                   

                    isPlayoffsActive = false;

                    btnShowPlayoffs.Enabled = true;
                    btnHidePlayoffs.Enabled = false;
                    btnUpdatePlayoffs.Enabled = false;
                }
        }
        #endregion

        #region Layers 21 vsPlayers
        private void showPlayerLeft(bool update = false)
        {
            string heroLeft1 = vsPlayersLeftHeroes01Combobox.Text;
            string heroLeft2 = vsPlayersLeftHeroes02Combobox.Text;
            string heroLeft3 = vsPlayersLeftHeroes03Combobox.Text;
            string heroLeft4 = vsPlayersLeftHeroes04Combobox.Text;

            string heroRight1 = vsPlayersRightHeroes01Combobox.Text;
            string heroRight2 = vsPlayersRightHeroes02Combobox.Text;
            string heroRight3 = vsPlayersRightHeroes03Combobox.Text;
            string heroRight4 = vsPlayersRightHeroes04Combobox.Text;

            string leftHero1Loss = "";
            string leftHero2Loss = "";
            string leftHero3Loss = "";

            string rightHero1Loss = "";
            string rightHero2Loss = "";
            string rightHero3Loss = "";

            try
            {
                cgData.Clear();

                if (heroLeft1 == "") { heroLeft1 = "empty"; }
                if (heroLeft2 == "") { heroLeft2 = "empty"; } 
                if (heroLeft3 == "") { heroLeft3 = "empty"; }
                if (heroLeft4 == "") { heroLeft4 = "empty"; }
                if (heroRight1 == "") { heroRight1 = "empty"; }
                if (heroRight2 == "") { heroRight2 = "empty"; }
                if (heroRight3 == "") { heroRight3 = "empty"; }
                if (heroRight4 == "") { heroRight4 = "empty"; }
                if (vsPlayersLeftHero1LossCheckbox.Checked == true) { leftHero1Loss = "_loss"; }
                if (vsPlayersLeftHero2LossCheckbox.Checked == true) { leftHero2Loss = "_loss"; }
                if (vsPlayersLeftHero3LossCheckbox.Checked == true) { leftHero3Loss = "_loss"; }
                if (vsPlayersRightHero1LossCheckbox.Checked == true) { rightHero1Loss = "_loss"; }
                if (vsPlayersRightHero2LossCheckbox.Checked == true) { rightHero2Loss = "_loss"; }
                if (vsPlayersRightHero3LossCheckbox.Checked == true) { rightHero3Loss = "_loss"; }

                cgData.SetData("tNameTop",  vsPlayersNicknameLeftTextbox.Text);
                cgData.SetData("tNameBottom", vsPlayerNicknameRightTextbox.Text);

                cgData.SetData("playerImageLeft", playerPictureFolder + "/" + templateFolder + "/" + vsPlayersImagePathLeftTextbox.Text);
                cgData.SetData("playerImageRight", playerPictureFolder + "/" + templateFolder + "/" + vsPlayerImagePathRightTextbox.Text);

                cgData.SetData("leftHero1", graphicsFolder + "/hearthstone/" + heroLeft1 + leftHero1Loss + ".png");
                cgData.SetData("leftHero2", graphicsFolder + "/hearthstone/" + heroLeft2 + leftHero2Loss + ".png");
                cgData.SetData("leftHero3", graphicsFolder + "/hearthstone/" + heroLeft3 + leftHero3Loss + ".png");
                cgData.SetData("leftHero4", graphicsFolder + "/hearthstone/" + heroLeft4 + "_ban.png");

                cgData.SetData("rightHero4", graphicsFolder + "/hearthstone/" + heroRight1 + rightHero1Loss + ".png");
                cgData.SetData("rightHero3", graphicsFolder + "/hearthstone/" + heroRight2 + rightHero2Loss + ".png");
                cgData.SetData("rightHero2", graphicsFolder + "/hearthstone/" + heroRight3 + rightHero3Loss + ".png");
                cgData.SetData("rightHero1", graphicsFolder + "/hearthstone/" + heroRight4 + "_ban.png");

                cgData.SetData("leftRace", graphicsFolder + "/starcraft/" + raceLeft + ".png");
                cgData.SetData("rightRace", graphicsFolder + "/starcraft/" + raceRight + ".png");

                cgData.SetData("tNameLeftTeam", vsPlayersTeamLeftTextbox.Text);
                cgData.SetData("tNameRightTeam", vsPlayerTeamRightTextbox.Text);

                cgData.SetData("tNameLeftNationality", vsPlayersNationalityLeftTextbox.Text);
                cgData.SetData("tNameRightNationality", vsPlayerNationalityRightTextbox.Text);

                cgData.SetData("tRoundOf", vsPlayersRoundOf.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 21, "vsPlayers");
                    }
                    else
                    {
                        casparDevice.SendString("LOADBG 1-20 " + templateFolder.ToUpper() + "_BACKGROUND MIX 50 LOOP");
                        casparDevice.SendString("PLAY 1-20");

                        casparAdd(1, 21, "vsPlayers");
                        isPlayersActive = true;

                        vsPlayersShow.Enabled = false;
                        vsPlayersHide.Enabled = true;
                    }
                }
            }
        }

        private void hidePlayers()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isPlayersActive)
                    {
                        casparDevice.SendString("LOADBG 1-20 EMPTY MIX 50 AUTO");
                        casparDevice.SendString("PLAY 1-20");

                        casparStop(1, 21);

                        isPlayersActive = false;

                        vsPlayersShow.Enabled = true;
                        vsPlayersHide.Enabled = false;
                    }
                }
            }
        }
        #endregion

        #region Layers 21 vsTeams
        private void showTeams(bool update = false)
        {
            try
            {
                cgData.Clear();
                
                cgData.SetData("tNameTop", vsTeamNameLeftTextbox.Text);
                cgData.SetData("tNameBottom", vsTeamNameRightTextbox.Text);

                cgData.SetData("teamImageLeft", "teams/" + vsTeamLogoPathLeftTextbox.Text);
                cgData.SetData("teamImageRight", "teams/" + vsTeamLogoPathRightTextbox.Text);

                cgData.SetData("tNameLeftNationality", vsTeamNationalityLeftTextbox.Text);
                cgData.SetData("tNameRightNationality", vsTeamNationalityRightTextbox.Text);

                cgData.SetData("tRoundOf", vsTeamsRoundOfTextbox.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (update == true)
                    {
                        casparUpdateInvoke(1, 21, "vsTeams");
                    }
                    else
                    {
                        casparDevice.SendString("LOADBG 1-20 " + templateFolder.ToUpper() + "_BACKGROUND MIX 50 LOOP");
                        casparDevice.SendString("PLAY 1-20");

                        casparAdd(1, 21, "vsTeams");
                        isTeamsActive = true;

                        vsTeamsShowButton.Enabled = false;
                        vsTeamsHideButton.Enabled = true;
                        vsTeamsUpdateButton.Enabled = true;
                    }
                }
            }
        }

        private void hideTeams()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    if (isTeamsActive)
                    {
                        casparDevice.SendString("LOADBG 1-20 EMPTY MIX 50 AUTO");
                        casparDevice.SendString("PLAY 1-20");
                        casparStop(1, 21);

                        isTeamsActive = false;

                        vsTeamsShowButton.Enabled = true;
                        vsTeamsHideButton.Enabled = false;
                        vsTeamsUpdateButton.Enabled = false;
                    }
                }
            }
        }
        #endregion

        #region Layer 21 EndCredits
        private void showEndCredits()
        {
            try
            {
                cgData.Clear();
                cgData.SetData("endCreditText", rtbCreditsText.Text);
                cgData.SetData("CreditsLength", tbCreditsLength.Text);
                cgData.SetData("CreditsStartPos", tbCreditsStartPos.Text);
            }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparDevice.SendString("LOADBG 1-20 " + templateFolder.ToUpper() + "_BACKGROUND MIX 50 LOOP");
                    casparDevice.SendString("PLAY 1-20");
                    casparAdd(1, 21, "EndCredits");

                    isEndCreditsActive = true;

                    btnShowEndCredits.Enabled = false;
                    btnHideEndCredits.Enabled = true;
                }
            }
        }

        private void hideEndCredits()
        {
            try { }
            catch { }
            finally
            {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                    casparDevice.SendString("LOADBG 1-20 EMPTY MIX 50 AUTO");
                    casparDevice.SendString("PLAY 1-20");
                    casparStop(1, 21);

                    isEndCreditsActive = false;

                    btnShowEndCredits.Enabled = true;
                    btnHideEndCredits.Enabled = false;
                }
            }
        }
        #endregion


        #region Database functions for Players and Teams

        private void insertPlayer(string name, string nickname, string text, string imagepath, string nationality, int team_id)
        {
            string nameNick = "";
            try
            {
                if (name != "")
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    string[] realName = name.Split(' ');
                    if(realName.Length == 2)
                         nameNick = realName[0] + " \"" + nickname + "\" " + realName[1];

                    cmd.CommandText = "INSERT INTO Players (name, nickname, image_path, nationality, text, team_id, namenick) values (@name, @nickname, @imagepath, @nationality, @text, @team_id, @namenick)";

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@nickname", nickname);
                    cmd.Parameters.AddWithValue("@imagepath", imagepath);
                    cmd.Parameters.AddWithValue("@nationality", nationality);
                    cmd.Parameters.AddWithValue("@text", text);
                    cmd.Parameters.AddWithValue("@team_id", team_id);
                    cmd.Parameters.AddWithValue("@namenick", nameNick);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();

                }
                else
                {
                    setWarningColors("You did not write a name");
                }

            }
            catch (SqlCeException e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                setInfoColors("Player " + name + " added");
                conn.Close();
                cmd.Connection.Close();
                loadPlayersToComboboxes();
            }
        }

        private void deletePlayer(ComboBox cbox)
        {
            try
            {
                string id = cbox.SelectedValue.ToString();

                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM Players where id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                setInfoColors("Player removed");
                conn.Close();
                cmd.Connection.Close();
                loadPlayersToComboboxes();
            }
        }

        private void loadDatabaseToCombobox(ComboBox cbox, string databasename)
        {
            string searchstring = "name";

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                if (databasename == "Players")
                    searchstring = "nickname";
                else if (databasename == "Teams")
                    searchstring = "name";

                cmd.CommandText = "SELECT * FROM "+ databasename +" ORDER BY " + searchstring + " ASC";

                dr = cmd.ExecuteReader();

                DataTable dt = new DataTable();

                dt.Columns.Add("id", typeof(int));

                if(databasename == "Teams")
                    dt.Columns.Add("name", typeof(string));
                else if(databasename == "Players")
                    dt.Columns.Add("nickname", typeof(string));
                
                dt.Load(dr);

                cbox.ValueMember = "id";
                if (databasename == "Teams")
                    cbox.DisplayMember = "name";
                else if (databasename == "Players")
                    cbox.DisplayMember = "nickname";
                cbox.DataSource = dt;
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                conn.Close();
                dr.Close();
            }
        }

        private void updatePlayer(int id, string name, string nickname, string nationality, int team_id, string imagepath, string text)
        {
            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                string[] realName = name.Split(' ');
                string nameNick = realName[0] + " \"" + nickname + "\" " + realName[1];

                cmd.Connection = conn;
                cmd.CommandText = "UPDATE Players SET name=@name, nationality=@nationality, nickname=@nickname, text=@text, image_path=@imagepath, namenick=@namenick, team_id=@team_id where id=@id ";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@nickname", nickname);
                cmd.Parameters.AddWithValue("@team_id", team_id);
                cmd.Parameters.AddWithValue("@imagepath", imagepath);
                cmd.Parameters.AddWithValue("@nationality", nationality);
                cmd.Parameters.AddWithValue("@text", text);
                cmd.Parameters.AddWithValue("@namenick", nameNick);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                setInfoColors("Player " + name + " updated");
                conn.Close();
                cmd.Connection.Close();
            }
        }

        private void insertTeam(string name, string nationality, string text, string imagepath)
        {
            try
            {
                if (name != "")
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    cmd.CommandText = "INSERT INTO Teams (name, nationality, image_path, text) values (@name, @nationality, @imagepath, @text)";

                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@nationality", nationality);
                    cmd.Parameters.AddWithValue("@imagepath", imagepath);
                    cmd.Parameters.AddWithValue("@text", text);
                    cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                }
                else
                {
                    setWarningColors("You did not insert a name.");
                }

            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                setInfoColors("Team " + name + " added");
                conn.Close();
                cmd.Connection.Close();
                loadTeamsToComboboxes();
            }
        }

        private void updateTeam(int id, string name, string nationality, string imagepath, string text)
        {
            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandText = "UPDATE Teams SET name=@name, text=@text, image_path=@imagepath, nationality=@nationality where id=@id ";

                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@name", name);
                cmd.Parameters.AddWithValue("@nationality", nationality);
                cmd.Parameters.AddWithValue("@imagepath", imagepath);
                cmd.Parameters.AddWithValue("@text", text);

                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();
            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                setInfoColors("Team " + name + " updated");
                conn.Close();
                cmd.Connection.Close();
            }
        }

        private void deleteTeam(ComboBox cbox, TextBox idbox)
        {
            try
            {
                string id = cbox.SelectedValue.ToString();

                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }

                cmd.Connection = conn;
                cmd.CommandText = "DELETE FROM Teams where id=@id";
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                cmd.Parameters.Clear();

            }
            catch (Exception e)
            {
                MessageBox.Show("An error occurred. Check Error log for more specific information", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                if (File.Exists(errorFile))
                {
                    using (StreamWriter sw = File.AppendText(errorFile))
                    {
                        sw.WriteLine(DateTime.Now);
                        sw.WriteLine(e.ToString());
                        sw.WriteLine(" ");
                    }
                }
            }
            finally
            {
                setInfoColors("Team removed");
                conn.Close();
                cmd.Connection.Close();
                loadTeamsToComboboxes();
            }
        }

        #endregion
        #region Database Players and Teams, button clicks and their executable commands

        private void cbPlayersLeft_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = "0";
            if(vsPlayersLeftCombobox.SelectedValue != null) 
                 ID = vsPlayersLeftCombobox.SelectedValue.ToString();

            int team_id = 0;
            vsPlayersIDLeftTextbox.Text = ID;

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Players WHERE id=@ID ORDER BY nickname ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    vsPlayersRealNameLeftTextbox.Text = dr["name"].ToString();
                    vsPlayersNicknameLeftTextbox.Text = dr["nickname"].ToString();
                    vsPlayersNationalityLeftTextbox.Text = dr["nationality"].ToString();
                    vsPlayersImagePathLeftTextbox.Text = dr["image_path"].ToString();
                    vsPlayerDescriptionLeftTextbox.Text = dr["text"].ToString();
                    team_id = System.Convert.ToInt32(dr["team_id"].ToString());
                }
                else
                { }


                cmd.CommandText = "SELECT name FROM Teams WHERE id=@team_id";
                cmd.Parameters.AddWithValue("@team_id", team_id);

                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    vsPlayersTeamLeftTextbox.Text = dr["name"].ToString();
                }

                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                cmd.Connection.Close();
                dr.Close();
            }
        }

        private void cbPlayersRight_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = "0";
            if(vsPlayersRightCombobox.SelectedValue != null)
                 ID = vsPlayersRightCombobox.SelectedValue.ToString();

            int team_id = 0;
            vsPlayersIDRightTextbox.Text = ID;

                try
                {
                    if (conn != null && conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    cmd.Connection = conn;
                    cmd.CommandText = "SELECT * FROM Players WHERE id=@ID ORDER BY nickname ASC";
                    cmd.Parameters.AddWithValue("@ID", ID);

                    dr = cmd.ExecuteReader();

                    if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                    {
                        vsPlayerRealNameRightTextbox.Text = dr["name"].ToString();
                        vsPlayerNicknameRightTextbox.Text = dr["nickname"].ToString();
                        vsPlayerNationalityRightTextbox.Text = dr["nationality"].ToString();
                        vsPlayerImagePathRightTextbox.Text = dr["image_path"].ToString();
                        vsPlayerDescriptionRightTextbox.Text = dr["text"].ToString();
                        team_id = System.Convert.ToInt32(dr["team_id"].ToString());
                    }
                    else
                    { }

                    cmd.Parameters.Clear();

                    cmd.CommandText = "SELECT name FROM Teams WHERE id=@team_id";
                    cmd.Parameters.AddWithValue("@team_id", team_id);

                    dr = cmd.ExecuteReader();

                    if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                    {
                        vsPlayerTeamRightTextbox.Text = dr["name"].ToString();
                    }

                    cmd.Parameters.Clear();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                }
                finally
                {
                    conn.Close();
                    dr.Close();
                    cmd.Connection.Close();
                }            
        }

        private void playersCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = "0";
            if (playersCombobox.SelectedValue != null)
            {
                ID = playersCombobox.SelectedValue.ToString();
                playerIdTextbox.Text = ID;
            }
            else
            {
                playerIdTextbox.Text = "";
                playerNameTextbox.Text = "";
                playerNicknameTextbox.Text = "";
                playerNationalityTextbox.Text = "";
                playersCombobox.Text = "";
            }

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Players WHERE id=@ID ORDER BY nickname ASC";
                cmd.Parameters.AddWithValue("@ID", ID);
                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    playerNameTextbox.Text = dr["name"].ToString();
                    playerNicknameTextbox.Text = dr["nickname"].ToString();
                    playerNationalityTextbox.Text = dr["nationality"].ToString();
                    playerPicturesCombobox.SelectedIndex = playerPicturesCombobox.FindString(dr["image_path"].ToString().ToLower());
                    //playerPicturesCombobox.Text = dr["image_path"].ToString();
                    playerFreeTextbox.Text = dr["text"].ToString();

                    playerTeamsCombobox.SelectedValue = System.Convert.ToInt16(dr["team_id"].ToString());
                }
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void teamsCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = teamsCombobox.SelectedValue.ToString();
            teamIdTextbox.Text = ID;

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Teams WHERE id=@ID ORDER BY name ASC";
                cmd.Parameters.AddWithValue("@ID", ID);
                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    teamNameTextbox.Text = dr["name"].ToString();
                    teamNationalityTextbox.Text = dr["nationality"].ToString();
                    teamLogoPathCombobox.SelectedIndex = teamLogoPathCombobox.FindStringExact(dr["image_path"].ToString().ToLower());
                    teamFreeTextbox.Text = dr["text"].ToString();
                }
                cmd.Parameters.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void btnRemovePlayerRight_Click(object sender, EventArgs e)
        {
            string id = vsPlayersLeftCombobox.SelectedValue.ToString();
            string name = vsPlayersLeftCombobox.Text;

            string message = "Oletko varma, että haluat poistaa tämän tietueen: [" + id + "] " + name;
            string caption = "Poista tietue";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            DialogResult result;

            result = MessageBox.Show(this, message, caption, buttons, MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                deletePlayer(vsPlayersRightCombobox);
                loadPlayersToComboboxes();
            }
            else if (result == DialogResult.No)
            {

            }
        }

        private void addPlayerButton_Click(object sender, EventArgs e)
        {
            int selectedValue1 = 0;
            int selectedValue2 = 0;

            if(vsPlayersLeftCombobox.SelectedValue != null) 
                selectedValue1 = System.Convert.ToInt32(vsPlayersLeftCombobox.SelectedValue.ToString());
            if (vsPlayersRightCombobox.SelectedValue != null) 
              selectedValue2 = System.Convert.ToInt32(vsPlayersRightCombobox.SelectedValue.ToString());

            insertPlayer(
                playerNameTextbox.Text,
                playerNicknameTextbox.Text,
                playerFreeTextbox.Text,
                playerPicturesCombobox.Text,
                playerNationalityTextbox.Text,
                System.Convert.ToInt32(playerTeamsCombobox.SelectedValue.ToString())
                );

            loadPlayersToComboboxes();

            vsPlayersLeftCombobox.SelectedValue = selectedValue1;
            vsPlayersRightCombobox.SelectedValue = selectedValue2;
        }

        private void updatePlayerButton_Click(object sender, EventArgs e)
        {
           int selectedValue1 = 0;
           int selectedValue2 = 0;

            if(vsPlayersLeftCombobox.SelectedValue != null) 
                selectedValue1 = System.Convert.ToInt32(vsPlayersLeftCombobox.SelectedValue.ToString());
            if (vsPlayersRightCombobox.SelectedValue != null) 
                selectedValue2 = System.Convert.ToInt32(vsPlayersRightCombobox.SelectedValue.ToString());

            int playerComboboxId = 0;
            if (playerTeamsCombobox.SelectedValue != null)
            {
                System.Convert.ToInt32(playerTeamsCombobox.SelectedValue.ToString());

                updatePlayer(
                    System.Convert.ToInt32(playersCombobox.SelectedValue.ToString()),
                    playerNameTextbox.Text,
                    playerNicknameTextbox.Text,
                    playerNationalityTextbox.Text,
                    playerComboboxId,
                    playerPicturesCombobox.Text,
                    playerFreeTextbox.Text
                    );

                loadPlayersToComboboxes();

                vsPlayersLeftCombobox.SelectedValue = selectedValue1;
                vsPlayersRightCombobox.SelectedValue = selectedValue2;
            }
            else
            {
                setWarningColors("An error occurred while attempting to update player data");
            }
        }

        private void deletePlayerButton_Click(object sender, EventArgs e)
        {
            deletePlayer(playersCombobox);
        }

        private void addTeamButton_Click(object sender, EventArgs e)
        {
            insertTeam(teamNameTextbox.Text, teamNationalityTextbox.Text, teamFreeTextbox.Text, teamLogoPathCombobox.Text);
            loadTeamsToComboboxes();
        }

        private void deleteTeamButton_Click(object sender, EventArgs e)
        {
            deleteTeam(teamsCombobox, teamIdTextbox);
        }

        private void updateTeamButton_Click(object sender, EventArgs e)
        {
            updateTeam(
                System.Convert.ToInt32(teamsCombobox.SelectedValue.ToString()), 
                teamNameTextbox.Text, 
                teamNationalityTextbox.Text,
                teamLogoPathCombobox.Text, 
                teamFreeTextbox.Text
            );
            loadTeamsToComboboxes();
        }     

        private void btnRemovePlayerLeft_Click(object sender, EventArgs e)
        {

        }
        #endregion

        #region Hotkeys
        void Form1_KeyDown(object sender, KeyEventArgs e)
        {

         // SCHEDULE
            if (e.Shift && e.KeyCode == toggleSchedule)
            {
                showScheduleLeft(true);
            }
            else if (e.KeyCode == toggleSchedule)
            {
                if (!isScheduleLeftActive)
                    showScheduleLeft();
                else
                    hideSchedule();
            }

         // INFORMATION
            if (e.Shift && e.KeyCode == toggleInformation)
            {
                showInfo(true);
            }
            else if (e.KeyCode == toggleInformation)
            {
                if (!isInfoActive)
                    showInfo();
                else
                    hideInfo();
            }         
            
         // CLOCK
            if (e.KeyCode == toggleClock)
            {
                if (!isClockActive)
                    showClock();
                else
                    hideClock();
            }

         // COUNTDOWN
            if (e.Shift && e.KeyCode == toggleCountdown)
            {
                showCountdown(true);
            }
            else if (e.KeyCode == toggleCountdown)
            {
                if (!isCountdownActive)
                    showCountdown();
                else
                    hideCountdown();
            }

          // CASTERS
            if (e.Shift && e.KeyCode == toggleCasters)
            {
                showCasterLeft(true);
                showCasterRight(true);
            }
            else if (e.KeyCode == toggleCasters)
            {
                if (!isCasterLeftActive || !isCasterRightActive)
                {
                    showCasterLeft();
                    showCasterRight();
                }
                else
                    hideCasters();
            }

          // TWITTER
            if (e.Shift && e.KeyCode == toggleTwitter)
            {
                showTwitter(true);
            }
            else if  (e.KeyCode == toggleTwitter)
            {
                if (!isTwitterActive)
                    showTwitter();
                else
                    hideTwitter();
            }

          // SPONSORS
            if (e.Shift && e.KeyCode == toggleSponsors)
            {
                showSponsors();
            }
            else if  (e.KeyCode == toggleSponsors)
            {
                if (!isSponsorsActive)
                    showSponsors();
                else
                    hideSponsors();
            }

         // INTERVIEW
            if (e.Shift && e.KeyCode == toggleInterview)
            {
                showInterview(true);
            }
            else if (e.KeyCode == toggleInterview)
            {
                if (!isInterviewActive)
                    showInterview();
                else
                    hideInterview();
            }

         // PRESENTATION
            if (e.Shift && e.KeyCode == togglePresentation)
            {
                showPresentation(true);
            }
            else if (e.KeyCode == togglePresentation)
            {
                if (!isPresentationActive)
                    showPresentation();
                else
                    hidePresentation();
            }


          // GROUPS
            if (e.Shift && e.KeyCode == toggleGroups)
            {
                showGroups(true);
            }
            else if  (e.KeyCode == toggleGroups)
            {
                if (!isGroupsActive)
                    showGroups();
                else
                    hideGroups();
            }

          // PLAYOFFS
            if (e.Shift && e.KeyCode == togglePlayoffs)
            {
                showPlayoffs(true);
            }
            else if (e.KeyCode == togglePlayoffs)
            {
                if (!isPlayoffsActive)
                    showPlayoffs();
                else
                    hidePlayoffs();
            }


         // vsPlayers
            if (e.Shift && e.KeyCode == toggleVsPlayers)
            {
                showPlayerLeft(true);
            }
            else if (e.KeyCode == toggleVsPlayers)
            {
                if (!isPlayersActive)
                    showPlayerLeft();
                else
                    hidePlayers();
            }
         // vsTeams
            if (e.Shift && e.KeyCode == toggleVsTeams)
            {
                showTeams(true);
            }
            else if (e.KeyCode == toggleVsTeams)
            {
                if (!isTeamsActive)
                    showTeams();
                else
                    hideTeams();
            }

          // END CREDITS
            if (e.KeyCode == toggleEndCredits)
            {
                if (!isEndCreditsActive)
                    showEndCredits();
                else
                    hideEndCredits();
            }

            // Panic mode
            if (e.KeyCode == hideAll)
            {
                stopGraphics();
            }

         // Panelist
            if (e.Shift && e.KeyCode == panelistHotkey)
            {
                showPanelist(true);
            }
            else if (e.KeyCode == panelistHotkey)
            {
                if (!isPanelistsActive)
                    showPanelist();
                else
                    hidePanelist();
            }

         // Guests
            if (e.Shift && e.KeyCode == guestsHotkey)
            {
                showGuestsLeft(true);
                showGuestsRight(true);
            }
            else if (e.KeyCode == guestsHotkey)
            {
                if (!isGuestsActive)
                {
                    showGuestsLeft();
                    showGuestsRight();
                }
                else
                    hideGuests();
            }
            // HS Ingame
            if (e.Shift && e.KeyCode == toggleHSIngame)
            {
                showScoreboard(true);
            }
            else if (e.KeyCode == toggleHSIngame)
            {
                if (!isScoreboardActive)
                {
                    showScoreboard();
                }
                else
                    hideScoreboard();
            }
            if (e.KeyCode == switchHSIngame)
            {
                if (isScoreboardActive)
                    showScoreboard(true);
            }
        }

        void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
           // e.Handled = true;
        }

        void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if (this.toggleInformation == Keys.None && Control.ModifierKeys == Keys.None)
            {
                return;
            }
        }       
        /// <summary>
        /// Handles some misc keys, such as Ctrl+Delete and Shift+Insert
        /// </summary>
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Delete || keyData == (Keys.Control | Keys.Delete))
            {
                return true;
            }

            if (keyData == (Keys.Shift | Keys.Insert)) // Paste
                return true; // Don't allow

            // Allow the rest
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void hotkeysToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Hotkeys hotkeysForm = new Hotkeys(this);
            hotkeysForm.Show();
        }

        private void textBox_Enter(object sender, EventArgs e)
        {
            KeyPreview = false;
        }

        private void textBox_Leave(object sender, EventArgs e)
        {
             KeyPreview = true;
        }

        #endregion
        private void hideScoreboardButton_Click(object sender, EventArgs e)
        {
            hideScoreboard();
        }

        private void showScoreboardButton_Click(object sender, EventArgs e)
        {
            showScoreboard();
        }

        private void updateScoreboardButton_Click(object sender, EventArgs e)
        {
            showScoreboard(true);
        }

        private void loadPlayersToComboboxes()
        {
            loadDatabaseToCombobox(vsPlayersLeftCombobox, "Players");
            loadDatabaseToCombobox(vsPlayersRightCombobox, "Players");
            loadDatabaseToCombobox(playersCombobox, "Players");
            loadDatabaseToCombobox(interviewCombobox, "Players");
            loadDatabaseToCombobox(guestsLeftCombobox, "Players");
            loadDatabaseToCombobox(guestsRightCombobox, "Players");
        }

        private void loadTeamsToComboboxes()
        {
            loadDatabaseToCombobox(teamsCombobox, "Teams");
            loadDatabaseToCombobox(presentationTeam1Combobox, "Teams");
            loadDatabaseToCombobox(presentationTeam2Combobox, "Teams");
            loadDatabaseToCombobox(playerTeamsCombobox, "Teams");
            loadDatabaseToCombobox(vsTeamsLeftCombobox, "Teams");
            loadDatabaseToCombobox(vsTeamsRightCombobox, "Teams");
        }
      
        private void vsTeamsLeftCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = vsTeamsLeftCombobox.SelectedValue.ToString();
            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Teams WHERE id=@ID ORDER BY name ASC";
                cmd.Parameters.AddWithValue("@ID", ID);
                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    vsTeamNameLeftTextbox.Text = dr["name"].ToString();
                    vsTeamNationalityLeftTextbox.Text = dr["nationality"].ToString();
                    vsTeamLogoPathLeftTextbox.Text = dr["image_path"].ToString();
                    vsTeamDescriptionLeftTextbox.Text = dr["text"].ToString();
                }
                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void vsTeamsRightCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = vsTeamsRightCombobox.SelectedValue.ToString();
            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Teams WHERE id=@ID ORDER BY name ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    vsTeamNameRightTextbox.Text = dr["name"].ToString();
                    vsTeamNationalityRightTextbox.Text = dr["nationality"].ToString();
                    vsTeamLogoPathRightTextbox.Text = dr["image_path"].ToString();
                    vsTeamDescriptionRightTextbox.Text = dr["text"].ToString();
                }
                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void presentationTeam1Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ID = System.Convert.ToInt32(presentationTeam1Combobox.SelectedValue.ToString());
            string logoPath = null;

            List<playerPresentation> players = new List<playerPresentation>();

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT nickname, name, nationality, team_id FROM Players WHERE team_id=@ID ORDER BY name ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    players.Add(new playerPresentation()
                    {
                        nickname = dr["nickname"].ToString(),
                        name = dr["name"].ToString(),
                        nationality = dr["nationality"].ToString()
                    });
                }

                cmd.CommandText = "SELECT image_path FROM Teams WHERE id=@ID ORDER BY name ASC";

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    logoPath = dr["image_path"].ToString();
                }
           
               cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                cmd.Connection.Close();
                dr.Close();
            }

            if (players.ElementAtOrDefault(0) != null)
            {
                presentationPanel01TitleTextbox.Text = players[0].nickname.ToString();
                presentationPanel01Text01Textbox.Text = players[0].name.ToString();
                presentationPanel01Text02Textbox.Text = players[0].nationality.ToString();
                presentationPanel01Text03Textbox.Text = logoPath;
            }
            if (players.ElementAtOrDefault(1) != null)
            {
                presentationPanel02TitleTextbox.Text = players[1].nickname.ToString();
                presentationPanel02Text01Textbox.Text = players[1].name.ToString();
                presentationPanel02Text02Textbox.Text = players[1].nationality.ToString();
                presentationPanel02Text03Textbox.Text = logoPath;
            }
            if (players.ElementAtOrDefault(2) != null)
            {
                presentationPanel03TitleTextbox.Text = players[2].nickname.ToString();
                presentationPanel03Text01Textbox.Text = players[2].name.ToString();
                presentationPanel03Text02Textbox.Text = players[2].nationality.ToString();
                presentationPanel03Text03Textbox.Text = logoPath;
            }
            if (players.ElementAtOrDefault(3) != null)
            {
                presentationPanel04TitleTextbox.Text = players[3].nickname.ToString();
                presentationPanel04Text01Textbox.Text = players[3].name.ToString();
                presentationPanel04Text02Textbox.Text = players[3].nationality.ToString();
                presentationPanel04Text03Textbox.Text = logoPath;
            }
            if (players.ElementAtOrDefault(4) != null)
            {
                presentationPanel05TitleTextbox.Text = players[4].nickname.ToString();
                presentationPanel05Text01Textbox.Text = players[4].name.ToString();
                presentationPanel05Text02Textbox.Text = players[4].nationality.ToString();
                presentationPanel05Text03Textbox.Text = logoPath;
            }
        }

        private void presentationPanel01_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel01);
        }

        private void presentationShowButton_Click(object sender, EventArgs e)
        {
            showPresentation();
        }

        private void presentationHideButton_Click(object sender, EventArgs e)
        {
            hidePresentation();
        }

        private void presentationUpdateButton_Click(object sender, EventArgs e)
        {
            showPresentation(true);
        }

        private void presentationTeam2Combobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int ID = System.Convert.ToInt32(presentationTeam2Combobox.SelectedValue.ToString());
            string logoPath = null;

            List<playerPresentation> playerss = new List<playerPresentation>();

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT nickname, name, nationality, team_id FROM Players WHERE team_id=@ID ORDER BY name ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    playerss.Add(new playerPresentation()
                    {
                        nickname = dr["nickname"].ToString(),
                        name = dr["name"].ToString(),
                        nationality = dr["nationality"].ToString()
                    });
                }

                cmd.CommandText = "SELECT image_path FROM Teams WHERE id=@ID ORDER BY name ASC";

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    logoPath = dr["image_path"].ToString();
                }

                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                cmd.Connection.Close();
                dr.Close();
            }

            if (playerss.ElementAtOrDefault(0) != null)
            {
                presentationPanel06TitleTextbox.Text = playerss[0].nickname.ToString();
                presentationPanel06Text01Textbox.Text = playerss[0].name.ToString();
                presentationPanel06Text02Textbox.Text = playerss[0].nationality.ToString();
                presentationPanel06Text03Textbox.Text = logoPath;
            }
            if (playerss.ElementAtOrDefault(1) != null)
            {
                presentationPanel07TitleTextbox.Text = playerss[1].nickname.ToString();
                presentationPanel07Text01Textbox.Text = playerss[1].name.ToString();
                presentationPanel07Text02Textbox.Text = playerss[1].nationality.ToString();
                presentationPanel07Text03Textbox.Text = logoPath;
            }
            if (playerss.ElementAtOrDefault(2) != null)
            {
                presentationPanel08TitleTextbox.Text = playerss[2].nickname.ToString();
                presentationPanel08Text01Textbox.Text = playerss[2].name.ToString();
                presentationPanel08Text02Textbox.Text = playerss[2].nationality.ToString();
                presentationPanel08Text03Textbox.Text = logoPath;
            }
            if (playerss.ElementAtOrDefault(3) != null)
            {
                presentationPanel09TitleTextbox.Text = playerss[3].nickname.ToString();
                presentationPanel09Text01Textbox.Text = playerss[3].name.ToString();
                presentationPanel09Text02Textbox.Text = playerss[3].nationality.ToString();
                presentationPanel09Text03Textbox.Text = logoPath;
            }
            if (playerss.ElementAtOrDefault(4) != null)
            {
                presentationPanel10TitleTextbox.Text = playerss[4].nickname.ToString();
                presentationPanel10Text01Textbox.Text = playerss[4].name.ToString();
                presentationPanel10Text02Textbox.Text = playerss[4].nationality.ToString();
                presentationPanel10Text03Textbox.Text = logoPath;
            }
        }

        private void panel16_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel02);
        }

        private void presentationPanel03_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel03);
        }

        private void presentationPanel04_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel04);
        }

        private void presentationPanel05_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel05);
        }

        private void presentationPanel06_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel06);
        }

        private void presentationPanel07_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel07);
        }

        private void presentationPanel08_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel08);
        }

        private void presentationPanel09_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel09);
        }

        private void presentationPanel10_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            togglePresentationPanel(presentationPanel10);
        }

        private void interviewCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = interviewCombobox.SelectedValue.ToString();
            int team_id = 0;

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Players WHERE id=@ID ORDER BY nickname ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {
                    interviewText01Textbox.Text = dr["nickname"].ToString();
                    interviewText02Textbox.Text = dr["name"].ToString();
                    team_id = System.Convert.ToInt32(dr["team_id"].ToString());
                }
                cmd.Parameters.Clear();

                cmd.CommandText = "SELECT * FROM Teams WHERE id=@team_id";
                cmd.Parameters.AddWithValue("@team_id", team_id);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    interviewText03Textbox.Text = dr["name"].ToString();
                }

                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void interviewShowButton_Click(object sender, EventArgs e)
        {
            showInterview();
        }

        private void interviewHideButton_Click(object sender, EventArgs e)
        {
            hideInterview();
        }

        private void interviewUpdateButton_Click(object sender, EventArgs e)
        {
            showInterview(true);
        }

        private void vsPlayersUpdate_Click(object sender, EventArgs e)
        {
            showPlayerLeft(true);
        }

        private void showScoreboardButton_Click_1(object sender, EventArgs e)
        {
            showScoreboard();
        }

        private void hideScoreboardButton_Click_1(object sender, EventArgs e)
        {
            hideScoreboard();
        }

        private void updateScoreboardButton_Click_1(object sender, EventArgs e)
        {
            showScoreboard(true);
        }

        private void vsTeamsShowButton_Click(object sender, EventArgs e)
        {
            showTeams();
        }

        private void vsTeamsHideButton_Click(object sender, EventArgs e)
        {
            hideTeams();
        }

        private void vsTeamsUpdateButton_Click(object sender, EventArgs e)
        {
            showTeams(true);
        }

        private void btnShowEndCredits_Click_2(object sender, EventArgs e)
        {
            showEndCredits();
        }

        private void btnHideEndCredits_Click_2(object sender, EventArgs e)
        {
            hideEndCredits();
        }

        private void escapeFocus_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                this.ActiveControl = null;
        }

        private void RaceView(object sender, EventArgs e)
        {
            foreach (var control in vsPlayersRaceLeftPanel.Controls)
            {
                RadioButton radio = control as RadioButton;

                if (radio != null && radio.Checked)
                {
                    raceLeft = radio.Text;
                }
            }
        }

        private void radioPlayerRightEmpty_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var control in vsPlayersRaceRightPanel.Controls)
            {
                RadioButton radio = control as RadioButton;

                if (radio != null && radio.Checked)
                {
                    raceRight = radio.Text;
                }
            }
        }

        private void btnShowSponsors_Click_1(object sender, EventArgs e)
        {
            showSponsors();
        }

        private void btnHideSponsors_Click_1(object sender, EventArgs e)
        {
            hideSponsors();
        }

        private void rbSponsorCamera_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var control in sponsorViewPanel.Controls)
            {
                RadioButton radio = control as RadioButton;

                if (radio != null && radio.Checked)
                {
                    sponsorView = "Sponsors_" + radio.Text;
                }
            }
        }

        private void loadPlayerPictures()
        {
            DirectoryInfo dinfo = new DirectoryInfo(@casparCGTemplatesFolderPath + "\\pictures\\" + templateFolder + "\\");

            if (dinfo.Exists)
            {
                playerPicturesCombobox.Items.Clear();
                FileInfo[] Files = dinfo.GetFiles("*.png");
                playerPicturesCombobox.Items.AddRange(Files);
            }
        }

        private void addPlayerRefreshPicturesComboboxButton_Click(object sender, EventArgs e)
        {
            loadPlayerPictures();
        }

        private void loadTeamLogos()
        {
            DirectoryInfo dinfo = new DirectoryInfo(@casparCGTemplatesFolderPath + "\\logos\\" + templateFolder + "\\");

            if (dinfo.Exists)
            {
                teamLogoPathCombobox.Items.Clear();
                FileInfo[] Files = dinfo.GetFiles("*.png");
                teamLogoPathCombobox.Items.AddRange(Files);
            }
        }

        private void teamRefreshLogosButton_Click(object sender, EventArgs e)
        {
            loadTeamLogos();
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            glowCover = false;
        }

        private void button2_Click_2(object sender, EventArgs e)
        {
            glowCover = true;
        }

        private void guestsRightCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = "0";

            if (guestsRightCombobox.SelectedValue != null)
            {
                ID = guestsRightCombobox.SelectedValue.ToString();
            }
            else
            {
                guestsRight01Textbox.Text = "";
                guestsRight02Textbox.Text = "";
                guestsRight03Textbox.Text = "";
            }

            int team_id = 0;

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Players WHERE id=@ID ORDER BY nickname ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {

                    guestsRight01Textbox.Text = dr["nickname"].ToString();
                    guestsRight02Textbox.Text = dr["name"].ToString();
                    team_id = System.Convert.ToInt32(dr["team_id"].ToString());
                }
                cmd.Parameters.Clear();

                cmd.CommandText = "SELECT * FROM Teams WHERE id=@team_id";
                cmd.Parameters.AddWithValue("@team_id", team_id);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    guestsRight03Textbox.Text = dr["name"].ToString();
                }

                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void guestsLeftCombobox_SelectedIndexChanged(object sender, EventArgs e)
        {
            string ID = "0";

            if(guestsLeftCombobox.SelectedValue != null) {
                ID = guestsLeftCombobox.SelectedValue.ToString();
            }

            int team_id = 0;

            try
            {
                if (conn != null && conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                cmd.Connection = conn;
                cmd.CommandText = "SELECT * FROM Players WHERE id=@ID ORDER BY nickname ASC";
                cmd.Parameters.AddWithValue("@ID", ID);

                dr = cmd.ExecuteReader();

                if (dr.Read() && dr.GetValue(0) != DBNull.Value)
                {

                    guestsLeft01Textbox.Text = dr["nickname"].ToString();
                    guestsLeft02Textbox.Text = dr["name"].ToString();
                    team_id = System.Convert.ToInt32(dr["team_id"].ToString());
                }
                cmd.Parameters.Clear();

                cmd.CommandText = "SELECT * FROM Teams WHERE id=@team_id";
                cmd.Parameters.AddWithValue("@team_id", team_id);

                dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    guestsLeft03Textbox.Text = dr["name"].ToString();
                }

                cmd.Parameters.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
            finally
            {
                conn.Close();
                dr.Close();
                cmd.Connection.Close();
            }
        }

        private void guestsShowButton_Click(object sender, EventArgs e)
        {
            showGuestsLeft();
            showGuestsRight();
        }

        private void guestsHideButton_Click(object sender, EventArgs e)
        {
            hideGuests();
        }

        private void guestsUpdateButton_Click(object sender, EventArgs e)
        {
            showGuestsLeft(true);
            showGuestsRight(true);
        }

        private void panelistShowButton_Click(object sender, EventArgs e)
        {
            showPanelist();
        }

        private void panelistUpdateButton_Click(object sender, EventArgs e)
        {
            showPanelist(true);
        }

        private void panelistHideButton_Click(object sender, EventArgs e)
        {
            hidePanelist();
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
                if (casparDevice.IsConnected && casparDevice.Channels.Count > 0)
                {
                        casparDevice.SendString(casparCommandTextbox.Text);
                }
        }

        private void scoreboardClearButton_Click(object sender, EventArgs e)
        {
            showScoreboard(true, "Clear");
        }
    }

    public class Match
    {
        public string MatchContestant1 { get; set; }
        public string MatchContestant2 { get; set; }
        public string MatchContestant1Score { get; set; }
        public string MatchContestant2Score { get; set; }

    }

    public class Group {
    
        public string Team { get; set; }
        public string TeamScore { get; set; }

        public override string ToString()
        {
            return "Team: " + Team + " Score: " + TeamScore;
        }
    }

    public class comboData
    {
        public string name { get; set; }
        public int id { get; set; }
        public string nickname { get; set; }
        public string nameNick { get { return name + " \"" + nickname + "\""; } }
    }

    public class playerPresentation
    {
        public string nickname { get; set; }
        public string name { get; set; }
        public string nationality { get; set; }
    }

    class ExceptionLogger
    {
        Exception _ex;

        public ExceptionLogger(Exception ex)
        {
            _ex = ex;
        }

        public void DoLog()
        {
            Console.WriteLine(_ex.ToString()); //Will display en-US message
        }
    }
}
