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
using TGPA.Game.Hitbox;
using TGPA.Utils;
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Weapons;

namespace TGPA.Game.BadGuys.Boss
{
    /// <summary>
    /// Real final boss ! :p
    /// </summary>
    public class CrazyDoc : TGPA.Game.Entities.Boss
    {
        private static int DocHP = 701;

        /// <summary>
        /// Facial expression
        /// </summary>
        public enum CrazyDocFaceState
        {
            Normal,
            Attack,
            Hit
        }

        public enum CrazyDocAttacks
        {
            None,
            Circular,
            Random,
            Wave
        }

        private CrazyDocFaceState faceState;
        private CrazyDocAttacks attackType;
        private double faceCooldown, attackCooldown;

        public CrazyDoc(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            //Body
            this.sRect = new Rectangle(0, 0, 256, 256);
            this.Scale = new Vector2(0.75f,0.75f);

            this.ttl = InfiniteTimeToLive;

            //Stats
            this.wpn = null;
            this.hp = DocHP;
            this.maxLifebarValue = this.hp;

            this.points = 182000;

            this.faceState = CrazyDocFaceState.Normal;

            this.flagsOnDeath = flags;
            this.Flip = SpriteEffects.None;
            this.InfiniteMovePattern = true;
            this.hitbox = new SquareHitbox(this, new Vector2(0.2f, 0.2f));

            this.speed = new Vector2(150f, 140.0f);

            this.DrawWarning = true;
            this.DrawLifebar = true;

            this.attackCooldown = 4000f;

            this.Pattern = new MovePattern();
            this.Pattern.AddPoint(new Point(850, 85));
            this.Pattern.AddPoint(new Point(750, 650));

            this.wpn = null;
        }

        public override void Update(GameTime gameTime)
        {
            bool attack = true;

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
                this.faceState = CrazyDocFaceState.Hit;
                this.faceCooldown = 500f;
            }
            //Change state is necessary
            else if (this.faceCooldown < 0f)
            {
                this.faceState = CrazyDocFaceState.Normal;
            }

            //Random Pattern
            if (this.Pattern.Points.Count < 3)
            {
                for (int i = 0; i < 5; i++)
                {
                    this.movePattern.AddPoint(new Point(
                                           RandomMachine.GetRandomInt(this.DstRect.Width, TGPAContext.Instance.ScreenWidth),
                                           RandomMachine.GetRandomInt(this.DstRect.Height, TGPAContext.Instance.ScreenHeight)));
                }
            }

            //***************************************************************************

            //Find a new attack
            if (attack)
            {
                //(Re)Initialisation
                this.faceState = CrazyDocFaceState.Attack;
                this.faceCooldown = 2000f;
                int rand = RandomMachine.GetRandomInt(0, 6);
                this.speed = new Vector2(150f, 140.0f);

                // Circular attack
                //****************************************************
                if ((rand < 3) && (this.attackType != CrazyDocAttacks.Circular))
                {
                    this.attackType = CrazyDocAttacks.Circular;
                    this.wpn = new CrazyDocCircularWeapon();
                    this.speed = Vector2.Zero;
                    this.attackCooldown = 2500f;
                }
                // Wave attack
                //****************************************************
                else if ((rand <= 5) && (this.attackType != CrazyDocAttacks.Wave))
                {
                    this.attackType = CrazyDocAttacks.Wave;
                    this.wpn = new CrazyDocWaveWeapon();
                    this.speed.X/=2;
                    this.attackCooldown = 3500f;
                }
            // Random shots attack
                //****************************************************
                else
                {
                    this.attackType = CrazyDocAttacks.Random;
                    this.wpn = new CrazyDocRandomWeapon();
                    this.attackCooldown = 2000f;
                }
            }

            //Sprite updates
            //************************************************
            //Facial animation
            switch (this.faceState)
            {
                case CrazyDocFaceState.Normal:
                    this.sRect.Y = 0;
                    break;

                case CrazyDocFaceState.Hit:
                    this.sRect.Y = 256;
                    break;
            }


            base.Update(gameTime);
        }

        public override void TodoOnDeath()
        {
            //Game is over !
            if (TGPAContext.Instance.Saver.SaveData.GameHasBeenEnded == false)
            {
                TGPAContext.Instance.Saver.SaveData.GameHasBeenEnded = true;
                TGPAContext.Instance.Saver.Save();
            }
            base.TodoOnDeath();
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/crazydoc");
        }
        #endregion
    }

    public class CrazyDocRandomWeapon : Weapon
    {
        public CrazyDocRandomWeapon()
            : base(true)
        {
            cooldown = 750.0f;
            name = "CrazyDocRandomWeapon";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            cooldown = RandomMachine.GetRandomFloat(250f, 1200f);

            CrazyDocShot shot = new CrazyDocShot(location, this, RandomMachine.GetRandomFloat(0, Math.PI * 2), this.Flip);
            newTirs.Add(shot);

            return newTirs;
        }
    }
    public class CrazyDocCircularWeapon : Weapon
    {
        public CrazyDocCircularWeapon()
            : base(true)
        {
            cooldown = 150.0f;
            name = "CrazyDocCircularWeapon";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            for (int i = 1; i <= 16; i++)
            {
                CrazyDocShot shot = new CrazyDocShot(location, this, i * (Math.PI / 8), this.Flip);
                newTirs.Add(shot);
            }
            return newTirs;
        }
    }
    public class CrazyDocWaveWeapon : Weapon
    {
        private float angle;

        public CrazyDocWaveWeapon()
            : base(true)
        {
            cooldown = 100.0f;
            name = "CrazyDocWaveWeapon";
            damage = 1;
            ammo = InfiniteAmmo;
            angle = RandomMachine.GetRandomFloat(0, Math.PI * 2);
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            CrazyDocShot shot = new CrazyDocShot(location, this, angle, this.Flip);
            newTirs.Add(shot);

            angle += 0.50f;

            return newTirs;
        }


    }

    public class CrazyDocShot : Shot
    {
        public CrazyDocShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(640, 14, 133, 120),    //Sprite
            new Vector2(700.0f, 700.0f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
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
