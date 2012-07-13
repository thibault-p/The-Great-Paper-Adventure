//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using TGPA.Game.Sound;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Change currently played song
    /// </summary>
    public class ChangeMusicCommand : Command
    {
        private MySong song;

        public ChangeMusicCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid changemusic command : " + commandLine);

        }

        public ChangeMusicCommand(MySong s)
        {
            this.song = s;
            this.IsEnded = false;
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            MusicEngine.Instance.ChangeMusic(song);
            IsEnded = true;
        }

        public override string ToString()
        {
            return "changemusic " + song.ToString();
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens2 = commandLine.Split('\"');

            if (tokens2.Length < 3) return false;

            song = new MySong(tokens2[0].Split(' ')[1], tokens2[1], tokens2[3]);
            IsEnded = false;

            return true;
        }

        public MySong Song
        {
            get { return song; }
            set { song = value; }
        }
    }
}
