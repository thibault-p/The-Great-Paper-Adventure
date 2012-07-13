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
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.Game.Hitbox
{
    /// <summary>
    /// Simple square for hitbox
    /// </summary>
    public class SquareHitbox : Hitbox
    {
        private BoundingBox square;
        private Vector2 delta;
        private int deltax, deltay;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        /// <param name="delta">% of hitbox reducing value</param>
        public SquareHitbox(Entity e, Vector2 delta)
            : base(e)
        {
            this.square = new BoundingBox(Vector3.Zero, Vector3.Zero);
            this.delta = delta;
        }

        public SquareHitbox(Entity e)
            : this(e, Vector2.Zero)
        {
            deltax = 0;
            deltay = 0;
        }

        public override bool Collide(BoundingBox b)
        {
            return this.square.Intersects(b);
        }

        public override bool Collide(BoundingSphere b)
        {
            return this.square.Intersects(b);
        }

        public override void Update(GameTime gameTime)
        {
            if (delta.X != 0)
            {
                deltax = (int)((float)entity.DstRect.Width * delta.X);
            }
            if (delta.Y != 0)
            {
                deltay = (int)((float)entity.DstRect.Height * delta.Y);
            }

            this.square = new BoundingBox(new Vector3(entity.DstRect.X + (deltax/2), entity.DstRect.Y + (deltay/2), 0),
                                           new Vector3(entity.DstRect.X + entity.DstRect.Width - (deltax / 2), entity.DstRect.Y + entity.DstRect.Height - (deltay/2), 0)
                                           );
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {

            Rectangle rect = new Rectangle(
                (int)square.Min.X, (int)square.Min.Y,
                (int)square.Max.X - (int)square.Min.X, (int)square.Max.Y - (int)square.Min.Y
                );

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(TGPAContext.Instance.NullTex, rect, Color.Red *0.5f);
            spriteBatch.End();
        }

        public BoundingBox Square
        {
            get { return square; }
            set { square = value; }
        }
    }
}