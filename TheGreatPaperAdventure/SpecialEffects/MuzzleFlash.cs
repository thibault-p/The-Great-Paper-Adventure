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
    /// Flash in circle
    /// </summary>
    public class MuzzleFlash : Particle
    {
        public MuzzleFlash(Vector2 loc, Vector2 traj, float size, Color _color)
            : base(_color.R, _color.G, _color.B)
        {
            location = loc;
            trajectory = traj;
            this.size = size; 
            rotation = RandomMachine.GetRandomFloat(0f, 6.28f);
            Exists = true;
            frame = 0.05f;
            Additive = false;
        }

        public override void Update(float gameTime, ParticleManager pMan)
        {
            a = (frame * 8f);

            base.Update(gameTime, pMan);
        }

        public override void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {
            sprite.Draw(tex, GameLocation, new Rectangle(64, 128, 64, 64),
                particuleColor,
                rotation,
                new Vector2(32.0f, 32.0f),
                size - frame,
                SpriteEffects.None,
                1.0f);
        }

    }
}
