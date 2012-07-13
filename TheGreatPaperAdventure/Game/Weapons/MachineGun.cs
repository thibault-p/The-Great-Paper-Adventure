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
    /// Basic weapon : The Machine gun shots small bullet at a very high speed rate
    /// </summary>
    public class MachineGun : Weapon
    {
        public MachineGun()
            : base(false)
        {
            cooldown = 150.0f;
            name = "Machine Gun";

            ammo = InfiniteAmmo;
            damage = 4;
            Rumble = new Vector2(0f, 0.2f);
        }

        public override void UpgradeWeapon()
        {
            if (upgradeLevel < MaxLevel)
            {
                upgradeLevel++;
            }
            if (ammo == InfiniteAmmo)
                ammo = 0;

            if (ammo < this.MaxAmmo * 1.2f)
            {
                ammo += 200;
            }
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? 0 : 1;
            direction.Y = this.Flip == SpriteEffects.FlipVertically ? -1 : 1;

            switch (upgradeLevel)
            {
                default:
                case 3:
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? Math.PI / 4 : 3 * Math.PI / 4, this.Flip));
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? -Math.PI / 4 : -3 * Math.PI / 4, this.Flip));
                    location.Y -= 30;
                    newTirs.Add(new MachineGunShot(location, this, 0.0f, this.Flip));
                    break;
                case 2:
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? Math.PI / 4 : 3 * Math.PI / 4, this.Flip));
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? -Math.PI / 4 : -3 * Math.PI / 4, this.Flip));
                    break;
                case 1:
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    location.Y -= 30;
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    break;

                case 0:
                    newTirs.Add(new MachineGunShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    break;
            }

            //MachinGun has infiniteAmmo only at first level
            if (upgradeLevel > 0)
            {
                ammo -= newTirs.Count;
            }

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
                if (this.Ammo <= 500)
                {
                    return UpgradeLevel == 0 ? this.Ammo : 500;
                }
                else
                {
                    return this.Ammo;
                }
            }
        }

        public override string FiringSound
        {
            get
            {
                return "MachineGunShot";
            }
        }
    }

    public class MachineGunShot : Shot
    {
        public MachineGunShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(340, 0, 100, 80),    //Sprite
            new Vector2(700.0f, 700.0f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, false)
        {
            ttl = InfiniteTimeToLive; //Time to live (ms)
            this.hitbox = new Hitbox.CircleHitbox(this, true, 0.85f);
        }


        public override void TodoOnDeath()
        {
            TGPAContext.Instance.ParticleManager.AddParticle(new Fire(
     Location + RandomMachine.GetRandomVector2(10.0f, 10.0f, -10.0f, 10.0f),
     RandomMachine.GetRandomVector2(-30.0f, 30.0f, -10.0f, -5.0f),
     RandomMachine.GetRandomFloat(0.25f, 0.5f),
     RandomMachine.GetRandomInt(0, 4))); ;
            base.TodoOnDeath();
        }
    }
}
