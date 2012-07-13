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
using TGPA.Game.Hitbox;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TGPA.Game.BadGuys
{

    /// <summary>
    /// Basic sandbag
    /// </summary>
    public class Trainee : BadGuy
    {
        private double cooldown;

        public Trainee(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 512, 1024), //Source sprite
                new Vector2(150.0f, 150.0f), //Speed
                new Vector2(0.25f, 0.25f),
                null
            )
        {
            //Stats
            hp = 30;
            points = 2000;
            Difficulty = 1;

            this.InfiniteMovePattern = true;
            this.hitbox = new SquareHitbox(this,new Vector2(0.3f,0.1f));
            this.cooldown = 0f;
        }

        public override void Update(GameTime gameTime)
        {
            if (cooldown > 0f)
            {
                cooldown -= gameTime.ElapsedGameTime.Milliseconds;
                this.sRect.X = 512;
            }
            else
            {
                if (IsHit)
                {
                    cooldown = 1000f;
                }

                this.sRect.X = 0;
            }
            base.Update(gameTime);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/trainee");
        } 
        #endregion
    }
}
