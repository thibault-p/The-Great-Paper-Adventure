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
using System.Diagnostics;
using TGPA.Utils;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Add an enemy to game engine
    /// </summary>
    public class AddEnemyCommand : Command
    {
        /// <summary>
        /// Enemy to add. The object contain all information, and it will be parsed by map loader
        /// </summary>
        public BadGuy Enemy { get; set; }

        public AddEnemyCommand(String commandLine, Vector2 screenResolution)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine, screenResolution))
                throw new Exception("Invalid script Line : " + commandLine);
        }

        public AddEnemyCommand(BadGuy enemy)
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
            return "addenemy " + Enemy.ToString(screenw, screenh);
        }

        public override string ToString()
        {
            return "addenemy " + Enemy.ToString();
        }

        public override bool ParseLine(string commandLine)
        {
            return this.ParseLine(commandLine, new Vector2(TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight));
        }

        /// <param name="commandLine">Format : addenemy type relativex relativey bonus pattern flag1,flag2 flip</param>
        public bool ParseLine(string commandLine, Vector2 screenResolution)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length != 8) return false;

            MovePattern pattern = null;

            //Convert relative position to absolute
            Vector2 location = TGPAConvert.RelativeToAbsoluteLoc(tokens[2], tokens[3], screenResolution);

            //Flip
            SpriteEffects flip = SpriteEffects.None;

            if (tokens[7].Equals(SpriteEffects.FlipHorizontally.ToString()))
            {
                flip = SpriteEffects.FlipHorizontally;
            }
            else if (tokens[7].Equals(SpriteEffects.FlipVertically.ToString()))
            {
                flip = SpriteEffects.FlipVertically;
            }

            //Flags
            //flag1,flag2,flag3
            string[] flags = null;

            if (!tokens[6].Equals(""))
                flags = tokens[6].Split(',');

            //Bonus
            Bonus bonus = null;
            if (!tokens[4].Equals("nobonus"))
            {
                try
                {
                    bonus = new Bonus(tokens[4]);
                }
                catch (Exception e)
                {
                    Logger.Log(LogLevel.Error, "TGPA Exception : " + e.Message);
                }
            }
            //Pattern read
            if (tokens[5].Equals("nopattern"))
                pattern = null;
            else
            {

                //pattern = new MovePattern();

                string[] points = tokens[5].Split('+');

                foreach (String couple in points)
                {
                    //Not a pattern ID
                    if (couple.Contains(",") && couple.Contains("("))
                    {
                        string[] xy = couple.Replace("(", "").Replace(")", "").Split(',');
                        //pattern.AddPoint(new Point(Convert.ToInt32(xy[0]), Convert.ToInt32(xy[1])));
                    }
                    else
                    {
                        MovePattern.DefinedMovePattern patternID = (MovePattern.DefinedMovePattern)Enum.Parse(typeof(MovePattern.DefinedMovePattern), couple, true);
                        pattern = new MovePattern(patternID);
                    }
                }

            }

            //Enemy creation
            this.Enemy = BadGuy.String2BadGuy(tokens[1], location, bonus, pattern, flip, flags);

            if (this.Enemy.Location.X == -1)
            {
                location.X = -this.Enemy.DstRect.Width - 1;
            }

            if (this.Enemy.Location.Y == -1)
            {
                location.Y = -this.Enemy.DstRect.Height - 1;
            }

            this.Enemy.Location = location;

            return true;
        }

    }
}
