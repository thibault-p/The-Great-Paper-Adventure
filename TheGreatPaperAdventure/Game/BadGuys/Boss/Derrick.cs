//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TGPA.Game.Hitbox;
using System.Diagnostics;
using TGPA.SpecialEffects;
using TGPA.Utils;
using System;
using System.Collections.Generic;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys.Boss
{
    /// <summary>
    /// FINAL BOSS !!!
    /// </summary>
    public class Derrick : TGPA.Game.Entities.Boss
    {
        //Debug HP
        //private static int EarHP = 15;
        //private static int PawnHP = 10;
        //private static int WheelHP = 10;
        //private static int BodyHP = 5;

        private static int EarHP = 825;
        private static int PawnHP = 850;
        private static int WheelHP = 600;
        private static int BodyHP = 350;

        #region Ears

        private class DerrickEar : BadGuy
        {
            private int deltaX;
            private int initHP;

            public DerrickEar(Vector2 loc) :
                base(loc, Vector2.Zero, null, null, null, SpriteEffects.None,
               new Rectangle(460, 200, 187, 439), //Source sprite
               Vector2.Zero, //Speed
               Vector2.One,
                null
            )
            {

                //Stats
                hp = EarHP;
                initHP = hp;
                points = 10000;
                Difficulty = 10;
                deltaX = 0;
                this.hitbox = new EmptyHitbox(this);
                this.Removable = false;
            }

            public void InitPart()
            {
                this.hitbox = new SquareHitbox(this, new Vector2(0.2f, 0.1f));
                this.Weapon = new EarLaserWeapon();
            }


            public override void TodoOnDeath()
            {
                //Change sprite
                this.sRect.Y += this.sRect.Height;

                base.TodoOnDeath();
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                //Do not draw

#if DEBUG
                //Except hitbox in debug mode
                if (TGPAContext.Instance.Options.DebugMode)
                {
                    this.hitbox.Draw(spriteBatch);
                }
#endif
            }


            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                this.dRect.X += deltaX;

                if ((this.hp <= 0) && (this.sRect != Rectangle.Empty))
                {
                    //Add smoke
                    Vector2 smokeLoc = new Vector2(this.dRect.Right - 50, this.dRect.Bottom - 50);
                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLoc
                        , RandomMachine.GetRandomVector2(20f, 100f, -50f, -150f),
1f, 1f, 1f, 1f, RandomMachine.GetRandomFloat(1.5f, 4.5f), RandomMachine.GetRandomInt(0, 4)), true);
                }
            }

            public void Shake(int deltaX)
            {
                this.deltaX = deltaX;
            }

            /// <summary>
            /// Protection
            /// </summary>
            public override Texture2D Sprite
            {
                get
                {
                    throw new Exception("No sprite, Derrick should draw every part separately !");
                }
            }
        }
        #endregion

        #region Hands

        private class DerrickHand : BadGuy
        {
            protected bool isFiring, ableToShot;
            private double destAngle;
            private int passes;
            private int deltaX;
            private int initHP;
            private Vector2 fireLocation;

            public DerrickHand(Vector2 loc) :
                base(loc, Vector2.Zero, null, null, null, SpriteEffects.None,
               new Rectangle(440, 40, 235, 140), //Source sprite
               Vector2.Zero, //Speed
               Vector2.One,
                null
            )
            {

                //Stats
                hp = PawnHP;
                initHP = hp;
                points = 12500;
                Difficulty = 10;
                deltaX = 0;
                this.hitbox = new EmptyHitbox(this);
                this.Removable = false;
                this.UseRotationWhenDrawing = true;
                this.SpriteOrigin = new Vector2(175, 75);
                this.Weapon = null;
                fireLocation = new Vector2(170, 0);
                ableToShot = isFiring = false;
                passes = 0;
                this.SmokeInBackground = true;
            }

            public double ComputedRotation
            {
                get { return rotation; }
            }

            public bool IsFiring
            {
                get { return isFiring; }
                set { isFiring = value; }
            }

            public bool AbleToShot
            {
                get { return ableToShot; }
            }

            public bool SmokeInBackground
            {
                get;
                set;
            }

            public void InitializeFire(int p, int direction)
            {
                isFiring = true;
                destAngle = direction; //(RandomMachine.GetRandomFloat(0, 1) > 0.5f) ? -1 : 1;
                destAngle *= 3 * Math.PI / 8;
                passes = p;// RandomMachine.GetRandomInt(2, 5); //number of passes
            }

            public void InitPart()
            {
                this.hitbox = new CircleHitbox(this, true, 1.25f);
            }

            public override void TodoOnDeath()
            {
                //Change sprite
                this.sRect.X += this.sRect.Width;
                hitbox = null;
                Weapon = null;

                base.TodoOnDeath();

            }


            private Vector2 computeFireLocation(Vector2 loc)
            {
                loc.X -= (float)(fireLocation.X * Math.Cos(Rotation) - fireLocation.Y * Math.Sin(Rotation));
                loc.Y -= (float)(fireLocation.X * Math.Sin(Rotation) - fireLocation.Y * Math.Cos(Rotation)); ;
                return loc;
            }



            public override void Update(GameTime gameTime)
            {
                //fire management
                if (isFiring)
                {
                    rotation += gameTime.ElapsedGameTime.Milliseconds * ((destAngle > Rotation) ? 1 : -1) * 0.0002f;
                    if (Math.Sign(destAngle) > 0)
                    {
                        if (rotation >= destAngle)
                        {
                            passes--;
                            destAngle *= -1;
                            if (!ableToShot) ableToShot = true;
                        }
                    }
                    else
                    {
                        if (rotation <= destAngle)
                        {
                            passes--;
                            destAngle *= -1;
                            if (!ableToShot) ableToShot = true;
                        }
                    }
                    if (passes == 0)
                    {
                        destAngle = 0;
                        ableToShot = isFiring = false;

                    }
                    if (ableToShot && this.wpn != null)
                    {
                        this.FiringLocation = computeFireLocation(location); //dont understand ><
                        Weapon.Fire(Vector2.Zero);
                    }
                }

                base.Update(gameTime);

                if (this.hitbox is CircleHitbox)
                {
                    BoundingSphere bs = ((CircleHitbox)this.hitbox).Circle;

                    Vector2 center = computeFireLocation(location);
                    center.Y +=  (int)(100f * Math.Sin(this.Rotation));
                    center.X += (int)(100f * Math.Cos(this.Rotation));

                    bs.Center = new Vector3(center, 0);

                    ((CircleHitbox)this.hitbox).Circle = bs;
                }

                this.dRect.X += deltaX;
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                //Do not draw

#if DEBUG
                //Except hitbox in debug mode
                if (TGPAContext.Instance.Options.DebugMode)
                {
                    this.hitbox.Draw(spriteBatch);
                }
#endif
            }

            public void Shake(int deltaX)
            {
                this.deltaX = deltaX;
            }

            /// <summary>
            /// Protection
            /// </summary>
            public override Texture2D Sprite
            {
                get
                {
                    throw new Exception("No sprite, Derrick should draw every part separately !");
                }
            }
        }
        #endregion

        #region Wheel

        private class DerrickWheels : BadGuy
        {
            private int deltaX;
            private int initHP;
            
            public DerrickWheels(Vector2 loc) :
                base(loc, Vector2.Zero, null, null, null, SpriteEffects.None,
               new Rectangle(664, 1106, 336, 82), //Source sprite
               Vector2.Zero, //Speed
               Vector2.One,
                null
            )
            {

                //Stats
                hp = WheelHP;
                initHP = hp;
                points = 12500;
                Difficulty = 10;
                deltaX = 0;
                this.hitbox = new EmptyHitbox(this);
                this.Removable = false;
            }

            public void InitPart()
            {
                this.hitbox = new SquareHitbox(this, new Vector2(0f, 0.05f));
            }

            public override void TodoOnDeath()
            {
                //Change sprite
                this.SrcRect = Rectangle.Empty; 

                base.TodoOnDeath();
            }

            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
                this.dRect.X += deltaX;

                if ((this.hp <= 0) && (this.sRect != Rectangle.Empty))
                {
                    //Add random smoke
                    Vector2 smokeLoc = RandomMachine.GetRandomVector2(dRect.Left+50, dRect.Right+250, dRect.Top + 50, dRect.Bottom+100);

                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLoc, RandomMachine.GetRandomVector2(-50f, 50f, -50f, 50f),
1f, 1f, 1f, 1f, RandomMachine.GetRandomFloat(1.5f, 4.5f), RandomMachine.GetRandomInt(0, 4)), false);
                }
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                //Do not draw

