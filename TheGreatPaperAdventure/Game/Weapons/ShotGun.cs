//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using TGPA.Utils;
using Microsoft.Xna.Framework;
using TGPA.SpecialEffects;
using Microsoft.Xna.Framework.Graphics;

namespace TGPA.Game.Weapons
{
    /// <summary>
    /// Improved basic weapon
    /// </summary>
    public class ShotGun : Weapon
    {
        public ShotGun()
            : base(false)
        {
            cooldown = 550.0f;
            name = "Shotgun";

            ammo = 80;
            damage = 15;
            Rumble = new Vector2(0.4f, 0.4f);
        }

        public override void UpgradeWeapon()
        {
            if (upgradeLevel < MaxLevel)
            {
                upgradeLevel++;
                damage += 5;
                cooldown -= 50f;
            }

            if (ammo < this.MaxAmmo * 1.2f)
            {
                ammo += 35;
            }
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? 0 : 1;
            direction.Y = this.Flip == SpriteEffects.FlipVertically ? -1 : 1;

            newTirs.Add(new ShotGunShot(location, this, direction.X > 0 ? Math.PI / 8 : 7 * Math.PI / 8, this.Flip));
            newTirs.Add(new ShotGunShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
            newTirs.Add(new ShotGunShot(location, this, direction.X > 0 ? -Math.PI / 8 : -7 * Math.PI / 8, this.Flip));

            ammo -= newTirs.Count;

            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? 0 : 1;
            direction.Y = this.Flip == SpriteEffects.FlipVertically ? -1 : 1;

            TGPAContext.Instance.ParticleManager.MakeFireFlash(location, RandomMachine.GetRandomVector2(0f, 2f, 0f, 2f), false);

            base.TodoOnFiring(location);
        }


        public override int MaxAmmo
        {
            get
            {
                return 100;
            }
        }

        public override string FiringSound
        {
            get
            {
                return "ShotGunShot";
            }
        }
    }

    public class ShotGunShot : Shot
    {
        public ShotGunShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(340, 0, 100, 80),    //Sprite
            new Vector2(1400.0f, 1400.0f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, false)
        {
            ttl = InfiniteTimeToLive; //Time to live (ms)
            this.hitbox = new Hitbox.CircleHitbox(this, true, 0.85f);
        }
    }
}
