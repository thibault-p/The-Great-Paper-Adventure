//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGPA.Game.Hitbox;
using TGPA.Utils;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// A pinguin. What else ? DO nothing, it's just a pinguin.
    /// </summary>
    public class Pingouin : BadGuy
    {
        private float visualRotation;

        public Pingouin(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 77, 119), //Source sprite
               new Vector2(300, 300), //Speed
               Vector2.One,
                null
            )
        {
            //Stats
            hp = 9;
            points = 2000;
            Difficulty = 2;
            visualRotation = RandomMachine.GetRandomFloat(0.0f,Math.PI);

            this.UseRotationWhenDrawing = true;
            this.visualRotation = 0f;
            this.UseSpriteOrigin();
            this.hitbox = new CircleHitbox(this,true,1.2f);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/pingouin");
        }
        #endregion

        public override void Update(GameTime gameTime)
        {
            this.visualRotation += 0.15f;
            base.Update(gameTime);
        }

       public override void Draw(Game.Graphics.TGPASpriteBatch spriteBatch)
        {
            double tmpRotation = this.rotation;
            this.rotation = this.visualRotation;

            base.Draw(spriteBatch);

            this.rotation = tmpRotation;
        }
    }
}