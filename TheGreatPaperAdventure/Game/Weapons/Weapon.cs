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
using TGPA.Utils;
using TGPA.SpecialEffects;
using TGPA.Game.Weapons;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Audio;

namespace TGPA
{
    /// <summary>
    /// Weapon's information : ammo, damages, ...
    /// </summary>
    public abstract class Weapon
    {
        /// <summary>
        /// Come get some !
        /// </summary>
        public static int InfiniteAmmo = 9999;

        /// <summary>
        /// Don't want to chose a weapon ? Use this method, it will give you a random one 
        /// </summary>
        /// <returns>A random WeaponType(or nothing if you are unlucky)</returns>
        public static Weapon GetRandomWeapon()
        {
            int wpn = RandomMachine.GetRandomInt(0, 9);

            if (wpn < 3)
                return new MachineGun();
            else if (wpn < 6)
                return new Flamethrower();
            else if (wpn < 9)
                return new RocketLauncher();
            else if (wpn == 9)
                return new Megabomb();

            return new MachineGun();
        }

        /// <summary>
        /// Transform a type into the right weapon
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static Weapon TypeToWeapon(String name)
        {
            if (name.Equals(typeof(MachineGun).Name))
                return new MachineGun();
            else if (name.Equals(typeof(Flamethrower).Name))
                return new Flamethrower();
            else if (name.Equals(typeof(RocketLauncher).Name))
                return new RocketLauncher();
            else if (name.Equals(typeof(LanceTruc).Name))
                return new LanceTruc();
            else if (name.Equals(typeof(Megabomb).Name))
                return new Megabomb();
            else if (name.Equals(typeof(ShotGun).Name))
                return new ShotGun();

            return new MachineGun();
        }

        public static Weapon TypeToWeapon(Type t)
        {
            return TypeToWeapon(t.Name);
        }

        protected int upgradeLevel;
        protected string name;
        protected float cooldown;
        protected double lastShot;
        protected int damage;
        protected int ammo;
        protected SpriteEffects flip;

        public Vector2 Rumble { get; set; }
        public virtual int MaxAmmo { get; set; }
        public Entity Owner { get; set; }

        /// <summary>
        /// Common initialisation for weapons
        /// </summary>
        /// <param name="randStart">Random the time of the first shot</param>
        protected Weapon(bool randStart)
        {
            lastShot = randStart ? RandomMachine.GetRandomFloat(0f, 5000f) : 0.0f;
            upgradeLevel = 0;
            flip = SpriteEffects.None;

            Rumble = Vector2.Zero;
            MaxAmmo = InfiniteAmmo;
        }

        /// <summary>
        /// Ding ! new Level for the weapon ! More damage, more shots, less cooldown, ...
        /// </summary>
        public virtual void UpgradeWeapon()
        {
        }

        /// <summary>
        /// Define shots to be fired when firing
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public virtual List<Shot> Fire(Vector2 location)
        {
            return null;
        }

        /// <summary>
        /// Animations or particle to draw when firing.
        /// Basically it plays the weapon's shot sound
        /// </summary>
        public virtual void TodoOnFiring(Vector2 location)
        {
            SoundEngine.Instance.PlaySound(this.FiringSound);
        }

        /// <summary>
        /// Current power level of the weapon.Each time the player get a bonus that is the same that is his current weapon, the level increase.
        /// </summary>
        public Int32 UpgradeLevel
        {
            get { return upgradeLevel; }
            set { upgradeLevel = value; }
        }

        /// <summary>
        /// Weapon's name
        /// </summary>
        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary>
        /// Time before player can shoot again (in ms)
        /// </summary>
        public float Cooldown
        {
            get { return cooldown; }
            set { cooldown = value; }
        }

        /// <summary>
        /// Last time the weapon has fire
        /// </summary>
        public double LastShot
        {
            get { return lastShot; }
            set { lastShot = value; }
        }

        /// <summary>
        /// Damage each shot from this weapon deal
        /// </summary>
        public int Damage
        {
            get { return damage; }
            set { damage = value; }
        }

        /// <summary>
        /// Life is like a box of ammos !
        /// </summary>
        public int Ammo
        {
            get { return ammo; }
            set { ammo = value; }
        }

        public SpriteEffects Flip
        {
            get { return flip; }
            set { flip = value; }
        }

        /// <summary>
        /// The cue name of the sound when firing
        /// </summary>
        public virtual String FiringSound
        {
            get
            {
                return null; //redefine this for children
            }
        }

        public virtual int MaxLevel
        {
            get
            {
                return 4;
            }
        }
    }
}
