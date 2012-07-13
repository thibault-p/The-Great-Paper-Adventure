//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using TGPA.Game.Weapons;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TGPA.Game.Save;
using TGPA.Game.Hitbox;
using TGPA.Audio;
using TGPA.Utils;
using TGPA.SpecialEffects;
using Microsoft.Xna.Framework.GamerServices;
using System;
using TGPA.Game.Other;
using TGPA.Utils.Input;

namespace TGPA
{
    public class Player : Entity
    {
        private Weapon wpn;
        private Weapon bomb;
        private double invincibleTime;
        private int frame;
        private Vector2 trajectory;
        private Device device;
        private bool enableCommands;
        public int Score { get; set; }
        public int Lives { get; set; }
        public PlayerIndex Index { get; set; }
        public double DeathTime { get; set; }
        public String Name { get; set; }

        /// <summary>
        /// GamePad Vibrations
        /// </summary>
        public Rumble[] Rumble { get; set; }

        /// <summary>
        /// Get where the gun of the player's ship is
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public static Vector2 GetGunLocationForPlayer(Player player)
        {
            switch (player.Index)
            {
                case PlayerIndex.One:
                    return new Vector2(player.Location.X + (280 * player.Scale.X), player.Location.Y + (230 * player.Scale.Y));

                case PlayerIndex.Two:
                    return new Vector2(player.Location.X + (170 * player.Scale.X), player.Location.Y + (220 * player.Scale.Y));
            }

            return player.Location;

        }

        /// <summary>
        /// Create a new player object,it will be initialized
        /// </summary>
        public Player(PlayerIndex playerIndex, Device playerDevice) :
            base(new Vector2(0, TGPAContext.Instance.ScreenHeight / 2),
            Rectangle.Empty,
            new Vector2(400.0f, 400.0f),
            new Vector2(0.35f, 0.35f),
            0.0f,
            InfiniteTimeToLive)
        {
            this.Index = playerIndex;
            this.Name = "";

            switch (Index)
            {
                case PlayerIndex.One:
                    sRect = new Rectangle(0, 0, 368, 292);
                    break;

                case PlayerIndex.Two:
                    sRect = new Rectangle(38, 358, 309, 255);
                    break;

            }

            dRect = ComputeDstRect(sRect);

            Rumble = new Rumble[4];
            for (int i = 0; i < Rumble.Length; i++)
            {
                Rumble[i] = new Rumble(0);
            }
            this.Device = playerDevice;
            RumbleAssignation(playerDevice);

            Initialize();
        }

        public void RumbleAssignation(Device playerDevice)
        {
            if (playerDevice.Type != DeviceType.Gamepad) return;

            for (int i = 0; i < Rumble.Length; i++)
            {
                Rumble[i].GamePadIndex = Device.Index;
            }
        }

        public void Initialize()
        {
            wpn = new MachineGun();
            bomb = new Megabomb();
            this.speed = new Vector2(400.0f, 400.0f); //In case of a flip before
            frame = 0;
            Score = 0;
            Lives = 0;
            invincibleTime = 3.0f;
            DeathTime = 0.0f;
            trajectory = new Vector2();
            Hitbox = new CircleHitbox(this, false, 3.5f);
            this.Flip = SpriteEffects.None;
            enableCommands = true;
        }

        public override void Update(GameTime gameTime)
        {
            if (this.Lives >= 0)
            {
                int direction = flips == SpriteEffects.FlipHorizontally ? -1 : 1;

                if (DeathTime > 0.0f)
                {
                    DeathTime -= gameTime.ElapsedGameTime.TotalSeconds;
                }

                frame++;
                base.Update(gameTime);

                UpdateRumbles(gameTime);

                //Reactor fire effects
                if (DeathTime <= 0)
                {
                    Vector2 tloc = location;

                    tloc.Y += DstRect.Height - 40;

                    if (direction < 0)
                    {
                        tloc.X += (dRect.Width - 40);
                    }
                    else
                    {
                        tloc.X += 40;
                    }
                    switch (Index)
                    {
                        case PlayerIndex.One:

                            if (TGPAContext.Instance.Cheatcodes.IsKawaii)
                            {
                                HeartWave kawai = new HeartWave(tloc, RandomMachine.GetRandomVector2(-300f * direction, -150f * direction, -40f, 40f),
                                1f, RandomMachine.GetRandomInt(0, 4));
                                TGPAContext.Instance.ParticleManager.AddParticle(kawai, true);
                            }
                            else
                            {
                                //Fire from engine
                                Smoke s = new Smoke(tloc, RandomMachine.GetRandomVector2(-300f * direction, -150f * direction, -20f, 20f), 1, 1, 1, 1,
                                    1f, RandomMachine.GetRandomInt(0, 4));


                                TGPAContext.Instance.ParticleManager.AddParticle(s, true);
                            }
                            break;


                        case PlayerIndex.Two:

                            //Girly hearts !!!
                            HeartWave kawai2 = new HeartWave(tloc, RandomMachine.GetRandomVector2(-300f * direction, -150f * direction, -40f, 40f),
                                1f, RandomMachine.GetRandomInt(0, 4));
                            TGPAContext.Instance.ParticleManager.AddParticle(kawai2, true);

                            break;
                    }
                }
            }
        }

