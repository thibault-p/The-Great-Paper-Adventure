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
    /// Something happening in the game : new enemy, music change, boss, etc
    /// </summary>
    public abstract class Command
    {
        public String OriginalScriptLine { get; set; }

        /// <summary>
        /// Do what you want (called in Update() Game method)
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Run(GameTime gameTime, Vector2 eventScrollValue);

        /// <summary>
        /// Command has finished
        /// </summary>
        public bool IsEnded { get; set; }

        /// <summary>
        /// Convert into a comprehensionnal String for reading/writing scripts
        /// </summary>
        /// <returns></returns>
        public abstract override String ToString();

        /// <summary>
        /// Convert a script line 
        /// (i.e addenemy Machin 5 1 nobonus nopattern lol -> new AddEnemyCommand(new Machin) object)
        /// </summary>
        /// <param name="commandLine"></param>
        public abstract bool ParseLine(String commandLine);
    }
}
