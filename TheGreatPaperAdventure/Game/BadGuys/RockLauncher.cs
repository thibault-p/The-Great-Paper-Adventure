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
    public class RockLauncher : BadGuy
    {
        private double cooldown;
        private bool started;

        public RockLauncher(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                Rectangle.Empty, //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                null)
        {
            cooldown = 5000;
            ttl = 40000;
            hitbox = new Hitbox.EmptyHitbox(this);
            hp = Int32.MaxValue;
            started = false;
        }


        public override void Update(GameTime gameTime)
        {
            //cooldown = 0;
            cooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            if (cooldown < 0)
            {
                started = true;
                Launch();
                cooldown = RandomMachine.GetRandomFloat(150, 500);
            }

            if (started == false)
            {
                TGPAContext.Instance.Player1.SetRumble(new Vector2(1.0f, 1.0f));

                if (TGPAContext.Instance.Player2 != null)
                {
                    TGPAContext.Instance.Player2.SetRumble(new Vector2(1.0f, 1.0f));
                }
            }
            base.Update(gameTime);
        }

        private void Launch()
        {
            for (int i = 0; i < RandomMachine.GetRandomInt(1, 4); i++)
            {
                BadGuy rock = new Rock(new Vector2(RandomMachine.GetRandomInt(0, TGPAContext.Instance.ScreenWidth + 1), 0), Vector2.Zero, null, null, SpriteEffects.None, null);
                rock.Location -= new Vector2(0, rock.DstRect.Height);
                
                float f = RandomMachine.GetRandomFloat(0.5f, 2.25f);
                rock.Scale = new Vector2(f, f);
                rock.HP = (int)(rock.HP * f) + 1;

                TGPAContext.Instance.AddEnemy(rock);
            }
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
