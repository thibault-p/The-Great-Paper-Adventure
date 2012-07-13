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
using TGPA.Game.Hitbox;
using TGPA.Utils;
using System.Diagnostics;
using TGPA.SpecialEffects;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys.Boss
{
    /// <summary>
    /// Hidden boss flying and escaping the player
    /// </summary>
    public class MaskedEsquimo : TGPA.Game.Entities.Boss
    {
        public enum MaskedEsquimoState
        {
            Hit,
            Normal
        }

        private MaskedEsquimoState state;
        private double faceCooldown, attackCooldown;
        private bool flee;

        public MaskedEsquimo(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            this.sRect = new Rectangle(0, 0, 573, 210);
            this.Scale = new Vector2(0.6f, 0.6f);

            this.scrollValue -= new Vector2(50, 0);
            this.location = new Vector2(this.dRect.X - this.dRect.Width, TGPAContext.Instance.ScreenHeight);

            this.Removable = false;
            this.ttl = InfiniteTimeToLive;

            //Stats
            this.hp = 2001;
            this.maxLifebarValue = hp;
            this.speed = new Vector2(200, 150);

            this.points = 110000;
            this.flee = false;

            this.hitbox = new SquareHitbox(this, new Vector2(0.45f, 0.20f));

            this.state = MaskedEsquimoState.Normal;
            this.attackCooldown = 2500f;
            this.faceCooldown = 0f;

            this.flagsOnDeath = flags;

            this.movePattern = new MovePattern();
            this.InfiniteMovePattern = true;

            Point randPoint = new Point(TGPAContext.Instance.ScreenWidth - this.dRect.Width, 150);
            movePattern.AddPoint(randPoint);

            randPoint = new Point(TGPAContext.Instance.ScreenWidth - this.dRect.Width, TGPAContext.Instance.TitleSafeArea.Bottom);
            movePattern.AddPoint(randPoint);

            //Boss attributes
            this.DrawLifebar = true;

            //Animation
            UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 350;
            this.spriteBox = new Vector2(573, 210);
        }

        public override void Update(GameTime gameTime)
        {
            bool attack = (!flee);

            //Decrease cooldowns
            if (this.faceCooldown > 0f)
            {
                this.faceCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (this.attackCooldown > 0f)
            {
                this.attackCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                attack = false;
            }

            //Boss is hit : change face
            if (IsHit)
            {
                this.state = MaskedEsquimoState.Hit;
                this.faceCooldown = 500f;
            }

            //Change state is necessary
            if (this.faceCooldown < 0f)
            {
                this.state = MaskedEsquimoState.Normal;
            }

            if (flee)
            {
                if (movePattern == null)
                {
                    this.movePattern = new MovePattern();
                    this.movePattern.AddPoint(TGPAContext.Instance.ScreenWidth - 250, TGPAContext.Instance.ScreenHeight / 2);
                    this.movePattern.AddPoint(TGPAContext.Instance.ScreenWidth * 10, TGPAContext.Instance.ScreenHeight / 2);
                }
                this.speed = new Vector2(350, 350);
            }
            else if (attack && !flee)
            {
                this.faceCooldown = 2000f;

                int rand = RandomMachine.GetRandomInt(0, 5);

                //Move
                if (rand < 3)
                {
                    this.wpn = null;
                    this.movePattern = new MovePattern();

                    for (int i = 0; i < 2; i++)
                    {
                        int x = TGPAContext.Instance.TitleSafeArea.Right - (this.dRect.Width / 2);
                        int y = RandomMachine.GetRandomInt(0, TGPAContext.Instance.ScreenHeight/2);

                        Point randPoint = new Point(x, y);
                        movePattern.AddPoint(randPoint);

                        y = RandomMachine.GetRandomInt(TGPAContext.Instance.ScreenHeight / 2, TGPAContext.Instance.ScreenHeight);

                        randPoint = new Point(x, y);
                        movePattern.AddPoint(randPoint);
                    }
                    this.speed = new Vector2(200, RandomMachine.GetRandomFloat(50, 250));

                    this.attackCooldown = RandomMachine.GetRandomFloat(500, 5000);
                }
                //Fire
                else
                {
                    this.wpn = new MaskedEsquimoGun();
                    this.attackCooldown = RandomMachine.GetRandomFloat(2000, 4000);
                    this.speed = Vector2.Zero;
                }

            }

            //Sprite animation
            if (!flee)
            {
                switch (this.state)
                {
                    case MaskedEsquimoState.Normal:
                        this.sRect.Y = 0;
                        break;

                    case MaskedEsquimoState.Hit:
                        this.sRect.Y = 210;
                        break;
                }
            }
            else
            {
                this.sRect.Y = 420;
            }

            //Contrail
            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(Vectors.ConvertPointToVector2(this.dRect.Center), RandomMachine.GetRandomVector2(-500f, -250f, -10f, 10f),
0.85f, 0.85f, 0.85f, 1f,
RandomMachine.GetRandomFloat(0.025f, 0.5f),
RandomMachine.GetRandomInt(0, 4)), true);

            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Color c = Color.White;

            if (IsHit)
            {
                c = (Color.Red *0.75f);
                IsHit = false;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.Sprite, dRect, sRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);
            spriteBatch.End();
        }

        public override void TodoOnDeath()
        {
            base.TodoOnDeath();

            this.sRect.X = 128;
            this.Hitbox = new EmptyHitbox(this);
            this.flee = true;
            this.wpn = null;
            this.movePattern = null;
            this.speed = new Vector2(150, 150);

            TGPAContext.Instance.ParticleManager.MakeExplosion(Vectors.ConvertPointToVector2(DstRect.Center), 30.0f);

        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/maskedEsquimo");
        }
        #endregion
    }

    /// <summary>
    /// Simple attack for the coward
    /// </summary>
    public class MaskedEsquimoGun : Weapon
    {
        public MaskedEsquimoGun()
            : base(true)
        {
            cooldown = 500.0f;
            name = "MaskedEsquimoGun";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            newTirs.Add(new MaskedEsquimoGunShot(location, this, RandomMachine.GetRandomFloat(3 * Math.PI / 4, 5 * Math.PI / 4), this.Flip));
            return newTirs;
        }
    }


    /// <summary>
    /// 
    /// </summary>
    public class MaskedEsquimoGunShot : Shot
    {
        private float visualRotation;

        public MaskedEsquimoGunShot(Vector2 loc, MaskedEsquimoGun wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(780, 717, 160, 160),    //Sprite
             new Vector2(250, 250),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        { }

        public override void Update(GameTime gameTime)
        {
            this.visualRotation += 0.05f;
            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            double tmpRotation = this.rotation;
            this.rotation = this.visualRotation;

            base.Draw(spriteBatch, texture);

            this.rotation = tmpRotation;
        }
    }
}
