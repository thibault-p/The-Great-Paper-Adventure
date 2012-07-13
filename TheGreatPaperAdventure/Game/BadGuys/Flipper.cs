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
using TGPA.SpecialEffects;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Laser dolphin
    /// </summary>
    public class Flipper : BadGuy
    {
        private bool firing;
        private double cooldown;

        public Flipper(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 256, 256), //Source sprite
                new Vector2(100, 0), //Speed
                new Vector2(0.75f, 0.75f),
                new FlipperWeapon()
            )
        {
            hp = 41;
            points = 5000;
            Difficulty = 20;

            this.firing = false;

            this.UseAnimation = false; //Change it later
            this.totalFrameNumber = 2;
            this.frameCooldown = 100;
            this.spriteBox = new Vector2(256, 256);

            this.hitbox = new CircleHitbox(this, false, 1.85f);

            //Special property
            ((FlipperWeapon)this.Weapon).Parent = this;
        }

        public override List<Shot> Fire()
        {
            if (firing)
            {
                return base.Fire();
            }
            else
            {
                return new List<Shot>();
            }
        }

        public override void Update(GameTime gameTime)
        {
            cooldown += gameTime.ElapsedGameTime.TotalMilliseconds;

            if ((cooldown > 1500) && (!firing))
            {
                firing = true;
                this.Weapon.Cooldown = 20f;

                sRect.Y = 256;
                UseAnimation = true;
            }
            else if ((cooldown > 5000) && (firing))
            {
                firing = false;
                this.Weapon.Cooldown = 100000f;

                sRect.X = 0;
                sRect.Y = 0;
                UseAnimation = false;
                cooldown = 0f;
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/flipper");
        }
        #endregion
    }

    /// <summary>
    /// Huge diag laser
    /// </summary>
    public class FlipperWeapon : Weapon
    {
        private Flipper parent;

        public FlipperWeapon()
            : base(true)
        {
            cooldown = 100000;
            name = "Flipper diag laser";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            if (parent.Flip == SpriteEffects.None)
            {

                Vector2 loc = new Vector2(
                    location.X - (this.parent.DstRect.Height / 6),
                    location.Y
                    );

                newTirs.Add(new FlipperWeaponShot(loc, this, (3 * Math.PI) / 4, this.Flip));
            }
            else if (parent.Flip == SpriteEffects.FlipHorizontally)
            {
                Vector2 loc = new Vector2(
                    location.X + (this.parent.DstRect.Height / 6),
                    location.Y
                    );

                newTirs.Add(new FlipperWeaponShot(loc, this, (Math.PI) / 4, this.Flip));
            }

            return newTirs;
        }

        /// <summary>
        /// The Weapon need its parent reference
        /// </summary>
        public Flipper Parent
        {
            set { parent = value; }
        }
    }

    /// <summary>
    /// Laser shot
    /// </summary>
    public class FlipperWeaponShot : Shot
    {
        private Color color;

        public FlipperWeaponShot(Vector2 loc, FlipperWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(455, 10, 225, 70),    //Sprite
            new Vector2(1000, 1000),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            this.hitbox = new CircleHitbox(this, true, 3f);

            this.additive = false;
            this.color = Color.White *0.7f;
        }

        public override void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            dRect.X = (int)location.X;
            dRect.Y = (int)location.Y;

            spriteBatch.Draw(texture, dRect, sRect, color, -(float)rotation, spriteOrigin, flips, 1.0f);
        }
    }
}