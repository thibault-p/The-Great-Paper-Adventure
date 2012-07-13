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
using TGPA.SpecialEffects;
using TGPA.Localization;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;

namespace TGPA.Game.Entities
{
    /// <summary>
    /// Simple displayable elements. 
    /// This provide a more simple way to create new background elements that player can destroy / collide with.
    /// </summary>
    public class BackgroundActiveElement : BadGuy
    {
        public static BackgroundActiveElement String2BackgroundActiveElement(String s, Vector2 loc, String parameter)
        {
            if (s == (typeof(Sapin)).Name.ToString())
            {
                return new Sapin(loc, (parameter == null) ? "1,0" : parameter);
            }
            if (s == (typeof(Sapin2)).Name.ToString())
            {
                return new Sapin2(loc, (parameter == null) ? "1,0" : parameter);
            }
            else if (s == (typeof(ExplosifAvalanche)).Name.ToString())
            {
                return new ExplosifAvalanche(loc, parameter);
            }
            else if (s == (typeof(SimpleTrigger)).Name.ToString())
            {
                return new SimpleTrigger(loc, parameter);
            }
            else if (s == (typeof(StartPanel)).Name.ToString())
            {
                return new StartPanel(loc);
            }
            else if (s == (typeof(ToxicRaft)).Name.ToString())
            {
                return new ToxicRaft(loc);
            }
            else if (s == (typeof(BGHitbox)).Name.ToString())
            {
                return new BGHitbox(loc, parameter);
            }
            else if (s == (typeof(DerrickAntenna)).Name.ToString())
            {
                return new DerrickAntenna(loc, parameter);
            }
            else if (s == (typeof(GameEndTrick)).Name.ToString())
            {
                return new GameEndTrick(loc);
            }
            else if (s == (typeof(CommandDisplayer)).Name.ToString())
            {
                return new CommandDisplayer(loc, parameter);
            }

            return null;
        }

        public BackgroundActiveElement(Vector2 location, int hp, Rectangle srcRect, bool Scale)
            : base(location, Vector2.Zero, null, null, null, SpriteEffects.None, Rectangle.Empty, Vector2.Zero, Vector2.One, null)
        {
            //Source sprite
            this.sRect = srcRect;

            //Stats
            this.wpn = null;
            this.hp = hp;
            this.speed = new Vector2(0.0f, 0.0f);

            this.points = 100;
            this.Difficulty = 1;

            //Game Sprite
            this.dRect = ComputeDstRect(sRect);

            this.Background = true;
            this.ttl = InfiniteTimeToLive;
        }

        protected static Texture2D theSpriteSheet;

        /// <summary>
        /// One spritesheet for all elements (they should be simple)
        /// </summary>
        public override Texture2D Sprite
        {
            get
            {
              return theSpriteSheet;
            }
        }

        /// <summary>
        /// One spritesheet for all elements (they should be simple)
        /// </summary>
        public static void LoadBGEContent(ContentManager cm)
        {
            theSpriteSheet = cm.Load<Texture2D>("gfx/Sprites/backgroundElements");
        }

        //*******************************************************************************************
        // Backgrounds elements : small classes, no need to make a file for each
        //*******************************************************************************************
        /// <summary>
        /// Mountain tree
        /// </summary>
        public class Sapin : TGPA.Game.Entities.BackgroundActiveElement
        {
            public Sapin(Vector2 location, String param)
                : this(location, (float)Convert.ToDouble(param))
            {

            }
            public Sapin(Vector2 location, float sc)
                : base(location, 18, new Rectangle(0, 1357, 367, 479),false)
            {
                this.Scale = new Vector2(sc, sc);
                this.Hitbox = new SquareHitbox(this, new Vector2(0.9f, 0.2f));

                int rand = RandomMachine.GetRandomInt(0, 2);
                if (rand == 1)
                {
                    this.Flip = SpriteEffects.FlipHorizontally;
                }
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);
            }
        }

