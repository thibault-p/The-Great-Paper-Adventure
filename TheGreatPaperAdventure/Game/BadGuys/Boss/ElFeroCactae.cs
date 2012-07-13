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
using TGPA.Game.Entities;
using TGPA.Utils;
using TGPA.Game.Hitbox;
using TGPA.SpecialEffects;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Very dangerous Pamp... CACTUS !
    /// </summary>
    public class ElFeroCactae : TGPA.Game.Entities.Boss
    {
        public enum CactusStateBoss
        {
            Attack,
            Hit
        }

        public enum AttackBoss2
        {
            Circluar,
            Pause,
            SmallCactus,
            Grow
        }


        public static double animationTime = 2000f;
        private const int basicHP = 1501;

        private double frametime;
        private bool go;
        private int deltaX;

        private CactusStateBoss etat;

        private TGPA.Game.Entities.BackgroundActiveElement.ElFeroPanel panel;

        private AttackBoss2 attacks;
        private ElFeroCactaeWPN2 wpn2;
        private ElFeroCactaeWPN3 wpn3;
        private ElFeroCactaeWPN4 wpn4;
        private double attackDuration;
        private SquareHitbox oldHitbox;


        public ElFeroCactae(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            sRect = new Rectangle(0, 0, 512, 512);
            Scale = new Vector2(1.3f, 1.3f);

            location = new Vector2(TGPAContext.Instance.TitleSafeArea.Right - this.dRect.Width, TGPAContext.Instance.ScreenHeight + 1);

            etat = CactusStateBoss.Attack;
            frametime = 400;

            ttl = InfiniteTimeToLive;

            //Stats
            wpn = new ElFeroCactaeWPN1();
            wpn2 = new ElFeroCactaeWPN2();
            wpn3 = new ElFeroCactaeWPN3();
            wpn4 = new ElFeroCactaeWPN4();
            attackDuration = 0.0f;

            hp = basicHP;
            maxLifebarValue = hp;
            speed = new Vector2(0f, 70.0f);
            Pattern = new MovePattern();
            Pattern.AddPoint((int)location.X + dRect.Width / 2, 0);

            frametime = 0f;
            points = 125000;
            go = false;
            deltaX = -1;
            attacks = AttackBoss2.Circluar;

            this.hitbox = new EmptyHitbox(this);

            this.flagsOnDeath = flags;

            panel = new BackgroundActiveElement.ElFeroPanel(new Vector2(150, TGPAContext.Instance.ScreenHeight + 1));
            TGPAContext.Instance.AddEnemy(panel);

            this.DrawLifebar = true;
            this.DrawWarning = true;
        }

        public override void Update(GameTime gameTime)
        {
            attackDuration -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (attackDuration < 0) attackDuration = 0;

            if (frametime != 0)
            {
                frametime -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (frametime < 0)
                {
                    frametime = 0;
                    etat = CactusStateBoss.Attack;
                }
            }

            if ((IsHit) && (attacks != AttackBoss2.Grow))
            {
                int oldWidth = this.dRect.Width;

                Vector2 newScale = this.Scale;
                newScale.X -= 0.003f;
                newScale.Y -= 0.003f;
                this.Scale = newScale;

                this.location.X += (oldWidth - this.dRect.Width) / 2;
                this.location.Y = TGPAContext.Instance.ScreenHeight - this.dRect.Height;
            }

            if (this.Scale.X < 0.35f)
            {
                attacks = AttackBoss2.Grow;
                attackDuration = 10000f;
                this.hitbox = new EmptyHitbox(this);
            }

            switch (etat)
            {
                case CactusStateBoss.Hit:
                    sRect.Y = 512;
                    dRect = ComputeDstRect(sRect);

                    break;

                case CactusStateBoss.Attack:
                    sRect.Y = 0;
                    dRect = ComputeDstRect(sRect);

                    break;
            }

            //Behavior
            //First thing : boss go to the middle of the screen
            if ((location.Y < (TGPAContext.Instance.ScreenHeight - dRect.Height)) && !go)
            {
                go = true;
                Background = true;
                speed = Vector2.Zero;
                Pattern = null;
                this.hitbox = new SquareHitbox(this, new Vector2(0.5f, 0f));
                oldHitbox = (SquareHitbox)this.hitbox;
            }
            else if (!go)
            {
                Vector2 loc = panel.Location;

                loc.Y = this.location.Y;
                if (loc.Y < TGPAContext.Instance.ScreenHeight / 2)
                {
                    loc.Y = TGPAContext.Instance.ScreenHeight / 2;
                }

                this.location.X = 450;
                loc.X += deltaX;

                panel.Location = loc;
                this.location.X += deltaX;
                deltaX = -deltaX;

                for (int i = 1; i <= 15; i++)
                {
                    Vector2 smokeLoc = new Vector2(i * TGPAContext.Instance.ScreenWidth / 15, TGPAContext.Instance.ScreenHeight);

                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(smokeLoc, RandomMachine.GetRandomVector2(-5f, 5f, -10f, 10f),
        0.70f, 0.70f, 0.70f, 1f,
        RandomMachine.GetRandomFloat(0.5f, 2.0f),
        RandomMachine.GetRandomInt(0, 4)), true);
                }

                TGPAContext.Instance.Player1.SetRumble(new Vector2(1.0f, 1.0f));
                if (TGPAContext.Instance.Player2 != null)
                {
                    TGPAContext.Instance.Player2.SetRumble(new Vector2(1.0f, 1.0f));
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Color c = Color.White;

            if (IsHit)
            {
                c = Color.Green *0.5f;
                IsHit = false;
            }

            if (attacks == AttackBoss2.Grow)
            {
                c = Color.Red;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.Sprite, dRect, sRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);
            spriteBatch.End();
        }

        public override List<Shot> Fire()
        {
            List<Shot> shots = new List<Shot>();

            if (!go) return shots;

            if (attackDuration == 0f)
            {

                if (this.attacks == AttackBoss2.Grow)
                {
                    this.hitbox = oldHitbox;
                }

                int rand = RandomMachine.GetRandomInt(0, 4);

                if (rand == 2)
                {
                    attacks = AttackBoss2.Pause;
                    attackDuration = 1000f;
                }
                else if (rand == 3)
                {
                    attacks = AttackBoss2.SmallCactus;
                    attackDuration = 50f;
                }
                else
                {
                    ((ElFeroCactaeWPN1)wpn).SpecialInit();
                    attacks = AttackBoss2.Circluar;
                    attackDuration = 2000f;
                }
            }

            switch (attacks)
            {
                case AttackBoss2.Circluar:
                    shots = wpn.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    break;

                case AttackBoss2.Pause:
                    //shots = wpn2.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    break;

                case AttackBoss2.SmallCactus:
                    shots = wpn3.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    break;

                case AttackBoss2.Grow:

                    int oldWidth = this.dRect.Width;

                    Vector2 newScale = this.Scale;
                    newScale.X += 0.025f;
                    newScale.Y += 0.025f;
                    this.Scale = newScale;

                    this.location.X -= (this.dRect.Width - oldWidth) / 2;
                    this.location.Y = TGPAContext.Instance.ScreenHeight - this.dRect.Height;

                    shots = wpn4.Fire(Vectors.ConvertPointToVector2(DstRect.Center));
                    break;

            }
            return shots;
        }

        public override int HP
        {
            get
            {
                return this.hp;
            }
            set
            {
                this.hp = value;
                this.etat = CactusStateBoss.Hit;
                this.frametime = 1000f;

                if (hp < basicHP / 2)
                {
                    sRect.X = 512;
                }
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/elFeroCactae");
        }
        #endregion

    }

    #region weapon 1

    /// <summary>
    /// Circular attack
    /// </summary>
    public class ElFeroCactaeWPN1 : Weapon
    {
        private double angle;

        public ElFeroCactaeWPN1()
            : base(true)
        {
            cooldown = 350.0f;
            name = "Circular";
            damage = 1;
            ammo = InfiniteAmmo;
            angle = 0f;
        }

        public void SpecialInit()
        {
            angle = (angle > 0) ? 0f : Math.PI / 12;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            for (int i = 0; i < 20; i++)
            {
                newTirs.Add(new ElFeroCactaeWPNShot1(location, this, ((i * Math.PI) / 6) + angle, this.Flip));
            }
            return newTirs;
        }
    }

    #endregion

    /// <summary>
    /// Concentrated attack
    /// </summary>
    public class ElFeroCactaeWPN2 : Weapon
    {
        public ElFeroCactaeWPN2()
            : base(true)
        {
            cooldown = 2000.0f;
            name = "Spine laser";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            float angle = RandomMachine.GetRandomFloat(Math.PI / 2, Math.PI);
            Vector2 shotLoc = location;

            for (int i = 0; i < 16; i++)
            {
                newTirs.Add(new ElFeroCactaeWPNShot1(shotLoc, this, angle, this.Flip));

                if (i % 2 == 0)
                {
                    shotLoc.X += 30;
                }
                else
                {
                    shotLoc.Y += 30;
                }

                if (i % 5 == 0)
                {
                    angle += (float)Math.PI / 8;
                }
            }

            return newTirs;
        }
    }

    /// <summary>
    /// Small cactus
    /// </summary>
    public class ElFeroCactaeWPN3 : Weapon
    {
        public ElFeroCactaeWPN3()
            : base(true)
        {
            cooldown = 500.0f;
            name = "Cactus plant launcher";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new ElFeroCactaeWPNShot2(location, this, RandomMachine.GetRandomFloat(0.0f, (float)(Math.PI)), this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Growing attack
    /// </summary>
    public class ElFeroCactaeWPN4 : Weapon
    {
        public ElFeroCactaeWPN4()
            : base(true)
        {
            cooldown = 0.0f;
            name = "Growing";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            for (int i = 0; i < 5; i++)
            {
                newTirs.Add(new ElFeroCactaeWPNShot1(location, this, RandomMachine.GetRandomFloat(-(3 * Math.PI / 2), (3 * Math.PI / 2)), this.Flip));
            }
            return newTirs;
        }
    }

    /// <summary>
    /// Spine : shot for cactus
    /// </summary>
    public class ElFeroCactaeWPNShot1 : Shot
    {
        public ElFeroCactaeWPNShot1(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(248, 291, 110, 65),    //Sprite
            new Vector2(200, 200),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            this.hitbox = new CircleHitbox(this, true, 3f);
        }

        public override string DeathSound
        {
            get
            {
                return null;
            }
        }
    }

    /// <summary>
    /// Cactus plant
    /// </summary>
    public class ElFeroCactaeWPNShot2 : Shot
    {
        public ElFeroCactaeWPNShot2(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(333, 95, 88, 134),    //Sprite
            new Vector2(350, 350),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            UseRotationWhenDrawing = true;
            this.Scale = Vector2.One;
            this.bounce = true;

            this.Destructable = true;
            this.Points = 500;
            this.Hp = 10;
        }

        public override void Update(GameTime gameTime)
        {
            rotation += 0.002f;

            base.Update(gameTime);
        }

        public override string DeathSound
        {
            get
            {
                return null;
            }
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
