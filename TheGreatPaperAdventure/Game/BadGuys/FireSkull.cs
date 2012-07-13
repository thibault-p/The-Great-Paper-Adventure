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
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// 
    /// </summary>
    public class FireSkull : BadGuy
    {
        public FireSkull(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 256, 256), //Source sprite
               Vector2.Zero, //Speed
               new Vector2(0.65f,0.65f),
                new FireballWeapon()
            )
        {

            //Stats
            this.ttl = 20000f;
            hp = 60;
            points = 2500;
            Difficulty = 25;
            UseAnimation = true; 
            this.totalFrameNumber = 2;
            this.frameCooldown = 150;
            this.spriteBox = new Vector2(256, 256);
            this.Background = true;

            this.hitbox = new PositionedCircleHitbox(this, new Vector2(this.dRect.Width / 2, 2 * this.dRect.Height / 3), this.dRect.Width / 3, false, 1f);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/fireSkull");
        }
        #endregion

    }

    public class FireballWeapon : Weapon
    {
        private double fireAngle;

        public FireballWeapon()
            : base(true)
        {
            cooldown = 750.0f;
            name = "Fireball";
            damage = 1;
            ammo = InfiniteAmmo;

            fireAngle = Math.PI;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            newTirs.Add(new FireballShot(location, this, fireAngle , this.Flip));

            fireAngle += Math.PI / 8;

            return newTirs;
        }
    }

    public class FireballShot : Shot
    {
        public FireballShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(665, 1017, 128, 75),    //Sprite
            new Vector2(250, 250),          //Speed
           new Vector2(0.25f, 0.25f),                    //Scale
            _flip, wpn, angle, true)
        {            
            UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 150;
            this.spriteBox = new Vector2(0, 74);

            this.Scale = new Vector2(0.4f, 0.4f);
        }
        
        public override string DeathSound
        {
            get
            {
                return null;
            }
        }
    }

}