        public class Sapin2 : TGPA.Game.Entities.BackgroundActiveElement
        {
            public Sapin2(Vector2 location, String param)
                : this(location, (float)Convert.ToDouble(param))
            {

            }
            public Sapin2(Vector2 location, float sc)
                : base(location, 18, new Rectangle(371, 1343, 551, 493),false)
            {
                this.Scale = new Vector2(sc, sc);
                this.Hitbox = new SquareHitbox(this, new Vector2(0.9f, 0.2f));

                int rand = RandomMachine.GetRandomInt(0, 2);
                if (rand == 1)
                {
                    this.Flip = SpriteEffects.FlipHorizontally;
                }
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                base.Draw(spriteBatch);
            }

        }



        //*******************************************************************************************
        /// <summary>
        /// Avalanche trigger
        /// </summary>
        public class ExplosifAvalanche : TGPA.Game.Entities.BackgroundActiveElement
        {
            private String flagToSet;

            public ExplosifAvalanche(Vector2 location, String param)
                : base(location, 15, new Rectangle(94, 0, 184, 125),false)
            {
                this.flagToSet = param;
            }

            public override void TodoOnDeath()
            {
                TGPAContext.Instance.Map.Flags.SetFlag(flagToSet);
                TGPAContext.Instance.ParticleManager.MakeCircularExplosion(this.location, 500f, 150);
                base.TodoOnDeath();
            }
        }
        //*******************************************************************************************
        /// <summary>
        /// Simple trigger
        /// </summary>
        public class SimpleTrigger : TGPA.Game.Entities.BackgroundActiveElement
        {
            private String id;

            public SimpleTrigger(Vector2 location, String param)
                : base(location, 1, new Rectangle(0, 128, 300, 204),false)
            {
                this.id = param;
                this.Removable = false;
                this.Background = true;
            }

            public override void TodoOnDeath()
            {
                this.speed = TGPAContext.Instance.Map.Background2.Speed;

                TGPAContext.Instance.Map.Flags.SetFlag(id);
                this.sRect.X = this.sRect.Width; ;
                this.hitbox = new EmptyHitbox(this);
            }
        }
        //*******************************************************************************************
        /// <summary>
        /// Unlighted panel displaying "Start !"
        /// </summary>
        public class StartPanel : TGPA.Game.Entities.BackgroundActiveElement
        {

            public StartPanel(Vector2 location)
                : base(location, Int32.MaxValue, new Rectangle(0, 490, 430, 540),false)
            {
                this.Removable = false;
                this.hitbox = new EmptyHitbox(this);

                UseAnimation = true;
                this.totalFrameNumber = 3;
                this.frameCooldown = 650;
                this.spriteBox = new Vector2(430, 600);
            }
        }
        //*******************************************************************************************
        /// <summary>
        /// Little raft with toxic barels
        /// </summary>
        public class ToxicRaft : TGPA.Game.Entities.BackgroundActiveElement
        {
            private static List<Vector2> barrelsLoc;
            private int deltaX;

            public ToxicRaft(Vector2 location)
                : base(location, 75, new Rectangle(0, 1034, 344, 300),false)
            {
                this.Hitbox = new SquareHitbox(this);

                barrelsLoc = new List<Vector2>();
                barrelsLoc.Add(new Vector2(70, 149));
                barrelsLoc.Add(new Vector2(89, 151));
                barrelsLoc.Add(new Vector2(122, 147));
                barrelsLoc.Add(new Vector2(252, 153));

                this.deltaX = 0;
            }

            public override void Update(GameTime gameTime)
            {
                this.Removable = false;

                if (hp > 0)
                {
                    //Toxic smoke
                    foreach (Vector2 v in barrelsLoc)
                    {
                        Vector2 v2 = v + location;

                        TGPAContext.Instance.ParticleManager.AddParticle(
                            new Smoke(v2, RandomMachine.GetRandomVector2(2f, 10f, 0f, -150f),
                            0.10f, 1f, 0.10f, 1f,
                            RandomMachine.GetRandomFloat(2f, 3f),
                            RandomMachine.GetRandomInt(0, 4))
                        , true);
                    }
                    base.Update(gameTime);
                }
                else
                {
                    Vector2 boomLoc = RandomMachine.GetRandomVector2(dRect.Left, dRect.Right, dRect.Top, dRect.Bottom);
                    TGPAContext.Instance.ParticleManager.MakeExplosion(boomLoc, 20f);

                    for (int i = 0; i < 3; i++)
                    {
                        boomLoc = RandomMachine.GetRandomVector2(TGPAContext.Instance.TitleSafeArea.Left, TGPAContext.Instance.TitleSafeArea.Right, TGPAContext.Instance.TitleSafeArea.Top, TGPAContext.Instance.TitleSafeArea.Bottom);

                        TGPAContext.Instance.ParticleManager.AddParticle(
                            new Smoke(boomLoc, RandomMachine.GetRandomVector2(2f, 10f, 0f, -150f),
                            0.10f, 1f, 0.10f, 1f,
                            RandomMachine.GetRandomFloat(30f, 70f),
                            RandomMachine.GetRandomInt(0, 4))
                        , true);
                    }

                    this.location.Y += 0.25f;
                    this.location.X += this.deltaX;
                    this.deltaX = -this.deltaX;

                    if (TGPAContext.Instance.Map.Ended == Map.EndMode.None)
                    {
                        TGPAContext.Instance.Player1.SetRumble(new Vector2(0.7f, 0.7f));

                        if (TGPAContext.Instance.Player2 != null)
                        {
                            TGPAContext.Instance.Player2.SetRumble(new Vector2(0.7f, 0.7f));
                        }
                    }
                }
            }

