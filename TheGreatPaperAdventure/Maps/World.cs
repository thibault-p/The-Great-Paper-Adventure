//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;

namespace TGPA.Maps
{
    /// <summary>
    /// Game world : contains one or more maps
    /// </summary>
    public class World
    {
        private static String mapFolder = "Content\\"+"maps\\";

        private String[] mapFiles;
        private int[] mapLevels;
        private int currentIndex;

        public World(String[] mapFiles, int[] levelIndexes)
        {
            this.mapFiles = mapFiles;
            this.mapLevels = levelIndexes;
            this.currentIndex = -1;
        }

        /// <summary>
        /// Return the first map of a level
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <returns></returns>
        public String GetMapFirstFile(int levelIndex) 
        {
            for (int i = 0; i < mapLevels.Length; i++)
            {
                if (mapLevels[i] == levelIndex)
                {
                    this.currentIndex = i;
                    return mapFolder+this.mapFiles[i];
                }
            }

            throw new Exception("Invalid level index : "+levelIndex);
        }

        /// <summary>
        /// Return the maps of the level
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <returns></returns>
        public String[] GetMaps(int levelIndex)
        {
            List<String> indices = new List<String>();

            for (int i = 0; i < mapLevels.Length; i++)
            {
                if (mapLevels[i] == levelIndex)
                {
                    indices.Add(this.mapFiles[i]);
                }
            }

            return indices.ToArray();
        }

        /// <summary>
        /// Return the indice of the maps
        /// </summary>
        /// <param name="levelIndex"></param>
        /// <returns></returns>
        public int GetMapIndice(String map)
        {
            for (int i = 0; i < this.mapFiles.Length; i++)
            {
                if (this.mapFiles[i].Equals(map))
                {
                    return i;
                }
            }

            throw new Exception("Invalid level : " + map);
        }

        public bool CanGoNextLevel()
        {
            return (currentIndex + 1 < mapFiles.Length);
        }

        public String GetNextLevel()
        {
            this.currentIndex++;
            return GetCurrentLevel();
        }

        public String GetCurrentLevel()
        {
            return mapFolder + this.mapFiles[currentIndex];
        }

        public int CurrentIndex
        {
            get { return currentIndex; }
        }
    }
}
