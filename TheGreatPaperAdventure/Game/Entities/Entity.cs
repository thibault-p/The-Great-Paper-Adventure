//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Hitbox;
using TGPA.Utils;
using TGPA.Audio;
using TGPA.Game.Graphics;

namespace TGPA
{
    /// <summary>
    /// Represent a displayable object. All children can be displayed, collide and used.
    /// </summary>
    public abstract class Entity
    {
        /// <summary>
        /// Time to live : infinite
        /// </summary>
        public static double InfiniteTimeToLive = 9999.0f;
        protected Vector2 location;
        protected double rotation;
        protected Rectangle initsRect, sRect;
        protected Rectangle dRect;
        protected Vector2 speed;
        protected Vector2 basicSpeed;
        protected double ttl;
        protected SpriteEffects flips;
        protected Hitbox hitbox;
        protected Vector2 spriteOrigin;
        protected int currentFrame, totalFrameNumber;
        protected double frameTime, frameCooldown;
        protected Vector2 spriteBox;
        private Vector2 scale;

        /// <summary>
        /// Draw method has to use the rotation value
        /// </summary>
        public bool UseRotationWhenDrawing { get; set; }

        /// <summary>
        /// Sprite is scaled before displayed
        /// </summary>
        public bool UseAnimation { get; set; }

        /// <summary>
        /// Create a new displayable element
        /// </summary>
        /// <param name="_location">Where the entity will start</param>
        /// <param name="_spriteRect">Location in spritefile of the sprite</param>
        /// <param name="_speed">Default speed</param>
        /// <param name="scale">Sprite scaling (if not, use Vector2.One) </param>
        /// <param name="angle">Sprite orientation</param>
        /// <param name="timeToLive">Time entity will stay in game (use 'InfiniteTimeToLive' if necessary)</param>
        public Entity(Vector2 _location, Rectangle _spriteRect, Vector2 _speed, Vector2 scale, double angle, double timeToLive)
        {
            this.location = _location;
            this.sRect = _spriteRect;
            this.scale = scale;

            this.dRect = ComputeDstRect(sRect);

            this.speed = _speed;
            this.ttl = timeToLive;
            this.rotation = angle;

            this.hitbox = new CircleHitbox(this, false);

            this.spriteOrigin = Vector2.Zero;

            //New parameters for animated sprites
            this.currentFrame = 0;
            this.frameTime = 0;
            this.totalFrameNumber = 0;
            this.frameCooldown = 0;
            this.initsRect = Rectangle.Empty;
            this.spriteBox = Vector2.Zero;
        }

        public virtual void Update(GameTime gameTime)
        {
            dRect.X = (int)location.X;
            dRect.Y = (int)location.Y;

            //Animate sprites
            if (UseAnimation)
            {
                //Next frame ?
                if (gameTime.TotalGameTime.TotalMilliseconds - frameTime > frameCooldown)
                {
                    currentFrame++;
                    if (currentFrame >= totalFrameNumber) currentFrame = 0;
                    frameTime = gameTime.TotalGameTime.TotalMilliseconds;
                }

                //Save initial sprite location data
                if (initsRect == Rectangle.Empty)
                {
                    initsRect = sRect;
                }

                sRect.X = initsRect.X + ((int)spriteBox.X * currentFrame);
            }

            hitbox.Update(gameTime);
        }

        public virtual void Draw(TGPASpriteBatch spriteBatch) { }

        /// <summary>
        /// Compute a sprite to the screen dimension using scaling data
        /// </summary>
        protected Rectangle ComputeDstRect(Rectangle src)
        {
            Rectangle dst = new Rectangle();

            dst.X = (int)location.X;
            dst.Y = (int)location.Y;
            dst.Width = (int)((float)src.Width * scale.X);
            dst.Height = (int)((float)src.Height * scale.Y);

            return dst;
        }

        public virtual void TodoOnDeath()
        {
            this.Hitbox = new EmptyHitbox(this);

            //Play death sound
            SoundEngine.Instance.PlaySound(this.DeathSound);
        }

        public void ResetSpeed()
        {
            speed = basicSpeed;
        }

        protected void UseSpriteOrigin()
        {
            this.spriteOrigin = new Vector2(
                this.sRect.Width / 2,
                this.sRect.Height / 2
                );
        }

        /// <summary>
        /// Entity location on the screen
        /// </summary>
        public Vector2 Location
        {
            get { return location; }
            set { location = value; }
        }

        /// <summary>
        /// Rotation angle (radian)
        /// </summary>
        public double Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        /// <summary>
        /// Source Rectangle, rectangle containing sprite from file
        /// </summary>
        public Rectangle SrcRect
        {
            get { return sRect; }
            set { sRect = value;
            this.dRect = ComputeDstRect(this.sRect);
            }
        }

        /// <summary>
        /// Destination Rectangle, where the sprite will be displayed.
        /// It must be define otherwise bad thing happend.
        /// </summary>
        public Rectangle DstRect
        {
            get { return dRect; }
            set { dRect = value; }
        }

        /// <summary>
        /// Entity's movements speed
        /// </summary>
        public Vector2 Speed
        {
            get { return speed; }
            set
            {
                ResetSpeed();
                basicSpeed = speed;
                speed = value;
            }
        }

        /// <summary>
        /// Time to live before deletion
        /// </summary>
        public virtual double TimeToLive
        {
            get { return ttl; }
            set { ttl = value; }
        }

        public SpriteEffects Flip
        {
            get { return flips; }
            set
            {
                flips = value;
                if (Flip == SpriteEffects.FlipHorizontally)
                {
                    speed.X = -speed.X;
                    rotation *= Math.PI;
                    UseRotationWhenDrawing = false;
                }
                else if (Flip == SpriteEffects.FlipVertically)
                {
                    speed.Y = -speed.Y;
                    rotation *= Math.PI / 2;
                    UseRotationWhenDrawing = false;
                }
            }
        }

        public Vector2 SpriteOrigin
        {
            get { return spriteOrigin; }
            set { spriteOrigin = value; }
        }

        /// <summary>
        /// Entity's hitbox, default hitbox is the same than the sprite's dimension
        /// </summary>
        public Hitbox Hitbox
        {
            get
            {
                return hitbox;
            }
            set { hitbox = value; }
        }

        /// <summary>
        /// Scale of the sprite (x and y)
        /// </summary>
        public Vector2 Scale
        {
            get { return this.scale; }
            set 
            { 
                this.scale = value;
                this.dRect = ComputeDstRect(sRect);
            }
        }

        /// <summary>
        /// The cue name of the sound when the enemy die
        /// </summary>
        public virtual String DeathSound
        {
            get { return null; }
        }
    }
}
