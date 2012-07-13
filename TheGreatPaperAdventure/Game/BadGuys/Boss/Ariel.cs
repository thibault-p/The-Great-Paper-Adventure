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
using TGPA.Utils;
using System.Diagnostics;
using TGPA;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys.Boss
{
    /// <summary>
    /// The little mermaid 
    /// </summary>
    public class Ariel : TGPA.Game.Entities.Boss
    {
        /// <summary>
        /// State of boss face and actions
        /// </summary>
        private enum ArielState
        {
            Normal,
            Attack,
            Hit
        }

        /// <summary>
        /// Boss attacks
        /// </summary>
        private enum ArialAttacks
        {
            None,
            Charge,
            Fishes,
            StarfishLine,
            StarfishRandom
        }

        private ArielState state;
        private ArialAttacks attackState;
        private double faceCooldown, attackCooldown;

        public Ariel(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            this.sRect = new Rectangle(0, 0, 512, 600);
            this.Scale = new Vector2(0.75f, 0.75f);

            this.scrollValue -= new Vector2(50, 0);
            this.location = new Vector2(this.dRect.X - this.dRect.Width, TGPAContext.Instance.ScreenHeight);

            this.ttl = InfiniteTimeToLive;

            //Stats
            this.wpn = null;

            this.hp = 5001;
            this.maxLifebarValue = hp;
            this.speed = Vector2.Zero;

            this.points = 169000;

            this.attackState = ArialAttacks.None;
            this.attackCooldown = 2500f;
            this.state = ArielState.Normal;

            this.flagsOnDeath = flags;
            this.Flip = SpriteEffects.FlipHorizontally;
            this.InfiniteMovePattern = true;
            this.hitbox = new EmptyHitbox(this);

            this.DrawLifebar = true;
            this.DrawWarning = true;
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

            //Change state is necessary
            if ((this.faceCooldown < 0f) && (this.state != ArielState.Normal))
            {
                this.state = ArielState.Normal;
            }

            //Boss is hit : change face
            if (IsHit)
            {
                this.state = ArielState.Hit;
                this.faceCooldown = 500f;
            }

            //Find attack
            if (attack)
            {
                if (this.attackState == ArialAttacks.None)
                {
                    this.hitbox = new SquareHitbox(this, new Vector2(0.5f, 0));
                }

                this.state = ArielState.Attack;
                this.faceCooldown = 2000f;
                this.speed = new Vector2(-150f, 50f);
                this.wpn = null;

                int rand = RandomMachine.GetRandomInt(0, 6);

                if ((this.movePattern == null) || (this.movePattern.Points.Count != 5))
                {
                    this.movePattern = new MovePattern();
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Left + 50, TGPAContext.Instance.ScreenHeight / 2));
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Left + 50 + (dRect.Width / 2), TGPAContext.Instance.ScreenHeight / 4));
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Left + 50 + dRect.Width, TGPAContext.Instance.ScreenHeight / 2));
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Left + 50 + (dRect.Width / 2), (3 * TGPAContext.Instance.ScreenHeight) / 4));
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.TitleSafeArea.Left + 50, TGPAContext.Instance.ScreenHeight / 2));
                }

                if ((rand == 0) && (this.attackState != ArialAttacks.Charge))
                {
                    this.attackState = ArialAttacks.Charge;
                    this.movePattern = new MovePattern();

                    int randY = RandomMachine.GetRandomInt(1, 4);

                    this.movePattern.AddPoint(new Point((7 * TGPAContext.Instance.ScreenWidth / 8), (randY * TGPAContext.Instance.ScreenHeight / 8)));
                    this.movePattern.AddPoint(new Point((1 * TGPAContext.Instance.ScreenWidth / 8), (randY * TGPAContext.Instance.ScreenHeight / 8)));
                    this.attackCooldown = 3500f;
                    this.speed = new Vector2(-500f, 275f);
                }
                else if ((rand == 2) && (this.attackState != ArialAttacks.StarfishLine))
                {
                    this.attackState = ArialAttacks.StarfishLine;
                    this.attackCooldown = 10000f;
                    this.wpn = new ArielStarFishLineLauncher();

                    this.movePattern = new MovePattern();
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.ScreenWidth /2 - (dRect.Width/8), TGPAContext.Instance.ScreenHeight / 2));
                    this.movePattern.AddPoint(new Point(TGPAContext.Instance.ScreenWidth / 2 - (dRect.Width / 2), TGPAContext.Instance.ScreenHeight / 2));
                }
                else if ((rand == 3) && (this.attackState != ArialAttacks.StarfishRandom))
                {
                    this.attackState = ArialAttacks.StarfishRandom;
                    this.attackCooldown = 5000f;
                    this.wpn = new ArielStarFishRandomLauncher();
                }
                else
                {
                    this.attackState = ArialAttacks.Fishes;
                    this.attackCooldown = 2000f;
                    this.wpn = new ArielFishGun();
                }

