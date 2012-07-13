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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TGPA.Game.Hitbox;
using System.Diagnostics;
using TGPA.SpecialEffects;
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys.Boss
{
    /// <summary>
    /// Frozen big boss !
    /// </summary>
    public class Esquimo : TGPA.Game.Entities.Boss
    {
        /// <summary>
        /// Facial expression
        /// </summary>
        public enum EsquimoFaceState
        {
            Normal,
            Attack,
            Hit,
            Concentrate
        }

        public enum EsquimoAttacks
        {
            None,
            Concentrate,
            Wave,
            Regen,
            StickRain
        }

        private EsquimoFaceState faceState;
        private EsquimoAttacks attackType;
        private double faceCooldown, attackCooldown, regenCooldown, concentrateCooldown;

        private Rectangle eyesSrcRect, eyesDstRect;
        private int initChocolateShieldHp, chocolateShieldHp;

        private double regenTimeConst = 20000f; //Boss heal itself 
        private int shake = -2;

        public Esquimo(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            this.sRect = new Rectangle(0, 0, 245, 591);
            this.Scale = Vector2.One;

            this.eyesSrcRect = new Rectangle(0, 600, 245, 460);
            this.eyesDstRect = ComputeDstRect(this.eyesSrcRect);

            this.ttl = InfiniteTimeToLive;

            //Stats
            this.wpn = null;

            this.hp = 2401;
            this.maxLifebarValue = hp;
            this.chocolateShieldHp = 300;
            this.initChocolateShieldHp = this.chocolateShieldHp;
            this.regenCooldown = regenTimeConst;
            this.concentrateCooldown = 0f;

            this.points = 182000;

            this.attackType = EsquimoAttacks.None;
            this.attackCooldown = 5000f;
            this.faceState = EsquimoFaceState.Normal;

            this.flagsOnDeath = flags;
            this.Flip = SpriteEffects.FlipHorizontally;
            this.InfiniteMovePattern = true;
            this.hitbox = new EmptyHitbox(this);

            this.speed = new Vector2(-50, 200);
            this.movePattern = new MovePattern();
            this.movePattern.AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2, TGPAContext.Instance.ScreenHeight / 2));

            this.DrawLifebar = true;
            this.DrawWarning = true;
        }

        public override void Update(GameTime gameTime)
        {
            bool attack = true;

            //Special attacks updates
            if (this.attackType == EsquimoAttacks.Regen)
            {
                if (this.chocolateShieldHp < initChocolateShieldHp)
                {
                    attack = false;
                    this.chocolateShieldHp++;

                    //Chocolate particules going on the boss
                    Vector2 particuleLocation = RandomMachine.GetRandomVector2(0, TGPAContext.Instance.ScreenWidth, 0, TGPAContext.Instance.ScreenHeight);
                    Vector2 middle = Vectors.ConvertPointToVector2(this.dRect.Center);
                    Vector2 particuleTrajectory = RandomMachine.GetRandomVector2(middle.X - dRect.Width / 3, middle.X + dRect.Width / 3, middle.Y - dRect.Height / 3, middle.Y + dRect.Height / 3) - particuleLocation;

                    ChocolatePiece choc = new ChocolatePiece(particuleLocation,
                        particuleTrajectory,
                        RandomMachine.GetRandomFloat(0.5f, 0.9f),
                        RandomMachine.GetRandomInt(0, 4));
                    TGPAContext.Instance.ParticleManager.AddParticle(choc, false);
                }
                else
                {
                    this.attackCooldown = 0f;
                    this.regenCooldown = regenTimeConst;
                    attack = true;
                }
            }
            else if (this.attackType == EsquimoAttacks.Concentrate)
            {
                this.location.X += shake;
                shake = -shake;

                if (concentrateCooldown <= 0f)
                {
                    if (this.wpn == null)
                        this.wpn = new EsquimoConcentrateGun();
                }
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

            if (this.concentrateCooldown > 0f)
            {
                this.concentrateCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (this.regenCooldown > 0f)
            {
                //Do not decrease cooldown if hp > 1/3
                if (this.chocolateShieldHp < (this.initChocolateShieldHp / 3))
                {
                    this.regenCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }
            else if (this.attackType != EsquimoAttacks.Regen)
            {
                attack = true; //Short circuit attack system to heal
            }

            //Change state is necessary
            if ((this.faceCooldown < 0f) && (this.faceState != EsquimoFaceState.Normal))
            {
                this.faceState = EsquimoFaceState.Normal;
            }

            //Boss is hit : change face
            if (IsHit)
            {
                this.faceState = EsquimoFaceState.Hit;
                this.faceCooldown = 2000f;

                if (this.chocolateShieldHp > 0)
                {
                    Vector2 particuleLocation = Vectors.ConvertPointToVector2(this.dRect.Center);

                    particuleLocation.X += RandomMachine.GetRandomInt(-this.dRect.Width / 4, this.dRect.Width / 4);

                    ChocolatePiece choc = new ChocolatePiece(particuleLocation,
                        RandomMachine.GetRandomVector2(-50, 50, 10, 200),
                        RandomMachine.GetRandomFloat(0.1f, 0.7f),
                        RandomMachine.GetRandomInt(0, 4));
                    TGPAContext.Instance.ParticleManager.AddParticle(choc, false);
                }
            }

            //Find a new attack
            if (attack)
            {
                //Initialisation for combat
                if (this.attackType == EsquimoAttacks.None)
                {
                    this.hitbox = new SquareHitbox(this, new Vector2(0.6f, 0.1f));
                    this.movePattern = new MovePattern();
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Right - this.dRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                }

                this.faceState = EsquimoFaceState.Attack;
                this.faceCooldown = 2000f;
                this.speed = new Vector2(-100f, 200f);

                //Special attack : Regeneration
                //****************************************************
                if ((this.regenCooldown <= 0f) && (this.attackType != EsquimoAttacks.Regen))
                {
                    this.wpn = null;
                    this.speed = new Vector2(-200f, 200f);
                    this.attackCooldown = 12000f;
                    this.attackType = EsquimoAttacks.Regen;

                    this.movePattern = new MovePattern();

                    for (int i = 0; i < 15; i++)
                    {
                        this.movePattern.AddPoint(new Point(RandomMachine.GetRandomInt(TGPAContext.Instance.ScreenWidth / 3, TGPAContext.Instance.TitleSafeArea.Right),
                            RandomMachine.GetRandomInt(TGPAContext.Instance.TitleSafeArea.Top, TGPAContext.Instance.TitleSafeArea.Bottom)));
                    }
                }
                else
                {
                    int rand = RandomMachine.GetRandomInt(0, 6);

                    //Wave Attack
                    //****************************************************
                    if ((rand < 4) && (this.attackType != EsquimoAttacks.Wave))
                    {
                        this.attackType = EsquimoAttacks.Wave;

                        this.attackCooldown = 7500f;
                        this.speed = new Vector2(-50, RandomMachine.GetRandomInt(75, 200));

                        this.movePattern = new MovePattern();

                        for (int i = 0; i < 10; i++)
                        {
                            this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Right - (this.dRect.Width / 2), RandomMachine.GetRandomInt(25, TGPAContext.Instance.TitleSafeArea.Bottom - 25)));
                        }

                        this.wpn = new EsquimoWaveGun();
                    }
                    //Sticks rain Attack
                    //****************************************************
                    else if ((rand == 4) && (this.attackType != EsquimoAttacks.StickRain))
                    {
                        this.attackType = EsquimoAttacks.StickRain;

                        this.attackCooldown = 10000f;
                        this.speed = new Vector2(-50, 75f);

                        this.movePattern = new MovePattern();

                        for (int i = 0; i < 3; i++)
                        {
                            this.movePattern.AddPoint(new Point(
                                            RandomMachine.GetRandomInt(TGPAContext.Instance.ScreenWidth / 2, 2 * TGPAContext.Instance.ScreenWidth / 3),
                                            TGPAContext.Instance.TitleSafeArea.Bottom / 2));
                        }

                        this.wpn = new EsquimoStickGun();
                    }
                    //Random nuts after concentration Attack
                    //****************************************************
                    else if ((rand == 5) && (this.attackType != EsquimoAttacks.Concentrate))
                    {
                        this.attackType = EsquimoAttacks.Concentrate;

                        this.faceCooldown = 6000f;
                        this.faceState = EsquimoFaceState.Concentrate;

                        this.attackCooldown = 6000f;
                        this.speed = new Vector2(-50, 25f);

                        this.movePattern = new MovePattern();
                        for (int i = 0; i < 3; i++)
                        {
                            this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Right - (this.dRect.Width / 2), RandomMachine.GetRandomInt(25, TGPAContext.Instance.TitleSafeArea.Bottom - 25)));
                        }

                        this.wpn = null;
                        this.concentrateCooldown = 2500f;
                    }
                }
            }

            //Sprite updates
            //************************************************
            //Facial animation
            switch (this.faceState)
            {
                case EsquimoFaceState.Normal:
                    eyesSrcRect.X = 0;
                    break;

                case EsquimoFaceState.Attack:
                    eyesSrcRect.X = 0;
                    break;

                case EsquimoFaceState.Hit:
                    eyesSrcRect.X = 245;
                    break;

                case EsquimoFaceState.Concentrate:
                    eyesSrcRect.X = 490;
                    break;
            }

            //Chocolate shield state
            if (chocolateShieldHp > (3 * initChocolateShieldHp) / 4)
            {
                this.sRect.X = 0;
            }
            else if (chocolateShieldHp > (initChocolateShieldHp / 2))
            {
                this.sRect.X = 245;
            }
            else if (chocolateShieldHp > (initChocolateShieldHp / 4))
            {
                this.sRect.X = 490;
            }
            else if (chocolateShieldHp > 0)
            {
                this.sRect.X = 735;
            }
            else
            {
                this.sRect.X = 980;
            }

            base.Update(gameTime);
            eyesDstRect = ComputeDstRect(eyesSrcRect);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Color c = Color.White;

            if (IsHit)
            {
                if (chocolateShieldHp > 0)
                {
                    c = (Color.SteelBlue *0.75f);
                    IsHit = false;
                }
                else
                {
                    c = (Color.Red *0.75f);
                    IsHit = false;
                }
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //Body
            spriteBatch.Draw(this.Sprite, dRect, sRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);
            //Eyes
            spriteBatch.Draw(this.Sprite, eyesDstRect, eyesSrcRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);
            spriteBatch.End();

#if DEBUG
            TGPAContext.Instance.TextPrinter.Write(spriteBatch,this.location, "(" + this.attackType.ToString() + ") Chocolate : " + this.chocolateShieldHp + "| Regen in :" + this.regenCooldown);
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/esquimo");
        }
        #endregion

        public override int HP
        {
            get
            {
                return base.HP;
            }
            set
            {
                if (this.chocolateShieldHp <= 0)
                {
                    base.HP = value;
                }
                else
                {
                    this.chocolateShieldHp -= (base.HP - value);
                }
            }
        }
    }

    /// <summary>
    /// Attack  1 for Esquimo
    /// </summary>
    public class EsquimoWaveGun : Weapon
    {
        private int randomSpace;

        public EsquimoWaveGun()
            : base(true)
        {
            cooldown = 200.0f;
            name = "EsquimoWaveGun";
            damage = 1;
            ammo = InfiniteAmmo;

            randomSpace = RandomMachine.GetRandomInt(100, 200);
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();


            location.Y -= randomSpace;
            newTirs.Add(new NutsShot(location, this, 0.0f, this.Flip));
            location.Y += randomSpace * 2;
            newTirs.Add(new NutsShot(location, this, 0.0f, this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Attack 2 for Esquimo
    /// </summary>
    public class EsquimoStickGun : Weapon
    {
        public EsquimoStickGun()
            : base(true)
        {
            cooldown = 600.0f;
            name = "EsquimoStickGun";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            Vector2 dstLoc = RandomMachine.GetRandomVector2(0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight - 50, TGPAContext.Instance.ScreenHeight);

            location.X = dstLoc.X;
            newTirs.Add(new StickShot(location, this, Math.PI / 2, this.Flip, dstLoc));

            return newTirs;
        }
    }

    /// <summary>
    /// Attack  3 for Esquimo
    /// </summary>
    public class EsquimoConcentrateGun : Weapon
    {
        public EsquimoConcentrateGun()
            : base(true)
        {
            cooldown = 75.0f;
            name = "EsquimoConcentrateGun";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            newTirs.Add(new FastNutsShot(location, this, RandomMachine.GetRandomFloat(0.0f, 32 * (Math.PI / 16)), this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Nuts. Because nuts are good with chocolate
    /// </summary>
    public class NutsShot : Shot
    {
        private float visualRotation;

        public NutsShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(780, 717, 160, 160),    //Sprite
            new Vector2(300, 300),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            //this.UseRotationWhenDrawing = true;
            this.visualRotation = 0f;
            this.UseSpriteOrigin();
            this.hitbox = new CircleHitbox(this, true, 2f);
        }

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

    public class FastNutsShot : NutsShot
    {
        public FastNutsShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc, wpn, angle, _flip)
        {
            this.speed = new Vector2(750, 700);
        }
    }

    /// <summary>
    /// Ice cream stick
    /// </summary>
    public class StickShot : Shot
    {
        private Vector2 dstLocation;
        private int speedY;
        private bool landing;

        public StickShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip, Vector2 dstLoc) :
            base(loc,
             new Rectangle(1000, 270, 105, 454),    //Sprite
             Vector2.Zero,          //Speed
             Vector2.One,                    //Scale
             _flip, wpn, angle, true)
        {
            this.hitbox = new SquareHitbox(this, new Vector2(0.5f, 0.2f));
            this.spriteOrigin = Vector2.Zero;
            this.dstLocation = dstLoc;
            this.ttl = 7000f;
            this.location.Y = -this.dRect.Height;
            this.speedY = 0;
            this.CanGoOffLimits = true;
            this.landing = false;
            this.Scale = new Vector2(0.4f, 0.4f);

            this.Destructable = true;
            this.Points = 200;
            this.Hp = 15;
        }

        public override void Update(GameTime gameTime)
        {
            speedY += 8;
            this.speed.Y = speedY;

            if ((this.location.Y + this.dRect.Height) > dstLocation.Y)
            {
                this.speed = Vector2.Zero;
                this.sRect.X = 1110;
                this.sRect.Width = 170;
                this.landing = true;
                this.hitbox = new SquareHitbox(this, new Vector2(0.5f, 0.5f));
            }

            base.Update(gameTime);

            if (!landing)
            {
                this.speed.Y = speedY;
            }
        }

        public override void TodoOnDeath()
        {
            TGPAContext.Instance.ParticleManager.MakeExplosion(this.location, 30f);
            base.TodoOnDeath();
        }

        public override void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            if (speedY > 0) //Hack :')
            {
                spriteBatch.Draw(texture, dRect, sRect, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
            }
        }
    }
}