            public override void TodoOnDeath()
            {
                TGPAContext.Instance.Map.Flags.SetFlag("toxic");
                this.hitbox = new EmptyHitbox(this);

                this.deltaX = 2;
            }
        }
        //*******************************************************************************************
        /// <summary>
        /// Hitbox for background collision
        /// </summary>
        public class BGHitbox : TGPA.Game.Entities.BackgroundActiveElement
        {
            private static int heightWidth = 56;

            public BGHitbox(Vector2 location, String parameter)
                : base(location, Int32.MaxValue, Rectangle.Empty,false)
            {
                if (parameter == "Left")
                {
                    sRect = new Rectangle(0, 0, heightWidth, TGPAContext.Instance.ScreenHeight);
                }
                else if (parameter == "Up")
                {
                    sRect = new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, heightWidth);
                }
                else if (parameter == "Right")
                {
                    sRect = new Rectangle(TGPAContext.Instance.ScreenWidth - heightWidth, 0, heightWidth, TGPAContext.Instance.ScreenHeight);
                }
                else if (parameter == "Down")
                {
                    sRect = new Rectangle(0, TGPAContext.Instance.ScreenHeight - heightWidth, TGPAContext.Instance.ScreenWidth, heightWidth);
                }
                else
                {
                    throw new Exception("Invalid BGHitbox parameter : " + parameter);
                }
                dRect = ComputeDstRect(sRect);

                this.Hitbox = new SquareHitbox(this);
                this.ttl = InfiniteTimeToLive;
                this.Removable = false;
            }

            public override void Update(GameTime gameTime)
            {
                if (location.X < 0)
                {
                    this.Speed = Vector2.Zero;
                    this.Background = false;
                    this.location.X = 0;
                }

                base.Update(gameTime);
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {

            }
        }
        //******************************************************************************************                            
        /// <summary>
        /// Boss 2 Panel
        /// </summary>
        public class ElFeroPanel : TGPA.Game.Entities.BackgroundActiveElement
        {
            public ElFeroPanel(Vector2 location)
                : base(location, Int32.MaxValue, new Rectangle(660, 0, 625, 485),false)
            {
                this.hitbox = new EmptyHitbox(this);
            }
        }
        //*******************************************************************************************
        /// <summary>
        /// Boss 5 Antenna
        /// </summary>
        public class DerrickAntenna : TGPA.Game.Entities.BackgroundActiveElement
        {
            public DerrickAntenna(Vector2 location, String param)
                : base(location, 100, new Rectangle(1350, 910, 55, 200),false)
            {
                this.flagsOnDeath = new String[] { param };
            }

            public override void Update(GameTime gameTime)
            {
                //Shine
                TGPAContext.Instance.ParticleManager.MakeBullet(new Vector2(this.dRect.Center.X+10,this.dRect.Top+20), RandomMachine.GetRandomVector2(-4f, 4f, -4f, 4f), true);

                base.Update(gameTime);
            }
        }
        //*******************************************************************************************
        //*******************************************************************************************
        /// <summary>
        /// End of the game
        /// </summary>
        public class GameEndTrick : TGPA.Game.Entities.BackgroundActiveElement
        {
            private float cooldown, alphaFadeOut;

