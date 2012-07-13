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
    /// Command to test if a flag is set up
    /// </summary>
    public class IfCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        /// <param name="useLess">Just here to differ from the other constructor</param>
        public IfCommand(String commandLine,bool useLess)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid if command : " + commandLine);
        }

        public IfCommand(String flag)
        {
            Flag = flag;
        }

        public String Flag { get; set; }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            if(TGPAContext.Instance.Map.Flags.GetFlag(Flag))
                IsEnded = true;
        }

        public override string ToString()
        {
            return "if " + Flag;
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
