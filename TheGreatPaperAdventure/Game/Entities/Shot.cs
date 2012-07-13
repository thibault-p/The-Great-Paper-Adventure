//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGPA.Game.Hitbox;
using TGPA.Utils;
using TGPA.Game.Graphics;

namespace TGPA
{
    /// <summary>
    /// Bullet, rocket, laser etc
    /// </summary>
    public abstract class Shot : Entity
    {
        /// <summary>
        /// Black contours for shots
        /// </summary>
        protected static Color defaultEnemyBehindColor = Color.Black *0.75f;
        protected static Color defaultPlayerBehindColor = Color.Black *0.75f;

        protected Weapon wpn;
        protected bool enemy;
        protected bool bounce;
        protected bool additive;
        protected Color behindColor;
        protected int hp;
        protected int points;

        /// <summary>
        /// HACK : Shot can be outside the screen
        /// </summary>
        public bool CanGoOffLimits { get; set; }

        /// <summary>
        /// Define if the shot is drawn after enemies. Default is yes
        /// </summary>
        public bool DrawBehindEnemies { get; set; }

        /// <summary>
        /// Define if the shot is can be destroyed be a player shot
        /// </summary>
        public bool Destructable { get; set; }

        public bool IsHit { get; set; }

        /// <summary>
        /// Create a new Shot. Must be used in subclasses. Do some initialisations
        /// </summary>
        ///<param name="loc">Screen location</param>
        ///<param name="srcSprite">Sprite sheet location and size</param>
        ///<param name="speed">Shot speed</param>
        ///<param name="scale">Sprite scaling</param>
        ///<param name="flip">Sprite flip</param>
        ///<param name="_wpn">Firing weapon</param>
        ///<param name="angle">Firing angle</param>
        ///<param name="_enemy">Shot by enemy or by player</param>
        protected Shot(Vector2 loc, Rectangle srcSprite, Vector2 speed, Vector2 scale, SpriteEffects flip, Weapon _wpn, double angle, bool _enemy)
            : base(loc, srcSprite, speed, new Vector2(0.25f, 0.25f), angle, 10000f)
        {
            this.flips = flip;
            this.wpn = _wpn;
            this.rotation = angle;
            this.enemy = _enemy;
            this.sRect = srcSprite;
            this.hitbox = new CircleHitbox(this, true, 2f);
            this.additive = false;
            this.bounce = false;
            this.hp = -1;
            this.points = 0;
            this.CanGoOffLimits = false;
            this.IsHit = false;

            if (this.EnemyFire)
            {
                this.behindColor = defaultEnemyBehindColor;

                if (TGPAContext.Instance.Cheatcodes.IsGiantMode)
                {
                    this.Scale = new Vector2(0.75f, 0.75f);
                }
            }
            else
            {
                this.behindColor = defaultPlayerBehindColor;

                if (TGPAContext.Instance.Cheatcodes.IsGiantMode)
                {
                    this.Scale = new Vector2(0.1f, 0.1f);
                }
            }

            this.DrawBehindEnemies = true;
        }

        /// <summary>
        /// Animations or particles to draw when the shot hit something.
        /// Basically it plays a sound
        /// </summary>
        public override void TodoOnDeath()
        {
            base.TodoOnDeath();
        }

        /// <summary>
        /// Shot update : Move
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            //Do it only once
            if (spriteOrigin == Vector2.Zero)
                spriteOrigin = new Vector2(sRect.Width / 2, sRect.Height / 2);

            //Care of time
            float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (tempsEcoule > 1) tempsEcoule = 1;

            int deplacementX = (int)(speed.X * tempsEcoule);
            int deplacementY = (int)(speed.Y * tempsEcoule);

            location.X += (((float)Math.Cos(rotation)) * (deplacementX));
            location.Y -= (((float)Math.Sin(rotation)) * (deplacementY));

            dRect.X = (int)location.X;
            dRect.Y = (int)location.Y;

            //Update the hitbox location and radius
            this.hitbox.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            throw new NotImplementedException("Use Draw(SpriteBatch,Texture2D) instead");
        }

        public virtual void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
        {
            dRect.X = (int)location.X;
            dRect.Y = (int)location.Y;

            Rectangle dstShotBehind = dRect;
            dstShotBehind.Width = (int)((float)dstShotBehind.Width * 1.2f);
            dstShotBehind.Height = (int)((float)dstShotBehind.Height * 1.2f);

            Color shotColor = Color.White;

            if (this.IsHit)
            {
                shotColor = Color.Red;
                this.IsHit = false;
            }

            if (this.EnemyFire)
            {
                if (this.DrawBehindShot)
                {
                    spriteBatch.Draw(texture, dstShotBehind, sRect, this.behindColor, -(float)rotation, spriteOrigin, this.flips, 1.0f);
                }
                spriteBatch.Draw(texture, dRect, sRect, shotColor, -(float)rotation, spriteOrigin, this.flips, 1.0f);
            }
            else
            {
                if (this.DrawBehindShot)
                {
                    spriteBatch.Draw(texture, dstShotBehind, sRect, this.behindColor, -(float)rotation, spriteOrigin, SpriteEffects.None, 1.0f);
                }
                spriteBatch.Draw(texture, dRect, sRect, Color.White, -(float)rotation, spriteOrigin, SpriteEffects.None, 1.0f);
            }
        }

        /// <summary>
        /// Weapon that has fired this shot
        /// </summary>
        public Weapon Weapon
        {
            get { return wpn; }
            set { wpn = value; }
        }

        /// <summary>
        /// Friendly fire or not
        /// </summary>
        public bool EnemyFire
        {
            get { return enemy; }
            set { enemy = value; }
        }

        /// <summary>
        /// Bouncing fire, very funny
        /// </summary>
        public bool Bounce
        {
            get { return bounce; }
            set { bounce = value; }
        }

        public override string DeathSound
        {
            get { return null; }
        }

        public virtual bool DrawBehindShot
        {
            get
            {
                return true;
            }
        }

        public Color BehindColor
        {
            get { return this.BehindColor; }
            set { this.behindColor = value;}
        }

        public int Hp
        {
            get { return this.hp; }
            set { this.hp = value; }
        }

        public int Points
        {
            get { return this.points; }
            set { this.points = value; }
        }
    }

}
