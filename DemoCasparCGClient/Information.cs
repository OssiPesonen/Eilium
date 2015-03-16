using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace DemoCasparCGClient
{
    public class Information
    {

        #region config
        public string TemplateFolder { get; set; }
        public string ServerAddress { get; set; }
        public string AMCPPort { get; set; }
        public string OSCPort { get; set; }
        #endregion  

        #region Casters
        public string CasterNicknameLeft { get; set; }
        public string CasterNameLeft { get; set; }
        public string CasterTwitterLeft { get; set; }

        public string CasterNicknameRight { get; set; }
        public string CasterNameRight { get; set; }
        public string CasterTwitterRight { get; set; }
        #endregion  

        #region Twitter
        public string Twitter1 { get; set; }
        public string Twitter2 { get; set; }
        #endregion

        #region Schedule
        public string BoxLeftTitle { get; set; }
        public string BoxLeftC1 { get; set; }
        public string BoxLeftC2 { get; set; }
        public string BoxLeftC3 { get; set; }
        public string BoxLeftC4 { get; set; }
        public string BoxLeftC5 { get; set; }
        public string BoxLeftC6 { get; set; }
        public string BoxLeftC7 { get; set; }
        public string BoxLeftC8 { get; set; }
        public string BoxLeftT1 { get; set; }
        public string BoxLeftT2 { get; set; }
        public string BoxLeftT3 { get; set; }
        public string BoxLeftT4 { get; set; }
        public string BoxLeftT5 { get; set; }
        public string BoxLeftT6 { get; set; }
        public string BoxLeftT7 { get; set; }
        public string BoxLeftT8 { get; set; }

        public string BoxRightTitle { get; set; }
        public string BoxRightC1 { get; set; }
        public string BoxRightC2 { get; set; }
        public string BoxRightC3 { get; set; }
        public string BoxRightC4 { get; set; }
        public string BoxRightC5 { get; set; }
        public string BoxRightC6 { get; set; }
        public string BoxRightC7 { get; set; }
        public string BoxRightC8 { get; set; }
        public string BoxRightT1 { get; set; }
        public string BoxRightT2 { get; set; }
        public string BoxRightT3 { get; set; }
        public string BoxRightT4 { get; set; }
        public string BoxRightT5 { get; set; }
        public string BoxRightT6 { get; set; }
        public string BoxRightT7 { get; set; }
        public string BoxRightT8 { get; set; }
        #endregion  

        #region Groups
        public string GroupJSONUrl { get; set; }
        public string GroupTitle { get; set; }

        public string Group1Title { get; set; }
        public string Group2Title { get; set; }
        public string Group3Title { get; set; }
        public string Group4Title { get; set; }

        // Group 1 
        public string Group1Name1 { get; set; }
        public string Group1Name2 { get; set; }
        public string Group1Name3 { get; set; }
        public string Group1Name4 { get; set; }

        public string Group1Wins1 { get; set; }
        public string Group1Wins2 { get; set; }
        public string Group1Wins3 { get; set; }
        public string Group1Wins4 { get; set; }

        public string Group1Losses1 { get; set; }
        public string Group1Losses2 { get; set; }
        public string Group1Losses3 { get; set; }
        public string Group1Losses4 { get; set; }

        public string Group1Scorename1 { get; set; }
        public string Group1Scorename2 { get; set; }
        public string Group1Scorename3 { get; set; }
        public string Group1Scorename4 { get; set; }
        public string Group1Scorename5 { get; set; }
        public string Group1Scorename6 { get; set; }
        public string Group1Scorename7 { get; set; }
        public string Group1Scorename8 { get; set; }
        public string Group1Scorename9 { get; set; }
        public string Group1Scorename10 { get; set; }

        public string Group1Namescore1 { get; set; }
        public string Group1Namescore2 { get; set; }
        public string Group1Namescore3 { get; set; }
        public string Group1Namescore4 { get; set; }
        public string Group1Namescore5 { get; set; }

        // Group 2 
        public string Group2Name1 { get; set; }
        public string Group2Name2 { get; set; }
        public string Group2Name3 { get; set; }
        public string Group2Name4 { get; set; }

        public string Group2Wins1 { get; set; }
        public string Group2Wins2 { get; set; }
        public string Group2Wins3 { get; set; }
        public string Group2Wins4 { get; set; }

        public string Group2Losses1 { get; set; }
        public string Group2Losses2 { get; set; }
        public string Group2Losses3 { get; set; }
        public string Group2Losses4 { get; set; }

        public string Group2Scorename1 { get; set; }
        public string Group2Scorename2 { get; set; }
        public string Group2Scorename3 { get; set; }
        public string Group2Scorename4 { get; set; }
        public string Group2Scorename5 { get; set; }
        public string Group2Scorename6 { get; set; }
        public string Group2Scorename7 { get; set; }
        public string Group2Scorename8 { get; set; }
        public string Group2Scorename9 { get; set; }
        public string Group2Scorename10 { get; set; }

        public string Group2Namescore1 { get; set; }
        public string Group2Namescore2 { get; set; }
        public string Group2Namescore3 { get; set; }
        public string Group2Namescore4 { get; set; }
        public string Group2Namescore5 { get; set; }

        // Group 3
        public string Group3Name1 { get; set; }
        public string Group3Name2 { get; set; }
        public string Group3Name3 { get; set; }
        public string Group3Name4 { get; set; }

        public string Group3Wins1 { get; set; }
        public string Group3Wins2 { get; set; }
        public string Group3Wins3 { get; set; }
        public string Group3Wins4 { get; set; }

        public string Group3Losses1 { get; set; }
        public string Group3Losses2 { get; set; }
        public string Group3Losses3 { get; set; }
        public string Group3Losses4 { get; set; }

        public string Group3Scorename1 { get; set; }
        public string Group3Scorename2 { get; set; }
        public string Group3Scorename3 { get; set; }
        public string Group3Scorename4 { get; set; }
        public string Group3Scorename5 { get; set; }
        public string Group3Scorename6 { get; set; }
        public string Group3Scorename7 { get; set; }
        public string Group3Scorename8 { get; set; }
        public string Group3Scorename9 { get; set; }
        public string Group3Scorename10 { get; set; }

        public string Group3Namescore1 { get; set; }
        public string Group3Namescore2 { get; set; }
        public string Group3Namescore3 { get; set; }
        public string Group3Namescore4 { get; set; }
        public string Group3Namescore5 { get; set; }

        // Group 4
        public string Group4Name1 { get; set; }
        public string Group4Name2 { get; set; }
        public string Group4Name3 { get; set; }
        public string Group4Name4 { get; set; }

        public string Group4Wins1 { get; set; }
        public string Group4Wins2 { get; set; }
        public string Group4Wins3 { get; set; }
        public string Group4Wins4 { get; set; }

        public string Group4Losses1 { get; set; }
        public string Group4Losses2 { get; set; }
        public string Group4Losses3 { get; set; }
        public string Group4Losses4 { get; set; }

        public string Group4Scorename1 { get; set; }
        public string Group4Scorename2 { get; set; }
        public string Group4Scorename3 { get; set; }
        public string Group4Scorename4 { get; set; }
        public string Group4Scorename5 { get; set; }
        public string Group4Scorename6 { get; set; }
        public string Group4Scorename7 { get; set; }
        public string Group4Scorename8 { get; set; }
        public string Group4Scorename9 { get; set; }
        public string Group4Scorename10 { get; set; }

        public string Group4Namescore1 { get; set; }
        public string Group4Namescore2 { get; set; }
        public string Group4Namescore3 { get; set; }
        public string Group4Namescore4 { get; set; }
        public string Group4Namescore5 { get; set; }
        #endregion 

        #region Playoffs
        public string PlayoffsTitle { get; set; }
        public string PlayoffsJSONUrl { get; set; }

        // RO 16
        public string Ro16Team1 { get; set; }
        public string Ro16Team2 { get; set; }
        public string Ro16Team3 { get; set; }
        public string Ro16Team4 { get; set; }
        public string Ro16Team5 { get; set; }
        public string Ro16Team6 { get; set; }
        public string Ro16Team7 { get; set; }
        public string Ro16Team8 { get; set; }
        public string Ro16Team9 { get; set; }
        public string Ro16Team10 { get; set; }
        public string Ro16Team11 { get; set; }
        public string Ro16Team12 { get; set; }
        public string Ro16Team13 { get; set; }
        public string Ro16Team14 { get; set; }
        public string Ro16Team15 { get; set; }
        public string Ro16Team16 { get; set; }
        public string Ro16Score1 { get; set; }
        public string Ro16Score2 { get; set; }
        public string Ro16Score3 { get; set; }
        public string Ro16Score4 { get; set; }
        public string Ro16Score5 { get; set; }
        public string Ro16Score6 { get; set; }
        public string Ro16Score7 { get; set; }
        public string Ro16Score8 { get; set; }
        public string Ro16Score9 { get; set; }
        public string Ro16Score10 { get; set; }
        public string Ro16Score11 { get; set; }
        public string Ro16Score12 { get; set; }
        public string Ro16Score13 { get; set; }
        public string Ro16Score14 { get; set; }
        public string Ro16Score15 { get; set; }
        public string Ro16Score16 { get; set; }

        // RO 8
        public string Ro8Team1 { get; set; }
        public string Ro8Team2 { get; set; }
        public string Ro8Team3 { get; set; }
        public string Ro8Team4 { get; set; }
        public string Ro8Team5 { get; set; }
        public string Ro8Team6 { get; set; }
        public string Ro8Team7 { get; set; }
        public string Ro8Team8 { get; set; }
        public string Ro8Score1 { get; set; }
        public string Ro8Score2 { get; set; }
        public string Ro8Score3 { get; set; }
        public string Ro8Score4 { get; set; }
        public string Ro8Score5 { get; set; }
        public string Ro8Score6 { get; set; }
        public string Ro8Score7 { get; set; }
        public string Ro8Score8 { get; set; }

        // RO 4
        public string Ro4Team1 { get; set; }
        public string Ro4Team2 { get; set; }
        public string Ro4Team3 { get; set; }
        public string Ro4Team4 { get; set; }
        public string Ro4Score1 { get; set; }
        public string Ro4Score2 { get; set; }
        public string Ro4Score3 { get; set; }
        public string Ro4Score4 { get; set; }

        // RO 2
        public string Ro2Team1 { get; set; }
        public string Ro2Team2 { get; set; }
        public string Ro2Score1 { get; set; }
        public string Ro2Score2 { get; set; }

        public bool Ro8Check { get; set; }
        #endregion

        #region End Credits
        public string EndCredits { get; set; }
        public string EndCreditsStartPos { get; set; }
        public string EndCreditsLength { get; set; }
        #endregion

        #region Information
        public string infoPanel1Title { get; set; }
        public string infoPanel1Row1 { get; set; }
        public string infoPanel1Row2 { get; set; }
        public string infoPanel1Row3 { get; set; }

        public string infoPanel2Title { get; set; }
        public string infoPanel2Row1 { get; set; }
        public string infoPanel2Row2 { get; set; }
        public string infoPanel2Row3 { get; set; }

        public string infoPanel3Title { get; set; }
        public string infoPanel3Row1 { get; set; }
        public string infoPanel3Row2 { get; set; }
        public string infoPanel3Row3 { get; set; }

        public string infoPanel4Title { get; set; }
        public string infoPanel4Row1 { get; set; }
        public string infoPanel4Row2 { get; set; }
        public string infoPanel4Row3 { get; set; }

        public string infoPanel5Title { get; set; }
        public string infoPanel5Row1 { get; set; }
        public string infoPanel5Row2 { get; set; }
        public string infoPanel5Row3 { get; set; }

        public string infoPanel6Title { get; set; }
        public string infoPanel6Row1 { get; set; }
        public string infoPanel6Row2 { get; set; }
        public string infoPanel6Row3 { get; set; }

        public string infoPanel7Title { get; set; }
        public string infoPanel7Row1 { get; set; }
        public string infoPanel7Row2 { get; set; }
        public string infoPanel7Row3 { get; set; }

        public string infoPanel8Title { get; set; }
        public string infoPanel8Row1 { get; set; }
        public string infoPanel8Row2 { get; set; }
        public string infoPanel8Row3 { get; set; }

        public string infoPanel9Title { get; set; }
        public string infoPanel9Row1 { get; set; }
        public string infoPanel9Row2 { get; set; }
        public string infoPanel9Row3 { get; set; }
        #endregion

        #region Scoreboard
        public string scoreBoardName1 { get; set; }
        public string scoreBoardName2 { get; set; }
        public string scoreBoardScore1 { get; set; }
        public string scoreBoardScore2 { get; set; }
        #endregion

        #region Clock & countdown
        public string clockTitle { get; set; }
        public string countdownTitle { get; set; }
        public string vsPlayersRoundof { get; set; }
        public string vsTeamsRoundOf { get; set; }
        #endregion
    }
}
