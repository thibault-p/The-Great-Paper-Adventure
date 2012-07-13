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
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Big door, preventing player to enter the death canyon
    /// </summary>
    public class RockDoor : BadGuy
    {
        public bool Dead { get; set; }

        public RockDoor(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(46, 0, 469, 926), //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                null
            )
        {
            this.Dead = false;

            //Stats
            hp = 1000;
            points = 50000;
            Difficulty = 100;

            this.Removable = false;
            this.Background = true;
            this.Hitbox = new SquareHitbox(this, new Vector2(0.5f, 0f));
        }

        public override void Update(GameTime gameTime)
        {
            if (!Dead)
            {
                if (hp < 100)
                {
                    Vector2 newLoc = location;
                    newLoc.X += 70;
                    newLoc.Y += 300;

                    TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(newLoc, RandomMachine.GetRandomVector2(-10f, 10f, 10f, 50f),
    0.60f, 0.60f, 0.60f, 1f,
    RandomMachine.GetRandomFloat(0.1f, .3f),
    RandomMachine.GetRandomInt(0, 4)), true);

                    Fire f = new Fire(newLoc, RandomMachine.GetRandomVector2(-4f, 4f, 10f, 90f),
                        0.25f, RandomMachine.GetRandomInt(0, 4));
                    TGPAContext.Instance.ParticleManager.AddParticle(f, true);
                }
            }
                base.Update(gameTime);
        }

        public override void TodoOnDeath()
        {
            base.TodoOnDeath();

            sRect = new Rectangle(554, 0, 469, 926);
            Dead = true;
            Hitbox = new EmptyHitbox(this);

            TGPAContext.Instance.ParticleManager.MakeExplosion(Vectors.ConvertPointToVector2(DstRect.Center), 30.0f);

        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/rockDoor");
        } 
        #endregion
    }
}