        public void UpdateRumbles(GameTime gameTime)
        {
            //Vibrations
            for (int i = 0; i < Rumble.Length; i++)
            {
                Rumble[i].Update(gameTime);
            }
        }

        public override void TodoOnDeath()
        {
            Lives--;
            Weapon.UpgradeLevel = 0;
            Invincible = 4.0f;
            DeathTime = 1.5f;

            TGPAContext.Instance.ParticleManager.MakeCircularExplosion(Location, 600f, 100);

            Score += 1;

            Hitbox mine = this.hitbox;
            base.TodoOnDeath();

            if (this.Lives >= 0)
            {
                this.hitbox = mine;
            }
            else
            {
                this.wpn = null;
                this.bomb.Ammo = 0;
            }
        }

        public bool IsPlayingWithAGamepad()
        {
            return this.Device.Type == DeviceType.Gamepad;
        }

        public bool IsPlayingWithAJoystick()
        {
            return this.Device.Type == DeviceType.Joystick;
        }

        public bool IsPlayingOnWindows()
        {
#if WINDOWS
            return true;
#else
            return false;
#endif
        }

        /// <summary>
        /// Primary Weapon
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
                wpn.Owner = this;
                wpn.Flip = this.Flip;
            }
        }

        /// <summary>
        /// Secondary Weapon : bomb
        /// </summary>
        public Weapon Bomb
        {
            get
            {
                return bomb;
            }
            set
            {
                bomb = value;
                wpn.Owner = this;
            }
        }


        /// <summary>
        /// Fire some shots with primary weapon.
        /// Tricky method : return n Tir object, depending on the Weapon.shotNumber
        /// </summary>
        /// <param name="freeSlots"></param>
        /// <returns></returns>
        public List<Shot> Fire()
        {
            wpn.Flip = this.flips;
            List<Shot> shots = wpn.Fire(Player.GetGunLocationForPlayer(this));
            wpn.Owner = this;

            SetRumble(wpn.Rumble);

            return shots;
        }

        /// <summary>
        /// Fire a (or more) bomb, secondary weapon
        /// </summary>
        /// <returns></returns>
        public List<Shot> DropBomb()
        {
            bomb.Flip = this.flips;
            List<Shot> shots = bomb.Fire(Player.GetGunLocationForPlayer(this));
            bomb.Owner = this;

            SetRumble(bomb.Rumble);

            return shots;
        }

        /// <summary>
        /// Vibrations ;)
        /// </summary>
        /// <param name="Rumble"></param>
        public void SetRumble(Vector2 leftright)
        {
            if (this.Lives >= 0)
            {
                if (Device.Type != DeviceType.Gamepad) return;

                for (int i = 0; i < Rumble.Length; i++)
                {
                    Rumble[i].Left = leftright.X;
                    Rumble[i].Right = leftright.Y;
                }
            }
        }

        public double Invincible
        {
            get { return invincibleTime; }
            set { invincibleTime = value; }
        }

        public int Frame
        {
            get { return frame; }
        }

        public Vector2 Trajectory
        {
            set { trajectory = value; }
            get { return trajectory; }
        }

        public Device Device
        {
            get { return device; }
            set
            {
                device = value;
                RumbleAssignation(device);
            }
        }

        public bool EnableCommands
        {
            set
            {
                enableCommands = value;
                trajectory = Vector2.Zero;
            }
            get { return enableCommands; }
        }

        #region Content Management

        private static Texture2D GlobalSprite;

        public static void LoadContent(ContentManager cm)
        {
            GlobalSprite = cm.Load<Texture2D>("gfx/Sprites/player");
        }

        public Texture2D Sprite
        {
            get { return GlobalSprite; }
        }

        #endregion

        public override string DeathSound
        {
            get { return "playerKill"; }
        }

#if XBOX

        public SignedInGamer XboxProfile
        {
            get
            {
                return Device.DeviceProfile(this.Device);
            }
        }
#endif
    }
}
