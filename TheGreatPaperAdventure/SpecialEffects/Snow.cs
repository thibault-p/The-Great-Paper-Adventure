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
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.SpecialEffects
{
    /// <summary>
    /// Snow from avalanche
    /// </summary>
    public class Snow : Particle
    {
        public Snow(Vector2 location,
            Vector2 trajectory,
            float size,
            int icon)
            : base(Color.White.R, Color.White.G, Color.White.B)
        {
            this.location = location;
            this.trajectory = trajectory;
            this.a = 1.0f;
            this.size = size;

            if (this.size > 1f) 
                this.size = Math.Min(this.size, 1.5f); //Dirt hack to fix huge sizes

            this.flag = icon;
            this.Exists = true;
            this.frame = 1.0f;
            this.Additive = false;

        }

        public override void Update(float gameTime, ParticleManager pMan)
        {

            if (frame < 0.5f)
            {
                if (trajectory.Y < -10.0f) trajectory.Y += gameTime * 500.0f;
                if (trajectory.X < -10.0f) trajectory.X += gameTime * 150.0f;
                if (trajectory.Y > 10.0f) trajectory.Y += gameTime * 150.0f;
            }

            rotation = Angles.GetAngle(location, trajectory);

            base.Update(gameTime, pMan);
        }

        public override void Draw(TGPASpriteBatch sprite, Texture2D tex)
        {
            Rectangle sRect = new Rectangle(flag * 64, 256, 64, 64);
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
                    size + (1.0f - frame),
                    SpriteEffects.None,
                    1.0f);

        }
    }
}
