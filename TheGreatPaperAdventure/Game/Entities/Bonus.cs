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
using TGPA.Game.Weapons;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Graphics;

namespace TGPA
{
    /// <summary>
    /// Available bonuses
    /// </summary>
    public enum BonusTypes
    {
        None,
        Weapon,
        Life,
        Coffee,
        Cassoulet
    }

    /// <summary>
    /// Bonus items, such as new weapons or extra lifes
    /// </summary>
    public class Bonus : Entity
    {
        private BonusTypes type;
        private Weapon bonusWpn;
        private Color color;
        private float alpha;

        /// <summary>
        /// Create a weapon bonus
        /// </summary>
        /// <param name="_type">Weapon name the bonus contain</param>
        public Bonus(String bonusName)
            : base(Vector2.Zero, Rectangle.Empty, new Vector2(200, 200), new Vector2(0.25f, 0.25f), 0.0f, 15000.0f)
        {
            this.location = Vector2.Zero;

            //Bonus for weapon
            if (bonusName.Equals(typeof(MachineGun).Name))
            {
                sRect = new Rectangle(688, 688, 330, 330);
                this.bonusWpn = new MachineGun();
                this.type = BonusTypes.Weapon;

            }
            else if (bonusName.Equals(typeof(ShotGun).Name))
            {
                sRect = new Rectangle(688, 0, 330, 330);
                this.bonusWpn = new ShotGun();
                this.type = BonusTypes.Weapon;
            }
            else if (bonusName.Equals(typeof(Flamethrower).Name))
            {
                sRect = new Rectangle(335, 692, 330, 330);
                this.bonusWpn = new Flamethrower();
                this.type = BonusTypes.Weapon;
            }
            else if (bonusName.Equals(typeof(RocketLauncher).Name))
            {
                sRect = new Rectangle(5, 682, 330, 330);
                 this.bonusWpn = new RocketLauncher();
                 this.type = BonusTypes.Weapon;
            }
            else if (bonusName.Equals(typeof(LanceTruc).Name))
            {
                sRect = new Rectangle(670, 331, 330, 330);
                this.bonusWpn = new LanceTruc();
                this.type = BonusTypes.Weapon;
            }
            else if (bonusName.Equals(typeof(Megabomb).Name))
            {
                sRect = new Rectangle(0, 340, 330, 330);
                this.bonusWpn = new Megabomb();
                this.type = BonusTypes.Weapon;
            }
            else if (bonusName.Equals(BonusTypes.Life.ToString()))
            {
                sRect = new Rectangle(328, 354, 330, 330);
                this.bonusWpn = null;
                this.type = BonusTypes.Life;
            }
            else if (bonusName.Equals(BonusTypes.Coffee.ToString()))
            {
                sRect = new Rectangle(0, 0, 330, 330);
                this.bonusWpn = null;
                this.type = BonusTypes.Coffee;
            }
            else if (bonusName.Equals(BonusTypes.Cassoulet.ToString()))
            {
                sRect = new Rectangle(332, 0, 330, 330);
                this.bonusWpn = null;
                this.type = BonusTypes.Cassoulet;
            }
            else
            {
                throw new Exception("Unknow bonus : "+bonusName);
            }

            dRect = ComputeDstRect(sRect);
            alpha = 1.0f;
            this.hitbox = new TGPA.Game.Hitbox.CircleHitbox(this, false, 0.75f);
        }

        public override void Update(GameTime gameTime)
        {
            if (this.ttl < 1000f)
            {
                alpha -= 0.02f;
            }
            base.Update(gameTime);
        }

       public override void Draw(Game.Graphics.TGPASpriteBatch spriteBatch)
        {
            throw new NotImplementedException("Use Draw(SpriteBatch,Texture2D) instead");
        }

        public void Draw(TGPASpriteBatch spriteBatch, Texture2D bonus_sprites)
        {
            this.color = Color.White *alpha;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            spriteBatch.Draw(bonus_sprites, dRect, sRect, color);

            spriteBatch.End();
        }

        /// <summary>
        /// Type of bonus (weapon, life, ...)
        /// </summary>
        public BonusTypes Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary>
        /// If bonus type is weapon, this is the weapon contained in the bonus
        /// </summary>
        public Weapon WeaponToDrop
        {
            get { return bonusWpn; }
            set { bonusWpn = value; }
        }

    }
}
