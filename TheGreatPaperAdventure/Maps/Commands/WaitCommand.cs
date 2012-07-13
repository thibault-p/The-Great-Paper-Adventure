using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGPA.Maps.Commands
{
    /// <summary>
    /// Wait x seconds
    /// </summary>
    public class WaitCommand : Command
    {
        /// <summary>
        /// Number of seconds to wait
        /// </summary>
        public int TimeToWait { get; set; }

        private double currentTime = -1;
        private String optionnalFlag;

        public WaitCommand(String commandLine)
        {
            OriginalScriptLine = commandLine;

            if (!this.ParseLine(commandLine))
                throw new Exception("Invalid wait command : " + commandLine);

        }

        public WaitCommand(int timeToWait) : this(timeToWait,null)
        {
            
        }

        public WaitCommand(int timeToWait, String optionnalFlag)
        {
            this.TimeToWait = timeToWait;
            this.IsEnded = false;
            this.optionnalFlag = optionnalFlag;
        }

        public override void Run(Microsoft.Xna.Framework.GameTime gameTime, Microsoft.Xna.Framework.Vector2 eventScrollValue)
        {
            if (currentTime == -1)
            {
                currentTime = gameTime.TotalGameTime.TotalSeconds;
            }
            else
            {
                if (gameTime.TotalGameTime.TotalSeconds - currentTime >= TimeToWait)
                {
                    if (optionnalFlag != null)
                    {
                        TGPAContext.Instance.Map.Flags.SetFlag(optionnalFlag);
                    }
                    this.IsEnded = true;
                }
            }
        }

        /// <summary>
        /// Reset command wait, so it can be reused
        /// </summary>
        public void Reset()
        {
            currentTime = -1;
            IsEnded = false;
        }

        public override string ToString()
        {
            return "wait " + TimeToWait;
        }

        public override bool ParseLine(string commandLine)
        {
            string[] tokens = commandLine.Split(' ');

            if (tokens.Length < 2) return false;

            this.TimeToWait = Convert.ToInt32(tokens[1]);
            this.IsEnded = false;

            this.optionnalFlag = "";
            if (tokens.Length == 3)
            {
                this.optionnalFlag = tokens[2];
            }

            return true;
        }
    }
}
