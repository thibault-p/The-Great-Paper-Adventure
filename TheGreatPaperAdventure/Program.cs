//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;

namespace TGPA
{
    /// <summary>
    /// Shmup in XNA Framework. Shoot them all !
    /// </summary>
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (TheGreatPaperGame game = new TheGreatPaperGame())
            {
                game.Run();
            }
        }
    }
}

