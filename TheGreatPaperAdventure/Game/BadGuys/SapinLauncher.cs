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
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys

{
    /// <summary>
    /// Random christmas tree creator. Only stops creation when the flag "stopSapin" is set
    /// </summary>
    public class SapinLauncher: BadGuy
    {
        private double cooldown;
        

        public SapinLauncher(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                Rectangle.Empty, //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                null)
        {
            cooldown = 1500;
            ttl = InfiniteTimeToLive;
            hitbox = new Hitbox.EmptyHitbox(this);
            hp = Int32.MaxValue;
        }


        public override void Update(GameTime gameTime)
        {
            if (!TGPAContext.Instance.Map.Flags.GetFlag("stopSapin"))
            {
                cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                if (cooldown < 0)
                {
                    Launch();
                    cooldown = RandomMachine.GetRandomFloat(200, 1750);
                }
            }
            else
            {
                this.hp = -1;
                this.ttl = 0f;
            }
            base.Update(gameTime);
        }

        private void Launch()
        {
            BadGuy sapin = null;

            if (RandomMachine.GetRandomFloat(0, 1) > 0.79f)
            {
                sapin = new TGPA.Game.Entities.BackgroundActiveElement.Sapin2(new Vector2(TGPAContext.Instance.ScreenWidth + 1, TGPAContext.Instance.ScreenHeight - 31), RandomMachine.GetRandomFloat(0.5f, 1f));
            }
            else
            {
                sapin = new TGPA.Game.Entities.BackgroundActiveElement.Sapin(new Vector2(TGPAContext.Instance.ScreenWidth + 1, TGPAContext.Instance.ScreenHeight - 31), RandomMachine.GetRandomFloat(0.5f, 1f));
            }

            Vector2 loc = sapin.Location;

            loc.Y -= sapin.DstRect.Height;

            sapin.Location = loc;

            TGPAContext.Instance.AddEnemy(sapin);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            //No sprite
        }


        #region Sprite

        public override void LoadContent(ContentManager cm)
        {
          
        }

        #endregion
    }
}
