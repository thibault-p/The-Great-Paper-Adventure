//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace TGPA.Utils
{
    /// <summary>
    /// Because of experimenting some problem between EN and FR Xbox when converting string to float, I am using my own class...
    /// </summary>
    public class TGPAConvert
    {
        private static String goodFloatSeparator = ",";
        private static String badFloatSeparator = ".";

        static TGPAConvert() {

            try
            {
                float bugDetect = (float)Convert.ToDouble("1,0");
                if (bugDetect == 10) //HACK
                {
                    throw new FormatException();
                }
            }
            catch (FormatException)
            {
                goodFloatSeparator = ".";
                badFloatSeparator = ",";

                Logger.Log(LogLevel.Warning, "Reverse float separator, now using : '" + goodFloatSeparator + "'");
            }
        }

        /// <summary>
        /// Convert 0,5 or 0.5 in 0.5f
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static float ToFloat(String s)
        {
            if (s.Contains(badFloatSeparator))
            {
                s = s.Replace(badFloatSeparator, goodFloatSeparator);
            }
            return (float)Convert.ToDouble(s);
        }

        /// <summary>
        /// Convert relative position to absolute
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="screenResolution"></param>
        /// <returns></returns>
        public static Vector2 RelativeToAbsoluteLoc(String x, String y, Vector2 screenResolution)
        {
            float relativeX = 0;

            relativeX = TGPAConvert.ToFloat(x);

            int finalX = 0;

            if (relativeX == -1) finalX = -1;
            else if (relativeX == 1) finalX = (int)screenResolution.X + 1;
            else
                finalX = (int)(relativeX * screenResolution.X);

            float relativeY = 0;

            relativeY = TGPAConvert.ToFloat(y);

            int finalY = 0;

            if (relativeY == -1) finalY = -1;
            else if (relativeY == 1) finalY = (int)screenResolution.Y + 1;
            else
                finalY = (int)(relativeY * screenResolution.Y);

            return new Vector2(finalX, finalY);
        }

    }
}