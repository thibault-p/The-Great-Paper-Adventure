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

namespace TGPA.Game.Hitbox
{
    /// <summary>
    /// Interface describing a collision mecanism
    /// </summary>
    public abstract class Hitbox
    {
        /// <summary>
        /// The entity the hitbox is related to
        /// </summary>
        protected Entity entity;

        public Hitbox(Entity e)
        {
            entity = e;
        }

        /// <summary>
        /// Is there a collision between the two elements ?
        /// </summary>
        /// <param name="e2"></param>
        /// <returns></returns>
        public virtual bool Collide(Hitbox h2)
        {
            if (h2.GetType() == typeof(CircleHitbox) || h2.GetType() == typeof(PositionedCircleHitbox) )
            {
                return Collide(((CircleHitbox)h2).Circle);
            }
            else if (h2.GetType() == typeof(SquareHitbox))
            {
                return Collide(((SquareHitbox)h2).Square);
            }

            return false;
        }

        public abstract bool Collide(BoundingBox b);

        public abstract bool Collide(BoundingSphere b);

        /// <summary>
        /// Update hitbox properties
        /// </summary>
        /// <param name="gameTime"></param>
        public abstract void Update(GameTime gameTime);

        /// <summary>
        /// Draw hitbox for debug mode
        /// </summary>
        /// <param name="spriteBatch"></param>
        public abstract void Draw(TGPASpriteBatch spriteBatch);

        /// <summary>
        /// The entity the hitbox is related to
        /// </summary>
        public Entity Entity
        {
            get { return entity; }
            set { entity = value; }
        }

    }
}
