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
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Hitbox;
using TGPA.Utils;
using TGPA.SpecialEffects;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Laser dolphin + zombie mode
    /// </summary>
    public class FlipperZombie : BadGuy
    {
        public FlipperZombie(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 256, 256), //Source sprite
                new Vector2(100, 0), //Speed
                new Vector2(0.75f, 0.75f),
                null
            )
        {
            hp = 31;
            points = 5000;
            Difficulty = 20;
            this.ttl = 60000;

            this.UseAnimation = false; //Change it later
            this.totalFrameNumber = 2;
            this.frameCooldown = 100;
            this.spriteBox = new Vector2(256, 256);

            this.hitbox = new CircleHitbox(this, false, 2f);
        }

      
        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/flipperZombie");
        }
        #endregion
    }

}