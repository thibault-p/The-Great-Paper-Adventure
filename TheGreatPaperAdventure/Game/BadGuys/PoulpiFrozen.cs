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
    /// Cold iced poulpi. Free a PoulpiSnow on death
    /// </summary>
    public class PoulpiFrozen : BadGuy
    {
        public PoulpiFrozen(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 198 ,170), //Source sprite
               Vector2.Zero, //Speed
               new Vector2(0.75f,0.75f),
                null
            )
        {
            //Stats
            hp = 30;
            points = 1500;
            Difficulty = 3;
            this.Background = true;
            this.hitbox = new SquareHitbox(this, new Vector2(0.2f, 0.1f));
        }

        public override void TodoOnDeath()
        {
            //Free a PoulpiSnow
            MovePattern mp = new MovePattern();

            switch (this.Flip)
            {
                case SpriteEffects.FlipHorizontally:
                    mp.AddPoint((int)this.location.X, (int)TGPAContext.Instance.ScreenHeight / 2);
                    mp.AddPoint((int)TGPAContext.Instance.ScreenWidth + 1, (int)TGPAContext.Instance.ScreenHeight / 2);
                    break;

                case SpriteEffects.None:
                    mp.AddPoint((int)this.location.X, (int)TGPAContext.Instance.ScreenHeight / 2);
                    mp.AddPoint(-500, (int)TGPAContext.Instance.ScreenHeight / 2);
                    break;
            }

            Snow s = new Snow(this.location, RandomMachine.GetRandomVector2(-100f, 20f, 50f, -300f), 800f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(s, false);

            PoulpiSnow ps = new PoulpiSnow(this.location, this.scrollValue, null, mp, this.Flip, this.flagsOnDeath);
            TGPAContext.Instance.AddEnemy(ps);

            //base.TodoOnDeath();
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/poulpiFrozen");
            new PoulpiSnow(Vector2.Zero, Vector2.Zero, null, null, SpriteEffects.None, null).LoadContent(cm);
        }
        #endregion
    }
}
