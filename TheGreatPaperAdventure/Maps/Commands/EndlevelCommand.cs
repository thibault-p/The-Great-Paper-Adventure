//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGPA.Game.Sound;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Win or lose current level
    /// </summary>
    public class EndLevelCommand : Command
    {

        public IfCommand WinCond {get;set;}
        public IfCommand LoseCond { get; set; }

        public EndLevelCommand(String winflag, String loseflag)
        {
            WinCond = new IfCommand(winflag);
            LoseCond = new IfCommand(loseflag);
        }


        public override void Run(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Vector2 eventScrollValue)
        {
            //Check if the flag for win is set
            WinCond.Run(gameTime, eventScrollValue);

            if (WinCond.IsEnded)
            {
                TGPAContext.Instance.EndLevel(true);
                this.IsEnded = true;
            }
            else
            {
                //Check for defeat
                LoseCond.Run(gameTime, eventScrollValue);

                if (LoseCond.IsEnded)
                {
                    TGPAContext.Instance.EndLevel(false);
                    this.IsEnded = true;
                }
            }

            //P1 (and P2) are dead
            bool lose = false;
            if (TGPAContext.Instance.Map.Flags.GetFlag("player1die"))
            {
                if (TGPAContext.Instance.Player2 != null)
                {
                    if (TGPAContext.Instance.Map.Flags.GetFlag("player2die"))
                    {
                        lose = true;
                    }
                }
                else
                {
                    lose = true;
                }
            }

            if (lose)
            {
                TGPAContext.Instance.Map.Flags.SetFlag("playersdie");
                MusicEngine.Instance.StopMusic();
            }

        }

        public override string ToString()
        {
            return "winflag " + WinCond.Flag + " \nloseflag " + LoseCond.Flag;
        }

        public override bool ParseLine(string commandLine)
        {
            throw new NotImplementedException();
        }
    }
}
