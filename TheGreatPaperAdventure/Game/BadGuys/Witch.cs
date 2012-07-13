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
using TGPA.Game.Hitbox;

namespace TGPA.Game.BadGuys.Boss
{
    public class Witch : BadGuy
    {
        public Witch(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
               new Rectangle(0, 0, 256, 256), //Source sprite
               new Vector2(200, 100), //Speed
              Vector2.One,
                new BlackCatLauncher()
            )
        {

            //Stats
            hp = 30;
            points = 1750;
            Difficulty = 15;

            this.hitbox = new Game.Hitbox.SquareHitbox(this, new Vector2(0.6f, 0.4f));
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/witch");
        }
        #endregion
    }

    /// <summary>
    /// Basic enemy weapon
    /// </summary>
    public class BlackCatLauncher : Weapon
    {
        public BlackCatLauncher()
            : base(true)
        {
            cooldown = 1300.0f;
            name = "BlackCatLauncher";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            BlackCatShot cat = new BlackCatShot(location, this, 0.0f, this.Flip);

            newTirs.Add(cat);

            return newTirs;
        }
    }

    /// <summary>
    /// Snow balls
    /// </summary>
    public class BlackCatShot : Shot
    {
        private float alphaColor;

        public BlackCatShot(Vector2 loc, Weapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(512, 990, 127, 157),    //Sprite
            new Vector2(150, 150),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            if (this.flips == SpriteEffects.FlipHorizontally)
            {
                this.speed.X = -this.speed.X;
            }

            this.Scale = new Vector2(0.75f, 0.75f);
            this.alphaColor = 1f;
        }

        public override void Update(GameTime gameTime)
        {
            this.rotation += 0.025f;

            base.Update(gameTime);

            if (this.ttl < 1000f)
            {
                this.hitbox = new EmptyHitbox(this);
                this.alphaColor -= 0.025f;

                this.Scale -= new Vector2(0.025f, 0.025f);
                if (this.Scale.X < Vector2.Zero.X) this.Scale = Vector2.Zero;

            }
            else
            {
                TGPAContext.Instance.ParticleManager.MakeBullet(location, RandomMachine.GetRandomVector2(location, 2f), true);
            }
        }

       public override void Draw(Game.Graphics.TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            Color c = Color.White;

            if (IsHit)
            {
                c = Color.Red;
                this.IsHit = false;
            }

            spriteBatch.Draw(texture, dRect, sRect,c * alphaColor, -(float)rotation, spriteOrigin, this.flips, 1.0f);
        }

        public override bool DrawBehindShot
        {
            get
            {
                return false;
            }
        }
    }
}
