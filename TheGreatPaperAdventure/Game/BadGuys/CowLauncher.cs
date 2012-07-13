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

namespace TGPA.Game
{
    /// <summary>
    /// Cow launcher !
    /// </summary>
    public class CowLauncher : BadGuy
    {
        public CowLauncher(Vector2 loc, Vector2 scroll, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags) :
            base(loc, scroll, bonus, pattern, flags, flip,
                Rectangle.Empty, //Source sprite
                Vector2.Zero, //Speed
                Vector2.One,
                new CowLauncherWeapon()
            )
        {
            //Stats
            hp = Int32.MaxValue;
            points = 0;
            ttl = 10000f;
            Difficulty = 50;

            this.hitbox = new EmptyHitbox(this);
            this.Removable = false;
            this.Background = true;
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            //No sprite
        }
    }

    public class CowLauncherWeapon : Weapon
    {
        public CowLauncherWeapon()
            : base(true)
        {
            cooldown = 1600.0f;
            name = "Mad cow";
            damage = 1;
            ammo = InfiniteAmmo;
        }

        public override List<Shot> Fire(Vector2 location)
        {
            List<Shot> newTirs = new List<Shot>();

            Vector2 newLoc = location;

            newTirs.Add(new CowLauncherWeaponShot(newLoc, this, Math.PI/2, this.Flip));

            for (int i = 0; i < RandomMachine.GetRandomInt(2,7); i++)
            {
                newLoc.X += 60;
                newLoc.Y -= 50 ;
                newTirs.Add(new CowLauncherWeaponShot(newLoc, this, Math.PI / 2, this.Flip)); 
            }

            return newTirs;
        }
    }

    public class CowLauncherWeaponShot : Shot
    {
        private double saveRotation = 0.0f;
        private double rotationDelta;

        public CowLauncherWeaponShot(Vector2 loc, CowLauncherWeapon wpn, double angle, SpriteEffects _flip) :
            base(loc,
            new Rectangle(495, 315, 490, 355),    //Sprite
            new Vector2(0, -400),          //Speed
           new Vector2(0.25f, 0.25f),                    //Scale
            _flip, wpn, angle, true)
        {
            this.UseRotationWhenDrawing = true;
            this.saveRotation = angle;
            this.rotationDelta = RandomMachine.GetRandomFloat(-0.2f, 0.2f);
            this.hitbox = new CircleHitbox(this, true, 2f);

            this.Destructable = true;
            this.Points = 250;
            this.Hp = 5;
        }

        public override void Update(GameTime gameTime)
        {
            double tmp = rotation;
            this.rotation = saveRotation;

            base.Update(gameTime);

            this.rotation = tmp;

            this.rotation += rotationDelta;
            this.speed.X = TGPAContext.Instance.Map.Background2.Speed.X;
            this.speed.Y += 0.001f;

            //X movement hack
            float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
            float deplacementX = (speed.X * tempsEcoule);
            this.location.X -= deplacementX;
        }

        public override string DeathSound
        {
            get
            {
                return null;
            }
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
