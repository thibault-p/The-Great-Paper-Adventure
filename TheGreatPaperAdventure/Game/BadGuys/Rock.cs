using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Hitbox;
using TGPA.Utils;

namespace TGPA.Game.BadGuys
{
    public class Rock : BadGuy
    {
        private float visualRotation;
        private int icon;
        private float speedYBis;

        public Rock(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               Rectangle.Empty, //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                null
            )
        {
            //Stats
            hp = 3;
            points = 1000;

            this.icon = RandomMachine.GetRandomInt(0, 3);
            if (this.icon == 3) this.icon = 1; //HACK

            sRect = new Rectangle(0 + (128 * icon), 0, 128, 128);
            dRect = ComputeDstRect(sRect);
            speedYBis = 0f;

            if (this.icon != 2)
            {
                visualRotation = 0f;
            }

            UseRotationWhenDrawing = true;
            UseSpriteOrigin();
            this.hitbox = new CircleHitbox(this, true,2f);
            this.Background = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.icon != 2)
            {
                visualRotation += 0.03f;
            }

            speedYBis += 5;
            this.speed.Y = speedYBis;

            float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deplacementY = (speed.Y * tempsEcoule);

            location.Y += deplacementY;

            base.Update(gameTime);

            this.speed.Y = speedYBis;
        }

       public override void Draw(Game.Graphics.TGPASpriteBatch spriteBatch)
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/rock");
        }
        #endregion
    }
}
