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
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Weapons;
using TGPA.Game.BadGuys;
using TGPA.Game.Entities;
using TGPA.Utils;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Add an enemy background element to game engine
    /// </summary>
    public class AddBackgroundElementCommand : Command
    {
        /// <summary>
        /// Enemy to add. The object contain all information, and it will be parsed by map loader
        /// </summary>
        public BadGuy Enemy { get; set; }

        public AddBackgroundElementCommand(String commandLine, Vector2 screenResolution)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine, screenResolution))
                throw new Exception("Invalid script Line : " + commandLine);
        }

        public AddBackgroundElementCommand(BadGuy enemy)
        {
            this.Enemy = enemy;
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            Enemy.ScrollValue = eventScrollValue;

            //Add enemy to game engine
            TGPAContext.Instance.Enemies.Add(Enemy);
            IsEnded = true;
        }

        public string ToString(float screenw, float screenh)
        {
            return "addbge " + Enemy.ToString(screenw, screenh).Split(' ')[0] + " " + Enemy.ScrollValue.X + "," + Enemy.ScrollValue.Y;
        }

        public override string ToString()
        {
            return "addbge " + Enemy.ToString().Split(' ')[0] + " " + Enemy.ScrollValue.X + "," + Enemy.ScrollValue.Y;
        }

        public override bool ParseLine(string commandLine)
        {
            return this.ParseLine(commandLine, new Vector2(TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight));
        }

        /// <param name="commandLine">Format : addenemy type relativex relativey bonus pattern flag1,flag2 flip</param>
        public bool ParseLine(string commandLine, Vector2 screenResolution)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length < 4) return false;

            //Convert relative position to absolute
            Vector2 location = TGPAConvert.RelativeToAbsoluteLoc(tokens[2], tokens[3], screenResolution);

            //Enemy creation
            String optionalParameter = "";

            if (tokens.Length == 5)
            {
                optionalParameter = tokens[4];
            }

            this.Enemy = BackgroundActiveElement.String2BackgroundActiveElement(tokens[1], location, optionalParameter);
            
            return true;
        }

    }
}
