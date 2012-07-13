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
    /// Basic enemy : few life, no power, no danger
    /// </summary>
    public class Poulpi : BadGuy
    {
        public Poulpi(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 322, 251), //Source sprite
                new Vector2(150.0f, 150.0f), //Speed
                new Vector2(0.25f, 0.25f),
                new PoulpiWeapon()
            )
        {
            //Stats
            hp = 3;
            points = 1000;
            Difficulty = 3;

            UseAnimation = true;
            this.totalFrameNumber = 4;
            this.frameCooldown = 150;
            this.spriteBox = new Vector2(320, 256);
            this.hitbox = new CircleHitbox(this, false, 2f);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/poulpi");
        }
        #endregion

        public override void TodoOnDeath()
        {
            TGPAContext.Instance.Saver.SaveData.KilledPoulpis += 1;

            base.TodoOnDeath();
        }

        public override string DeathSound
        {
            get
            {
                return "PoulpiKill";
            }
        }
    }

    /// <summary>
    /// Basic enemy weapon
    /// </summary>
    public class PoulpiWeapon : Weapon
    {
        public PoulpiWeapon()
            : base(true)
        {
            cooldown = 2000.0f;
            name = "Bubulles";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            if (this.Flip == SpriteEffects.FlipHorizontally)
            {
                newTirs.Add(new PoulpiWeaponShot(location, this, Math.PI, this.Flip));
                newTirs.Add(new PoulpiWeaponShot(location, this, Math.PI / 4f, this.Flip));
            }
            else
            {
                newTirs.Add(new PoulpiWeaponShot(location, this, Math.PI, this.Flip));
                newTirs.Add(new PoulpiWeaponShot(location, this, (3 * Math.PI) / 4f, this.Flip));
            }
            return newTirs;
        }

        public override string FiringSound
        {
            get
            {
                return "PoulpiWeaponShot";
            }
        }
    }

    /// <summary>
    /// Basic shots, nothing special
    /// </summary>
    public class PoulpiWeaponShot : Shot
    {
        public PoulpiWeaponShot(Vector2 loc, PoulpiWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(112, 268, 105, 90),    //Sprite
            new Vector2(250, 250),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        { }
    }

}
