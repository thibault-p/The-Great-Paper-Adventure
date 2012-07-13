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
    /// Flying insect dropping small bombs
    /// </summary>
    public class Moustik : BadGuy
    {
        public Moustik(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(23, 0, 204, 256), //Source sprite
                new Vector2(220.0f, 220.0f), //Speed
                new Vector2(0.25f, 0.25f),
                new MoustikWeapon()
            )
        {
            //Stats
            hp = 3;
            Difficulty = 30;
            points = 1000;

            UseAnimation = true;
            this.totalFrameNumber = 2;
            this.frameCooldown = 250;
            this.spriteBox = new Vector2(256, 256);

            this.hitbox = new CircleHitbox(this, true, 1.5f);

            UseSpriteOrigin();
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            Rectangle dstBehind = dRect;
            dstBehind.Width = (int)((float)dstBehind.Width * 1.4f);
            dstBehind.Height = (int)((float)dstBehind.Height * 1.4f);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.Sprite, dstBehind, this.SrcRect, Color.SeaShell *0.8f, 0.0f, this.spriteOrigin, Flip, 1.0f);
            spriteBatch.End();

            base.Draw(spriteBatch);
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/moustik");
        }
        #endregion
    }

    public class MoustikWeapon : Weapon
    {
        public MoustikWeapon()
            : base(true)
        {
            cooldown = 800.0f;
            name = "Small bombs";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();
            location.Y -= 20;
            newTirs.Add(new MoustikWeaponShot(location, this, 0.0f, this.Flip));
            return newTirs;
        }
    }

    public class MoustikWeaponShot : Shot
    {
        private static Color moustikShotColor = Color.Black;
        private static Color moustikBlinkShotColor = Color.Tomato *0.75f;

        private int nextBlink;

        public MoustikWeaponShot(Vector2 loc, MoustikWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(340, 408, 105, 100),    //Sprite
            Vector2.Zero,          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        {
            nextBlink = 1000;

            this.Destructable = true;
            this.Points = 200;
            this.Hp = 10;
            Scale = new Vector2(0.35f, 0.35f); //Hack
        }

        public override void TodoOnDeath()
        {
            TGPAContext.Instance.ParticleManager.MakeExplosion(location, 5f);
            base.TodoOnDeath();
        }

        public override void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            this.behindColor = moustikShotColor;

            Color c = Color.White;

            if ((5000 - ttl) > nextBlink)
            {
                switch (nextBlink)
                {
                    case 1000:
                        nextBlink = 2000;
                        break;

                    case 2000:
                        nextBlink = 3000;
                        break;

                    case 3000:
                        nextBlink = 3500;
                        break;

                    case 4000:
                        nextBlink = 4200;
                        break;

                    case 4200:
                        nextBlink = 4300;
                        break;
                    case 4400:
                        nextBlink = 4600;
                        break;

                    case 4600:
                        nextBlink = 4800;
                        break;

                    default:
                        nextBlink = 5000;
                        break;
                }

                c = Color.Red;
                this.behindColor = moustikBlinkShotColor;
            }

            if (IsHit)
            {
                c = Color.Red;
                this.IsHit = false;
            }

            Rectangle dstShotBehind = dRect;
            dstShotBehind.Width = (int)((float)dstShotBehind.Width * 1.35f);
            dstShotBehind.Height = (int)((float)dstShotBehind.Height * 1.35f);

            spriteBatch.Draw(texture, dstShotBehind, sRect, this.behindColor, -(float)rotation, spriteOrigin, this.flips, 1.0f);
            spriteBatch.Draw(texture, dRect, sRect, c, -(float)rotation, spriteOrigin, flips, 1.0f);
        }
    }
}