            public GameEndTrick(Vector2 location)
                : base(location, Int32.MaxValue, Rectangle.Empty,false)
            {
                this.cooldown = 0f;
                this.alphaFadeOut = 0f;
            }

            public override void Update(GameTime gameTime)
            {
                TGPAContext.Instance.Player1.EnableCommands = false;

                if (TGPAContext.Instance.Player2 != null)
                {
                    TGPAContext.Instance.Player2.EnableCommands = false;
                }

                this.alphaFadeOut += 0.005f;

                if (this.cooldown <= 0f)
                {
                    this.cooldown = RandomMachine.GetRandomFloat(50f, 300f);

                    Vector2 loc = RandomMachine.GetRandomVector2(0, TGPAContext.Instance.ScreenWidth, 0, TGPAContext.Instance.ScreenHeight);

                    TGPAContext.Instance.ParticleManager.MakeCircularExplosion(loc, 150f, 200);
                }
                else
                {
                    this.cooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                }

                base.Update(gameTime);
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                spriteBatch.Draw(TGPAContext.Instance.NullTex,
                    new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), Color.White *alphaFadeOut);

                spriteBatch.End();

                base.Draw(spriteBatch);
            }
        }
        //*******************************************************************************************
        /// <summary>
        /// Command displayer.
        /// Display a command on the top of player ship
        /// </summary>
        public class CommandDisplayer : TGPA.Game.Entities.BackgroundActiveElement
        {
            private String button;
            private float alphaColor;
            private Vector2 locationP1;
            private bool decreaseAlpha;

            public CommandDisplayer(Vector2 location, String param)
                : base(location, Int32.MaxValue, Rectangle.Empty,false)
            {
                this.button = param;
                this.alphaColor = 1f;
                this.decreaseAlpha = false;

                this.locationP1 = Vector2.Zero;
            }

            public override void Update(GameTime gameTime)
            {
                if (decreaseAlpha)
                {
                    this.alphaColor -= 0.005f;

                    if (this.alphaColor <= 0f)
                    {
                        this.ttl = 0f;
                        TGPAContext.Instance.Map.Flags.SetFlag(button);
                    }
                    else if (this.alphaColor <= 0.7f)
                    {
                        this.alphaColor -= 0.01f;
                    }
                }

                locationP1 = new Vector2(
                    TGPAContext.Instance.Player1.Location.X - 50,
                    TGPAContext.Instance.Player1.Location.Y - 75
                    );

                //if (!decreaseAlpha)
                //    TGPAContext.Instance.PManager.MakeBullet(locationP1 + new Vector2(125, 0), RandomMachine.GetRandomVector2(-4f, 4f, -4f, 4f), false);

                //Check for input
                if (!decreaseAlpha)
                {
                    switch (button)
                    {
                        case "#Move":
                            decreaseAlpha = (TGPAContext.Instance.InputManager.PlayerGoDown(TGPAContext.Instance.Player1) > 0f)
                                || (TGPAContext.Instance.InputManager.PlayerGoUp(TGPAContext.Instance.Player1) > 0f)
                                || (TGPAContext.Instance.InputManager.PlayerGoLeft(TGPAContext.Instance.Player1)> 0f)
                                || (TGPAContext.Instance.InputManager.PlayerGoRight(TGPAContext.Instance.Player1)> 0f);
                            break;

                        case "#Fire":
                            decreaseAlpha = TGPAContext.Instance.InputManager.PlayerPressButtonFire(TGPAContext.Instance.Player1);
                            break;

                        case "#Bomb":
                            decreaseAlpha = TGPAContext.Instance.InputManager.PlayerPressButtonBomb(TGPAContext.Instance.Player1);
                            break;
                    }
                }

                base.Update(gameTime);
            }

            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, button, TGPAContext.Instance.Player1.Device.Type, locationP1, Color.White *alphaColor);

                TGPAContext.Instance.TextPrinter.Color = Color.Black *alphaColor;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,locationP1.X + ((button == "#Move" && TGPAContext.Instance.Player1.Device.Type == DeviceType.KeyboardMouse) ? 150 : 50), locationP1.Y, LocalizedStrings.GetString(button.Replace("#", "Player")), 128);
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }
        //*******************************************************************************************
    }
}
