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
using TGPA.Utils;
using TGPA.Game.BadGuys;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Generate parameterized random waves of enemies
    /// </summary>
    public class EnemyAutoGenerationCommand : Command
    {
        /// <summary>
        /// Scroll values between the generation has to proceed
        /// </summary>
        private Vector2 startScrollValue, stopScrollValue;
        /// <summary>
        /// Mark for the required difficulty (from 0 to 100)
        /// </summary>
        private int difficulty;
        /// <summary>
        /// Enemies to use for the generation
        /// </summary>
        private String[] availableEnemies;
        /// <summary>
        /// Result of the generation process
        /// </summary>
        public List<Event> GeneratedEnemies;

        public EnemyAutoGenerationCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid enemy generation command : " + commandLine);
        }

        public override void Run(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Vector2 eventScrollValue)
        {
            //Generate and event and add them to the current game

            this.IsEnded = true;
        }

        /// <summary>
        /// autogen startX startY stopX stopY difficulty type1,type2,...
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            String s = "";

            s += startScrollValue.X + " " + startScrollValue.Y + " " + stopScrollValue.X + " " + stopScrollValue.Y;

            s += " " + difficulty;

            foreach (String t in availableEnemies)
                s += " " + t;

            return s;
        }

        /// <summary>
        /// Return a random position on the screen corner and borner
        /// </summary>
        /// <returns></returns>
        public Vector2 RandomScreenBorderPosition()
        {
            int border = RandomMachine.GetRandomInt(0, 3);

            switch (border)
            {
                //Right
                case 0:
                    return new Vector2(1, RandomMachine.GetRandomFloat(0.2, 0.8));

                //Down
                case 1:
                    return new Vector2(RandomMachine.GetRandomFloat(0.2, 0.8),1);

                //Left
                case 2:
                    return new Vector2(0, RandomMachine.GetRandomFloat(0.2, 0.8));

                //Up
                default:
                case 3:
                    return new Vector2(RandomMachine.GetRandomFloat(0.2, 0.8), 0);

            }
        }

        /// <summary>
        /// Enemy waves generation on a particular scroll interval
        /// </summary>
        private void Generate()
        {
            //Vector2 currentScroll;
            //int difficultyCounter = 0;

            //GeneratedEnemies = new List<Event>();

            ////Each 100 scroll, generate waves of enemy
            //currentScroll = startScrollValue;

            //while ((currentScroll.X < stopScrollValue.X) || (currentScroll.Y < stopScrollValue.Y))
            //{
            //    Event newEvent = new Event(currentScroll, null);

            //    //On each interval, we have n diffculty points to spent.
            //    //Each badguy has a difficulty value, so we just have to pick random ones until difficultyCounter reach its maximum                
            //    while (difficultyCounter < difficulty)
            //    {

            //        int rand = RandomMachine.GetRandomInt(0, availableEnemies.Length);

            //        BadGuy bg = BadGuy.String2BadGuy(availableEnemies[rand]);
            //        AddEnemyCommand addEnemy = new AddEnemyCommand(bg);
            //        newEvent.AddCommand(addEnemy);
            //    }

            //    difficultyCounter = 0;

            //    GeneratedEnemies.Add(newEvent);

            //    currentScroll.X += 100;
            //    currentScroll.Y += 100;
            //}
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length < 5) return false;

            this.startScrollValue = new Vector2(
                Convert.ToInt32(tokens[1]),
                Convert.ToInt32(tokens[2])
                );

            this.stopScrollValue = new Vector2(
                Convert.ToInt32(tokens[1]),
                Convert.ToInt32(tokens[2])
                );

            this.difficulty = Convert.ToInt32(tokens[3]);

            availableEnemies = tokens[4].Split(',');

            return true;

        }
    }
}
