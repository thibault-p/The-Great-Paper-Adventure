//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGPA.Game.Hitbox;
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Complex pinguin military weapon 
    /// </summary>
    public class PingouinLauncher : BadGuy
    {
        protected bool firing=false;
        private int bumper_timer=0;
        private Vector2 shotlocation; 
        public PingouinLauncher(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 256, 192), //Source sprite
               Vector2.Zero, //Speed
               Vector2.One,
                null
            )
        {

            //Stats
            hp = 251;
            points = 5000;
            Difficulty = 20;

            this.Background = true;
            shotlocation = new Vector2(105, 180);
            this.ttl = InfiniteTimeToLive;
            
            this.hitbox = new SquareHitbox(this, new Vector2(0.25f, 0.25f));

            base.Weapon = new PingouinLauncherWeapon(this);

            bunker1Rect = new Rectangle(220, 0, 580, 240);
            soliRect = new Rectangle(8, 500, 57, 100);
            bunker2Rect = new Rectangle(220, 240, 580, 240);
            bumper1Rect = new Rectangle(0, 0, 225, 240);
            bumper2Rect = new Rectangle(0, 240, 225, 240);
        }

        public bool Firing{
            set {   firing = value;
                    if (firing==true)  bumper_timer = 0;
            }
            get { return firing; }
        }


        public override List<Shot> Fire()
        {
            //Firing = true;

            base.Weapon.TodoOnFiring(this.location + shotlocation);
            return base.Weapon.Fire(this.location + shotlocation);
            
        }

        #region Sprite
        protected static Texture2D theSprite;
        protected Rectangle bunker1Rect, soliRect, bunker2Rect, bumper1Rect, bumper2Rect;
        protected int offset=0;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/pingouinLauncher");
            //Need to load Pingouin sprite too
            new Pingouin(Vector2.Zero, Vector2.Zero, null, null, SpriteEffects.None, null).LoadContent(cm);
            bunker1Rect = new Rectangle(220,0,580,240);
            soliRect = new Rectangle(8,500,57,100);
            bunker2Rect = new Rectangle(220,240,580,240);
            bumper1Rect = new Rectangle(0,0,225,240);
            bumper2Rect = new Rectangle(0, 240, 225, 240);

        }

        #endregion

        public override void Update(GameTime gameTime)
        {
            this.dRect = bunker2Rect;

            if (firing)
            {
                bumper_timer += gameTime.ElapsedGameTime.Milliseconds;
                offset = 0;
                if (bumper_timer > 800) firing = false;
            }
            else
            {
                offset += gameTime.ElapsedGameTime.Milliseconds;
                offset = Math.Min(80, offset);
            }
            base.Update(gameTime);
            base.FiringLocation = new Vector2(105, 150) +this.location;
        }

        
        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Color c = Color.White;

            if (IsHit)
            {
                c = (Color.Red * 0.75f);
                IsHit = false;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            //draw major
            spriteBatch.Draw(theSprite, new Vector2(dRect.X, dRect.Y), bunker1Rect, c);
            //draw soldier
            for (int i = 0; i < 3; i++ ){   
                spriteBatch.Draw(theSprite, new Vector2(dRect.X + 190+(i*105) - offset, dRect.Y + 115), soliRect, c);
            }
            //draw bunker
            spriteBatch.Draw(theSprite, new Vector2(dRect.X,dRect.Y), bunker2Rect,c);

            //draw bumper
            if (!firing)
            {
                spriteBatch.Draw(theSprite, new Vector2(dRect.X, dRect.Y), bumper1Rect, c);
            }
            else
            {
                spriteBatch.Draw(theSprite, new Vector2(dRect.X, dRect.Y), bumper2Rect, c);
            }

            spriteBatch.End();
        }
    }

    public class PingouinLauncherWeapon : Weapon
    {
        BadGuy owner;
        public PingouinLauncherWeapon(BadGuy o)
            : base(true)
        {
            owner = o;
            cooldown = 1050.0f;
            name = "Lance pingouin !";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            float fireAngle = 0.0f;
            int rand = RandomMachine.GetRandomInt(0, 1);

            if ((rand == 0) && (TGPAContext.Instance.Player2 != null))
                fireAngle = Angles.GetAngle(location, Vectors.ConvertPointToVector2(TGPAContext.Instance.Player2.DstRect.Center));
            else
                fireAngle = Angles.GetAngle(location, Vectors.ConvertPointToVector2(TGPAContext.Instance.Player1.DstRect.Center));

            fireAngle += (float)(Math.PI + RandomMachine.GetRandomFloat(Math.PI/8, -Math.PI/8));

            newTirs.Add(new PingouinLauncherWeaponShot(location, this, -fireAngle, this.Flip));
            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            ((PingouinLauncher)owner).Firing = true;
            //Smoke and fire
            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(location, RandomMachine.GetRandomVector2(-20f, 20f, 20f, 20f),
                0.8f, 0.8f, 0.8f, 1f, RandomMachine.GetRandomFloat(1.5f, 4.5f), RandomMachine.GetRandomInt(0, 4)), false);
        }
    }

    public class PingouinLauncherWeaponShot : Shot
    {

        public PingouinLauncherWeaponShot(Vector2 loc, PingouinLauncherWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(120, 0, 125, 60),    //Sprite
            new Vector2(400, 400),          //Speed
           Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            this.UseRotationWhenDrawing = true;

            this.Scale = new Vector2(1f, 1f);
            this.dRect = ComputeDstRect(this.sRect);

            this.Destructable = true;
            this.Points = 1000;
            this.Hp = 20;
        }


        public override void Update(GameTime gameTime)
        {
            speed += new Vector2(10, 10);
            this.rotation += RandomMachine.GetRandomFloat(0.001f, 0.01f);
            base.Update(gameTime);
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