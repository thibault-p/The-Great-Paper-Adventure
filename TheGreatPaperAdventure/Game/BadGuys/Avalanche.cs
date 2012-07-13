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
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Avalanche hunting player
    /// </summary>
    public class Avalanche : BadGuy
    {
        private bool firstStepPassed, endLevel;

        public Avalanche(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                 new Rectangle(0, 0, 610, 768), //Source sprite
                Vector2.Zero, //Speed
                Vector2.One, //Scale
                null //Weapon
            )
        {

            //Stats
            this.hp = Int32.MaxValue;
            this.points = 0;
            this.Difficulty = 0;

            this.Background = false;
            this.Removable = false;

            this.UseAnimation = true; //Will change later
            this.totalFrameNumber = 3;
            this.frameCooldown = 75f;
            this.spriteBox = new Vector2(613, 768);

            this.firstStepPassed = false;
            this.endLevel = false;
            this.hitbox = new PositionedCircleHitbox(this, new Vector2(-450, 730), 900, false, 1);
            this.ttl = InfiniteTimeToLive;
        }

        public override void Update(GameTime gameTime)
        {
            //Kill avalanche if flag is set
            if (TGPAContext.Instance.Map.Flags.GetFlag("endAvalanche"))
            {
                this.endLevel = true;
            }

            //Get the nearest player
            float locP1 = TGPAContext.Instance.Player1.Location.X;
            Player player = TGPAContext.Instance.Player1;

            if (TGPAContext.Instance.Player2 != null)
            {
                float locP2 = TGPAContext.Instance.Player2.Location.X;

                if (locP2 < locP1)
                {
                    player = TGPAContext.Instance.Player2;
                }
            }

            if (!endLevel)
            {
                if ((player.DstRect.Left < TGPAContext.Instance.ScreenWidth / 3) || (firstStepPassed == false))
                {
                    this.location.X += 1f;

                    if (this.location.X > 0)
                    {
                        this.location.X = 0;

                        if (firstStepPassed == false)
                        {
                            this.wpn = new SnowballLauncher();
                            this.firstStepPassed = true;
                        }
                    }
                }
                else if ((player.DstRect.Left > TGPAContext.Instance.ScreenWidth / 3) && (firstStepPassed))
                {
                    this.location.X -= 1f;

                    if (this.location.X < -256)
                    {
                        this.location.X = -256;
                    }
                }
            }
            else
            {
                this.location.X -= 5f;
            }

            //Particules
            for (int i = 0; i < 10; i++)
            {
                Snow s = new Snow(new Vector2(this.location.X + (i * (this.DstRect.Width / 10)) + (this.DstRect.Width / 10), this.location.Y + this.DstRect.Height), RandomMachine.GetRandomVector2(-100f, 20f, 50f, -300f), 2f, RandomMachine.GetRandomInt(0, 4));
                TGPAContext.Instance.ParticleManager.AddParticle(s, false);
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/avalanche");
        }

        #endregion
    }

    public class SnowballLauncher : Weapon
    {
        public SnowballLauncher()
            : base(true)
        {
            cooldown = 1500;
            name = "Snowball Launcher";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            cooldown = RandomMachine.GetRandomFloat(250, 2500);

            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new SnowballLauncherShot(location, this, RandomMachine.GetRandomFloat(Math.PI / 16, 3 * Math.PI / 8), this.Flip));

            return newTirs;
        }

        /// <summary>
        /// Little spines
        /// </summary>
        public class SnowballLauncherShot : Shot
        {
            private float visualRotation;

            public SnowballLauncherShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
                base(loc,
                Rectangle.Empty,    //Sprite
                 new Vector2(250, 250),          //Speed
                Vector2.One,                    //Scale
                _flip, wpn, angle, true)
            {
                this.visualRotation = 0f;
                this.UseSpriteOrigin();

                if (RandomMachine.GetRandomFloat(0, 1) > 0.59f)
                {
                    this.sRect = new Rectangle(768, 895, 107, 105);
                }
                else
                {
                    this.sRect = new Rectangle(893, 891, 124, 116);
                }


                float newScale = RandomMachine.GetRandomFloat(0.3f, 1.2f);
                this.Scale = new Vector2(newScale, newScale);

                int newSpeed = RandomMachine.GetRandomInt(250, 450);
                this.speed = new Vector2(newSpeed, newSpeed);

                this.hitbox = new CircleHitbox(this, true, 2f);

                this.Destructable = true;
                this.Points = 250;
                this.Hp = 15;
            }

            public override void Update(GameTime gameTime)
            {
                this.visualRotation += 0.15f;

                this.rotation -= RandomMachine.GetRandomFloat(0.001f, 0.01f);

                Snow s = new Snow(this.location, RandomMachine.GetRandomVector2(-10f, 10f, -10f, 10f), 1f, RandomMachine.GetRandomInt(0, 4));
                TGPAContext.Instance.ParticleManager.AddParticle(s, true);

                base.Update(gameTime);
            }

            public override void Draw(Game.Graphics.TGPASpriteBatch spriteBatch, Texture2D texture)
            {
                double tmpRotation = this.rotation;
                this.rotation = this.visualRotation;

                base.Draw(spriteBatch, texture);

                this.rotation = tmpRotation;
            }

            public override bool DrawBehindShot
            {
                get
                {
                    return false;
                }
            }
        }
    }


}
