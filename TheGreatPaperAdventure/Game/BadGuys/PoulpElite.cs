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

namespace TGPA.Game.BadGuys
{
    public class PoulpElite : Poulpi
    {
        public PoulpElite(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flip, flags)
        {
            this.sRect = new Rectangle(0, 0, 327, 251);
            this.dRect = ComputeDstRect(sRect);

            this.UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 200;
            this.spriteBox = new Vector2(327, 251);

            this.speed.Y = 250;
            this.speed.Y = 250;
            this.wpn = null;

            this.hp *= 3;

        }

        #region Sprite
        protected static Texture2D eliteSprite;

        public override Texture2D Sprite
        {
            get { return eliteSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            eliteSprite = cm.Load<Texture2D>("gfx/Sprites/poulpElite");
        }
        #endregion
    }
}
