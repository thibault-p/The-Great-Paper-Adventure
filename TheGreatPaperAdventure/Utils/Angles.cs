using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TGPA.Utils
{
    /// <summary>
    /// Function for easier trigonometry
    /// </summary>
    public class Angles
    {

        /// <summary>
        /// Calculate the angle between 2 Vector2
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <returns></returns>
        public static float GetAngle(Vector2 v1, Vector2 v2)
        {
            Vector2 d = new Vector2(v2.X - v1.X, v2.Y - v1.Y);
            if (d.X == 0.0f)
            {
                if (d.Y < 0.0f)
                    return MathHelper.Pi * 0.5f;
                else if (d.Y > 0.0f)
                    return MathHelper.Pi * 1.5f;
            }

            if (d.Y == 0.0f)
            {
                if (d.X < 0.0f)
                    return 0.0f;
                else if (d.X > 0.0f)
                    return MathHelper.Pi;
            }

            float a = (float)Math.Atan(Math.Abs(d.Y) / Math.Abs(d.X));

            if ((d.X < 0.0f) || (d.Y > 0.0f)) a = MathHelper.Pi - a;
            if ((d.X < 0.0f) || (d.Y < 0.0f)) a = MathHelper.Pi + a;
            if ((d.X > 0.0f) || (d.Y < 0.0f)) a = MathHelper.Pi *2.0f - a;

            if (a < 0) a = a + MathHelper.Pi * 2.0f;


            return a;
        }

    }
}
