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
using TGPA.Utils;
using Microsoft.Xna.Framework.Graphics;
using TGPA.SpecialEffects;
using TGPA.Game.Hitbox;

namespace TGPA.Game.Weapons
{
    /// <summary>
    /// Fire big and teleguided but slow rockets
    /// </summary>
    public class RocketLauncher : Weapon
    {
        public RocketLauncher()
            : base(false)
        {
            cooldown = 300.0f;
            name = "Rockets";
            damage = 15;
            ammo = 30;
            MaxAmmo = 75;
            Rumble = new Vector2(0.3f, 0.3f);
        }


        public override void UpgradeWeapon()
        {
            if (upgradeLevel < MaxLevel)
            {
                upgradeLevel++;
                damage += 4;
                cooldown -= 20f;
            }
            if (ammo < this.MaxAmmo * 1.2f)
            {
                ammo += 15;
            }
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? 0 : 1;

            switch (upgradeLevel)
            {
                default:
                    newTirs.Add(new RocketShot(location, this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                    break;
            }
            ammo -= newTirs.Count;
            return newTirs;

        }

        public override string FiringSound
        {
            get
            {
                return "RocketLauncherShot";
            }
        }
    }

    public class RocketShot : Shot
    {

        public RocketShot(Vector2 loc, RocketLauncher wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(128, 557, 188, 180),    //Sprite
            new Vector2(100.0f, 100.0f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, false)
        {
            ttl = InfiniteTimeToLive;
            this.hitbox = new CircleHitbox(this, true, 0.7f);
        }


        public override void TodoOnDeath()
        {
            float power = 50.0f * (Weapon.UpgradeLevel);

            power = (power == 0 ? 75f : power);

            TGPAContext.Instance.ParticleManager.MakeCircularExplosion(location, power * 2, (int)power/2);
            TGPAContext.Instance.ParticleManager.MakeExplosion(location, (int)power * 2);
            TGPAContext.Instance.KillEnemiesInArea(wpn,location, (int)power);

            base.TodoOnDeath();
        }

        public override void Update(GameTime gameTime)
        {
            //Speed up !
            speed += Vector2.One * 10;

            if (TGPAContext.Instance.Cheatcodes.IsKawaii)
            {
                //Contrail
                TGPAContext.Instance.ParticleManager.AddParticle(new HeartWave(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(-3f, 7f, -10f, 10f),
        RandomMachine.GetRandomFloat(0.1f, .2f),
        RandomMachine.GetRandomInt(0, 4)), true);
            }
            else
            {
                //Contrail
                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(-3f, 7f, -10f, 10f),
                1.0f, 1.0f, 1.0f, 1f,
        RandomMachine.GetRandomFloat(0.1f, .2f),
        RandomMachine.GetRandomInt(0, 4)), true);
            }

            base.Update(gameTime);
        }

    }
}
