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

namespace TGPA.Utils
{
    /// <summary>
    /// Create random values
    /// </summary>
    public class RandomMachine
    {
        private static Random random;

        static RandomMachine()
        {
            random = new Random((int)System.DateTime.Now.Millisecond);
        }

        /// <summary>
        /// Return a random float between two values
        /// </summary>
        /// <param name="fMin"></param>
        /// <param name="fmax"></param>
        /// <returns></returns>
        public static float GetRandomFloat(double fMin, double fmax)
        {
            return (float)(random.NextDouble() * (fmax - fMin) + fMin);
        }

#if WINDOWS

        /// <summary>
        /// Return a random float between two values
        /// </summary>
        /// <param name="fMin"></param>
        /// <param name="fmax"></param>
        /// <returns></returns>
        public static float GetRandomFloat(float fMin, float fmax)
        {
            return (float)random.NextDouble() * (fmax - fMin) + fMin;
        }
#endif

        /// <summary>
        /// Return a random integer between two values
        /// </summary>
        /// <param name="fMin"></param>
        /// <param name="fmax"></param>
        /// <returns></returns>
        public static int GetRandomInt(int min, int max)
        {
            return random.Next(min,max);
        }

        /// <summary>
        /// Return a random vector2
        /// </summary>
        /// <param name="fMin"></param>
        /// <param name="fmax"></param>
        /// <returns></returns>
        public static Vector2 GetRandomVector2(float xMin, float xMax, float yMin, float yMax)
        {
            return new Vector2(GetRandomFloat(xMin,xMax),
                GetRandomFloat(yMin,yMax));
        }

        /// <summary>
        /// Return a random vector2
        /// TODO DMA : FIX ME
        /// </summary>
        /// <param name="fMin"></param>
        /// <param name="fmax"></param>
        /// <returns></returns>
        public static Vector2 GetRandomVector2(Vector2 center, double radius)
        {
            double randomTeta = (double)GetRandomFloat(0, 360);

            double x = Math.Cos(randomTeta) * radius;
            double y = Math.Sin(randomTeta) * radius;

            return new Vector2((float)x, (float)y);
        }

    }
}
