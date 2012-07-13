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
    /// Change current value of the scrolling of a background
    /// </summary>
    public class NewScrollingSpeedCommand : Command
    {
        public int BackgroundNumber { get; set; }
        public Vector2 NewScrollingValue { get; set; }

        public NewScrollingSpeedCommand(String commandLine) {

            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine)) throw new Exception("Invalid scroll change command : " + commandLine);
        }

        public NewScrollingSpeedCommand(int bgNumber, Vector2 scrollMod)
        {
            BackgroundNumber = bgNumber;
            NewScrollingValue = scrollMod;
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            switch (BackgroundNumber)
            {
                case 1:
                    TGPAContext.Instance.Map.Background1.Speed = NewScrollingValue;
                    break;

                case 2:
                    TGPAContext.Instance.Map.Background2.Speed = NewScrollingValue;
                    break;

                case 3:
                    TGPAContext.Instance.Map.Background3.Speed = NewScrollingValue;
                    break;
            }

            IsEnded = true;
        }

        public override string ToString()
        {
            return "scrollspeed " + BackgroundNumber + " "+NewScrollingValue.X+" "+NewScrollingValue.Y;
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length != 4) return false;

            BackgroundNumber = Convert.ToInt32(tokens[1]);
            NewScrollingValue = new Vector2(Convert.ToInt32(tokens[2]),Convert.ToInt32(tokens[3]));

            return true;
        }
    }

    /// <summary>
    /// Reset background scrolling speed value to the original one
    /// </summary>
    public class ResetScrollingSpeedCommand : Command
    {
        public int BackgroundNumber { get; set; }
        public Vector2 NewScrollingValue { get; set; }

        public ResetScrollingSpeedCommand(String commandLine)
        {

            if (!this.ParseLine(commandLine)) throw new Exception("Invalid scroll change command : " + commandLine);
        }

        public ResetScrollingSpeedCommand(int bgNumber, Vector2 scrollMod)
        {
            BackgroundNumber = bgNumber;
            NewScrollingValue = scrollMod;
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            switch (BackgroundNumber)
            {
                case 1:
                    TGPAContext.Instance.Map.Background1.ResetSpeed();
                    break;

                case 2:
                    TGPAContext.Instance.Map.Background2.ResetSpeed();
                    break;

                case 3:
                    TGPAContext.Instance.Map.Background3.ResetSpeed();
                    break;
            }

            IsEnded = true;
        }

        public override string ToString()
        {
            return "scrollspeedreset " + BackgroundNumber;
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length != 2) return false;

            BackgroundNumber = Convert.ToInt32(tokens[1]);

            return true;
        }

    }


}
