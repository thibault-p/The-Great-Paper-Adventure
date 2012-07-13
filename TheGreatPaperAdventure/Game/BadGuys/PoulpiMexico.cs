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
    /// <summary>
    /// Basic enemy : few life, no power, no danger + Sombrero
    /// </summary>
    public class PoulpiMexico : Poulpi
    {
        public PoulpiMexico(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flip,flags)
        {
            this.sRect = new Rectangle(0, 0, 320, 320);
            this.dRect = ComputeDstRect(sRect);
            this.spriteBox = new Vector2(320, 320);
        }

        #region Sprite
        protected static Texture2D theMexicanSprite;

        public override Texture2D Sprite
        {
            get { return theMexicanSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theMexicanSprite = cm.Load<Texture2D>("gfx/Sprites/poulpiMexico");
        }
        #endregion
    }
}
