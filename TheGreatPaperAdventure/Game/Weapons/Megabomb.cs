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
using TGPA.Game.Hitbox;

namespace TGPA.Game.Weapons
{
    /// <summary>
    /// Huge nuclear bomb to blast everything on the screen
    /// </summary>
    public class Megabomb : Weapon
    {
        public Megabomb()
            : base(false)
        {
            cooldown = 2500.0f;
            name = "Megabomb";
            damage = 70;
            ammo = 2;
            Rumble = new Vector2(0.3f, 0f);
        }

        public override void UpgradeWeapon()
        {
            ammo++;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            Vector2 direction = Vector2.One;
            direction.X = this.Flip == SpriteEffects.FlipHorizontally ? -1 : 1;

            if (ammo > 0)
            {
                newTirs.Add(new MegabombShot(new Vector2(location.X, location.Y), this, direction.X > 0 ? 0.0f : Math.PI, this.Flip));
                ammo -= newTirs.Count;
            }
            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            Vector2 loc = location;
            loc.X += 50;
            TGPAContext.Instance.ParticleManager.MakeCircularExplosion(loc, 75f, 16);
            base.TodoOnFiring(location);
        }

        public override string FiringSound
        {
            get
            {
                return "MegabombShot";
            }
        }
    }
    public class MegabombShot : Shot
    {
        private double saveRotation = 0.0f;

        public MegabombShot(Vector2 loc, Megabomb wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(830, 0, 180, 236),    //Sprite
            new Vector2(450, 350),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, false)
        {
            ttl = InfiniteTimeToLive;
            saveRotation = angle;
            UseRotationWhenDrawing = true;

            hitbox = new CircleHitbox(this,true,0.5f);
        }

        public override void TodoOnDeath()
        {
            TGPAContext.Instance.ParticleManager.MakeCircularExplosion(location, 750.0f, 100);
            TGPAContext.Instance.ParticleManager.MakeExplosion(location, 100);
            base.TodoOnDeath();
        }

        public override void Update(GameTime gameTime)
        {
            double tmp = rotation;
            rotation = saveRotation;

            base.Update(gameTime);

            rotation = tmp;
            rotation += 0.10f;

            //Fire and smoke
            Vector2 loc = Vectors.ConvertPointToVector2(DstRect.Center);

            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(3f, 5f, -5f, 5f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.01f, .03f),
    RandomMachine.GetRandomInt(0, 4)), true);

            Fire f = new Fire(loc, RandomMachine.GetRandomVector2(3f, 5f, -5f, 5f),
                0.25f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(f, true);
        }
    }

}
