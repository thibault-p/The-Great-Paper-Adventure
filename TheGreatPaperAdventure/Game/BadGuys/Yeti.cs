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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Utils;
using TGPA.Game.Hitbox;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Super Yeti. Like superman, but in deep mountains
    /// </summary>
    public class Yeti : BadGuy
    {
        public Yeti(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 400, 300), //Source sprite
               new Vector2(150,150), //Speed
               new Vector2(0.6f,0.6f),
                null
            )
        {
            //Stats
            hp = 32;
            points = 3000;
            Difficulty = 5;

            this.UseSpriteOrigin();
            
            this.hitbox = new CircleHitbox(this, true, 2f);

            this.UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 150;
            this.spriteBox = new Vector2(400, 300);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            double updateRotation = rotation;
            this.rotation -= Math.PI / 4;

            base.Draw(spriteBatch);

            this.rotation = updateRotation;
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/yeti");
        }

        #endregion
    }
}
