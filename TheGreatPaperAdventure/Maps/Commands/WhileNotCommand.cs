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
    /// While(Condition) Do Commands Every x Seconds
    /// </summary>
    public class WhileNotCommand : Command
    {
        /// <summary>
        /// Actions that will be repeated until the condition is satisfied
        /// </summary>
        public List<String> CommandsLineToRepeat { get; set; }
        /// <summary>
        /// Condition to satisfy
        /// </summary>
        public IfCommand Condition { get; set; }

        /// <summary>
        /// While speed can be regulated by a wait command
        /// </summary>
        public WaitCommand Wait { get; set; }

        public WhileNotCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
            {
                throw new Exception("Invalid whilenot command : " + commandLine);
            }
        }

        public WhileNotCommand(IfCommand _condition, WaitCommand _wait)
        {
            CommandsLineToRepeat = new List<String>();
            Condition = _condition;
            Wait = _wait;
        }

        public override void Run(GameTime gameTime, Vector2 eventScrollValue)
        {
            if (!Condition.IsEnded)
            {
                //Check condition
                Condition.Run(gameTime, eventScrollValue);

                //Check Wait
                Wait.Run(gameTime, eventScrollValue);

                if (Wait.IsEnded)
                {
                    //Do Actions
                    foreach (String line in CommandsLineToRepeat)
                    {
                        Command c = null;

                        //Interprete Command
                        switch (line.Split(' ')[0])
                        {
                            case "addenemy":
                                c = new AddEnemyCommand(line, new Vector2(TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight));
                                break;

                            case "if":
                                c = new IfCommand(line);
                                break;

                            case "wait":
                                c = new WaitCommand(line);
                                break;

                            case "scrollspeedreset":
                                c = new ResetScrollingSpeedCommand(line);
                                break;

                            case "scrollspeed":
                                c = new NewScrollingSpeedCommand(line);
                                break;

                            case "addrandombonus":
                                c = new AddRandomBonusCommand(line);
                                break;

                            default:
                                throw new Exception("Invalid while-available TGPA script command " + line);
                        }

                        c.Run(gameTime, eventScrollValue);
                    }

                    Wait.Reset();
                }
            }

            IsEnded = Condition.IsEnded;
        }

        /// <summary>
        /// Add collection of script line
        /// </summary>
        /// <param name="cs"></param>
        public void AddCommands(List<String> cs)
        {
            foreach (String s in cs)
                CommandsLineToRepeat.Add(s);
        }

        public override string ToString()
        {
            String s = "whilenot " + Condition.ToString() + " " + Wait.TimeToWait + "\n ";

            foreach (String c in CommandsLineToRepeat)
                s += c + " \n ";

            s += "done";

            return s;
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length != 3) return false;

            CommandsLineToRepeat = new List<String>();
            IsEnded = false;
            Condition = new IfCommand(tokens[1]);
            Wait = new WaitCommand(Convert.ToInt32(tokens[2]));

            return true;
        }
    }
}
