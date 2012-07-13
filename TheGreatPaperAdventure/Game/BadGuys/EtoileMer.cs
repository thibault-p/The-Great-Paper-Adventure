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
using TGPA.Game.Hitbox;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Little starfish
    /// </summary>
    public class EtoileMer : BadGuy
    {
        private float visualRotation;

        public EtoileMer(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                 new Rectangle(0, 0, 256, 256), //Source sprite
                new Vector2(300, 300), //Speed
                new Vector2(0.25f,0.25f),
                null
            )
        {
            hp = 10;
            points = 1500;
            Difficulty = 20;

            this.UseRotationWhenDrawing = true;
            this.visualRotation = 0f;
            this.UseSpriteOrigin();
            this.hitbox = new CircleHitbox(this, true);
        }

        public override void Update(GameTime gameTime)
        {
            this.visualRotation += 0.2f;
            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            double tmpRotation = this.rotation;
            this.rotation = this.visualRotation;
            
            base.Draw(spriteBatch);

            this.rotation = tmpRotation;
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/etoilemer");
        } 
        #endregion
    }
}
