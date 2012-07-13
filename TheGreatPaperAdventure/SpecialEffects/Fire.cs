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
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Graphics;

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Fire particle effect
    /// </summary>
    public class Fire : Particle
    {

        public Fire(Vector2 loc,
            Vector2 traj,
            float size,
            int icon)
            : base(Color.White.R, Color.White.G, Color.White.B)
        {
            this.location = loc;
            this.trajectory = traj;
            this.size = size;

            if (this.size > 1f) this.size = Math.Min(this.size,2); //Dirty hack to fix huge sizes

            flag = icon;
            Exists = true;
            frame = 0.5f;
            Additive = true;

            this.a = 1f;
        }

        public override void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {

            if (frame > 0.5f) return;

            Rectangle sRect = new Rectangle(flag * 64, 64, 64, 64);
            float bright = frame * 0.5f;
            float tsize;

            if (frame > 0.4f)
            {
                r = 1.0f;
                g = 1.0f;
                b = (frame - 0.4f) * 10.0f;

                if (frame > 0.45f)
                    tsize = (0.5f - frame) * size * 20.0f;
                else
                    tsize = size;
            }
            else if (frame > 0.3f)
            {
                r = 1.0f;
                g = (frame - 00.0f) * 10.0f;
                b = 0.0f;
                tsize = size;
            }
            else
            {
                r = frame * 3.3f;
                g = 0.0f;
                b = 0.0f;
                tsize = (frame * 0.3f) * size;
            }

            if (flag % 2 == 0)
                rotation = (frame * 7.0f + size * 20.0f);
            else
                rotation = (-frame * 11.0f + size * 20.0f);

            sprite.Draw(tex, GameLocation, sRect, particuleColor, rotation,
                new Vector2(32.0f, 32.0f), tsize, SpriteEffects.None, 0.0f);

        }

    }
}
