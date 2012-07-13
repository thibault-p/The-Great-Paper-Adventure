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
using TGPA.SpecialEffects;
using Microsoft.Xna.Framework.Graphics;

namespace TGPA.Game.Weapons
{

    /// <summary>
    /// Best weapon : fire random useless item 
    /// </summary>
    public class LanceTruc : Weapon
    {
        public LanceTruc()
            : base(false)
        {
            cooldown = 700.0f;
            name = "Lance Truc";
            damage = 10;
            ammo = 20;
            MaxAmmo = 30;
            Rumble = new Vector2(0.4f, 0.4f);
        }

        public override void UpgradeWeapon()
        {
            if (upgradeLevel < MaxLevel)
            {
                upgradeLevel++;
                damage += 10;
            }
            if (ammo < this.MaxAmmo * 1.2f)
            {
                ammo += 10;
            }
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? (float)Math.PI : 0.0f;
            Vector2 gunLocation = new Vector2(location.X, location.Y);

            for (int i = 0; i <= upgradeLevel; i++)
            {
                newTirs.Add(new LanceTrucShot(gunLocation, this, direction.X + -(Math.PI / 4) + (i * Math.PI / 8), this.Flip));
            }
            ammo -= newTirs.Count;
            return newTirs;

        }

        public override void TodoOnFiring(Vector2 location)
        {
            Vector2 gunLocation = new Vector2(location.X, location.Y);
            base.TodoOnFiring(gunLocation);

            TGPAContext.Instance.ParticleManager.MakeFireFlash(gunLocation, RandomMachine.GetRandomVector2(-4f, 4f, -4f, 4f), false);
        }

        public override string FiringSound
        {
            get
            {
                return "LanceTrucShot";
            }
        }
    }

    public class LanceTrucShot : Shot
    {
        public LanceTrucShot(Vector2 loc, LanceTruc wpn, double angle, SpriteEffects _flip) :
            base(loc,
            Rectangle.Empty,    //Sprite
            new Vector2(550.0f, 450.0f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, false)
        {
            //Random item between 12 ones (each one = 128*128 sprite)
            int x = RandomMachine.GetRandomInt(0, 768) / 128;
            int y = RandomMachine.GetRandomInt(0, 256) / 128;

            sRect = new Rectangle(128 * x, 742 + (128 * y), 128, 128);
            this.Scale = new Vector2(0.5f, 0.5f);
            bounce = true;
            ttl = 7000f;

            UseRotationWhenDrawing = true;
        }

        public override void Update(GameTime gameTime)
        {
            rotation += RandomMachine.GetRandomFloat(0.005f, 0.025f);
            base.Update(gameTime);

            TGPAContext.Instance.ParticleManager.MakeBullet(location, RandomMachine.GetRandomVector2(location, 2f), true);
        }

    }
}
