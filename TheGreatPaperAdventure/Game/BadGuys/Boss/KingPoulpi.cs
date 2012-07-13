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

namespace TGPA.Game.BadGuys.Boss
{
    /// <summary>
    /// Secret boss ! Giant Poulpi !
    /// </summary>
    public class KingPoulpi : TGPA.Game.Entities.Boss
    {
        /// <summary>
        /// State of boss face and actions
        /// </summary>
        private enum KingPoulpiState
        {
            Normal,
            Attack,
            Hit
        }

        /// <summary>
        /// Boss attacks
        /// </summary>
        private enum KingPoulpiAttacks
        {
            None,
        }

        private KingPoulpiState state;
        private KingPoulpiAttacks attackState;
        private double faceCooldown, attackCooldown;

        private Rectangle eyesSRect, appendiceSRect;
        private Vector2 eyesLoc, appendiceLoc;

        public KingPoulpi(Vector2 scroll, String[] flags)
            : base(scroll)
        {
            //Source sprite
            this.sRect = new Rectangle(0, 0, 600, 720);
            dRect = ComputeDstRect(sRect);

            eyesSRect = new Rectangle(645, 47, 154, 131);
            eyesLoc = new Vector2(110, 285);

            appendiceSRect = new Rectangle(26, 861, 160, 144);
            appendiceLoc = new Vector2(240, 570);

            this.location = new Vector2(TGPAContext.Instance.ScreenWidth / 2, 150);//TODO DMA

            this.ttl = InfiniteTimeToLive;

            //Stats
            this.wpn = null;
            this.hp = 10001;
            this.maxLifebarValue = hp;
            this.speed = Vector2.Zero;

            this.points = 500000;

            this.attackState = KingPoulpiAttacks.None;
            this.attackCooldown = 2500f;
            this.state = KingPoulpiState.Normal;

            this.flagsOnDeath = flags;
            this.InfiniteMovePattern = true;
            this.hitbox = new EmptyHitbox(this);

            this.DrawLifebar = true;
            this.DrawWarning = true;
        }

        public override void Update(GameTime gameTime)
        {
            bool attack = true;

            //Decrease cooldowns
            if (this.faceCooldown > 0f)
            {
                this.faceCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
            }

            if (this.attackCooldown > 0f)
            {
                this.attackCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;
                attack = false;
            }

            //Change state is necessary
            if ((this.faceCooldown < 0f) && (this.state != KingPoulpiState.Normal))
            {
                this.state = KingPoulpiState.Normal;
            }

            //Boss is hit : change face
            if (IsHit)
            {
                this.state = KingPoulpiState.Hit;
                this.faceCooldown = 500f;
            }

            //Find attack
            if (attack)
            {
                if (this.attackState == KingPoulpiAttacks.None)
                {
                    this.hitbox = new SquareHitbox(this, new Vector2(0.5f, 0));
                }

                this.state = KingPoulpiState.Attack;
                this.faceCooldown = 2000f;
                this.speed = new Vector2(-150f, 50f);
                this.wpn = null;

                int rand = RandomMachine.GetRandomInt(0, 6);

                if ((this.movePattern == null) || (this.movePattern.Points.Count != 5))
                {

                }
            }


            //Facial animation
            switch (this.state)
            {
                case KingPoulpiState.Normal:
                    //sRect.X = 0;
                    break;

                case KingPoulpiState.Attack:
                    //sRect.X = 512;
                    break;

                case KingPoulpiState.Hit:
                    //sRect.X = 1024;
                    break;
            }

            base.Update(gameTime);
        }

        public override void Draw(Graphics.TGPASpriteBatch spriteBatch)
        {
            Color c = Color.White;

            if (IsHit)
            {
                c = (Color.Red * 0.75f);
                IsHit = false;
            }

            Rectangle eyesDstRect = new Rectangle(
                (int)(this.location.X + eyesLoc.X),
                (int)(this.location.Y + eyesLoc.Y),
                eyesSRect.Width,
                eyesSRect.Height
                );

            Rectangle appendiceDstRect = new Rectangle(
                    (int)(this.location.X + eyesLoc.X),
                    (int)(this.location.Y + eyesLoc.Y),
                    appendiceSRect.Width,
                    appendiceSRect.Height
                    );

            spriteBatch.Begin();

            //Appendice
            spriteBatch.Draw(this.Sprite, appendiceDstRect, appendiceSRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);

            //Body
            spriteBatch.Draw(this.Sprite, dRect, sRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);

            //Eyes
            spriteBatch.Draw(this.Sprite, eyesDstRect, eyesSRect, c, 0.0f, Vector2.Zero, Flip, 1.0f);

            spriteBatch.End();
        }

        #region Sprite
        protected static Texture2D theSprite;

        public override Texture2D Sprite
        {
            get { return theSprite; }
        }

        public override void LoadContent(ContentManager cm)
        {
            theSprite = cm.Load<Texture2D>("gfx/Sprites/kingPoulpi");
        }
        #endregion
    }
}
