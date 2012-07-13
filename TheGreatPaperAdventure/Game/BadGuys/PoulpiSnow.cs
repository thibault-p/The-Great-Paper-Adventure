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
using TGPA.SpecialEffects;
using TGPA.Utils;
using TGPA.Audio;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Basic enemy : few life, no power, no danger + Christmas hat !
    /// </summary>
    public class PoulpiSnow : Poulpi
    {
        public PoulpiSnow(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flip, flags)
        {
            this.sRect = new Rectangle(0, 0, 350, 300);
            this.spriteBox = new Vector2(350, 300);
            this.hp *= 2;
            this.Difficulty *= 2;
            this.Scale = new Vector2(0.3f, 0.3f);
            this.Weapon = new PoulpiSnowWeapon();
        }

        #region Sprite
        protected static Texture2D theSnowSprite;

        public override Texture2D Sprite
        {
            get { return theSnowSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSnowSprite = cm.Load<Texture2D>("gfx/Sprites/poulpiSnow");
        }
        #endregion
    }

    /// <summary>
    /// Basic enemy weapon
    /// </summary>
    public class PoulpiSnowWeapon : Weapon
    {
        public PoulpiSnowWeapon()
            : base(true)
        {
            cooldown = 4000.0f;
            name = "PoulpiSnowWeapon";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new PoulpiSnowWeaponShot(location, this, 3*Math.PI / 4f, this.Flip));
            newTirs.Add(new PoulpiSnowWeaponShot(location, this, Math.PI, this.Flip));
            newTirs.Add(new PoulpiSnowWeaponShot(location, this, 5*Math.PI / 4f, this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Snow balls
    /// </summary>
    public class PoulpiSnowWeaponShot : Shot
    {
        private float visualRotation;

        public PoulpiSnowWeaponShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(112, 268, 105, 90),    //Sprite
            new Vector2(250, 250),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            if (RandomMachine.GetRandomFloat(0, 1) > 0.59f)
            {
                this.sRect = new Rectangle(768, 895, 107, 105);
            }
            else
            {
                this.sRect = new Rectangle(893, 891, 124, 116);
            }

            //Hack ?
            this.hitbox = new CircleHitbox(this, true, 2f);
        }

        public override void Update(GameTime gameTime)
        {
            this.visualRotation += 0.15f;
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
