//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                  By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace TGPA.Utils
{
    public class Vectors
    {
        /// <summary>
        /// Rotate the vector and return the new vector.
        /// </summary>
        /// <param name="A">Vector to rotate</param>
        /// <param name="r">Angle of rotation</param>
        /// <returns>Rotated vector</returns>
        public static Vector2 RotateVector2(Vector2 A, float r)
        {
            Vector2 vNewA = Vector2.Zero;

            vNewA.X = (float)(Math.Cos(r) * A.X - Math.Sin(r) * A.Y);
            vNewA.Y = (float)(Math.Cos(r) * A.Y + Math.Sin(r) * A.X);

            return vNewA;
        }

        public static Vector2 ConvertVector3ToVector2(Vector3 v)
        {
            return new Vector2(v.X, v.Y);
        }

        public static Vector2 ConvertPointToVector2(Point p)
        {
            return new Vector2(p.X,p.Y);
        }

        /// <summary>
        /// Retourne une position sur la courbe de Bézier linéaire
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 BezierLinear(Vector2 pos1, Vector2 pos2, float t) 
        {
            return pos1 = pos1 + (pos2 - pos1) * t;
        }

        /// <summary>
        /// Retourne une position sur la courbe de Bézier cubique
        /// </summary>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 BezierQuad(Vector2 pos1, Vector2 pos2, Vector2 pos3, float t)
        {
            return (1 - t) * (1 - t) * pos1 + 2 * t * (1 - t) * pos2 + t * t * pos3;
        }

        /// <summary>
        /// Calcule l'angle de l'objet qui suit une courbe de bézier linéaire
        /// </summary>
        /// <param name="currentPos"></param>
        /// <param name="pos1"></param>
        /// <param name="pos2"></param>
        /// <param name="pos3"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static float BezierCalcRotation(Vector2 currentPos, Vector2 pos1, Vector2 pos2, Vector2 pos3, float t)
        {
            float r;

            Vector2 q1 = BezierLinear(pos1, pos2, t);
            Vector2 q2 = BezierLinear(pos2, pos3, t);
            float d1 = Vector2.Distance(q1, new Vector2(currentPos.X, pos1.Y));
            float d2 = Vector2.Distance(q2, new Vector2(currentPos.X, pos1.Y));
            r = (float)Math.Atan2(d1, d2);
            r = -((float)Math.PI - ((float)Math.PI / 4) - r);

            return r;
        }
    }
}
