//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TGPA.Game.Graphics;

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Small pieces of BN falling down
    /// </summary>
    public class ChocolatePiece : Particle
    {
        public ChocolatePiece(Vector2 location,
            Vector2 trajectory,
            float size,
            int icon)
            : base(Color.White.R, Color.White.G, Color.White.B)
        {
            this.location = location;
            this.trajectory = trajectory;

            this.flag = icon;
            this.owner = -1;
            this.Exists = true;
            this.frame = 1.0f;
            this.Additive = false;
            this.size = size;
        }

        public override void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {
            Rectangle sRect = new Rectangle(flag * 64, 384, 64, 64);

            float frameAlpha;

            if (frame > 0.9f)
                frameAlpha = (1.0f - frame) * 10.0f;
            else
                frameAlpha = (frame / 0.9f);

            a = frameAlpha;

            sprite.Draw(tex,
                GameLocation,
                sRect,
                particuleColor,
                rotation,
                new Vector2(32.0f, 32.0f),
                size,
                SpriteEffects.None,
                1.0f);

        }
    }
}
