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
using TGPA.SpecialEffects;
using TGPA.Utils;
using Microsoft.Xna.Framework.Content;
using TGPA.Audio;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Little bird chasing a player -the nearest between p1 and p2)
    /// </summary>
    public class Bird : BadGuy
    {
        private bool targetFound;

        public Bird(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 256, 128), //Source sprite
                new Vector2(350, 350.0f), //Speed
                new Vector2(0.3f, 0.3f),
                null
            )
        {

            //Stats
            hp = 3;

            points = 1000;
            Difficulty = 5;
            targetFound = false;
            this.UseSpriteOrigin();
            UseRotationWhenDrawing = flip == SpriteEffects.None;
            Hitbox = new CircleHitbox(this, true, 3f);
        }

        public override void Update(GameTime gameTime)
        {
            if (!targetFound)
            {
                Pattern = null;

                //Chasing the player : build a move pattern on him
                MovePattern mp = new MovePattern();

                int rand = RandomMachine.GetRandomInt(0, 1);

                Player p = TGPAContext.Instance.Player1;
                if ((rand == 0) && (TGPAContext.Instance.Player2 != null))
                    p = TGPAContext.Instance.Player2;

                mp.AddPoint(p.DstRect.Center);

                if (location.X > (int)p.Location.X)
                {
                    mp.AddPoint(new Point(-50, (int)p.Location.Y));
                }
                else
                {
                    mp.AddPoint(new Point(TGPAContext.Instance.ScreenWidth + 100, (int)p.Location.Y));
                }
                Pattern = mp;

                targetFound = true;
            }

            base.Update(gameTime);
        }

        #region Sprite

        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/bird");
        }

        #endregion

        public override bool IsOnScreen
        {
            get { return base.IsOnScreen; }
            set
            {
                if (value)
                {
                    //Play sound when bird appear on screen
                    SoundEngine.Instance.PlaySound("specialBird");
                }
                base.IsOnScreen = value;
            }
        }
    }
}
