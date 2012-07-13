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

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Basic enemy : few life, no power, no danger + Ghoulsn'n ghost armor
    /// </summary>
    public class PoulpiKnight : Poulpi
    {
        public PoulpiKnight(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flip, flags)
        {
            this.sRect = new Rectangle(0, 0, 350, 320);
            this.dRect = ComputeDstRect(sRect);
            this.spriteBox = new Vector2(350, 320);

            this.speed.Y = 250;
            this.wpn = new PoulpiKnightWeapon();

            this.hp *= 3;

        }

        #region Sprite
        protected static Texture2D theGnGSprite;

        public override Texture2D Sprite
        {
            get { return theGnGSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theGnGSprite = cm.Load<Texture2D>("gfx/Sprites/poulpiKnight");
        }
        #endregion
    }

    /// <summary>
    /// 
    /// </summary>
    public class PoulpiKnightWeapon : PoulpiWeapon
    {
        private int pattern;

        public PoulpiKnightWeapon()
            : base()
        {
            cooldown = 750.0f;
            name = "PoulpiKnightWeapon";
            damage = 1;
            ammo = InfiniteAmmo;

            pattern = 0;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            pattern++;
            if (pattern > 3)
                pattern = 0;

            switch (pattern)
            {
                case 0:
                    newTirs.Add(new PoulpiWeaponShot(location, this, -Math.PI/4, this.Flip));
                    break;

                case 1:
                    newTirs.Add(new PoulpiWeaponShot(location, this, Math.PI, this.Flip));
                    break;

                case 2:
                    newTirs.Add(new PoulpiWeaponShot(location, this, 0.0f, this.Flip));
                    break;

                case 3:
                    newTirs.Add(new PoulpiWeaponShot(location, this, 3*Math.PI / 4, this.Flip));
                    break;
            }

            return newTirs;
        }
    }
}
