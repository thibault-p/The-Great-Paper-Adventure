//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using TGPA.SpecialEffects;
using TGPA.Utils;
using TGPA.Game.Hitbox;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Desert plant firing spines
    /// </summary>
    public class BouleEpine : BadGuy
    {
        public BouleEpine(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(20, 137, 200, 110), //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                new BEWeapon()
            )
        {

            //Stats
            hp = 30;
            Background = true;
            points = 3000;
            Difficulty = 10;

            Hitbox = new CircleHitbox(this, false, 2f);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/bouleEpine");
        }

        #endregion
    }

    /// <summary>
    /// Spine launcher
    /// </summary>
    public class BEWeapon : Weapon
    {
        public BEWeapon()
            : base(true)
        {
            cooldown = 3127.0f;
            name = "Spines";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            for (int i = 0; i < 9; i++)
            {
                newTirs.Add(new BEWeaponShot(location, this, i * (Math.PI / 8), this.Flip));
            }

            return newTirs;
        }

        public override void TodoOnFiring(Vector2 location)
        {
            for (int i = 0; i < 5; i++)
            {
                //White smoke
                TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(location, RandomMachine.GetRandomVector2(-20f, 20f, 20f, 20f),
        1.5f, 1.5f, 1.5f, 1f, RandomMachine.GetRandomFloat(1f, 2.5f), RandomMachine.GetRandomInt(0, 4)), false);
            }
        }
    }

    /// <summary>
    /// Little spines
    /// </summary>
    public class BEWeaponShot : Shot
    {
        public BEWeaponShot(Vector2 loc, BEWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(248, 291, 110, 65),    //Sprite
             new Vector2(250, 250),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        { }
    }
}
