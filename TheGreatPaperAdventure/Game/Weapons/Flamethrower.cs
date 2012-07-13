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
using TGPA.SpecialEffects;
using TGPA.Utils;
using Microsoft.Xna.Framework.Graphics;

namespace TGPA.Game.Weapons
{
    public class Flamethrower : Weapon
    {
        private static double soundCooldown = 20f; //In shots, not in milliseconds, cause weapons are not related to time
        private double currentSoundCooldown;

        public Flamethrower()
            : base(false)
        {
            currentSoundCooldown = soundCooldown + 1;
            cooldown = 2.0f;
            name = "Fl4me Thr0w3r";
            damage = 1;
            ammo = 500;
            MaxAmmo = 1250;
            Rumble = new Vector2(0.2f, 0.2f);
        }

        public override void UpgradeWeapon()
        {
            if (upgradeLevel < MaxLevel)
            {
                upgradeLevel++;
                damage += 2;
            }
            if (ammo < this.MaxAmmo * 1.2f)
            {
                ammo += 500;
            }
            base.UpgradeWeapon();
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? 0 : 1;
            direction.Y = this.Flip == SpriteEffects.FlipVertically ? -1 : 1;

            Vector2 gunLocation = new Vector2(location.X, location.Y);

            switch (upgradeLevel)
            {
                default:
                    newTirs.Add(new FlamethrowerShot(gunLocation, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    break;
            }
            ammo -= newTirs.Count;
            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? 0 : 1;
            direction.Y = this.Flip == SpriteEffects.FlipVertically ? -1 : 1;

            Vector2 gunLocation = new Vector2(location.X, location.Y);
            currentSoundCooldown++;

            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? -1 : 1;

            if (currentSoundCooldown > soundCooldown)
            {
                currentSoundCooldown = 0;
                //Sound
                base.TodoOnFiring(location);
            }
        }

        public override string FiringSound
        {
            get
            {
                return "FlameThrowerShot";
            }
        }
    }

    /// <summary>
    /// Fake shot. No sprite, but hitbox
    /// </summary>
    public class FlamethrowerShot : Shot
    {
        private float fireCooldown;

        public FlamethrowerShot(Vector2 loc, Flamethrower wpn, double angle, SpriteEffects _flip) :
            base(loc,
            Rectangle.Empty,    //Sprite
            new Vector2(1200.0f, 1200.0f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, false)
        {
            ttl = 300;
            dRect = ComputeDstRect(new Rectangle(0, 0, 350, 350));
            fireCooldown = RandomMachine.GetRandomFloat(0f,25f);
        }

        public override void Update(GameTime gameTime)
        {
            fireCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;

            if (fireCooldown <= 0f) //Reduce flame effect for performance issues
            {
                fireCooldown = 25f;

                //Flame effect
                Fire f = new Fire(this.location + RandomMachine.GetRandomVector2(5f, -5.0f, -10.0f, 10.0f),
                                    RandomMachine.GetRandomVector2(-5.0f, 5.0f, -2.0f, 2.0f),
                                    1f,
                                    RandomMachine.GetRandomInt(0, 4));
                f.Additive = false;

                TGPAContext.Instance.ParticleManager.AddParticle(f);
            }
            base.Update(gameTime);
        }
    }
}
