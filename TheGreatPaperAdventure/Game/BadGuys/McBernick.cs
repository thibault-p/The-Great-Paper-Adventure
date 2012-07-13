using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Graphics;

namespace TGPA.Game.BadGuys
{
    /// <summary>
    /// Inspired by 'The pirate family'
    /// </summary>
    public class McBernick : BadGuy
    {
        private Rectangle armSrc, armDst;
        private float fireAngle = 0.0f;
        private Vector2 armAnchor;

        public McBernick(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                new Rectangle(0, 0, 634, 600), //Source sprite
                new Vector2(75.0f, 15.0f), //Speed
                new Vector2(0.25f, 0.25f),
                new McBernikWeapon()
            )
        {
            //Common stats
            hp = 35;
            points = 10000;
            Difficulty = 6;

            //Weapon sprite
            armSrc = new Rectangle(720, 0, 230, 430);

            armDst = new Rectangle();
            armDst.Width = (int)(armSrc.Width * Scale.X);
            armDst.Height = (int)(armSrc.Height * Scale.Y);

            armAnchor = new Vector2(armSrc.Width / 2, armSrc.Height - (armSrc.Height / 8));

            //Special property
            ((McBernikWeapon)this.Weapon).Parent = this;
        }

        public override void Update(GameTime gameTime)
        {
            //Aim a player
            int rand = RandomMachine.GetRandomInt(0, 1);

            if ((rand == 0) && (TGPAContext.Instance.Player2 != null))
                fireAngle = Angles.GetAngle(Vectors.ConvertPointToVector2(armDst.Center), Vectors.ConvertPointToVector2(TGPAContext.Instance.Player2.DstRect.Center));
            else
                fireAngle = Angles.GetAngle(Vectors.ConvertPointToVector2(armDst.Center), Vectors.ConvertPointToVector2(TGPAContext.Instance.Player1.DstRect.Center));

            fireAngle -= (float)(Math.PI / 2);

            base.Update(gameTime);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            armDst.X = (int)location.X + dRect.Width / 2;
            armDst.Y = (int)location.Y + dRect.Height / 2;

            //Due to arm orietation, there is a Pi/8 difference in the angle to draw and to use
            double drawAngle = fireAngle + Math.PI / 8;

            //Draw arm with angle
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.Sprite, armDst, armSrc, Color.White, (float)drawAngle,
                armAnchor,
                this.Flip, 1.0f);
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
            theSprite = cm.Load<Texture2D>("gfx/Sprites/mcbernick");
        }

        #endregion

        public float FireAngle
        {
            get { return fireAngle; }
        }
    }

    /// <summary>
    /// Pirate weapon
    /// </summary>
    public class McBernikWeapon : Weapon
    {
        public McBernick parent;

        public McBernikWeapon()
            : base(true)
        {
            cooldown = 1000.0f;
            name = "Pirate gun";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            double drunkAngle = -parent.FireAngle + Math.PI / 2;
            
            newTirs.Add(new McBernikWeaponShot(location, this, drunkAngle, this.Flip));

            return newTirs;
        }

        /// <summary>
        /// The Weapon need its parent reference
        /// </summary>
        public McBernick Parent
        {
            set { parent = value; }
        }
    }

    /// <summary>
    /// Canon bullet
    /// </summary>
    public class McBernikWeaponShot : Shot
    {
        public McBernikWeaponShot(Vector2 loc, McBernikWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(0, 0, 80, 80),    //Sprite
            new Vector2(450, 450),          //Speed
            Vector2.One,                    //Scale
            _flip, wpn, angle, true)
        { }

        public override void Update(GameTime gameTime)
        {
            Vector2 loc = Vectors.ConvertPointToVector2(DstRect.Center);

            //Add Smoke and fire
            TGPAContext.Instance.ParticleManager.AddParticle(new Smoke(loc, RandomMachine.GetRandomVector2(-1f, 1f, -2f, 2f),
    0.30f, 0.30f, 0.30f, 1f,
    RandomMachine.GetRandomFloat(0.01f, .03f),
    RandomMachine.GetRandomInt(0, 4)), true);

            Fire f = new Fire(loc, RandomMachine.GetRandomVector2(-1f, 1f, -2f, 2f),
                0.25f, RandomMachine.GetRandomInt(0, 4));
            TGPAContext.Instance.ParticleManager.AddParticle(f, true);

            base.Update(gameTime);
        }
    }

}
