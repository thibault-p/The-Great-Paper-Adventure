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

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Set a flag
    /// </summary>
    public class SetFlagCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="useLess">Just here to differ from the other constructor</param>
        public SetFlagCommand(String commandLine, bool useLess)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid set command : " + commandLine);
        }

        public SetFlagCommand(String flag)
        {
            Flag = flag;
        }

        public String Flag { get; set; }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            TGPAContext.Instance.Map.Flags.SetFlag(Flag);
            IsEnded = true;
        }

        public override string ToString()
        {
            return "set " + Flag;
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length != 2) return false;

            this.Flag = tokens[1];

            return true;


        }
    }
}
