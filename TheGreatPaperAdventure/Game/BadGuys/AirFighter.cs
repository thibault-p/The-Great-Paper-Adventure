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
    /// Little plane firing some canon bullets
    /// </summary>
    public class AirFighter : BadGuy
    {
        public AirFighter(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 750, 400), //Source sprite
                new Vector2(150.0f, 15.0f), //Speed
                new Vector2(0.25f,0.25f),
                new AirFighterWeapon()
            )
        {
            //Stats
            hp = 25;
            speed = new Vector2(350.0f, 350.0f);
            points = 1500;
            Difficulty = 15;
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/airfighter");
        }

        #endregion
    }

    /// <summary>
    /// Plane canon
    /// </summary>
    public class AirFighterWeapon : Weapon
    {
        public AirFighterWeapon()
            : base(true)
        {
            cooldown = 1200.0f;
            name = "Boulay d'Canon";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            Vector2 newLoc = location;
            newLoc.Y += 22;

            newTirs.Add(new AirFighterWeaponShot(newLoc, this, 0.0f, this.Flip));
            newTirs.Add(new AirFighterWeaponShot(newLoc, this, -Math.PI / 4, this.Flip));
            newTirs.Add(new AirFighterWeaponShot(newLoc, this, Math.PI / 4, this.Flip));

            return newTirs;
        }
    }

    /// <summary>
    /// Canon bullets
    /// </summary>
    public class AirFighterWeaponShot : Shot
    {
        protected double frame;
        protected bool smoked;

        public AirFighterWeaponShot(Vector2 loc, AirFighterWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(0, 0, 73, 76),    //Sprite
            new Vector2(200, -200),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        { }

        public override void Update(GameTime gameTime)
        {
            Vector2 loc = Vectors.ConvertPointToVector2(DstRect.Center);

            //Add Smoke and fire
            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(-1f, 1f, -2f, 2f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.01f, .03f),
    RandomMachine.GetRandomInt(0, 4)), true);

            Fire f = new Fire(loc, RandomMachine.GetRandomVector2(-1f, 1f, -2f, 2f),
                0.25f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(f, true);

            base.Update(gameTime);
        }

    }


}
