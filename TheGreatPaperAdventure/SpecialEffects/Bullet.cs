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
using TGPA.Utils;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Graphics;

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Bullet effect
    /// </summary>
    public class Bullet : Particle
    {

        public Bullet(Vector2 loc, Vector2 traj)
            : base(1f, 0.8f, 0.6f)
        {
            location = loc;
            trajectory = traj;
            Exists = true;
            frame = 0.5f;
            Additive = true;
            rotation = Angles.GetAngle(Vector2.Zero, traj);

            this.a = 0.2f;
        }

        public override void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {
            sprite.Draw(tex, GameLocation,
                new Rectangle(0, 128, 64, 64),
                this.particuleColor,
                rotation, new Vector2(32.0f, 32.0f),
                new Vector2(1f, 0.1f),
                SpriteEffects.None, 1.0f);
        }

    }
}
