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
using Microsoft.Xna.Framework;
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.Game.Hitbox
{
    /// <summary>
    /// Bounding Sphere Hitbox
    /// </summary>
    public class CircleHitbox : Hitbox
    {
        protected BoundingSphere circle;
        protected float scale;
        protected bool shot;

        public CircleHitbox(Entity e, bool shot)
            : base(e)
        {
            this.shot = shot;
            this.scale = 1f;
            this.circle = new BoundingSphere(Vector3.Zero, 0.0f);

        }

        public CircleHitbox(Entity e, bool shot, float scale)
            : this(e, shot)
        {
            this.scale = scale;
        }

        public override bool Collide(BoundingBox b)
        {
            return this.circle.Intersects(b);
        }

        public override bool Collide(BoundingSphere b)
        {
            return this.circle.Intersects(b);
        }

        public override void Update(GameTime gameTime)
        {
            //Update the hitbox location and radius
            if (shot)
            {
                this.circle.Center.X = entity.Location.X;
                this.circle.Center.Y = entity.Location.Y;
            }
            else
            {
                this.circle.Center.X = entity.Location.X + entity.DstRect.Width / 2;
                this.circle.Center.Y = entity.Location.Y + entity.DstRect.Height / 2;
            }

            this.circle.Radius = entity.DstRect.Width / (2 * scale);

        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {

        }

        public BoundingSphere Circle
        {
            get { return circle; }
            set { circle = value; }
        }
    }
}