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

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Get random parameter for a cow
    /// </summary>
    public class RandomCow : Cow
    {
        public RandomCow(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags)
            : base(loc,scroll,bonus,pattern,flip,flags)
        {
            //Randomize scale for fun
            float scale = RandomMachine.GetRandomFloat(0.35f, 1.3f);
            this.Scale = new Vector2(scale, scale);
        }
    }

    /// <summary>
    /// Simple cow. Do nothing, it's a cow
    /// </summary>
    public class Cow : BadGuy
    {
        public Cow(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 250, 178), //Source sprite
               Vector2.Zero, //Speed
               Vector2.One,
                null
            )
        {
            //Stats
            hp = 17;
            points = 1000;
            Difficulty = 1;

            this.Background = true;

            UseAnimation = true;
            this.totalFrameNumber = 3;
            this.frameCooldown = 2000;
            this.spriteBox = new Vector2(256, 256);
            this.currentFrame = RandomMachine.GetRandomInt(0, 2);
            this.ttl = InfiniteTimeToLive;

            this.hitbox = new CircleHitbox(this, false,2f);
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/cow");
        }

        #endregion
    }
}
