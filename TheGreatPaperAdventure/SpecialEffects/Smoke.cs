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
    /// Smoke effect
    /// </summary>
    public class Smoke : Particle
    {
        public Smoke(Vector2 location,
            Vector2 trajectory,
            float r,
            float g,
            float b,
            float a,
            float size,
            int icon) : base(r,g,b)
        {
            this.location = location;
            this.trajectory = trajectory;
            this.a = a;

            //this.size = size; //BUG
            this.size = 0.2f;

            this.flag = icon;
            this.owner = -1;
            this.Exists = true;
            this.frame = 1.0f;
            this.Additive = false;
        }

        public override void Update(float gameTime, ParticleManager pMan)
        {
            this.size = 3 - (frame * 3);
            if (size > 1) size = 1;

            if (frame < 0.5f)
            {
                if (trajectory.Y < -10.0f) trajectory.Y += gameTime * 500.0f;
                if (trajectory.X < -10.0f) trajectory.X += gameTime * 150.0f;
                if (trajectory.Y > 10.0f) trajectory.Y += gameTime * 150.0f;
            }

            //float frameAlpha;

            //if (frame > 0.9f)
            //    frameAlpha = (1.0f - frame) * 10.0f;
            //else
            //    frameAlpha = (frame / 0.9f);

            //a = frameAlpha;

            a = frame;

            base.Update(gameTime, pMan);
        }

        public override void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {
            Rectangle  sRect = new Rectangle(flag * 64, 0, 64, 64);

            sprite.Draw(tex,
                GameLocation,
                sRect,
                particuleColor,
                rotation,
                new Vector2(32.0f, 32.0f),
                size + (1.0f - frame),
                SpriteEffects.None,
                1.0f);

        }
    }
}
