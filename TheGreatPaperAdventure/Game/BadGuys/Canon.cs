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
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Non moving canon firing on player
    /// </summary>
    public class Canon : BadGuy
    {
        private Rectangle canonSrc;
        private Vector2 canonCenter, structureLinkPoint;
        private float fireAngle = 0.0f;
        private Vector2 firstReactorRelativeLoc, secondReactorRelativeLoc;

        public Canon(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 136, 104), //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                new CanonWeapon()
            )
        {
            this.canonSrc = new Rectangle(135, 0, 121, 78);
            this.canonCenter = new Vector2(80, 42);
            this.structureLinkPoint = new Vector2(84, 38); //Canon center has to be on this point

            //Stats
            this.hp = 150;
            this.Difficulty = 50;
            this.points = 2500;
            this.ttl = Entity.InfiniteTimeToLive;

            this.Background = true;

            this.firstReactorRelativeLoc = new Vector2(41, 98);
            this.secondReactorRelativeLoc = new Vector2(107, 98);

            this.Hitbox = new CircleHitbox(this, true);
            ((CanonWeapon)this.wpn).Parent = this;
            this.UseRotationWhenDrawing = false;
        }

        /// <summary>
        /// Update angle to aim player
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            Vector2 tmplocation = location - structureLinkPoint;

            Vector2 firstReactorLoc = tmplocation + firstReactorRelativeLoc;
            Vector2 secondReactorLoc = tmplocation + secondReactorRelativeLoc;

            int rand = RandomMachine.GetRandomInt(0, 1);

            if ((rand == 0) && (TGPAContext.Instance.Player2 != null))
                fireAngle = Angles.GetAngle(this.Location, Vectors.ConvertPointToVector2(TGPAContext.Instance.Player2.DstRect.Center));
            else
                fireAngle = Angles.GetAngle(this.Location, Vectors.ConvertPointToVector2(TGPAContext.Instance.Player1.DstRect.Center));

            //Jetpacks for plateform
            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(firstReactorLoc, RandomMachine.GetRandomVector2(-4f, 4f, 10f, 40f),
                0.30f, 0.30f, 0.30f, 1f,
                RandomMachine.GetRandomFloat(0.1f, .3f),
                RandomMachine.GetRandomInt(0, 4)), true);

            Fire f = new Fire(firstReactorLoc, RandomMachine.GetRandomVector2(-10f, 10f, 10f, 40f),
                0.25f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(f, true);

            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(secondReactorLoc, RandomMachine.GetRandomVector2(-4f, 4f, 10f, 40f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.1f, .3f),
    RandomMachine.GetRandomInt(0, 4)), true);

            f = new Fire(secondReactorLoc, RandomMachine.GetRandomVector2(-10f, 10f, 10f, 40f),
                0.25f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(f, true);

            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            //Draw canon
            dRect = ComputeDstRect(canonSrc);

            Color c = Color.White;

            if (IsHit)
            {
                c = Color.Red *0.5f;
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.Sprite, dRect, this.canonSrc, c, fireAngle, structureLinkPoint, Flip, 1.0f);

            //Draw structure
            dRect = ComputeDstRect(sRect);
            dRect.X += 26;
            this.DstRect = dRect;

            if (IsHit)
            {
                c = (Color.Red * 0.5f);
                IsHit = false;
            }

            spriteBatch.Draw(this.Sprite, dRect, this.SrcRect, c, 0.0f, structureLinkPoint, Flip, 1.0f);
            spriteBatch.End();
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/canon");
        }

        #endregion

        public float FireAngle
        {
            get { return fireAngle; }
        }

        public Vector2 StructureLinkPoint
        {
            get { return structureLinkPoint; }
        }

    }

    public class CanonWeapon : Weapon
    {
        private Canon parent;

        public CanonWeapon()
            : base(true)
        {
            cooldown = 1300.0f;
            name = "Canon";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            //Erase parameters
            location = parent.Location - (parent.StructureLinkPoint / 2);

            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new CanonWeaponShot(location, this, -parent.FireAngle, this.Flip));

            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            //Erase parameters
            location = parent.Location - (parent.StructureLinkPoint / 2);

            for (int i = 0; i < 5; i++)
            {
                //Smoke and fire
                TGPAContext.Instance.ParticleManager.AddParticle(new Fire(location, RandomMachine.GetRandomVector2(-20f, 20f, 20f, 20f)
                    , RandomMachine.GetRandomFloat(0.5f, 1.5f), RandomMachine.GetRandomInt(0, 4)), false);

                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(location, RandomMachine.GetRandomVector2(-20f, 20f, 20f, 20f),
        0.2f, 0.2f, 0.2f, 1f, RandomMachine.GetRandomFloat(1f, 2.5f), RandomMachine.GetRandomInt(0, 4)), false);

            }
        }

        /// <summary>
        /// The Weapon need its parent reference
        /// </summary>
        public Canon Parent
        {
            set { parent = value; }
        }
    }

    /// <summary>
    /// Basic shots, nothing special
    /// </summary>
    public class CanonWeaponShot : Shot
    {
        public CanonWeaponShot(Vector2 loc, CanonWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(0, 0, 73, 76),    //Sprite
             new Vector2(-650, -650),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        { }
    }

}