#if DEBUG
#if WINDOWS
                Trace.WriteLine("TGPA DEBUG : Ariel Attack : " + this.attackState.ToString());
#endif
#endif
            }


            //Facial animation
            switch (this.state)
            {
                case ArielState.Normal:
                    sRect.X = 0;
                    break;

                case ArielState.Attack:
                    sRect.X = 512;
                    break;

                case ArielState.Hit:
                    sRect.X = 1024;
                    break;
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/ariel");
        }
        #endregion
    }

    /// <summary>
    /// Multishot attack for Ariel
    /// </summary>
    public class ArielFishGun : Weapon
    {
        private static double oldAngle = 0f;

        public ArielFishGun()
            : base(true)
        {
            cooldown = 600.0f;
            name = "FishGun";
            damage = 1;
            ammo = InfiniteAmmo;

            oldAngle = (oldAngle == Math.PI / 32 ? 0f : Math.PI / 32);
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            for (int i = 4; i < 12; i++)
            {
                double fireAngle = ((i * Math.PI) / 16) + oldAngle;
                newTirs.Add(new FishGunShot(location, this, fireAngle, this.Flip));
            }

            return newTirs;
        }
    }

    /// <summary>
    /// Line attack for Ariel
    /// </summary>
    public class ArielStarFishLineLauncher : Weapon
    {
        private double fireAngleDelta;

        public ArielStarFishLineLauncher()
            : base(true)
        {
            cooldown = 310.0f;
            name = "StarFishLineLauncher";
            damage = 1;
            ammo = InfiniteAmmo;
            fireAngleDelta = 0f;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            for (int i = 0; i < 4; i++)
            {
                newTirs.Add(new StarFishShot(location, this, (i*Math.PI/2) + fireAngleDelta, this.Flip));
            }

            fireAngleDelta += (Math.PI) / 32;

            return newTirs;
        }
    }

    /// <summary>
    /// Random starfish attack for Ariel
    /// </summary>
    public class ArielStarFishRandomLauncher : Weapon
    {
        public ArielStarFishRandomLauncher()
            : base(true)
        {
            cooldown = 150.0f;
            name = "StarFishRandomLauncher";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new StarFishShot(location, this, RandomMachine.GetRandomFloat(0.0f, 2*Math.PI), this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Dead fish
    /// </summary>
    public class FishGunShot : Shot
    {
        private float visualRotation;

        public FishGunShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(460, 110, 178, 76),    //Sprite
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

    /// <summary>
    /// Dead fish missile
    /// </summary>
    public class StarFishShot : Shot
    {
        private float visualRotation;

        public StarFishShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(1024, 0, 256, 256),    //Sprite
            new Vector2(550, 550),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            //this.UseRotationWhenDrawing = true;
            this.UseSpriteOrigin();
            this.hitbox = new CircleHitbox(this, true, 2.5f);

            this.Destructable = true;
            this.Points = 300;
            this.Hp = 10;
        }

        public override void Update(GameTime gameTime)
        {
            this.visualRotation += 0.2f;
            base.Update(gameTime);
        }

       public override void Draw(Game.Graphics.TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            double tmpRotation = this.rotation;
            this.rotation = this.visualRotation;

            base.Draw(spriteBatch, texture);

            this.rotation = tmpRotation;
        }
    }
}
