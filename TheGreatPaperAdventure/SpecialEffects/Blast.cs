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

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Blast effect
    /// </summary>
    public class Blast
    {
        private float val;
        private float mag;

        public Vector2 Center;

        public void Update(GameTime gameTime)
        {
            if (val >= 0f)
            {
                val -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            }
            else if (val < 0f)
                val = 0f;
        }

        public float Value
        {
            get { return val; }
            set { val = value; }
        }

        public float Magnitude
        {
            get { return mag; }
            set { mag = value; }
        }


    }
}
