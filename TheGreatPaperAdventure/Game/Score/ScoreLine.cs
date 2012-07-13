//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using TGPA.Localization;

namespace TGPA.Game.Save
{
    /// <summary>
    /// Define if score if for coop or single game
    /// </summary>
    public enum ScoreType
    {
        Single,
        Coop
    }

    /// <summary>
    /// One line of the score board
    /// </summary>
    public class ScoreLine
    {
        public int Rank { get; set; }
        public DateTime Date { get; set; }
        public string Player { get; set; }
        public Int32 Score { get; set; }
        public Difficulty Difficulty { get; set; }

        /// <summary>
        /// Create an empty validated score line
        /// </summary>
        /// <param name="rank">Should be at least 1</param>
        /// <returns></returns>
        public static ScoreLine GetDefaultScoreLine(int rank)
        {
            rank = rank < 1 ? 1 : rank;

            return new ScoreLine(rank, DateTime.Now, "n00b", 0, Difficulty.Normal);
        }

        public ScoreLine()
        {
            Rank = -1;
            Date = DateTime.Now;
            Player = "PlayerName";
            Score = 0;
        }

        /// <summary>
        /// Fill information
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="dateTime"></param>
        /// <param name="Player"></param>
        /// <param name="score"></param>
        public ScoreLine(int rank, DateTime dateTime, string Player, Int32 score, Difficulty difficulty)
        {
            this.Rank = rank;
            this.Date = dateTime;
            this.Player = Player;
            this.Score = score;
            this.Difficulty = difficulty;
        }

        /// <summary>
        /// Compatibility with old savegames
        /// </summary>
        /// <param name="rank"></param>
        /// <param name="dateTime"></param>
        /// <param name="Player"></param>
        /// <param name="score"></param>
        public ScoreLine(int rank, DateTime dateTime, string Player, Int32 score)
            : this(rank, dateTime, Player, score, Difficulty.Normal)
        { }

        public override string ToString()
        {
            return Score + " pts - " + Player;
        }

        public string GetDifficultyString()
        {
            return " (" + LocalizedStrings.GetString(this.Difficulty.ToString()) + ")";
        }
    }
}
