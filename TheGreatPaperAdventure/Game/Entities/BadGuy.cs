
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
using TGPA.Game.BadGuys;
using Microsoft.Xna.Framework.Content;
using TGPA.Utils;
using TGPA.Audio;
using TGPA.Game.Hitbox;
using TGPA.Game.BadGuys.Boss;
using TGPA.Game;
using TGPA.Game.Graphics;

namespace TGPA
{
    /// <summary>
    /// Game enemy (IA)
    /// </summary>
    public abstract class BadGuy : Entity
    {
        protected Weapon wpn;
        protected int hp;
        protected Bonus dropOnDeath;
        protected int points;
        protected MovePattern movePattern;
        protected bool bounce;
        protected bool onScreen;
        protected Vector2 scrollValue;
        protected String[] flagsOnDeath;

        /// <summary>
        /// Collision has been detected in the last frame
        /// </summary>
        public bool IsHit { get; set; }

        /// <summary>
        /// Badguy move with background
        /// </summary>
        public bool Background { get; set; }

        /// <summary>
        /// Will Badguy be deleted from memory if hp lower than 0 ?
        /// </summary>
        public bool Removable { get; set; }

        /// <summary>
        /// For enemy random generation : indicator of the difficulty of fighting this ennemy
        /// </summary>
        public virtual int Difficulty { get; set; }

        /// <summary>
        /// Repeat the Move Pattern
        /// </summary>
        public bool InfiniteMovePattern { get; set; }

        /// <summary>
        /// From where shots come
        /// </summary>
        public Vector2 FiringLocation { get; set; }

