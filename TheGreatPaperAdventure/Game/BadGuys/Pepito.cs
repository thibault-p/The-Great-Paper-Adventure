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

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Mexican dwarves with jetpacks are extremely dangerous !
    /// </summary>
    public class Pepito : BadGuy
    {
        public Pepito(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 310, 218), //Source sprite
                new Vector2(100.0f, 100.0f), //Speed
                new Vector2(0.35f, 0.35f),
                new PepitoWeapon()
            )
        {
            hp = 12;
            points = 3000;
            Difficulty = 20;
        }

        public override void Update(GameTime gameTime)
        {
            Vector2 jetpackLoc = new Vector2(location.X + 70, location.Y + 60);
            Vector2 direction = (flips == SpriteEffects.FlipHorizontally ? new Vector2(-1, 1) : Vector2.One);

            //Add fire to his jetpack ! Aïe Caramba !
            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(jetpackLoc, RandomMachine.GetRandomVector2(3f, 7f, 10f, 40f) * direction,
                0.30f, 0.30f, 0.30f, 1f,
                RandomMachine.GetRandomFloat(0.1f, .3f),
                RandomMachine.GetRandomInt(0, 4)), true);

            Fire f = new Fire(jetpackLoc, RandomMachine.GetRandomVector2(10f, 10f, 10f, 40f) * direction,
                0.25f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(f, true);

            base.Update(gameTime);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/pepito");
        } 
        #endregion
    }

    /// <summary>
    /// Rocket Launcher that fire 3 bad quality rockets
    /// </summary>
    public class PepitoWeapon : Weapon
    {
        public PepitoWeapon()
            : base(true)
        {
            cooldown = 2500;
            name = "Pepito rocka louncher";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            newTirs.Add(new PepitoWeaponShot(new Vector2(location.X, location.Y), this, 0.0f, this.Flip));

            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            TGPAContext.Instance.ParticleManager.MakeExplosionWithoutQuake(location);
            base.TodoOnFiring(location);
        }

        public override string FiringSound
        {
            get
            {
                return "PepitoWeaponShot";
            }
        }
    }

    public class PepitoWeaponShot : Shot
    {
        public PepitoWeaponShot(Vector2 loc, PepitoWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(76, 387, 173, 132),    //Sprite
            RandomMachine.GetRandomVector2(-250f, -450f, -200f, 200f),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            //this.Destructable = true;
            //this.Points = 200;
            //this.Hp = 5;
        }

        public override void Update(GameTime gameTime)
        {
            //Draw contrail
    //        TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(Vectors.ConvertPointToVector2(DstRect.Center), RandomMachine.GetRandomVector2(-3f, 7f, 0f, 10f),
    //RandomMachine.GetRandomFloat(0.0f, 1.0f), RandomMachine.GetRandomFloat(0.0f, 1.0f), RandomMachine.GetRandomFloat(0.0f, 1.0f), 1f,
    //RandomMachine.GetRandomFloat(0.1f, .2f),
    //RandomMachine.GetRandomInt(0, 4)), true);

            speed.Y = (float)Math.Sin((double)ttl * 13.0f) * 200;

            location += speed * (float)gameTime.ElapsedGameTime.TotalSeconds;

            Vector2 tmploc = location; //Need to save the location, otherwise the rocket will follow the normal path, or we want it to go straight forward
            base.Update(gameTime); //We need to call The Entity.Update() method, but so we call badGuy.Update() too...
            location = tmploc;
        }
    }
}
