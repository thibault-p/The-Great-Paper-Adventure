//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGPA.Game.Graphics;

namespace TGPA.Game.Hitbox
{
    /// <summary>
    /// Void hitbox : you can't hit it !
    /// </summary>
    public class EmptyHitbox : Hitbox
    {
        public EmptyHitbox(Entity e)
            : base(e)
        {

        }

        public override bool Collide(Microsoft.Xna.Framework.BoundingBox b)
        {
            return false;
        }

        public override bool Collide(Microsoft.Xna.Framework.BoundingSphere b)
        {
            return false;
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            
        }
    }
}
