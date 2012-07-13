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
using TGPA.SpecialEffects;
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Pirate ship
    /// </summary>
    public class Pirate : BadGuy
    {
        public Pirate(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 520, 390), //Source sprite
                new Vector2(100.0f, 100.0f), //Speed
                Vector2.One,
                new PirateWeapon()
            )
        {
            //Stats
            hp = 50;
            points = 1000;
            Difficulty = 100;
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/pirate");
        } 
        #endregion
    }

    /// <summary>
    /// No weapon for pirate : only kamikaze for sabordage ?
    /// </summary>
    public class PirateWeapon : Weapon
    {
        public PirateWeapon()
            : base(true)
        {
            cooldown = 00.0f;
            name = "Sabordage";
            damage = 0;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            return newTirs;
        }
    }
}
