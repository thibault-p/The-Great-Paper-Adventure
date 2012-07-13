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
    /// Play/Pause the current music
    /// </summary>
    public class ChangeMusicStateCommand : Command
    {
        /// <summary>
        /// "play" or "pause"
        /// </summary>
        private string command;

        public ChangeMusicStateCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid changemusicstate command : " + commandLine);

        }

        public ChangeMusicStateCommand()
        {
            this.IsEnded = false;
        }

        public override void Run(GameTime gameTime, Microsoft.Xna.Framework.Vector2 eventScrollValue)
        {
            if (command.Equals("play"))
            {
                MusicEngine.Instance.Resume();
            }
            else if (command.Equals("pause"))
            {
                MusicEngine.Instance.Pause();
            }
            this.IsEnded = true;
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens2 = commandLine.Split(' ');

            if (tokens2.Length < 2) return false;

            IsEnded = false;

            command = tokens2[1];

            if (!command.Equals("play") && !command.Equals("pause"))
                return false;

            return true;
        }
    }
}
