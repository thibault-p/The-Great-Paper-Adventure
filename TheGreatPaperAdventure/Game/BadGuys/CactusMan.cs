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
using TGPA.SpecialEffects;
using Microsoft.Xna.Framework.Content;
using TGPA.Utils;
using TGPA.Audio;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Canyon plant : it fly through the screen to hit the player
    /// </summary>
    public class CactusMan : BadGuy
    {
        private int step;
        private Vector2 direction;
        private Boolean p1, p2;

        public CactusMan(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 9, 97, 120), //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                null
            )
        {
            //Stats
            hp = 10;
            speed = new Vector2(0.0f, 0.0f);

            points = 1500;
            Difficulty = 30;
            Background = true;

            step = 0;
        }

        public override void Update(GameTime gameTime)
        {
            if (step == 1)
            {
                speed.Y += 7;

                float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float deplacementY = (speed.Y * tempsEcoule);

                location.Y += (deplacementY * direction.Y);

                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(0f, 0f, 10f, 40f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.1f, .3f),
    RandomMachine.GetRandomInt(0, 4)), true);

                Fire f = new Fire(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(0f, 0f, 10f, 40f),
                    0.25f, RandomMachine.GetRandomInt(0, 4));
                TGPAContext.Instance.ParticleManager.AddParticle(f, true);

                if (p1)
                {
                    if (((direction.Y > 0) && (location.Y >= TGPAContext.Instance.Player1.Location.Y))
                        || ((direction.Y < 0) && (location.Y <= TGPAContext.Instance.Player1.Location.Y)))
                    {
                        step = 2;
                        speed.Y = 0;
                        SoundEngine.Instance.PlaySound("specialPlop");
                    }
                }
                else if (p2)
                {
                    if (((direction.Y > 0) && (location.Y >= TGPAContext.Instance.Player2.Location.Y))
                        || ((direction.Y < 0) && (location.Y <= TGPAContext.Instance.Player2.Location.Y)))
                    {
                        step = 2;
                        speed.Y = 0;
                        SoundEngine.Instance.PlaySound("specialPlop");
                    }
                }

            }
            else if (step == 2)
            {
                speed.X += 10;

                float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
                float deplacementX = (speed.X * tempsEcoule);

                location.X += (deplacementX * direction.X);

                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(10f, 40f, 0f, 0f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.1f, .3f),
    RandomMachine.GetRandomInt(0, 4)), true);

                Fire f = new Fire(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(10f, 40f, 0f, 0f),
                    0.25f, RandomMachine.GetRandomInt(0, 4));
                TGPAContext.Instance.ParticleManager.AddParticle(f, true);

                //Mon dieu ça c'est moche :
                sRect = new Rectangle(140, 30, 117, 99);
            }
            else
            {//Check if a player is near the cactus

                p1 = Math.Abs(TGPAContext.Instance.Player1.Location.X - location.X) < 500;

                p2 = false;

                if (TGPAContext.Instance.Player2 != null)
                {
                    p2 = Math.Abs(TGPAContext.Instance.Player2.Location.X - location.X) < 500;
                }

                if (p1 || p2)
                {
                    step = 1;
                    SoundEngine.Instance.PlaySound("specialPlop");
                    Background = false;
                    direction = new Vector2();

                    if (p1)
                    {
                        if (TGPAContext.Instance.Player1.Location.X < location.X) direction.X = -1;
                        else direction.X = 1;
                        if (TGPAContext.Instance.Player1.Location.Y < location.Y) direction.Y = -1;
                        else direction.Y = 1;
                    }
                    else if (p2)
                    {
                        if (TGPAContext.Instance.Player2.Location.X < location.X) direction.X = -1;
                        else direction.X = 1;
                        if (TGPAContext.Instance.Player2.Location.Y < location.Y) direction.Y = -1;
                        else direction.Y = 1;
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/cactusMan");
        }

        #endregion
    }
}
