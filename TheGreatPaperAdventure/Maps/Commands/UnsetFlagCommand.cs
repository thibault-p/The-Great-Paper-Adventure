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
    /// Unset a flag
    /// </summary>
    public class UnsetFlagCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="useLess">Just here to differ from the other constructor</param>
        public UnsetFlagCommand(String commandLine, bool useLess)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid unset command : " + commandLine);
        }

        public UnsetFlagCommand(String flag)
        {
            Flag = flag;
        }

        public String Flag { get; set; }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            TGPAContext.Instance.Map.Flags.UnsetFlag(Flag);
            IsEnded = true;
        }

        public override string ToString()
        {
            return "unset " + Flag;
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