#if DEBUG
                //Except hitbox in debug mode
                if (TGPAContext.Instance.Options.DebugMode)
                {
                    this.hitbox.Draw(spriteBatch);
                }
#endif
            }

            public void Shake(int deltaX)
            {
                this.deltaX = deltaX;
            }

            /// <summary>
            /// Protection
            /// </summary>
            public override Texture2D Sprite
            {
                get
                {
                    throw new Exception("No sprite, Derrick should draw every part separately !");
                }
            }
        }
        #endregion

        /// <summary>
        /// Facial expression
        /// </summary>
        public enum DerrickFaceState
        {
            Normal,
            Attack,
            Hit
        }

        public enum DerrickAttacks
        {
            None,
            Carrote,
            CarotteWave,
            Laser,
            ChargeWait,
            Charge,
            Pause
        }

        /// <summary>
        /// Source sprite parts
        /// </summary>
        private Rectangle antennaSrcRect;

        /// <summary>
        /// Rabbit ears
        /// </summary>
        private DerrickEar leftEar, rightEar;
        private Vector2 leftEarRelativeLoc, rightEarRelativeLoc;

        /// <summary>
        /// Rabbit hands
        /// </summary>
        private DerrickHand leftHand, rightHand;
        private Vector2 leftHandRelativeLoc, rightHandRelativeLoc;

        /// <summary>
        /// Rabbit wheels
        /// </summary>
        private DerrickWheels leftWheels, rightWheels;
        private Vector2 leftWheelsRelativeLoc, rightWheelsRelativeLoc;

        private DerrickFaceState faceState;
        private DerrickAttacks attackType;
        private double faceCooldown, attackCooldown;

        private bool go, initializeY;
        private int deltaX;
        private Vector2 displayedLocation;

        private bool drawPoster;
        private Rectangle posterDst, posterSrc;

        private float alarmColor;
        private float alarmDelta;

        public Derrick(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            //Body
            this.sRect = new Rectangle(0, 0, 400, 600);
            this.antennaSrcRect = new Rectangle(30, 725, 258, 270);
            this.Scale = Vector2.One;

            this.ttl = InfiniteTimeToLive;

            //Stats
            this.wpn = null;
            this.go = false;
            this.hp = BodyHP;

            this.points = 182000;

            this.attackType = DerrickAttacks.None;
            this.attackCooldown = 5000f;
            this.faceState = DerrickFaceState.Normal;

            this.flagsOnDeath = flags;
            this.Flip = SpriteEffects.None;
            this.InfiniteMovePattern = true;
            this.hitbox = new EmptyHitbox(this);

            this.speed = new Vector2(0f, 140.0f);
            this.Pattern = new MovePattern();
            this.Pattern.AddPoint((int)location.X + dRect.Width / 2, 0);

            this.DrawWarning = true;

            this.initializeY = false;
            this.deltaX = 0;

            this.Removable = false;
            this.drawPoster = false;
            this.alarmColor = 0.0f;
            this.alarmDelta = 0.05f;

            this.posterSrc = new Rectangle(888,202,132,100);
            this.posterDst = this.posterSrc;
            this.posterDst.X = 123;
            this.posterDst.Y = 176;
        }

        /// <summary>
        /// Complex initialization for the boss : create parts, add them to game engine, etc.
        /// This is called when boss comes to screen
        /// </summary>
        public void InitBoss()
        {
            //Parts
            leftEarRelativeLoc = new Vector2(75, -205);
            leftEar = new DerrickEar(this.location + leftEarRelativeLoc);
            leftEar.SrcRect = new Rectangle(666, 197, 200, 386);
            TGPAContext.Instance.AddEnemy(leftEar);

            rightEarRelativeLoc = new Vector2(160, -260);
            rightEar = new DerrickEar(this.location + rightEarRelativeLoc);
            TGPAContext.Instance.AddEnemy(rightEar);

            leftHandRelativeLoc = new Vector2(20, 300);
            leftHand = new DerrickHand(this.location + leftHandRelativeLoc);
            leftHand.SmokeInBackground = true;
            TGPAContext.Instance.AddEnemy(leftHand);

            rightHandRelativeLoc = new Vector2(0, 305);
            rightHand = new DerrickHand(this.location + rightHandRelativeLoc);
            rightHand.SmokeInBackground = false;
            TGPAContext.Instance.AddEnemy(rightHand);

            rightWheelsRelativeLoc = new Vector2(0, 538);
            rightWheels = new DerrickWheels(this.location + rightWheelsRelativeLoc);
            TGPAContext.Instance.AddEnemy(rightWheels);

            leftWheelsRelativeLoc = new Vector2(3, 516);
            leftWheels = new DerrickWheels(this.location + leftWheelsRelativeLoc);
            TGPAContext.Instance.AddEnemy(leftWheels);

            this.DrawLifebar = true;

            this.maxLifebarValue = hp + leftEar.HP + leftHand.HP + leftWheels.HP;
        }

        public override void Update(GameTime gameTime)
        {
            bool attack = go;

            if (this.hp <= 0)
            {
                //Add smoke
                Vector2 smokeLoc = new Vector2(this.dRect.Left + 175, this.dRect.Top + 235);
                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLoc
                    , RandomMachine.GetRandomVector2(20f, 100f, -50f, -150f),
1f, 1f, 1f, 1f, RandomMachine.GetRandomFloat(1.5f, 4.5f), RandomMachine.GetRandomInt(0, 4)), true);

                this.attackType = DerrickAttacks.None;
                this.wpn = null;

                attack = false;
            }

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

            //Change state is necessary
            if ((this.faceCooldown < 0f) && (this.faceState != DerrickFaceState.Normal))
            {
                this.faceState = DerrickFaceState.Normal;
            }

            //Boss is hit : change face
            if (IsHit)
            {
                this.faceState = DerrickFaceState.Hit;
                this.faceCooldown = 2000f;
            }

            //Special updates
            //***************************************************************************
            //Only one of the pair has to be killed. If one die, we kill the other
            int brokenThingsCount = 0;

            if (leftEar.HP <= 0)
            {
                brokenThingsCount++;

                if (rightEar.HP > -999)
                {
                    rightEar.HP = -999;
                    rightEar.TodoOnDeath();
                    this.attackCooldown = 200f;
                    TGPAContext.Instance.AddBonus(new Bonus(Weapon.GetRandomWeapon().GetType().Name.ToString()));
                }
            }

            if (rightHand.HP <= 0)
            {
                brokenThingsCount++;

                if (leftHand.HP > -999)
                {
                    leftHand.HP = -999;
                    leftHand.TodoOnDeath();
                    this.attackCooldown = 200f;
                    TGPAContext.Instance.AddBonus(new Bonus("Life"));
                }
            }


            if (rightWheels.HP <= 0)
            {
                brokenThingsCount++;

                if (leftWheels.HP > -999)
                {
                    leftWheels.HP = -999;
                    leftWheels.TodoOnDeath();
                    this.attackCooldown = 250f;
                    TGPAContext.Instance.AddBonus(new Bonus(Weapon.GetRandomWeapon().GetType().Name.ToString()));
                }
            }

            if ((brokenThingsCount >= 2) && (hp > 0))
            {
                this.drawPoster = true;
                this.sRect.Y = 600;               
            }

            if ((brokenThingsCount >= 3) && (hp > 0))
            {
                this.alarmColor += this.alarmDelta;

                if ((this.alarmColor >= 2f) || (this.alarmColor < 0f))
                {
                    this.alarmDelta = -this.alarmDelta;
                }
            }

            //In case of charge : Each shot make Derrick go back a little
            if (this.attackType == DerrickAttacks.Charge)
            {
                this.Speed += new Vector2(30, 0);
            }

            //***************************************************************************

            //Find a new attack
            if (attack)
            {
                //Initialisation for combat
                if (this.attackType == DerrickAttacks.None)
                {
                    leftEar.InitPart();
                    rightHand.InitPart();
                    rightWheels.InitPart();
                    this.hitbox = new SquareHitbox(this, new Vector2(0.5f, 0.32f));
                }


                this.faceState = DerrickFaceState.Attack;
                this.faceCooldown = 2000f;
                this.speed = Vector2.Zero;
                this.movePattern = null;
                this.Background = true;
                int rand = RandomMachine.GetRandomInt(0, 8);
                this.deltaX = 0;
                this.FiringLocation = Vector2.Zero;

                this.leftHand.Weapon = null;
                this.rightHand.Weapon = null;

                //Launch charge !
                //****************************************************
                if (this.attackType == DerrickAttacks.ChargeWait)
                {
                    this.attackType = DerrickAttacks.Charge;
                    this.attackCooldown = 2500f;
                    this.wpn = null;

                    this.movePattern = new MovePattern();

                    if (leftEar.HP >= 0)
                    {
                        this.movePattern.AddPoint(new Point(300, (int)this.DstRect.Center.Y));
                        this.speed.X = 1;
                    }
                    else
                    {
                        this.movePattern.AddPoint(new Point(150, (int)this.DstRect.Center.Y));
                        this.speed.X = 20;
                    }

                    this.movePattern.AddPoint(new Point(850, (int)this.DstRect.Center.Y));

                    this.InfiniteMovePattern = true;
                    this.Background = false;
                    this.wpn = new CarotteWaveLauncherWeapon(false);
                }
                else
                {
                    //Simple carottes Attack
                    //****************************************************
                    if ((rand < 2) && (rightHand.HP > 0))
                    {
                        this.attackType = DerrickAttacks.Carrote;
                        this.wpn = null;

                        int p = RandomMachine.GetRandomInt(2, 5);
                        int d = (RandomMachine.GetRandomFloat(0, 1) > 0.5f) ? 1 : -1;

                        this.attackCooldown = p * 2500f;

                        this.rightHand.InitializeFire(p, d);
                        this.leftHand.InitializeFire(p, -1 * d);

                        this.leftHand.Weapon = new CarotteLauncherWeapon(this.leftHand);
                        this.rightHand.Weapon = new CarotteLauncherWeapon(this.rightHand);
                    }
                    //Wave carottes (from behind) Attack
                    //****************************************************
                    else if (rand < 4)
                    {
                        this.attackType = DerrickAttacks.CarotteWave;
                        this.attackCooldown = 1500f;

                        this.wpn = new CarotteWaveLauncherWeapon(true);

                        this.movePattern = new MovePattern();
                        this.movePattern.AddPoint(new Point(800, (int)this.DstRect.Center.Y));
                        this.movePattern.AddPoint(new Point(850, (int)this.DstRect.Center.Y));
                        this.Speed = RandomMachine.GetRandomVector2(0, 50, 0, 0);
                    }
                    //Charge !!!
                    //****************************************************
                    else if ((rand < 5) && (this.attackType != DerrickAttacks.Charge) && (this.rightWheels.HP > 0))
                    {
                        this.attackType = DerrickAttacks.ChargeWait;
                        this.attackCooldown = 4000f;
                        this.deltaX = 2;
                    }
                    //Laser (from ears) Attack
                    //****************************************************
                    else if ((rand >= 5) && (this.rightEar.HP > 0))
                    {
                        this.attackType = DerrickAttacks.Laser;
                        this.attackCooldown = 2000f;
                        this.wpn = new EarLaserWeapon();
                        this.FiringLocation = new Vector2(100, 280) + this.location;
                    }
                }
            }

            //Sprite updates
            //************************************************
            //Facial animation
            switch (this.faceState)
            {
                case DerrickFaceState.Normal:

                    break;

                case DerrickFaceState.Attack:

                    break;

                case DerrickFaceState.Hit:

                    break;
            }

            //Updates part
            leftEar.Location = this.location + this.leftEarRelativeLoc + leftEar.SpriteOrigin;
            leftEar.Update(gameTime);

            rightEar.Location = this.location + this.rightEarRelativeLoc + rightEar.SpriteOrigin;
            rightEar.Update(gameTime);

            leftHand.Location = this.location + this.leftHandRelativeLoc + leftHand.SpriteOrigin;
            leftHand.Update(gameTime);

            rightHand.Location = this.location + this.rightHandRelativeLoc + rightHand.SpriteOrigin;
            rightHand.Update(gameTime);

            leftWheels.Location = this.location + this.leftWheelsRelativeLoc;
            leftWheels.Update(gameTime);

            rightWheels.Location = this.location + this.rightWheelsRelativeLoc;
            rightWheels.Update(gameTime);

            //First thing : boss come from the ground
            if ((location.Y < (TGPAContext.Instance.ScreenHeight - dRect.Height)) && !go)
            {
                this.go = true;
                this.speed = Vector2.Zero;
                this.Pattern = null;
            }
            else if (!go)
            {
                if (!this.initializeY)
                {
                    this.deltaX = -1;
                    this.location.Y += rightEar.DstRect.Height;
                    this.initializeY = true;
                }

                this.location.X = TGPAContext.Instance.ScreenWidth - this.dRect.Width;
                deltaX = -deltaX;

                for (int i = 7; i <= 15; i++)
                {
                    Vector2 smokeLoc = new Vector2(i * TGPAContext.Instance.ScreenWidth / 15, TGPAContext.Instance.ScreenHeight);

                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLoc, RandomMachine.GetRandomVector2(-5f, 5f, -10f, 10f),
        0.70f, 0.70f, 0.70f, 1f,
        RandomMachine.GetRandomFloat(0.5f, 2.0f),
        RandomMachine.GetRandomInt(0, 4)), true);
                }
            }

            this.displayedLocation = location;

            //Shake shake shake
            //****************************************************************
            if (deltaX != 0)
            {
                this.displayedLocation.X += deltaX;
                TGPAContext.Instance.Player1.SetRumble(new Vector2(1.0f, 1.0f));
                if (TGPAContext.Instance.Player2 != null)
                {
                    TGPAContext.Instance.Player2.SetRumble(new Vector2(1.0f, 1.0f));
                }

                this.leftEar.Shake(deltaX);
                this.rightEar.Shake(deltaX);
                this.leftHand.Shake(deltaX);
                this.rightHand.Shake(deltaX);
            }

            //Need to update before changing manually dstRect.X value
            //Hack : Stop displaying lifebar...
            base.Update(gameTime);

            this.dRect.X = (int)this.displayedLocation.X;
            //****************************************************************

            //Smoke
            if ((this.attackType == DerrickAttacks.Charge) || (this.attackType == DerrickAttacks.ChargeWait))
            {
                Vector2 smokeLocBehind = this.displayedLocation + new Vector2(370, 245);

                for (int i = 0; i < 4; i++)
                {
                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLocBehind, RandomMachine.GetRandomVector2(10f, 75f, 10f, 40f),
                        0.10f, 0.10f, 0.10f, 1f,
                        RandomMachine.GetRandomFloat(1f, 4.0f),
                        RandomMachine.GetRandomInt(0, 4)), true);
                }

                Vector2 smokeLocBis = this.displayedLocation + new Vector2(70, 255);

                for (int i = 0; i < 4; i++)
                {
                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLocBis, RandomMachine.GetRandomVector2(10f, 75f, 10f, 40f),
                        0.10f, 0.10f, 0.10f, 1f,
                        RandomMachine.GetRandomFloat(1f, 4.0f),
                        RandomMachine.GetRandomInt(0, 4)), false);
                }
            }
        }

        public override void TodoOnPatternEnd()
        {
            //In charge
            if (this.attackType == DerrickAttacks.Charge)
            {
                //If pattern is ended, change attack
                this.attackCooldown = 0f;
            }
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Color bodyColor = Color.White;

            if (IsHit)
            {
                if ((this.rightWheels.HP <= 0)
                    && (this.rightHand.HP <= 0)
                    && (this.rightEar.HP <= 0))
                {
                    bodyColor = (Color.Red *0.75f);
                }
                IsHit = false;
            }

            Color wheelsColor = Color.White;

            if (rightWheels.IsHit)
            {
                wheelsColor =(Color.Red *0.75f);
                rightWheels.IsHit = false;
            }

            Color armColor = Color.White;

            if ((rightHand.IsHit) || (leftHand.IsHit))
            {
                armColor = (Color.Red * 0.75f);
                rightHand.IsHit = false;
            }

            Color earColor = Color.White;

            if (leftEar.IsHit)
            {
                earColor = (Color.Red * 0.75f);
                leftEar.IsHit = false;
            }

            Color posterColor = Color.Red*this.alarmColor;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            //Left wheels
            spriteBatch.Draw(this.Sprite, leftWheels.DstRect, leftWheels.SrcRect, wheelsColor, (float)leftWheels.Rotation, leftWheels.SpriteOrigin, Flip, 1.0f);

            //Left ear
            spriteBatch.Draw(this.Sprite, leftEar.DstRect, leftEar.SrcRect, earColor, (float)leftEar.Rotation, leftEar.SpriteOrigin, Flip, 1.0f);

            //Left arm
            spriteBatch.Draw(this.Sprite, leftHand.DstRect, leftHand.SrcRect, armColor, (float)leftHand.ComputedRotation, leftHand.SpriteOrigin, Flip, 1.0f);

            //Poster ?
            if (drawPoster)
            {
                Rectangle realPosterDst = posterDst;
                realPosterDst.X = this.dRect.X + posterDst.X;
                realPosterDst.Y = this.dRect.Y + posterDst.Y;
                spriteBatch.Draw(this.Sprite, realPosterDst, posterSrc, Color.White, 0.0f, Vector2.Zero, Flip, 1.0f);
                spriteBatch.Draw(this.Sprite, realPosterDst, posterSrc, posterColor, 0.0f, Vector2.Zero, Flip, 1.0f);
            }

            //Body
            spriteBatch.Draw(this.Sprite, dRect, sRect, bodyColor, 0.0f, Vector2.Zero, Flip, 0.0f);

            //Right arm
            spriteBatch.Draw(this.Sprite, rightHand.DstRect, rightHand.SrcRect, armColor, (float)rightHand.ComputedRotation, rightHand.SpriteOrigin, Flip, 1.0f);

            //Right ear
            spriteBatch.Draw(this.Sprite, rightEar.DstRect, rightEar.SrcRect, earColor, (float)rightEar.Rotation, rightEar.SpriteOrigin, Flip, 1.0f);

            //Right wheels
            spriteBatch.Draw(this.Sprite, rightWheels.DstRect, rightWheels.SrcRect, wheelsColor, (float)rightWheels.Rotation, rightWheels.SpriteOrigin, Flip, 1.0f);

            spriteBatch.End();

#if DEBUG
            TGPAContext.Instance.TextPrinter.Write(spriteBatch,this.location, "(" + this.attackType.ToString() + ")");
#endif
        }



        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/derrick");
        }
        #endregion

        public override bool IsOnScreen
        {
            get
            {
                return base.IsOnScreen;
            }
            set
            {
                base.IsOnScreen = value;
                this.InitBoss();
            }
        }

        public override int HP
        {
            get
            {
                return hp;
            }
            set
            {
                //Boss can die only when all parts are destroyed
                if ((this.rightWheels.HP <= 0)
                    && (this.rightHand.HP <= 0)
                    && (this.rightEar.HP <= 0))
                {
                    base.HP = value;
                }
                else
                {
                    this.IsHit = false;
                }
            }
        }

        public override double CurrentLifeBarValue
        {
            get
            {
                double realHps = hp;

                if (rightHand.HP > 0)
                {
                    realHps += rightHand.HP;
                }

                if (leftEar.HP > 0)
                {
                    realHps += leftEar.HP;
                }

                if (rightWheels.HP > 0)
                {
                    realHps += rightWheels.HP;
                }

                return realHps / (double)maxLifebarValue;
            }
        }

        public override void TodoOnDeath()
        {
            //Change sprite
            this.drawPoster = false;
            this.hitbox = new EmptyHitbox(this);

            //Free crazy doc
            CrazyDoc doc = new CrazyDoc(Vector2.Zero, new String[] { "crazyDocIsDead" });
            doc.Location = new Vector2(this.dRect.Left + 175, this.dRect.Top + 235);
            doc.IsOnScreen = true;
            TGPAContext.Instance.AddEnemy(doc);

            this.DrawLifebar = false;

            this.Background = true;

            //Kill parts
            leftEar.Removable = true;
            leftHand.Removable = true;
            leftWheels.Removable = true;
            rightEar.Removable = true;
            rightHand.Removable = true;
            rightWheels.Removable = true;

            //Hack : "delete" sprites. Yeah it's dirty
            leftEar.SrcRect = Rectangle.Empty;
            rightEar.SrcRect = Rectangle.Empty;
            leftHand.SrcRect = Rectangle.Empty;
            rightHand.SrcRect = Rectangle.Empty;
            rightWheels.SrcRect = Rectangle.Empty;
            leftWheels.SrcRect = Rectangle.Empty;

            base.TodoOnDeath();
        }




        private class CarotteLauncherWeapon : Weapon
        {
            private DerrickHand parent;

            public CarotteLauncherWeapon(DerrickHand parent)
                : base(true)
            {
                this.parent = parent;
                cooldown = 450.0f;
                name = "CarotteLauncher";
                damage = 1;
                ammo = InfiniteAmmo;
            }

            public override List<Shot> Fire(Vector2 location)
            {

                List<Shot> newTirs = new List<Shot>();
                if (parent.AbleToShot)
                {
                    CarotteShot c = new CarotteShot(location, this, -1 * parent.Rotation + Math.PI, this.Flip);
                    c.DrawBehindEnemies = false;
                    newTirs.Add(c);

                    for (int i = 0; i < 3; i++)
                    {
                        TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(location, RandomMachine.GetRandomVector2(-50f, 50f, -50f, 50f),
    1f, 0.4f, 0f, 1f, RandomMachine.GetRandomFloat(1.5f, 4.5f), RandomMachine.GetRandomInt(0, 4)), parent.SmokeInBackground);
                    }
                }
                return newTirs;
            }

            public override void TodoOnFiring(Vector2 location)
            {

            }
        }


        public class CarotteWaveLauncherWeapon : Weapon
        {
            public CarotteWaveLauncherWeapon(bool hard)
                : base(true)
            {
                if (hard)
                {
                    cooldown = 350.0f;
                }
                else
                {
                    cooldown = 600.0f;
                }
                name = "CarotteWaveLauncherWeapon";
                damage = 1;
                ammo = InfiniteAmmo;

            }

            public override List<Shot> Fire(Vector2 location)
            {
                List<Shot> newTirs = new List<Shot>();
                CarotteShot carotte = new CarotteShot(RandomMachine.GetRandomVector2(TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenWidth + 1, 0, TGPAContext.Instance.ScreenHeight), this, Math.PI, this.Flip);

                carotte.Location = carotte.Location + (new Vector2(carotte.DstRect.Width, 0));
                carotte.Speed += new Vector2(150, 0);

                newTirs.Add(carotte);
                return newTirs;
            }
        }

        public class EarLaserWeapon : Weapon
        {
            private double fireAngle;
            private double fireDirectionDelta;
            private int randomHole;
            private int currentShot;

            public EarLaserWeapon()
                : base(true)
            {
                cooldown = 150.0f;
                name = "EarLaserWeapon";
                damage = 1;
                ammo = InfiniteAmmo;
                fireAngle = Math.PI;

                fireDirectionDelta = RandomMachine.GetRandomInt(-1, 1);
                fireDirectionDelta = (fireDirectionDelta == -1) ? -0.20f : 0.20f;
                randomHole = RandomMachine.GetRandomInt(3, 6);
                currentShot = 0;
            }

            public override List<Shot> Fire(Vector2 location)
            {
                List<Shot> newTirs = new List<Shot>();

                currentShot++;

                if (currentShot != randomHole)
                {
                    CarotteShot carotte = new CarotteShot(location, this, fireAngle, Flip);
                    newTirs.Add(carotte);
                }

                fireAngle += fireDirectionDelta;
                if ((fireAngle > 3 * Math.PI / 2) || (fireAngle < Math.PI / 2))
                {
                    this.cooldown = 15000f; //Stop firing
                }

                return newTirs;
            }

            public override void TodoOnFiring(Vector2 location)
            {
                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(location, RandomMachine.GetRandomVector2(-20f, 20f, 20f, 20f),
0.8f, 0.8f, 0.8f, 1f, RandomMachine.GetRandomFloat(1.5f, 4.5f), RandomMachine.GetRandomInt(0, 4)), false);

                base.TodoOnFiring(location);
            }
        }


        public class CarotteShot : Shot
        {
            public CarotteShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
                base(loc,
                new Rectangle(10, 1025, 341, 134),    //Sprite
                new Vector2(700, 600),          //Speed
               new Vector2(0.35f, 0.35f),                    //Scale
                _flip, wpn, angle, true)
            {
                this.hitbox = new CircleHitbox(this, true, 2.75f);
                this.CanGoOffLimits = true;
            }
            public override void Update(GameTime gameTime)
            {
                base.Update(gameTime);
            }
        }
    }
}