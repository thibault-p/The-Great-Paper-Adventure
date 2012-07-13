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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Utils;
using TGPA.Audio;

namespace TGPA.Game.Entities
{
    /// <summary>
    /// Subclasses for bosses
    /// </summary>
    public class Boss : BadGuy
    {
        protected int maxLifebarValue;

        public bool DrawLifebar {get; set;}
        public bool DrawWarning { get; set; }

        public Boss(Vector2 scrollValue) :
            base(new Vector2(TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight / 3), scrollValue, null, null, null, SpriteEffects.None, Rectangle.Empty, Vector2.Zero, Vector2.One, null)
        {
            this.maxLifebarValue = 0;
            this.DrawLifebar = false;
            this.DrawWarning = false;
        }

        public override void Update(GameTime gameTime)
        {
            TGPAContext.Instance.Hud.DrawBossLifebar = this.DrawLifebar;
            TGPAContext.Instance.Hud.DrawWarning = this.DrawWarning;
            TGPAContext.Instance.Hud.BossLifePurcent = this.CurrentLifeBarValue;

            base.Update(gameTime);
        }

        public override void TodoOnDeath()
        {
            for (int i = 0; i < 12; i++)
            {
                Vector2 boomLoc = RandomMachine.GetRandomVector2(dRect.Left, dRect.Right, dRect.Top, dRect.Bottom);

                TGPAContext.Instance.ParticleManager.MakeExplosion(boomLoc, 150f);
            }

            SoundEngine.Instance.PlaySound("bigExplosion");

            base.TodoOnDeath(); //Sounds and flags
        }

        public virtual double CurrentLifeBarValue
        {
            get
            {
                return (double)HP / (double)maxLifebarValue;
            }       
        }
    }
}
