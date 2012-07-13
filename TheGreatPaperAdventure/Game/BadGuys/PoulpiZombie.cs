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
using TGPA.Audio;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Basic enemy : few life, no power, no danger + Zombie mode
    /// </summary>
    public class PoulpiZombie : BadGuy
    {
        public PoulpiZombie(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 322, 251), //Source sprite
                new Vector2(35.0f, 35.0f), //Speed
                new Vector2(0.25f, 0.25f),
                null //No weapon, it's a zombie
            )
        {
            //Stats
            hp = 12;
            points = 200;
            Difficulty = 10;

            this.ttl = 60000;

            UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 500;
            this.spriteBox = new Vector2(320, 256);
            this.currentFrame = RandomMachine.GetRandomInt(0, 1);
            this.hitbox = new CircleHitbox(this, false, 2f);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/poulpiZombie");
        }
        #endregion

        public override void TodoOnDeath()
        {
            TGPAContext.Instance.Saver.SaveData.KilledPoulpis += 1;

            base.TodoOnDeath();
        }
    }
}