        /// <summary>
        /// Return instance of the given enemy type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BadGuy String2BadGuy(String type, Vector2 location, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags)
        {
            if (type.Equals(typeof(Poulpi).Name.ToString()))
            {
                return new Poulpi(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpiMexico).Name.ToString()))
            {
                return new PoulpiMexico(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpiZombie).Name.ToString()))
            {
                return new PoulpiZombie(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpiMarin).Name.ToString()))
            {
                return new PoulpiMarin(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpiSnow).Name.ToString()))
            {
                return new PoulpiSnow(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpiFrozen).Name.ToString()))
            {
                return new PoulpiFrozen(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpiKnight).Name.ToString()))
            {
                return new PoulpiKnight(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PoulpElite).Name.ToString()))
            {
                return new PoulpElite(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Pepito).Name.ToString()))
            {
                return new Pepito(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Bird).Name.ToString()))
            {
                return new Bird(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Caca).Name.ToString()))
            {
                return new Caca(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Moustik).Name.ToString()))
            {
                return new Moustik(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Pirate).Name.ToString()))
            {
                return new Pirate(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(AirFighter).Name.ToString()))
            {
                return new AirFighter(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Rock).Name.ToString()))
            {
                return new Rock(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(RockDoor).Name.ToString()))
            {
                return new RockDoor(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(BouleEpine).Name.ToString()))
            {
                return new BouleEpine(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(CactusMan).Name.ToString()))
            {
                return new CactusMan(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Canon).Name.ToString()))
            {
                return new Canon(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(RockDoor).Name.ToString()))
            {
                return new RockDoor(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(BouleEpine).Name.ToString()))
            {
                return new BouleEpine(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(CactusMan).Name.ToString()))
            {
                return new CactusMan(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Pumpkin).Name.ToString()))
            {
                return new Pumpkin(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(McBernickZombie).Name.ToString()))
            {
                return new McBernickZombie(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(McBernick).Name.ToString()))
            {
                return new McBernick(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Avalanche).Name.ToString()))
            {
                return new Avalanche(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Trainee).Name.ToString()))
            {
                return new Trainee(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Witch).Name.ToString()))
            {
                return new Witch(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Flipper).Name.ToString()))
            {
                return new Flipper(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(FlipperZombie).Name.ToString()))
            {
                return new FlipperZombie(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(EtoileMer).Name.ToString()))
            {
                return new EtoileMer(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(CowLauncher).Name.ToString()))
            {
                return new CowLauncher(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(LittleUndergroundCreature).Name.ToString()))
            {
                return new LittleUndergroundCreature(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Cow).Name.ToString()))
            {
                return new Cow(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(RandomCow).Name.ToString()))
            {
                return new RandomCow(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(FireSkull).Name.ToString()))
            {
                return new FireSkull(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Pingouin).Name.ToString()))
            {
                return new Pingouin(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(PingouinLauncher).Name.ToString()))
            {
                return new PingouinLauncher(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(Yeti).Name.ToString()))
            {
                return new Yeti(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(SapinLauncher).Name.ToString()))
            {
                return new SapinLauncher(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(RockLauncher).Name.ToString()))
            {
                return new RockLauncher(location, Vector2.Zero, bonus, pattern, flip, flags);
            }
            else if (type.Equals(typeof(BN).Name.ToString()))
            {
                return new BN(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(ElFeroCactae).Name.ToString()))
            {
                return new ElFeroCactae(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(Ariel).Name.ToString()))
            {
                return new Ariel(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(KingPoulpi).Name.ToString()))
            {
                return new KingPoulpi(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(MaskedEsquimo).Name.ToString()))
            {
                return new MaskedEsquimo(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(Esquimo).Name.ToString()))
            {
                return new Esquimo(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(Derrick).Name.ToString()))
            {
                return new Derrick(Vector2.Zero, flags);
            }
            else if (type.Equals(typeof(CrazyDoc).Name.ToString()))
            {
                return new CrazyDoc(Vector2.Zero, flags);
            }
            else
            {
                throw new Exception("Invalid TGPA map : Enemy " + type + " not found");
            }
        }

        /// <summary>
        /// Return instance of the given enemy type
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static BadGuy Type2BadGuy(Type type, Vector2 location, Bonus bonus, MovePattern pattern, SpriteEffects flip, String[] flags)
        {
            return String2BadGuy(type.ToString(), location, bonus, pattern, flip, flags);
        }

        /// <summary>
        /// Super constructor for enemies. You must call it in subclasses
        /// </summary>
        /// <param name="_location">Screen location</param>
        /// <param name="_pattern">Move pattern to use</param>
        /// <param name="_scrollValue">Scroll value trigger</param>
        /// <param name="flags">Flags to set on death</param>
        /// <param name="flip">Type of flip</param>
        /// <param name="scale">Sprite scaling (use Vector2.One if not necessary)</param>
        /// <param name="speed">Speed of the enemy</param>
        /// <param name="spriteSrc">Spritesheet rectangle selection</param>
        /// <param name="_bonusDroppedOnDeath">Bonus the enemy will drop on death</param>
        /// <param name="weapon">Enemy weapon</param>
        protected BadGuy(Vector2 _location, Vector2 _scrollValue, Bonus _bonusDroppedOnDeath, MovePattern _pattern, String[] flags, SpriteEffects flip, Rectangle spriteSrc, Vector2 speed, Vector2 scale, Weapon weapon) :
            base(_location, spriteSrc, speed, scale, 0.0f, 30000f) //30sec ttl
        {
            this.dropOnDeath = _bonusDroppedOnDeath;
            this.movePattern = _pattern;
            this.scrollValue = _scrollValue;
            this.Flags = flags;

            if (_pattern != null)
            {
                bounce = false;
            }
            else
            {
                bounce = true;
            }

            this.onScreen = false;
            this.rotation = 0.0f;

            this.Flip = flip;
            this.Weapon = weapon;

            if (this.Weapon != null)
            {
                this.Weapon.Flip = flip;
            }

            this.Hitbox = new SquareHitbox(this);
            this.Removable = true;
            this.InfiniteMovePattern = false;
            this.Difficulty = 0;
            this.ttl = 120000;
            this.FiringLocation = Vectors.ConvertPointToVector2(DstRect.Center);
        }

        /// <summary>
        /// Badguy update : firing
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            if (Background)
            {
                this.Speed = TGPAContext.Instance.Map.Background2.Speed;
            }

            //Hitbox update
            base.Update(gameTime);

            //Default action : firing
            if (wpn != null)
            {
                //Respect cooldown
                if ((wpn.LastShot == 0) || (gameTime.TotalGameTime.TotalMilliseconds - wpn.LastShot >= wpn.Cooldown))
                {
                    List<Shot> tmp = null;
                    tmp = Fire();
                    wpn.TodoOnFiring(Vectors.ConvertPointToVector2(DstRect.Center));

                    if (tmp != null)
                    {
                        //Add n shots to game
                        for (int i = 0; i < tmp.Count; i++)
                        {
                            tmp[i].Flip = this.Flip;
                        }

                        if (tmp.Count > 0)
                        {
                            TGPAContext.Instance.Shots.AddRange(tmp);
                        }
                        wpn.LastShot = gameTime.TotalGameTime.TotalMilliseconds;
                    }
                }
            }
            this.FiringLocation = Vectors.ConvertPointToVector2(DstRect.Center);
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
            dRect.X = (int)this.Location.X;
            dRect.Y = (int)this.Location.Y;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            Color c = Color.White;

            if (IsHit)
            {
                c = Color.Red *0.95f;
                IsHit = false;
            }

            //Rotation can be used in update but not in draw
            float finalrotation = UseRotationWhenDrawing ? (float)rotation : 0.0f;

            spriteBatch.Draw(this.Sprite, this.DstRect, this.SrcRect, c, finalrotation, this.spriteOrigin, Flip, 1.0f);

            spriteBatch.End();
        }

        /// <summary>
        /// Redefine this method to load custom enemy sprite
        /// </summary>
        /// <param name="c"></param>
        public virtual void LoadContent(ContentManager c)
        {

        }

        /// <summary>
        /// Every badguy have to overload this property to return the texture specific to its class
        /// </summary>
        public virtual Texture2D Sprite
        {
            get { return null; }
        }

        public override void TodoOnDeath()
        {
            //Set flags
            if (flagsOnDeath != null)
            {
                foreach (String flag in flagsOnDeath)
                {
                    TGPAContext.Instance.Map.Flags.SetFlag(flag);
                }
            }

            //Explosion
            TGPAContext.Instance.ParticleManager.MakeExplosion(Vectors.ConvertPointToVector2(dRect.Center), 0.9f);

            base.TodoOnDeath();
        }

        /// <summary>
        /// Script line
        /// </summary>
        /// <param name="screenWidth"></param>
        /// <param name="screenHeight"></param>
        /// <returns></returns>
        public string ToString(float screenWidth, float screenHeight)
        {
            String bonus = BonusToDrop == null ? "nobonus" : BonusToDrop.GetType().Name;
            String pattern = "";

            if (movePattern != null)
            {
                foreach (Point p in movePattern.Points)
                    pattern += "(" + p.X.ToString() + "," + p.Y.ToString() + ")+";

                //Remove last ,
                pattern = pattern.Substring(0, pattern.Length - 1);
            }
            else
            {
                pattern = "nopattern";
            }

            pattern += " ";

            String flags = "";
            if (flagsOnDeath != null)
            {
                foreach (String flag in flagsOnDeath)
                    flags += flag + ",";

                //Remove last ,
                flags = flags.Substring(0, flags.Length - 1);

                if ((flags == " ") || (flags == ""))
                    flags = "noflags";
            }
            else
            {
                flags = "noflags";
            }

            //Save relative location (in percent), not absolute location value anymore. Conversion have to be done when saving / loading map
            //Value : -1 if at the left border, 1 for the right border, and value*screenWidth otherwise
            float relativeLocationX = 0f;
            if (this.Location.X < 0) relativeLocationX = -1;
            else if (this.Location.X > screenWidth) relativeLocationX = 1;
            else
                relativeLocationX = this.Location.X / screenWidth;

            //Adjust enemies that are a little bit on the screen
            if (relativeLocationX < 0.05) relativeLocationX = -1;
            else if (relativeLocationX > 0.95) relativeLocationX = 1;

            float relativeLocationY = 0f;
            if (this.Location.Y < 0) relativeLocationY = -1;
            else if (this.Location.Y > screenHeight) relativeLocationY = 1;
            else
                relativeLocationY = this.Location.Y / screenHeight;

            if (relativeLocationY < 0.05) relativeLocationY = -1;
            else if (relativeLocationY > 0.95) relativeLocationY = 1;

            return this.GetType().Name + " " + relativeLocationX.ToString("0.##") + " " + relativeLocationY.ToString("0.##") + " " + bonus + " " + pattern + flags + " " + flips.ToString();

        }

        public override string ToString()
        {
            return this.ToString(TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight);
        }

        /// <summary>
        /// Make the enemy fire some shots
        /// </summary>
        /// <returns></returns>
        public virtual List<Shot> Fire()
        {
            this.wpn.Flip = this.flips;
            this.wpn.Owner = this;
            return wpn.Fire(this.FiringLocation);
        }

        /// <summary>
        /// Specify behavior when the enemy is hit by a megabomb. Default is death
        /// </summary>
        public virtual void TodoOnBombing(int damage)
        {
            this.HP -= damage;
        }

        /// <summary>
        /// Specify behavior when the badguy's pattern has reached the last point, infinite or not. Default is nothing.
        /// </summary>
        public virtual void TodoOnPatternEnd()
        {

        }

        /// <summary>
        /// Enemy weapon
        /// </summary>
        public Weapon Weapon
        {
            get
            {
                return wpn;
            }
            set
            {
                wpn = value;
            }
        }

        /// <summary>
        /// Remaining Health Points
        /// </summary>
        public virtual int HP
        {
            get { return hp; }
            set { hp = value; }
        }

        /// <summary>
        /// Bonus that will be dropped on death
        /// </summary>
        public Bonus BonusToDrop
        {
            get { return dropOnDeath; }
            set { dropOnDeath = value; }
        }

        /// <summary>
        /// Add those points to score 
        /// </summary>
        public int Points
        {
            get { return points; }
            set { points = value; }
        }

        /// <summary>
        /// Define a path the enemy will follow
        /// </summary>
        public MovePattern Pattern
        {
            get { return movePattern; }
            set { movePattern = value; }
        }

        /// <summary>
        /// Bouncing enemy, like a PONG ball
        /// </summary>
        public bool Bounce
        {
            get { return bounce; }
            set { bounce = value; }
        }

        /// <summary>
        /// If the badguy has made his shown at least one time
        /// </summary>
        public virtual bool IsOnScreen
        {
            get { return onScreen; }
            set { onScreen = value; }
        }

        /// <summary>
        /// Value of the scroll when the bad guys will appear.
        /// This is different from his position !
        /// </summary>
        public Vector2 ScrollValue
        {
            get { return scrollValue; }
            set { scrollValue = value; }
        }

        /// <summary>
        /// Flags that will be set when the enemy die
        /// </summary>
        public String[] Flags
        {
            get { return flagsOnDeath; }
            set { flagsOnDeath = value; }
        }

        /// <summary>
        /// The cue name of the sound when the enemy die
        /// </summary>
        public override String DeathSound
        {
            get { return "defaultKill"; }
        }
    }

}
