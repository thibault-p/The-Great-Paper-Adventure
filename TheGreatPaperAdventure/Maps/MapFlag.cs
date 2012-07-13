//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                             By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TGPA.Maps.Scripts
{
    /// <summary>
    /// Flags set for the current map
    /// </summary>
    public class MapFlags
    {
        String[] flags;

        public MapFlags(int size)
        {
            //Init flags to ""
            flags = new String[size];
            for (int i = 0; i < flags.Length; i++)
            {
                flags[i] = "";
            }
        }

        /// <summary>
        /// Check if a flag is set
        /// </summary>
        /// <param name="f">Flag you want to look for</param>
        /// <returns></returns>
        public bool GetFlag(String f)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].Equals(f))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Set a flag 
        /// </summary>
        /// <param name="f">Flag to set</param>
        /// <returns></returns>
        public void SetFlag(String f)
        {
            if (f == null) return;

            if (GetFlag(f)) return;

            for (int i = 0; i < flags.Length; i++)
            {
                //Find a free space
                if (flags[i] == "")
                {
                    flags[i] = f;
                    return;
                }
            }

            throw new Exception("Too much flags for the map !");
        }

        /// <summary>
        /// Unset a flag 
        /// </summary>
        /// <param name="f">Flag to unset</param>
        /// <returns></returns>
        public void UnsetFlag(String f)
        {
            for (int i = 0; i < flags.Length; i++)
            {
                if (flags[i].Equals(f))
                {
                    flags[i] = "";
                    return;
                }
            }
        }
    }
}
