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


namespace TGPA.Game.Hitbox
{
    class PositionedCircleHitbox: CircleHitbox
    {
        protected Vector2 location;
        protected float size;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e">Entity</param>
        /// <param name="l">Location</param>
        /// <param name="s">Size</param>
        /// <param name="sh">Is a shot</param>
        /// <param name="sc">Scale</param>
        public PositionedCircleHitbox(Entity e,Vector2 l, float s,bool sh,float sc)
            :base(e,sh)
        {
            size=s;
            location=l;
            scale=sc;
            circle = new BoundingSphere(new Vector3((e.Location + location),0f),size*scale);
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
                this.circle.Center.X = entity.Location.X + location.X;
                this.circle.Center.Y = entity.Location.Y + location .Y;
            } 
        }


    }




}
