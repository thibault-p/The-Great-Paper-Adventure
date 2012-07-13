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
using TGPA.SpecialEffects;
using TGPA.Utils;
using TGPA.Audio;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Little sh*** flying in the air
    /// </summary>
    public class Caca : BadGuy
    {
        private bool launching;
        private float speedBis;

        public Caca(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 128, 128), //Source sprite
               Vector2.Zero, //Speed
               new Vector2(0.25f,0.25f),
                null
            )
        {

            //Stats
            hp = 1;
            points = 1000;
            Difficulty = 10;

            this.Background = true;

            speedBis = 0f;
            launching = false;
        }

        /// <summary>
        /// Caca are stuck on the ground until player fly around them. Then the caca enable its powerfull jetpack to hit the player.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (!launching)
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
                    launching = true;

                    //Smile on caca's face
                    sRect.X = 128;

                    //Play a funny sound
                    SoundEngine.Instance.PlaySound("specialPlop");
                }
            }

            //Fly !
            if (launching)
            {
                speedBis -= 5;
                this.speed.Y = speedBis;

                float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float deplacementY = (speed.Y * tempsEcoule);

                location.Y += deplacementY;

                Vector2 newLoc = Vectors.ConvertPointToVector2(DstRect.Center);

                //Little smoke contrail
                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(newLoc, RandomMachine.GetRandomVector2(0f, 0f, 10f, 40f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.1f, .3f),
    RandomMachine.GetRandomInt(0, 4)), true);

                Fire f = new Fire(newLoc, RandomMachine.GetRandomVector2(0f, 0f, 10f, 40f),
                    0.25f, RandomMachine.GetRandomInt(0, 4));
                TGPAContext.Instance.ParticleManager.AddParticle(f, true);

            }

            base.Update(gameTime);

            if (launching)
            {
                this.speed.Y = speedBis;
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/caca");
        }

        #endregion
    }
}
