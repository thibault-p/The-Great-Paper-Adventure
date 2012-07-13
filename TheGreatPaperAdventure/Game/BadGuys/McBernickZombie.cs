using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Inspired by 'The pirate family' + zombie Mode
    /// </summary>
    public class McBernickZombie : BadGuy
    {
        public McBernickZombie(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 634, 600), //Source sprite
                new Vector2(75.0f, 15.0f), //Speed
                new Vector2(0.25f, 0.25f),
                null
            )
        {
            //Common stats
            hp = 50;
            points = 10000;
            Difficulty = 6;
            this.ttl = 60000;
            this.hitbox = new CircleHitbox(this, false, 1.75f);
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/mcbernickZombie");
        }

        #endregion

    }
}
