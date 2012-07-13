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
using TGPA.Game.Entities;
using TGPA.SpecialEffects;
using TGPA.Game.Hitbox;
using TGPA.Audio;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys.Boss
{
    public enum StateBoss1
    {
        Moving,
        Attack,
        Hit
    }

    public enum AttackBoss1
    {
        Circluar,
        OneShot,
        Multidirectionnal
    }

    /// <summary>
    /// ! Herr Gooter Fürher Chokolade !
    /// </summary>
    public class BN : TGPA.Game.Entities.Boss
    {
        public static double animationTime = 1000f;
        private static int basicHP = 1500;
        private double frametime;
        private bool go;

        private AttackBoss1 attacks;
        private MiniBNMachineGun wpn2;
        private MiniBNMultidirectionnalGun wpn3;
        private int nShots;
        private double cooldown;
        private int explosionsCount;

        public BN(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            sRect = new Rectangle(0, 0, 300, 300);
            this.Scale = new Vector2(0.60f, 0.60f);

            location = new Vector2(TGPAContext.Instance.ScreenWidth, 200);
            ttl = InfiniteTimeToLive;

            //Stats
            wpn = new MiniBNCircularShield();
            wpn2 = new MiniBNMachineGun();
            wpn3 = new MiniBNMultidirectionnalGun();
            cooldown = 5000.0f;

            hp = basicHP;
            maxLifebarValue = hp;
            speed = new Vector2(150.0f, 50.0f);

            frametime = 0f;
            points = 100000;
            go = false;
            nShots = 0;
            explosionsCount = 0;
            attacks = AttackBoss1.Circluar;

            this.Flip = SpriteEffects.FlipHorizontally;
            this.Weapon.Flip = SpriteEffects.FlipHorizontally;
            this.flagsOnDeath = flags;

            this.DrawLifebar = true;
            this.DrawWarning = true;

            this.InfiniteMovePattern = true;
            this.Pattern = new MovePattern();
            this.Pattern.AddPoint(TGPAContext.Instance.ScreenWidth - 300,200);
        }

        public override void Update(GameTime gameTime)
        {
            cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (cooldown < 0) cooldown = 0;

            if (IsHit)
            {
                BNMiettes miette1 = new BNMiettes(new Vector2(RandomMachine.GetRandomInt(this.DstRect.Center.X - this.DstRect.Width / 2, this.DstRect.Center.X + this.DstRect.Width / 2),
                    this.DstRect.Bottom - this.DstRect.Height / 2), RandomMachine.GetRandomVector2(-50, 50, 100, 500),
                    RandomMachine.GetRandomInt(0, 4));
                BNMiettes miette2 = new BNMiettes(new Vector2(RandomMachine.GetRandomInt(this.DstRect.Center.X - this.DstRect.Width / 2, this.DstRect.Center.X + this.DstRect.Width / 2),
                    this.DstRect.Bottom), RandomMachine.GetRandomVector2(-50, 50, 100, 500),
                    RandomMachine.GetRandomInt(0, 4));

                TGPAContext.Instance.ParticleManager.AddParticle(miette1);
                TGPAContext.Instance.ParticleManager.AddParticle(miette2);
            }

            //Change BN if low HP
            if ((hp < (2 * basicHP / 3)) && (hp > basicHP/2))
            {
                sRect.X = 300;

                if (explosionsCount < 1)
                {
                    TGPAContext.Instance.ParticleManager.MakeExplosion(new Vector2(this.dRect.Left,this.dRect.Bottom), 10f);
                    explosionsCount++;
                    SoundEngine.Instance.PlaySound("bigExplosion");
                }
            }
            else if ((hp < basicHP / 2) && (hp > basicHP/3))
            {
                sRect.X = 600;

                if (explosionsCount < 2)
                {
                    TGPAContext.Instance.ParticleManager.MakeExplosion(new Vector2(this.dRect.Right, this.dRect.Top), 10f);
                    TGPAContext.Instance.ParticleManager.MakeExplosion(new Vector2(this.dRect.Right, this.dRect.Bottom), 10f);
                    explosionsCount++;
                    SoundEngine.Instance.PlaySound("bigExplosion");
                }
            }
            else if (hp < basicHP/3)
            {
                sRect.X = 900;

                if (explosionsCount < 3)
                {
                    TGPAContext.Instance.ParticleManager.MakeExplosion(new Vector2(this.dRect.Left, this.dRect.Top), 10f);
                    explosionsCount++;
                    SoundEngine.Instance.PlaySound("bigExplosion");
                }
            }

            if (frametime != 0)
            {
                frametime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (frametime < 0)
                {
                    frametime = 0;
                    sRect.Y = 0;
                }
            }

            if (IsHit)
            {
                sRect.Y = 300;
                frametime = animationTime;
            }

            //Behavior
            //First thing : boss go to the middle of the screen
            if (location.X < (TGPAContext.Instance.ScreenWidth - 150) && !go)
            {
                go = true;
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

            base.Update(gameTime);
        }

        public override List<Shot> Fire()
        {
            if (!go) return null;

            List<Shot> shots = new List<Shot>();

            if (cooldown != 0) return shots;

            switch (attacks)
            {
                case AttackBoss1.Circluar:

                    shots = wpn.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    nShots += shots.Count;

                    if (nShots > 70)
                    {
                        int rand = RandomMachine.GetRandomInt(0, 5);

                        if (rand < 2)
                        {
                            attacks = AttackBoss1.OneShot;
                            wpn2.Direction = -wpn2.Direction;
                        }
                        else if (rand == 5)
                            attacks = AttackBoss1.Circluar;
                        else
                            attacks = AttackBoss1.Multidirectionnal;

                        cooldown = 1000f;
                        nShots = 0;
                    }
                    break;


                case AttackBoss1.OneShot:

                    shots = wpn2.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    nShots += shots.Count;

                    if (nShots > 15)
                    {
                        int rand = RandomMachine.GetRandomInt(0, 5);

                        if (rand == 0)
                            attacks = AttackBoss1.Circluar;
                        else if (rand == 1)
                        {
                            attacks = AttackBoss1.OneShot;
                            wpn2.Direction = -wpn2.Direction;
                        }
                        else
                            attacks = AttackBoss1.Multidirectionnal;

                        cooldown = 600f;
                        nShots = 0;
                    }

                    break;

                case AttackBoss1.Multidirectionnal:

                    shots = wpn3.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    nShots += shots.Count;

                    if (nShots > 15)
                    {
                        int rand = RandomMachine.GetRandomInt(0, 5);

                        if (rand == 1)
                            attacks = AttackBoss1.Circluar;
                        else if (rand == 2)
                            attacks = AttackBoss1.Multidirectionnal;
                        else
                        {
                            attacks = AttackBoss1.OneShot;
                            wpn2.Direction = -wpn2.Direction;
                        }

                        cooldown = 600f;
                        nShots = 0;
                    }

                    break;

            }



            return shots;
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/bn");
        }
        #endregion
    }

    #region weapon 1

    /// <summary>
    /// Circular attack for BN
    /// </summary>
    public class MiniBNCircularShield : Weapon
    {
        protected double fireAngle;

        public MiniBNCircularShield()
            : base(true)
        {
            fireAngle = 0f;
            cooldown = 70.0f;
            name = "Circular";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            fireAngle += 0.157;
            if (fireAngle > Math.PI * 2) fireAngle = 0;

            newTirs.Add(new MiniBNMachineGunShot(location, this, fireAngle, this.Flip));

            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {

        }
    }

    #endregion

    /// <summary>
    /// Circular attack for BN
    /// </summary>
    public class MiniBNMultidirectionnalGun : Weapon
    {
        private double fireAngle;

        public MiniBNMultidirectionnalGun()
            : base(true)
        {
            fireAngle = 0f;
            cooldown = 1000.0f;
            name = "MultidirectionnalGun";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            fireAngle += Math.PI / 8;
            if (fireAngle > Math.PI * 2) fireAngle = 0;

            newTirs.Add(new MiniBNMachineGunShot(location, this, fireAngle, this.Flip));
            newTirs.Add(new MiniBNMachineGunShot(location, this, ((Math.PI / 2) + fireAngle), this.Flip));
            newTirs.Add(new MiniBNMachineGunShot(location, this, ((Math.PI) + fireAngle), this.Flip));
            newTirs.Add(new MiniBNMachineGunShot(location, this, (((3 * Math.PI) / 2) + fireAngle), this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Multishot attack for BN
    /// </summary>
    public class MiniBNMachineGun : Weapon
    {
        public int Direction { get; set; }

        public MiniBNMachineGun()
            : base(true)
        {
            cooldown = 100.0f;
            name = "MiniBN";
            damage = 1;
            ammo = InfiniteAmmo;
            Direction = 1;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new MiniBNMachineGunShot(location, this, RandomMachine.GetRandomFloat((float)(15 * (Math.PI / 8)) * Direction, (float)(17 * (Math.PI / 8))), this.Flip));

            return newTirs;
        }
    }


    public class MiniBNMachineGunShot : Shot
    {
        public MiniBNMachineGunShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(104, 106, 141, 134),    //Sprite
            new Vector2(300, 300),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            this.UseSpriteOrigin();
            this.UseRotationWhenDrawing = true;
        }

        public override void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            base.Draw(spriteBatch, texture);
        }
    }

}
