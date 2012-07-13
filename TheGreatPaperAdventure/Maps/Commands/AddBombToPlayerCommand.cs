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
    /// Grant the player a new megabomb
    /// </summary>
    public class AddBombToPlayerCommand : Command
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandLine"></param>
        public AddBombToPlayerCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid set command : " + commandLine);
        }

        public AddBombToPlayerCommand()
        {
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            TGPAContext.Instance.Player1.Bomb.Ammo++;

            if (TGPAContext.Instance.Player2 != null)
            {
                TGPAContext.Instance.Player2.Bomb.Ammo++;
            }

            IsEnded = true;
        }

        public override string ToString()
        {
            return "addbomb";
        }

        public override bool ParseLine(string commandLine)
        {
            return true;
        }
    }
}
