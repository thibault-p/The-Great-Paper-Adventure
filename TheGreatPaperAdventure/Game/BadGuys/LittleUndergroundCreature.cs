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
using TGPA.Audio;
using TGPA.Utils;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Kind of Mario ennemy : start on roof, fall when player is near, then walk on floor
    /// </summary>
    public class LittleUndergroundCreature : BadGuy
    {
        private bool fall;
        private float speedBis;

        public LittleUndergroundCreature(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                 new Rectangle(0, 0, 256, 256), //Source sprite
                new Vector2(150, 0), //Speed
                new Vector2(0.25f,0.25f),
                null
            )
        {
            hp = 12;
            points = 1000;
            Difficulty = 5;
            fall = false;

            UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 250;
            this.spriteBox = new Vector2(256, 256);
        }

        public override void Update(GameTime gameTime)
        {
            if (hp < 7)
            {
                sRect.Y = 256;
            }

            if (!fall)
            {
                //Check if player is near the caca
                bool p1 = Math.Abs(TGPAContext.Instance.Player1.DstRect.Center.X - location.X) < 130;

                bool p2 = false;

                if (TGPAContext.Instance.Player2 != null)
                {
                    p2 = Math.Abs(TGPAContext.Instance.Player2.DstRect.Center.X - location.X) < 130;
                }

                if (p1 || p2)
                {
                    fall = true;

                    //Play a funny sound
                    SoundEngine.Instance.PlaySound("specialPlop");
                }
            }
            else {

                //Floor @ y = 64
                if (location.Y < TGPAContext.Instance.ScreenHeight - this.dRect.Height - 64)
                {
                    speedBis += 15;
                    this.speed.Y = speedBis;

                    float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    float deplacementY = (speed.Y * tempsEcoule);

                    location.Y += deplacementY;
                }
                else
                {
                    this.flips = SpriteEffects.FlipVertically;
                    if (hp < 7)
                    {
                        speed = new Vector2(-350f, 0f);
                    }
                    else
                    {
                        speed = new Vector2(250f, 0f);
                    }
                }
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/littleUndergroundCreature");
        }
        #endregion
    }
}
