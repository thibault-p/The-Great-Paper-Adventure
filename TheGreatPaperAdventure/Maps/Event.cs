//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using TGPA.Maps.Commands;
using Microsoft.Xna.Framework;

namespace TGPA.Maps
{
    /// <summary>
    /// Event scrippted in map files  : pop-up enemy, slow down game, etc
    /// </summary>
    public class Event
    {
        /// <summary>
        /// Build a new event with the timer and conditions. Must be filled with actions later
        /// </summary>
        /// <param name="_scrollValue"></param>
        /// <param name="_startC"></param>
        /// <param name="_endC"></param>
        public Event(Vector2 _scrollValue, IfCommand _startC)
        {
            this.Commands = new List<Command>();
            this.ScrollValue = _scrollValue;
            this.StartCondition = _startC;
            this.IsEnded = false;
        }

        public void AddCommand(Command c)
        {
            this.Commands.Add(c);
        }

        public void AddCommands(List<Command> cs)
        {
            this.Commands.AddRange(cs);
        }

        /// <summary>
        /// Play all actions and check if the event can end up or not
        /// </summary>
        public void PlayEvent(GameTime gameTime)
        {
            //Check for start condition
            if (StartCondition != null)
            {
                StartCondition.Run(gameTime, ScrollValue);
                //Condition not satisfied : do not launch event's actions
                if (!StartCondition.IsEnded) return;
            }

            //Launch actions
            List<Command> commandsToDelete = new List<Command>();

            foreach (Command c in Commands)
            {
                c.Run(gameTime, ScrollValue);

                if (c.IsEnded) commandsToDelete.Add(c);
                else break; //Do not execute more commands if you didn't finish this one. This is useful for a if or wait
            }

            foreach (Command cd in commandsToDelete)
                Commands.Remove(cd);

            //No more actions
            if (Commands.Count == 0)
            {
                IsEnded = true;
            }
        }

        /// <summary>
        /// Determine when the event have to be used
        /// </summary>
        public Vector2 ScrollValue { get; set; }
        /// <summary>
        /// List of commands contain in the event
        /// </summary>
        public List<Command> Commands { get; set; }
        /// <summary>
        /// Condition (= flag that must be set) for this event to launch
        /// </summary>
        public IfCommand StartCondition { get; set; }
        /// <summary>
        /// Event is terminated and can be deleted
        /// </summary>
        public bool IsEnded { get; set; }

    }
}
