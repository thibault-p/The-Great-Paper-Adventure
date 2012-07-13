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

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Growing pumpkin
    /// </summary>
    public class Pumpkin : BadGuy
    {
        private double hitCooldown;
        private bool bombed;

        public Pumpkin(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 256, 256), //Source sprite
               Vector2.Zero, //Speed
               new Vector2(0.5f,0.5f),
                null
            )
        {

            //Stats
            hp = 60;
            points = 1200;
            Difficulty = 10;
            hitCooldown = 0f;
            this.hitbox = new SquareHitbox(this, new Vector2(0.25f,0.25f));
            this.bombed = false;
            this.Background = true;
        }

        public override void Update(GameTime gameTime)
        {
            bool grow = false;

            //Decrease cooldowns
            if (this.hitCooldown > 0f)
            {
                this.hitCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }
            else
            {
                grow = true;
            }

            this.hitbox = new SquareHitbox(this, new Vector2(0.4f, 0.4f));

            if ((this.IsHit) && (grow))
            {
                int oldTailleW = dRect.Width;
                int oldTailleH = dRect.Height;

                if (hp < 26)
                {
                    this.sRect = new Rectangle(512,0 , 256, 256);
                    this.Scale += new Vector2(0.025f, 0.025f);
                }
                else if (hp < 46)
                {
                    this.sRect = new Rectangle(256, 0, 256, 256);
                    this.Scale += new Vector2(0.05f, 0.05f);
                }
                else
                {
                    this.Scale += new Vector2(0.10f, 0.10f);
                }

                location.X -= (dRect.Width - oldTailleW) / 2;
                location.Y -= (dRect.Height - oldTailleH) / 2;

                this.hitCooldown = 100f;
            }

            base.Update(gameTime);
        }

        public override void TodoOnBombing(int damage)
        {
            if (!bombed)
            {
                this.hp = 9;
                this.sRect = new Rectangle(512, 0, 256, 256);
                this.Scale += new Vector2(0.5f, 0.5f);
                bombed = true;
            }
            else
            {
                base.TodoOnBombing(damage);
            }
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/pumpkin");
        }
        #endregion

    }
}