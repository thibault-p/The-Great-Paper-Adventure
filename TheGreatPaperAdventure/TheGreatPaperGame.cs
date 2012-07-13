//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//                             http://www.thegreatpaperadventure.com
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Threading;
using EasyStorage;
using MapEditor;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGPA.Audio;
using TGPA.Game;
using TGPA.Game.Entities;
using TGPA.Game.Graphics;
using TGPA.Game.Hitbox;
using TGPA.Game.Sound;
using TGPA.Game.Weapons;
using TGPA.Maps;
using TGPA.MenuItems;
using TGPA.Screens;
using TGPA.Screens.Controls;
using TGPA.Utils;
using TGPA.Utils.Input;
using System.Diagnostics;

namespace TGPA
{
    /// <summary>
    /// State of this program
    /// </summary>
    public enum GameState
    {
        /// <summary>
        /// Loading program
        /// </summary>
        None,
        /// <summary>
        /// Exit program
        /// </summary>
        Exit,
        /// <summary>
        /// Play, Options or exit
        /// </summary>
        TitleScreen,
        /// <summary>
        /// Choose the level to play
        /// </summary>
        LevelSelectionScreen,
        /// <summary>
        /// The game
        /// </summary>
        Game,
        /// <summary>
        /// Display a loading screen
        /// </summary>
        Loading,
        /// <summary>
        /// Display credits
        /// </summary>
        Credits,
        /// <summary>
        /// End of demo
        /// </summary>
        DemoEnd
    }

    /// <summary>
    /// The Great Paper Adventure Of The Nameless In A Fantastic And Papered World ! 
    /// http://www.thegreatpaperadventure.com
    /// </summary>
    public class TheGreatPaperGame : Microsoft.Xna.Framework.Game
    {
        /// <summary>
        /// Displayed version
        /// </summary>
        public static String Version = "1.0.7.0";

        /// <summary>
        /// Air friction for player
        /// </summary>
        public static float Friction = 2500.0f;

        /// <summary>
        /// XNA Graphics manager
        /// </summary>
        private GraphicsDeviceManager graphics;
        /// <summary>
        /// Custom magic sprite classes
        /// </summary>
        private TGPASpriteBatch spriteBatch;
        /// <summary>
        /// For code lisibility
        /// </summary>
        private TGPAContext context;

        /// <summary>
        /// Gamepad sprite
        /// </summary>
        private Texture2D gamePadSprite;
        private Rectangle padSrc;

#if WINDOWS
        /// <summary>
        /// Current mouse position
        /// </summary>
        private Point mouseXY;
        /// <summary>
        /// Sprite for mouse
        /// </summary>
        private Rectangle mouseSrc;
#endif

        /// <summary>
        /// Total Shots number shot by player
        /// </summary>
        private int totalshots;
        /// <summary>
        /// Total of context.Enemies killed by player in a game
        /// 
        private int enemieskilled;

        /// <summary>
        /// Player datas
        /// </summary>
        private Player tmpPlayer2;

        private List<Shot> shotsToDelete;
        private List<BadGuy> enemiesToDelete;

        /// <summary>
        /// Scores displayed on screen after enemie's death
        /// </summary>
        private List<FlyingScores> flyScores;

        /// <summary>
        /// Title screen
        /// </summary>
        private TitleScreen titleScreen;
        /// <summary>
        /// Selection level screen
        /// </summary>
        private LevelSelectionScreen levelSelectionScreen;
        /// <summary>
        /// End level screen
        /// </summary>
        private EndLevelScreen endLevelScreen;
        /// <summary>
        /// Little splashscreen
        /// </summary>
        private SplashScreen splashScreen;
        /// <summary>
        /// Credits screen
        /// </summary>
        private CreditsScreen creditScreen;
        /// <summary>
        /// End of the demo
        /// </summary>
        private DemoEndScreen demoEndScreen;

        /// <summary>
        /// Loading screen
        /// </summary>
        private LoadingScreen loadingLayer;

        /// <summary>
        /// Game is paused
        /// </summary>
        private bool paused;

        private bool disconnectDetected;

        /// <summary>
        /// diconnected pads ?
        /// </summary>
        private bool padsOk;

        /// <summary>
        /// It's back !
        /// </summary>
        private float darkening;

        /// <summary>
        /// Pushed start in the start menu
        /// </summary>
        private bool pushedStart;

        #region Initialization

        /// <summary>
        /// Game general initialization
        /// </summary>
        public TheGreatPaperGame()
        {
            context = TGPAContext.Instance;
            context.GameStateChanged += new Action(this.GamestateChanged); ;
            context.EndLevel = this.LaunchEndLevel;
            context.NextLevel = this.NextLevel;
            context.PrepareGame = this.PrepareGame;

            //Create a log file
            //************************************************************************
            string filePath = "";
#if WINDOWS
            //Logging system init
            string sgPath = System.IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Saved Games");
            sgPath = System.IO.Path.Combine(sgPath, "The Great Paper Adventure");

            filePath = System.IO.Path.Combine(sgPath, "tgpa.log");

            if (System.IO.Directory.Exists(sgPath) == false)
            {
                System.IO.Directory.CreateDirectory(sgPath);
            }
#endif
            Logger.Initialization(filePath + "");

            padSrc = new Rectangle(7, 3, 141, 88);

            //Load saved game (+ options)
            //************************************************************************
            this.InitSaveDevice();

#if WINDOWS
            Logger.Active = context.Saver.OptionsData.EnableLog;
#endif

            //Create screens
            //************************************************************************
            titleScreen = new TitleScreen();
            levelSelectionScreen = new LevelSelectionScreen();
            endLevelScreen = new EndLevelScreen();
            splashScreen = new SplashScreen();
            creditScreen = new CreditsScreen();
            demoEndScreen = new DemoEndScreen();

            //Other initialization
            //************************************************************************
            //Init video
            graphics = new GraphicsDeviceManager(this);
            context.Graphics = graphics;

            paused = false;
            disconnectDetected = false;
            Content.RootDirectory = "Content";

            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.English);

#if XBOX
            Components.Add(new GamerServicesComponent(this));

            this.Activated += new EventHandler<EventArgs>(TheGreatPaperGame_Activated);
#else 
             //TODO DMA : Mais à quoi ça sert ça ?!
            //mouseXY = new Point((int)(context.ScreenWidth / 2) - 290, (int)(context.ScreenHeight / 2) - 360);
            mouseSrc = new Rectangle(800, 70, 25, 35); //sprite to draw the mouse
#endif

            //Audio
            //************************************************************************
            SoundEngine.InitSoundEngineInstance();
        }

        void TheGreatPaperGame_Activated(object sender, EventArgs e)
        {
            //Here is the trick !
            context.IsTrialMode = Guide.IsTrialMode;
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            Logger.Log(LogLevel.Info, "Initializing Game Data");

            GameInit();

            base.Initialize();

        }

        private void GameInit()
        {
            totalshots = 0;
            enemieskilled = 0;

            context.Shots.Clear();
            context.Enemies.Clear();

            flyScores = new List<FlyingScores>();
            shotsToDelete = new List<Shot>();
            enemiesToDelete = new List<BadGuy>();

            context.Bonuses = new List<Bonus>();
            context.Hud = new HUD();

            paused = false;

            if (TGPAContext.Instance.ParticleManager != null)
            {
                TGPAContext.Instance.ParticleManager.Clear();
            }
        }

        /// <summary>
        /// Initialize required components for saving game.
        /// This method is not called at the same moment if you're on PC or Xbox
        /// </summary>
        public void InitSaveDevice()
        {
#if XBOX
            EasyStorageSettings.SetSupportedLanguages(Language.French, Language.Spanish, Language.English, Language.German, Language.Japanese, Language.Italian);

            SharedSaveDevice sharedSaveDevice = new SharedSaveDevice();
            Components.Add(sharedSaveDevice);

            TGPAContext.Instance.SaveDevice = sharedSaveDevice;

            sharedSaveDevice.DeviceSelectorCanceled += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceDisconnected += (s, e) => e.Response = SaveDeviceEventResponse.Force;
            sharedSaveDevice.DeviceSelected += new EventHandler<EventArgs>(DeviceSelectedEventResponse);

            sharedSaveDevice.PromptForDevice();
#else
            context.Saver.Load();
#endif
        }

#if XBOX

        /// <summary>
        /// Player disconnecting profile : Force reconnect
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckSignedOut()
        {
            int deconnectedElements = 0;
            int playerNumber = 1;

            //Force reconnection
            //Player 1
            if (context.Player1 != null)
            {
                if (Device.DeviceHasProfile(context.Player1.Device) == false)
                {
                    deconnectedElements++;
                }

                if (context.Player2 != null)
                {
                    playerNumber++;

                    if (Device.DeviceHasProfile(context.Player2.Device) == false)
                    {
                        deconnectedElements++;
                    }
                }
            }

            if (deconnectedElements > 0)
            {
                if (Guide.IsVisible == false)
                {
                    Guide.ShowSignIn(playerNumber, false);
                }
            }

        }
#endif

        /// <summary>
        /// Action called when a device has been selected
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        public void DeviceSelectedEventResponse(object sender, EventArgs args)
        {
            context.Saver.Load();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Logger.Log(LogLevel.Info, "Loading main game content");

            // Create a new SpriteBatch, which can be used to draw textures.
            SpriteBatch xnaSpriteBatch = new SpriteBatch(GraphicsDevice);
            spriteBatch = new TGPASpriteBatch(xnaSpriteBatch);

            //Graphics setup
            //************************************************************************
            #region Graphics setup

            int realWidth = 1024;
            int realHeight = 768;
            bool fullScreen = true;

#if WINDOWS
            // Windows specific code 
            // We use resolution indenpedant feature so the game should always believe it is 1024*768
            realWidth = context.Saver.OptionsData.Width;
            realHeight = context.Saver.OptionsData.Height;
            fullScreen = context.Saver.OptionsData.FullScreen;

            TGPAContext.Instance.TitleSafeArea = new Viewport(0, 0, 1024, 768).TitleSafeArea;
#else
            // XBOX360 specific code 
            int width = this.Window.ClientBounds.Width; //Use best resolution
            int height = this.Window.ClientBounds.Height;

            if (width < 1024) width = 1024; //Scale on old TV
            if (height < 768) height = 768; //HACK : Force 768 to avoid bugs

            realWidth = width;
            realHeight = height;
            fullScreen = true;
#endif

            graphics.PreferMultiSampling = true;

            Logger.Log(LogLevel.Info, "Trying to apply resolution " + realWidth + "x" + realHeight + " Fullscreen : " + fullScreen);

            Resolution.Initialize(graphics, realWidth, realHeight, context.ScreenWidth, context.ScreenHeight, fullScreen);

            #endregion

            //Load global resources
            context.LoadContent(Content);

            //Sprites for player and messages
            Player.LoadContent(Content);
            HUD.LoadWarningContent(Content);
            BackgroundActiveElement.LoadBGEContent(Content);

            gamePadSprite = Content.Load<Texture2D>(@"gfx/DeviceSelection/buttons");

            //For debug infos
            Node.LoadContent(Content);

            TGPAControl.LoadContent(Content);
            ListElement.LoadContent(Content);

            //Menu screens
            LoadingScreen.LoadContent(Content);
            titleScreen.LoadContent(Content);
            levelSelectionScreen.LoadContent(Content);
            endLevelScreen.LoadContent(Content);
            splashScreen.LoadContent(Content);
            creditScreen.LoadContent(Content);
            demoEndScreen.LoadContent(Content);

            //Sounds
            MusicEngine.Instance.LoadContent(Content);

            Logger.Log(LogLevel.Info, "Loading content done");
        }


        #endregion

        #region Unload Content

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            if (context.Map != null)
            {
                context.Map.UnloadMapContent();
            }

            Content.Unload();
        }

        protected override void Dispose(bool disposing)
        {
            Logger.Log(LogLevel.Info, "Exiting The Great Paper Adventure");

            base.Dispose(disposing);
        }

        #endregion

        #region Updating game

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            context.IsActive = this.IsActive;
            context.InputManager.StartUpdate();

            PlayerIndex disconnectedIndex = PlayerIndex.Four;

            if (context.Player1 != null)
            {
                padsOk = context.InputManager.CheckIfPadIsConnectedForPlayer(context.Player1);
                disconnectedIndex = context.Player1.Index;
            }
            else
            {
                padsOk = true;
            }
            if (context.Player2 != null)
            {
                padsOk &= context.InputManager.CheckIfPadIsConnectedForPlayer(context.Player2);
                disconnectedIndex = context.Player1.Index;
            }

            if (!padsOk)
            {
                darkening = 0.95f;

                if (TGPAContext.Instance.CurrentGameState == GameState.Game)
                {
                    if (TGPAContext.Instance.Map.Ended == Map.EndMode.None)
                    {
                        paused = true;
                    }

                }
                context.PausingPlayerIndex = disconnectedIndex;
            }


#if WINDOWS
            //Get mouse location
            mouseXY = new Point(Mouse.GetState().X, Mouse.GetState().Y);
#endif

#if XBOX
            CheckSignedOut();
#endif
            if (context.CurrentGameState == GameState.None)
            {
                splashScreen.Update(gameTime);
            }
            //*********************************************************************************
            // Device selection screen
            //*********************************************************************************
            else
            {
                context.SongInfo.Update();

                //*********************************************************************************
                // Title screen
                //*********************************************************************************
                if (context.CurrentGameState == GameState.TitleScreen)
                {
                    context.ParticleManager.Clear();

                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);

                    titleScreen.Update(gameTime);
                }
                //*********************************************************************************
                // Level Selection screen
                //*********************************************************************************
                else if (context.CurrentGameState == GameState.LevelSelectionScreen)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);

                    levelSelectionScreen.Update(gameTime);
                }
                //*********************************************************************************
                // Loading screen
                //*********************************************************************************
                else if (context.CurrentGameState == GameState.Loading)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);

                    loadingLayer.Update(gameTime);
                }
                //*********************************************************************************
                // Credit screen
                //*********************************************************************************
                else if (context.CurrentGameState == GameState.Credits)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);

                    creditScreen.Update(gameTime);
                }
                //*********************************************************************************
                // End demo screen
                //*********************************************************************************
                else if (context.CurrentGameState == GameState.DemoEnd)
                {
                    GamePad.SetVibration(PlayerIndex.One, 0f, 0f);
                    GamePad.SetVibration(PlayerIndex.Two, 0f, 0f);

                    demoEndScreen.Update(gameTime);
                }
                //*********************************************************************************
                // The game
                //*********************************************************************************
                else if (context.CurrentGameState == GameState.Game)
                {
                    //If the game isn't on focus or if gamepad is disconnected the code far below will not be executed
                    if (!IsActive)
                    {
                        if (context.Player1 != null)
                        {
                            context.InputManager.SetVibrations(PlayerIndex.One, Vector2.Zero);
                            context.Player1.UpdateRumbles(gameTime);
                        }
                        if (context.Player2 != null)
                        {
                            context.InputManager.SetVibrations(PlayerIndex.Two, Vector2.Zero);
                            context.Player2.UpdateRumbles(gameTime);
                        }

                        //Put game in pause
                        if (this.context.CurrentGameState == GameState.Game)
                        {
                            if (context.Map.Ended == Map.EndMode.None)
                            {
                                this.paused = true;
                            }
                        }
                    }

                    if (context.Map.StartTime <= 0)
                        context.Map.StartTime = gameTime.TotalGameTime.TotalMilliseconds;

                    if (paused)
                    {
                        darkening += 0.05f;
                        if (darkening > 0.8f) darkening = 0.8f;

                        UpdatePlayerPause(gameTime, context.PausingPlayerIndex);

                        if (context.Cheatcodes.Active)
                        {
                            context.Cheatcodes.Update(gameTime);
                        }
                    }
                    else
                    {
                        //Particles
                        context.ParticleManager.UpdateParticles((float)gameTime.ElapsedGameTime.TotalSeconds);

                        UpdateMap(context.Map.Background1, gameTime);
                        UpdateMap(context.Map.Background2, gameTime);
                        UpdateMap(context.Map.Background3, gameTime);

                        if (context.Map.Ended != Map.EndMode.Lose)
                        {
                            UpdatePlayer(gameTime, PlayerIndex.One);

                            if (context.Player2 != null)
                            {
                                UpdatePlayer(gameTime, PlayerIndex.Two);
                            }
                            else
                            {
                                if (tmpPlayer2 != null)
                                {
#if XBOX
                                    if (Gamer.SignedInGamers.Count >= 2)
                                    {
#endif
                                        Logger.Log(LogLevel.Info, "Adding player 2 on device " + tmpPlayer2.Device);

                                        context.Player2 = new Player(PlayerIndex.Two, tmpPlayer2.Device);
                                        this.tmpPlayer2 = null;
                                        InitializeP2();
#if XBOX
                                        context.Player2.Name = context.Player2.XboxProfile.Gamertag;
#else
                                    context.Player2.Name = LocalizedStrings.GetString("Guest");
#endif
                                        //Take one live to player 1
                                        int initLives = context.Player1.Lives;
                                        context.Player1.Lives -= 1;
                                        context.Player2.Lives = context.Player1.Lives;
#if XBOX
                                    }
                                    else
                                    {
                                        if (!Guide.IsVisible)
                                        {
                                            tmpPlayer2 = null;
                                        }
                                    }
#endif
                                }
                                else if (context.Player1.Lives > 1)
                                {
                                    //Scan if P2 pressed Start Button
                                    Device newPlayerDevice = context.InputManager.Player2JoinedTheGame(context.Player1);

                                    //Player 2 joined the game !
                                    if (newPlayerDevice != null)
                                    {
                                        //Create variable
                                        this.tmpPlayer2 = new Player(PlayerIndex.Two, newPlayerDevice);
#if XBOX
                                        //Force sign in on XBOX
                                        if ((!this.paused) && (Device.DeviceHasProfile(newPlayerDevice) == false))
                                        {
                                            this.paused = true;
                                            this.context.PausingPlayerIndex = PlayerIndex.One;
                                            Guide.ShowSignIn(2, false);
                                        }
#endif
                                    }
                                }
                            }
                        }
                        else
                        {//Rumble update
                            context.Player1.Update(gameTime);

                            if (context.Player2 != null)
                                context.Player2.Update(gameTime);
                        }

                        UpdateShots(gameTime);

                        //Scripts
                        List<Event> eventsToDelete = new List<Event>();

                        for (int i = 0; i < context.Map.Events.Count; i++)
                        {
                            //Run
                            if ((Math.Abs(context.Map.Events[i].ScrollValue.X) - Math.Abs(context.Map.Scroll.X) <= 0)
                             && (Math.Abs(context.Map.Events[i].ScrollValue.Y) - Math.Abs(context.Map.Scroll.Y) <= 0))
                            {
                                context.Map.Events[i].PlayEvent(gameTime);
                            }

                            //Delete ended scripts
                            if (context.Map.Events[i].IsEnded)
                            {
                                eventsToDelete.Add(context.Map.Events[i]);
                            }

                        }

                        foreach (Event e in eventsToDelete)
                            context.Map.Events.Remove(e);


                        UpdateEnemies(gameTime);

                        UpdateBonuses(gameTime);

                        //Scores
                        foreach (FlyingScores fs in flyScores)
                        {
                            fs.Update(gameTime);
                        }

                        //End level ?
                        if (context.Map.Ended != Map.EndMode.None)
                        {
                            endLevelScreen.Update(gameTime);
                        }

                        if (darkening > 0.0f)
                        {
                            darkening -= 0.2f;
                            if (darkening < 0.0f) darkening = 0f;
                        }

                        //HUD
                        context.Hud.Update(gameTime);

                        //Clear lists every 5 sec
                        if (true)
                        {
                            foreach (BadGuy bg in enemiesToDelete)
                            {
                                context.Enemies.Remove(bg);
                            }
                            enemiesToDelete.Clear();

                            foreach (Shot shiot in shotsToDelete)
                            {
                                context.Shots.Remove(shiot);
                            }
                            shotsToDelete.Clear();
                        }
                    }
                }
                else if (context.CurrentGameState == GameState.Exit)
                {
                    this.Exit();
                }

                SoundEngine.Instance.Update(gameTime);
            }
            base.Update(gameTime);

            context.InputManager.EndUpdate();
        }


        private void GamestateChanged()
        {
            //Gamestate should be changed
            switch (context.CurrentGameState)
            {
                case GameState.Credits:
                    LaunchCreditsScreen();
                    break;

                case GameState.DemoEnd:
                    LaunchEndDemoScreen();
                    break;

                case GameState.Game:
                    LaunchGame();
                    break;

                case GameState.LevelSelectionScreen:
                    LaunchLevelSelectionScreen();
                    break;

                case GameState.TitleScreen:
                    if (pushedStart) LaunchTitleScreen(false);
                    else { pushedStart = true; LaunchTitleScreen(true); }
                    break;
            }
        }

        /// <summary>
        /// Scrolling
        /// </summary>
        /// <param name="?"></param>
        /// <param name="?"></param>
        protected void UpdateMap(ScrollingBackground bg, GameTime gameTime)
        {
            if (bg == null) return;

            bg.Update(gameTime);
        }

        /// <summary>
        /// Wait for press on "Pause" touch and display players controllers
        /// </summary>
        protected void UpdatePlayerPause(GameTime gameTime, PlayerIndex playerIndex)
        {
            bool keyPause = false;
            bool keyEsc = false;
            bool keyCheat = false;
            bool keyRetryLevel;

            Player player = context.GetPlayer(playerIndex);
            KeyboardState keyboard = Keyboard.GetState();

            player.SetRumble(Vector2.Zero);

            keyPause = context.InputManager.PlayerPressButtonPause(player);
            keyEsc = context.InputManager.PlayerPressButtonEsc(player);
            keyCheat = context.InputManager.PlayerPressButtonCheat(player);
            keyRetryLevel = context.InputManager.PlayerPressButtonRetryLevel(player);

            if (keyPause)
            {
                paused = false;
                MusicEngine.Instance.Resume();
            }

            if (keyEsc)
            {
                paused = false;
                TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
            }

            if ((keyRetryLevel) && (context.Cheatcodes.Active == false))
            {
                paused = false;
                NextLevel(false);
            }

            if ((keyCheat) && (context.Cheatcodes.Active == false))
            {
                context.Cheatcodes.Update(gameTime);
            }

            player.UpdateRumbles(gameTime);
        }

        /// <summary>
        /// Check input, move player's ship, fire, bomb, ...
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdatePlayer(GameTime gameTime, PlayerIndex playerIndex)
        {
            if (!this.IsActive) return;

            Player player = context.GetPlayer(playerIndex);

            int livesBeforeUpdate = player.Lives;

            float keyLeft = 0.0f;
            float keyRight = 0.0f;
            float keyUp = 0.0f;
            float keyDown = 0.0f;
            bool keyAttack = false;
            bool keyBomb = false;
            bool keyEsc = false;
            bool keyPause = false;
            bool keySkipMessage = false;

            //Care of time
            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            player.Invincible -= elapsedTime;
            if (player.Invincible < 0) player.Invincible = 0;

            if (context.Cheatcodes.IsGiantMode)
            {
                player.Scale = new Vector2(0.2f, 0.2f);
            }

            //Input Management
            //--------------------------

            #region Update player location

            //Update player location
            Vector2 location = player.Location;
            location += player.Trajectory * elapsedTime;

            //Update trajectory by decreasing it with the friction
            Vector2 traj = player.Trajectory;

            if (traj.X > 0)
            {
                traj.X -= Friction * elapsedTime;
                if (traj.X < 0f) traj.X = 0f;
            }

            if (traj.X < 0)
            {
                traj.X += Friction * elapsedTime;
                if (traj.X > 0f) traj.X = 0f;
            }

            if (traj.Y > 0)
            {
                traj.Y -= Friction * elapsedTime;
                if (traj.Y < 0f) traj.Y = 0f;
            }

            if (traj.Y < 0)
            {
                traj.Y += Friction * elapsedTime;
                if (traj.Y > 0f) traj.Y = 0f;
            }

            #endregion

            #region Input Management for moving player

            keyLeft = context.InputManager.PlayerGoLeft(player);
            keyRight = context.InputManager.PlayerGoRight(player);
            keyUp = context.InputManager.PlayerGoUp(player);
            keyDown = context.InputManager.PlayerGoDown(player);
            keyAttack = context.InputManager.PlayerPressButtonFire(player);
            keyBomb = context.InputManager.PlayerPressButtonBomb(player);
            keyPause = context.InputManager.PlayerPressButtonPause(player) && player.Lives >= 0; //Player has to be alive to pause the game
            keySkipMessage = context.InputManager.PlayerPressButtonConfirm(player); //A to skip message

            //Set trajectory to move player
            Vector2 direction = Vector2.One;
            direction.X = player.Flip == SpriteEffects.FlipHorizontally ? -1 : 1;
            direction.Y = player.Flip == SpriteEffects.FlipVertically ? -1 : 1;

            if (player.EnableCommands)
            {
                //If player is dead : no attack, no move
                if (player.DeathTime <= 0)
                {
                    if (keyLeft > 0f)
                    {
                        traj.X = -player.Speed.X * direction.X * keyLeft;
                    }
                    else if (keyRight > 0f)
                    {
                        traj.X = player.Speed.X * direction.X * keyRight;
                    }
                    if (keyUp > 0f)
                    {
                        traj.Y = -player.Speed.Y * direction.Y * keyUp;
                    }
                    else if (keyDown > 0f)
                    {
                        traj.Y = player.Speed.Y * direction.Y * keyDown;
                    }
                }
#if DEBUG
                if (context.InputManager.PlayerPressDebugButton(player))
                    context.Options.DebugMode = !context.Options.DebugMode; //Debug Mode On/Off
                if (context.InputManager.PlayerPressXButton(player))
                    context.Map.Flags.SetFlag("playerdie"); //Instant death
                if (context.InputManager.PlayerPressYButton(player))
                    context.Map.Flags.SetFlag("endlevel"); //Instant win
#endif

                player.Trajectory = traj;

                if (disconnectDetected) { paused = true; keyPause = true; }

            #endregion

                //Return to menu if player press Esc button
                if (keyEsc)
                {
                    TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
                }

                if (keyPause)
                {
                    if (!paused)
                    {
                        paused = true;
                        context.PausingPlayerIndex = playerIndex;
                        MusicEngine.Instance.Pause();

                        context.Cheatcodes.Initialize();
                    }
                }

                //If player is dead : no attack, no move
                if (player.DeathTime <= 0)
                {
                    //Fire bombs or bullets
                    if (keyAttack)
                    {
                        if (context.Cheatcodes.HasMaxPower)
                        {
                            player.Weapon.UpgradeLevel = 5;
                            player.Weapon.Ammo = Weapon.InfiniteAmmo;
                        }

                        if (player.Weapon.Ammo <= 0)
                        {
                            player.Weapon = Weapon.TypeToWeapon(context.Map.InitialWeapon.GetType());
                            context.Hud.ChangeWeaponIcon(player.Index, player.Weapon.GetType());
                        }

                        if ((player.Weapon.LastShot == 0) || (gameTime.TotalGameTime.TotalMilliseconds - player.Weapon.LastShot >= player.Weapon.Cooldown))
                        {

                            List<Shot> tmp = null;

                            //Primary attack
                            tmp = player.Fire();
                            player.Weapon.TodoOnFiring(Player.GetGunLocationForPlayer(player));

                            context.Shots.AddRange(tmp);
                            totalshots += tmp.Count;
                            player.Weapon.LastShot = gameTime.TotalGameTime.TotalMilliseconds;
                        }

                    }
                    if (keyBomb)
                    {
                        if (player.Bomb.Ammo > 0)
                        {
                            if ((player.Bomb.LastShot == 0) || (gameTime.TotalGameTime.TotalMilliseconds - player.Bomb.LastShot >= player.Bomb.Cooldown))
                            {

                                List<Shot> tmp = null;

                                //Primary attack
                                tmp = player.DropBomb();
                                player.Bomb.TodoOnFiring(Player.GetGunLocationForPlayer(player));

                                context.Shots.AddRange(tmp);
                                totalshots += tmp.Count;
                                player.Bomb.LastShot = gameTime.TotalGameTime.TotalMilliseconds;


                                if (context.Cheatcodes.HasMaxPower)
                                {
                                    player.Bomb.Ammo++;
                                }
                            }
                        }
                    }
                }

                //Bounds
                if (location.X < 0)
                    location.X = 0;
                else if (location.X > context.ScreenWidth - player.DstRect.Width)
                    location.X = context.ScreenWidth - player.DstRect.Width;

                if (location.Y < 0)
                    location.Y = 0;
                else if (location.Y > context.ScreenHeight - player.DstRect.Height)
                    location.Y = context.ScreenHeight - player.DstRect.Height;

                CollisionPlayer(gameTime, PlayerIndex.One);

                if (context.Player2 != null)
                    CollisionPlayer(gameTime, PlayerIndex.Two);
            }
            if (livesBeforeUpdate == player.Lives)
            {
                player.Location = location;
            }

            //Basic Entity update
            player.Update(gameTime);
        }

        /// <summary>
        /// Move context.Shots, make them disapear if they have to
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateShots(GameTime gameTime)
        {
            //Care of time
            float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (tempsEcoule > 1) tempsEcoule = 1;

            for (int i = 0; i < context.Shots.Count; i++)
            {
                if (shotsToDelete.Contains(context.Shots[i])) continue;

                //Shots can have a time to live
                if (context.Shots[i].TimeToLive != Shot.InfiniteTimeToLive)
                {
                    context.Shots[i].TimeToLive -= gameTime.ElapsedGameTime.Milliseconds;

                    //Kill shot if no more time to live on this beautiful binary world
                    if (context.Shots[i].TimeToLive <= 0)
                    {
                        context.Shots[i].TodoOnDeath();
                        context.Shots.RemoveAt(i);
                        continue;
                    }
                }

                bool killed = false;

                //Shot touching screen limit (limit is far away from screen border for enemy context.Shots
                if (context.Shots[i].EnemyFire)
                {
                    if ((context.Shots[i].Location.X < -(context.ScreenWidth / 2)) || (context.Shots[i].Location.X > (context.ScreenWidth + context.ScreenWidth / 2)))
                    {
                        if (!context.Shots[i].CanGoOffLimits)
                        {
                            killed = true;
                        }
                    }
                    else if ((context.Shots[i].Location.Y < -context.ScreenHeight) || (context.Shots[i].Location.Y > (context.ScreenHeight + context.ScreenHeight / 2)))
                    {
                        if (!context.Shots[i].CanGoOffLimits)
                        {
                            killed = true;
                        }
                    }
                }
                else
                {
                    if ((context.Shots[i].Location.X < -(context.ScreenWidth / 2)) || (context.Shots[i].Location.X > (context.ScreenWidth + context.Shots[i].DstRect.Width)))
                    {
                        if (!context.Shots[i].CanGoOffLimits)
                        {
                            killed = true;
                        }
                    }
                    else if ((context.Shots[i].Location.Y < -context.ScreenHeight) || (context.Shots[i].Location.Y > (context.ScreenHeight + context.Shots[i].DstRect.Height)))
                    {
                        if (!context.Shots[i].CanGoOffLimits)
                        {
                            killed = true;
                        }
                    }

                    //Check for collision between context.Shots
                    CollisionShots(gameTime, context.Shots[i]);
                }

                //Bounce on screen limite
                if (context.Shots[i].Bounce)
                {
                    if ((context.Shots[i].Location.X < 0) || (context.Shots[i].Location.X > (context.ScreenWidth)))
                    {
                        Vector2 s = context.Shots[i].Speed;
                        s.X = -context.Shots[i].Speed.X;
                        context.Shots[i].Speed = s;
                    }
                    else if ((context.Shots[i].Location.Y < 0) || (context.Shots[i].Location.Y > context.ScreenHeight))
                    {
                        Vector2 s = context.Shots[i].Speed;
                        s.Y = -context.Shots[i].Speed.Y;
                        context.Shots[i].Speed = s;
                    }
                }

                if (!killed)
                {
                    //Shots with no speed has to follow the background speed
                    Vector2 oldSpeed = context.Shots[i].Speed;
                    Vector2 newSpeed = context.Shots[i].Speed;

                    if (newSpeed.X == 0)
                    {
                        newSpeed.X = -context.Map.Background2.Speed.X;
                    }
                    if (newSpeed.Y == 0) { newSpeed.Y = -context.Map.Background2.Speed.Y; }

                    context.Shots[i].Speed = newSpeed;

                    context.Shots[i].Update(gameTime);

                    if ((oldSpeed.X == 0) || (oldSpeed.Y == 0))
                    {
                        context.Shots[i].Speed = oldSpeed; //Reset speed because BG speed is not constant
                    }
                }
                else
                {
                    KillShot(context.Player1.Index, context.Shots[i], true, gameTime);
                }
            }

        }

        /// <summary>
        /// AI
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateEnemies(GameTime gameTime)
        {
            //Care of time
            float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (tempsEcoule > 1) tempsEcoule = 1;

            //Move context.Enemies
            for (int i = 0; i < context.Enemies.Count; i++)
            {
                if (enemiesToDelete.Contains(context.Enemies[i])) continue;

                //Enemy is on the screen
                if ((Math.Abs(context.Enemies[i].ScrollValue.X) - Math.Abs(context.Map.Scroll.X) <= 0)
                && (Math.Abs(context.Enemies[i].ScrollValue.Y) - Math.Abs(context.Map.Scroll.Y) <= 0))
                {
                    if (!context.Enemies[i].IsOnScreen)
                    {
                        context.Enemies[i].IsOnScreen = true;

                        if (context.Cheatcodes.IsGiantMode)
                        {
                            context.Enemies[i].Scale = context.Enemies[i].Scale * 2;
                        }
                    }
                }

                if (context.Enemies[i].IsOnScreen)
                {
                    if ((context.Enemies[i].HP < 0) && (context.Enemies[i].Removable))
                    {
                        KillEnemy(PlayerIndex.One, context.Enemies[i], false, gameTime);

                    }

                    //Enemies can have a time to live
                    if (context.Enemies[i].TimeToLive != Entity.InfiniteTimeToLive)
                    {
                        context.Enemies[i].TimeToLive -= gameTime.ElapsedGameTime.Milliseconds;

                        //Kill enemy if no more time to live on this beautiful binary world or no more digital hit points
                        if (context.Enemies[i].TimeToLive <= 0)
                        {
                            if (context.Enemies[i].Removable)
                            {
                                if (!enemiesToDelete.Contains(context.Enemies[i]))
                                {
                                    enemiesToDelete.Add(context.Enemies[i]);
                                }
                                continue;
                            }
                        }
                    }

                    Vector2 location = context.Enemies[i].Location;

                    float deplacementX, deplacementY;

                    if (context.Enemies[i].Background)
                    {
                        deplacementX = (context.Enemies[i].Speed.X * tempsEcoule);
                        deplacementY = (context.Enemies[i].Speed.Y * tempsEcoule);
                    }
                    else
                    {
                        deplacementX = (context.Enemies[i].Speed.X * tempsEcoule);
                        deplacementY = (context.Enemies[i].Speed.Y * tempsEcoule);
                    }


                    if (context.Enemies[i].Pattern != null)
                    {
                        //Init pattern
                        if (!context.Enemies[i].Pattern.Initialized)
                        {
                            context.Enemies[i].Pattern.Initialize(context.Enemies[i]);
                        }

                        //Apply Pattern
                        Point currentPoint = context.Enemies[i].Pattern.GetCurrentPoint();

                        //Enemy on the point : get the next one
                        if (new Rectangle(context.Enemies[i].DstRect.Center.X - 25, context.Enemies[i].DstRect.Center.Y - 25, 50, 50).Contains(currentPoint))
                        {
                            currentPoint = context.Enemies[i].Pattern.GetNextPoint();

                            if (currentPoint == MovePattern.DeadPoint)
                            {
                                context.Enemies[i].TodoOnPatternEnd();

                                if (context.Enemies[i].InfiniteMovePattern)
                                {
                                    context.Enemies[i].Pattern.ResetPattern();
                                }
                                else
                                {
                                    KillEnemy(context.Player1.Index, context.Enemies[i], true, false, gameTime);
                                }
                            }
                        }

                        Vector2 currentPointVector = new Vector2(currentPoint.X, currentPoint.Y);
                        context.Enemies[i].Rotation = Angles.GetAngle(Vectors.ConvertPointToVector2(context.Enemies[i].DstRect.Center), currentPointVector);
                    }
                    //No pattern : kill if far out of screen
                    else
                    {
                        if (((context.Enemies[i].Location.X < -(context.ScreenWidth * 2)) || (context.Enemies[i].Location.X > (context.ScreenWidth * 2)))
                            || ((context.Enemies[i].Location.Y < -(context.ScreenHeight * 2)) || (context.Enemies[i].Location.Y > (context.ScreenHeight * 2))))
                        {
                            KillEnemy(context.Player1.Index, context.Enemies[i], true, false, gameTime);
                        }
                    }

                    //Move
                    if (context.Enemies[i].Flip == SpriteEffects.None)
                    {
                        location.X -= (((float)Math.Cos(context.Enemies[i].Rotation)) * (deplacementX));
                        location.Y -= (((float)Math.Sin(context.Enemies[i].Rotation)) * (deplacementY));
                    }
                    else if (context.Enemies[i].Flip == SpriteEffects.FlipHorizontally)
                    {
                        location.X += (((float)Math.Cos(context.Enemies[i].Rotation)) * (deplacementX));
                        location.Y -= (((float)Math.Sin(context.Enemies[i].Rotation)) * (deplacementY));
                    }
                    else if (context.Enemies[i].Flip == SpriteEffects.FlipVertically)
                    {
                        location.X -= (((float)Math.Cos(context.Enemies[i].Rotation)) * (deplacementX));
                        location.Y += (((float)Math.Sin(context.Enemies[i].Rotation)) * (deplacementY));
                    }

                    context.Enemies[i].Location = location;

                    //Check collisions and delete if required
                    if (!(context.Enemies[i].Hitbox is EmptyHitbox)) //Little optimization
                    {
                        if (CollisionEnemy(gameTime, context.Enemies[i]))
                        {
                            if (context.Enemies[i].Removable)
                            {
                                if (!enemiesToDelete.Contains(context.Enemies[i]))
                                {
                                    enemiesToDelete.Add(context.Enemies[i]);
                                }
                            }
                        }
                    }

                    //Update
                    context.Enemies[i].Update(gameTime);
                }
            } //for
        }

        /// <summary>
        /// Move and kill context.Bonuses
        /// </summary>
        /// <param name="gameTime"></param>
        protected void UpdateBonuses(GameTime gameTime)
        {
            //Care of time
            float tempsEcoule = (float)gameTime.ElapsedGameTime.TotalSeconds;

            for (int i = 0; i < context.Bonuses.Count; i++)
            {
                Vector2 location = context.Bonuses[i].Location;

                //Look at remaining time to live
                context.Bonuses[i].TimeToLive -= gameTime.ElapsedGameTime.Milliseconds;

                if (context.Bonuses[i].TimeToLive < 0)
                {
                    KillBonus(context.Bonuses[i], false);
                    continue;
                }

                //Move bonus
                int deplacementX = (int)(context.Bonuses[i].Speed.X * tempsEcoule);
                int deplacementY = (int)(context.Bonuses[i].Speed.Y * tempsEcoule);

                location.X += (float)deplacementX;
                location.Y -= (float)deplacementY;

                context.Bonuses[i].Location = location;

                //Enemy can have been destroy
                if (context.Bonuses[i] == null) continue;

                Vector2 tmpSpeed = context.Bonuses[i].Speed;
                if ((context.Bonuses[i].Location.Y < 0) || (context.Bonuses[i].Location.Y > context.ScreenHeight - context.Bonuses[i].DstRect.Height))
                {
                    tmpSpeed.Y = -tmpSpeed.Y;
                    //Hack
                    location = context.Bonuses[i].Location;
                    location.Y -= (context.Bonuses[i].Location.Y > 500 ? 5 : -5);
                    context.Bonuses[i].Location = location;
                }
                else if ((context.Bonuses[i].Location.X < 0) || (context.Bonuses[i].Location.X > context.ScreenWidth - context.Bonuses[i].DstRect.Width))
                {
                    tmpSpeed.X = -tmpSpeed.X;
                    //Hack
                    location = context.Bonuses[i].Location;
                    location.X -= (context.Bonuses[i].Location.X > 500 ? 5 : -5);
                    context.Bonuses[i].Location = location;
                }

                context.Bonuses[i].Speed = tmpSpeed;

                context.Bonuses[i].Update(gameTime);
            }
        }

        #endregion

        #region Collision routines

        /// <summary>
        /// Check collisions between player and context.Bonuses, context.Enemies, context.Shots
        /// </summary>
        /// <param name="gameTime"></param>
        protected void CollisionPlayer(GameTime gameTime, PlayerIndex playerIndex)
        {
            Player player = context.GetPlayer(playerIndex);

            //Player hurt a shot
            for (int i = 0; i < context.Shots.Count; i++)
            {
                //Player die
                if (player.Hitbox.Collide(context.Shots[i].Hitbox) && context.Shots[i].EnemyFire)
                {
                    if (KillPlayer(gameTime, playerIndex)) return;
                    KillShot(playerIndex, context.Shots[i], true, gameTime);
                }
            }

            //Player hurt an enemy
            for (int i = 0; i < context.Enemies.Count; i++)
            {
                //Enemy take damage, player die
                if (player.Hitbox.Collide(context.Enemies[i].Hitbox))
                {
                    //Invisible player can't kill with collisions
                    if (player.Invincible == 0f)
                    {
                        context.Enemies[i].HP -= 10;
                        KillEnemy(playerIndex, context.Enemies[i], false, false, gameTime);
                        if (KillPlayer(gameTime, playerIndex)) return;
                    }
                }
            }

            //Player get bonus !
            for (int i = 0; i < context.Bonuses.Count; i++)
            {
                //Bonus die :( Player get the bonus
                if (player.Hitbox.Collide(context.Bonuses[i].Hitbox))
                {
                    if (player.DeathTime <= 0f)
                    {
                        KillBonus(context.Bonuses[i], true, playerIndex);
                    }
                }

            }

        }

        /// <summary>
        /// Collision between context.Enemies and context.Shots
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="x"></param>
        protected bool CollisionEnemy(GameTime gameTime, BadGuy enemy)
        {
            //Enemy hurt a shot
            for (int i = 0; i < context.Shots.Count; i++)
            {
                if (enemy.Hitbox.Collide(context.Shots[i].Hitbox) && !context.Shots[i].EnemyFire)
                {
                    //Destroy enemy
                    enemy.IsHit = true;

                    int dmg = (int)((float)context.Shots[i].Weapon.Damage * context.DamageMulti);
                    if (dmg == 0) dmg = 1; //Flamethrower

                    enemy.HP -= dmg;
                    KillShot(context.Player1.Index, context.Shots[i], true, gameTime);

                    Player player = (Player)context.Shots[i].Weapon.Owner;

                    if (KillEnemy(player.Index, enemy, false, gameTime))
                    {
                        return true;
                    }
                }
            }

            return false;

        }

        /// <summary>
        /// Collision between player context.Shots and badguys context.Shots
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="x"></param>
        protected void CollisionShots(GameTime gameTime, Shot playerShot)
        {
            for (int i = 0; i < context.Shots.Count; i++)
            {
                if ((context.Shots[i].EnemyFire == false) || ((context.Shots[i].Destructable == false) && (context.Options.Difficulty != Difficulty.Easy)) || (context.Options.Difficulty == Difficulty.Hard))
                {
                    continue;
                }

                if (playerShot.Hitbox.Collide(context.Shots[i].Hitbox))
                {
                    //Damage shot
                    context.Shots[i].IsHit = true;
                    context.Shots[i].Hp -= (int)((float)playerShot.Weapon.Damage * context.DamageMulti);

                    if (context.Options.Difficulty != Difficulty.Easy)
                    {
                        KillShot(context.Player1.Index, playerShot, true, gameTime);
                    }
                    KillShot(context.Player1.Index, context.Shots[i], false, gameTime);
                }
            }
        }

        #endregion

        #region Draw things


        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            // Clear Screen
            spriteBatch.ClearDevice(Color.Black);
            spriteBatch.ClearViewport(Color.Black);

            if (context.CurrentGameState == GameState.None)
            {
                splashScreen.Draw(spriteBatch);
            }
            else
            {
                if (context.CurrentGameState == GameState.TitleScreen)
                {
                    titleScreen.Draw(spriteBatch);

                    DrawMouse();
                }
                else if (context.CurrentGameState == GameState.LevelSelectionScreen)
                {
                    levelSelectionScreen.Draw(spriteBatch);

                    DrawMouse();
                }
                else if (context.CurrentGameState == GameState.Credits)
                {
                    creditScreen.Draw(spriteBatch);

                    DrawMouse();
                }
                else if (context.CurrentGameState == GameState.DemoEnd)
                {
                    demoEndScreen.Draw(spriteBatch);

                    DrawMouse();
                }
                else if (context.CurrentGameState == GameState.Loading)
                {
                    loadingLayer.Draw(spriteBatch);
                }
                else if (context.CurrentGameState == GameState.Game)
                {
                    //Maps and parallax
                    DrawMap(context.Map.Background1, gameTime);

                    DrawMap(context.Map.Background2, gameTime);

                    //Background interactive elements
                    DrawBackgroundElements(gameTime);

                    //Particles in background
                    context.ParticleManager.DrawParticles(spriteBatch, true);

                    //Player
                    if (context.Map.Ended != Map.EndMode.Lose)
                    {
                        DrawPlayer(gameTime, PlayerIndex.One);

                        if (context.Player2 != null)
                            DrawPlayer(gameTime, PlayerIndex.Two);
                    }

                    DrawShots(gameTime, false);
                    DrawEnemies(gameTime);
                    DrawShots(gameTime, true);
                    DrawBonuses(gameTime);

                    //Particles
                    context.ParticleManager.DrawParticles(spriteBatch, false);

                    DrawMap(context.Map.Background3, gameTime);

                    //Scores
                    foreach (FlyingScores fs in flyScores)
                    {
                        fs.Draw(spriteBatch);
                    }

                    //Screen become darker in pause mode and in end level
                    spriteBatch.Begin();
                    spriteBatch.Draw(context.NullTex, new Rectangle(0, 0, context.ScreenWidth, context.ScreenHeight), (new Color(Color.Black.R, Color.Black.G, Color.Black.B, darkening)));
                    spriteBatch.End();

                    if (paused)
                    {
                        Rectangle pauseRectSrc = new Rectangle(0, 455, 425, 140);
                        Rectangle pauseRectDst = pauseRectSrc;
                        pauseRectDst.X = (context.ScreenWidth / 2) - (pauseRectSrc.Width / 2);
                        pauseRectDst.Y = context.TitleSafeArea.Y + (context.ScreenHeight / 3) - pauseRectSrc.Height;

                        spriteBatch.Begin();
                        spriteBatch.Draw(context.HudTex, pauseRectDst, pauseRectSrc, Color.White, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                        spriteBatch.End();

                        //Retrieve pausing player
                        Player pausingPlayerInstance = context.GetPlayer(context.PausingPlayerIndex);

                        //Display player index
                        context.TextPrinter.Color = Color.White;
                        context.TextPrinter.Write(spriteBatch, pauseRectDst.Right + 30, pauseRectDst.Center.Y, LocalizedStrings.GetString("Player") + " " + (int)(context.PausingPlayerIndex + 1), 80);

                        //Display commands
                        context.ButtonPrinter.Draw(spriteBatch, "#Move", pausingPlayerInstance.Device.Type, pauseRectDst.Center.X - pauseRectDst.Width / 4, pauseRectDst.Y + pauseRectDst.Height);
                        context.ButtonPrinter.Draw(spriteBatch, "#Fire", pausingPlayerInstance.Device.Type, pauseRectDst.Center.X - pauseRectDst.Width / 4, pauseRectDst.Y + pauseRectDst.Height + 100);
                        context.ButtonPrinter.Draw(spriteBatch, "#Bomb", pausingPlayerInstance.Device.Type, pauseRectDst.Center.X - pauseRectDst.Width / 4, pauseRectDst.Y + pauseRectDst.Height + 150);

                        context.ButtonPrinter.Draw(spriteBatch, "#Pause", pausingPlayerInstance.Device.Type, pauseRectDst.Center.X - pauseRectDst.Width / 4, pauseRectDst.Y + pauseRectDst.Height + 220);
                        context.ButtonPrinter.Draw(spriteBatch, "#Retry", pausingPlayerInstance.Device.Type, pauseRectDst.Center.X - pauseRectDst.Width / 4, pauseRectDst.Y + pauseRectDst.Height + 270);
                        context.ButtonPrinter.Draw(spriteBatch, "#ExitFromPause", pausingPlayerInstance.Device.Type, pauseRectDst.Center.X - pauseRectDst.Width / 4, pauseRectDst.Y + pauseRectDst.Height + 320);

                        context.TextPrinter.Color = Color.LightBlue;

                        if (pausingPlayerInstance.IsPlayingWithAGamepad() == false)
                        {
                            context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 150, pauseRectDst.Y + pauseRectDst.Height + 20, LocalizedStrings.GetString("PlayerMove"), 80);
                        }
                        else
                        {
                            context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 50, pauseRectDst.Y + pauseRectDst.Height + 20, LocalizedStrings.GetString("PlayerMove"), 80);
                        }

                        context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 50, pauseRectDst.Y + pauseRectDst.Height + 120, LocalizedStrings.GetString("PlayerFire"), 80);
                        context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 50, pauseRectDst.Y + pauseRectDst.Height + 170, LocalizedStrings.GetString("PlayerBomb"), 80);
                        context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 50, pauseRectDst.Y + pauseRectDst.Height + 250, LocalizedStrings.GetString("PlayerPause"), 80);
                        context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 50, pauseRectDst.Y + pauseRectDst.Height + 300, LocalizedStrings.GetString("PlayerRetry"), 80);
                        context.TextPrinter.Write(spriteBatch, pauseRectDst.Center.X - pauseRectDst.Width / 4 + 50, pauseRectDst.Y + pauseRectDst.Height + 350, LocalizedStrings.GetString("PlayerBack"), 80);
                        context.TextPrinter.Color = Color.White;

                        if (context.Cheatcodes.Active)
                        {
                            context.Cheatcodes.Draw(spriteBatch);
                        }
                    }

                    context.Hud.Draw(spriteBatch);

                    //End level ?
                    if (context.Map.Ended != Map.EndMode.None)
                    {
                        endLevelScreen.Draw(spriteBatch);
                        DrawMouse();
                    }
                }

#if DEBUG
                if (context.Saver.OptionsData.DebugMode)
                {
                    context.TextPrinter.Color = Color.Tomato;
                    context.TextPrinter.Write(spriteBatch, context.TitleSafeArea.Left, context.TitleSafeArea.Top, "Debug Mode Activated");
                    context.TextPrinter.Color = Color.Black;
                }

                if (context.IsTrialMode)
                {
                    context.TextPrinter.Color = Color.Blue;
                    context.TextPrinter.Write(spriteBatch, context.TitleSafeArea.Left, 740, "Trial Mode Activated");
                    context.TextPrinter.Color = Color.Black;
                }
#endif

                context.SongInfo.Draw(spriteBatch, context.PaperRect, context.HudTex, context.CurrentGameState);

                if (!padsOk)
                {
                    spriteBatch.Begin();
                    spriteBatch.Draw(context.NullTex, new Rectangle(0, 0, context.ScreenWidth, context.ScreenHeight), new Color(Color.Black.R, Color.Black.G, Color.Black.B, darkening));
                    spriteBatch.End();

                    //Tell player that one or more controller is disconnected
                    spriteBatch.Begin();

                    Rectangle dst = new Rectangle((context.ScreenWidth / 2) - (this.padSrc.Width / 2),
                                        (context.ScreenHeight / 2) - (this.padSrc.Height / 2),
                                        this.padSrc.Width, this.padSrc.Height);

                    spriteBatch.Draw(this.gamePadSprite, dst, this.padSrc, Color.White);
                    spriteBatch.End();

                    context.TextPrinter.Color = Color.Tomato;
                    context.TextPrinter.Write(spriteBatch, dst.X - dst.Width, dst.Y + dst.Height + 30, LocalizedStrings.GetString("DisconnectedPadPart1"), 128);

                    context.TextPrinter.Color = Color.White;
                    context.TextPrinter.Write(spriteBatch, dst.X - dst.Width, dst.Y + dst.Height + 50, LocalizedStrings.GetString("DisconnectedPadPart2"), 128);

                    context.TextPrinter.Color = Color.Black;
                }
            }
            base.Draw(gameTime);
        }

        /// <summary>
        /// Print mouse on the screen
        /// </summary>
        public void DrawMouse()
        {
#if WINDOWS
            //Draw Mouse if player is on PC
            Rectangle dst = mouseSrc;
            dst.X = mouseXY.X - mouseSrc.Width / 2;
            dst.Y = mouseXY.Y - mouseSrc.Height / 2;

            context.MouseDst = dst;

            spriteBatch.Begin();
            spriteBatch.Draw(context.HudTex, context.MouseDst, mouseSrc, Color.White);
            spriteBatch.End();

            //Debug mode : show hitboxes, infos
            if (context.Saver.OptionsData.DebugMode)
            {
                PrintRectangle(context.MouseDst, new Color(0.5f, 0.1f, 0.1f, 0.1f));
            }
#endif
        }

        /// <summary>
        /// Print player on the screen
        /// </summary>
        /// <param name="gameTime"></param>
        protected void DrawPlayer(GameTime gameTime, PlayerIndex playerIndex)
        {
            Player player = context.GetPlayer(playerIndex);
            bool draw = player.Lives >= 0;

            if (player.DeathTime > 0)
            {
                if (player.Frame % 25 == 0)
                    draw = false;
            }
            else if (player.Invincible > 0)
            {
                //Blind when invincible
                if (player.Frame % 25 == 0)
                    draw = false;
            }

            if (draw)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if (player.DeathTime > 0)
                {
                    spriteBatch.Draw(player.Sprite, player.DstRect, player.SrcRect, new Color(Color.White.R, Color.White.G, Color.White.B, 0.4f), (float)player.Rotation, Vector2.Zero, player.Flip, 1.0f);
                }
                else
                {
                    spriteBatch.Draw(player.Sprite, player.DstRect, player.SrcRect, Color.White, (float)player.Rotation, Vector2.Zero, player.Flip, 1.0f);
                }
                spriteBatch.End();
            }

            //Debug mode : show hitboxes, infos
            if (context.Saver.OptionsData.DebugMode)
            {
                player.Hitbox.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Print context.Shots on the screen
        /// </summary>
        /// <param name="gameTime"></param>
        protected void DrawShots(GameTime gameTime, bool drawBehindState)
        {

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            for (int i = 0; i < context.Shots.Count; i++)
            {
                if (shotsToDelete.Contains(context.Shots[i])) continue;

                if (context.Shots[i].DrawBehindEnemies == drawBehindState)
                {
                    context.Shots[i].Draw(spriteBatch, context.ShotsTex);

                    if (context.Saver.OptionsData.DebugMode)
                    {
                        context.Shots[i].Hitbox.Draw(spriteBatch);
                    }
                }
            }

            spriteBatch.End();
        }

        /// <summary>
        /// Draw special context.Enemies : background ones
        /// </summary>
        /// <param name="gameTime"></param>
        protected void DrawBackgroundElements(GameTime gameTime)
        {
            for (int i = 0; i < context.Enemies.Count; i++)
            {
                if (enemiesToDelete.Contains(context.Enemies[i])) continue;

                //Do not draw BGE context.Enemies
                if ((context.Enemies[i].IsOnScreen) && (context.Enemies[i] is BackgroundActiveElement))
                {
                    context.Enemies[i].Draw(spriteBatch);

                    if (context.Saver.OptionsData.DebugMode)
                    {
                        context.Enemies[i].Hitbox.Draw(spriteBatch);
                    }
                }
            }
        }

        /// <summary>
        /// Print context.Enemies on the screen
        /// </summary>
        /// <param name="gameTime"></param>
        protected void DrawEnemies(GameTime gameTime)
        {
            for (int i = 0; i < context.Enemies.Count; i++)
            {
                if (enemiesToDelete.Contains(context.Enemies[i])) continue;

                //Do not draw BGE context.Enemies
                bool draw = (context.Enemies[i].IsOnScreen) && (!(context.Enemies[i] is BackgroundActiveElement));

                //HACK Except the fu***ing game ending that needs to be drawed after the boss yeah it's really dirty here too
                draw |= context.Enemies[i] is TGPA.Game.Entities.BackgroundActiveElement.GameEndTrick;

                if (draw)
                {
                    context.Enemies[i].Draw(spriteBatch);

                    if (context.Saver.OptionsData.DebugMode)
                    {
                        context.Enemies[i].Hitbox.Draw(spriteBatch);

                        //Print hp too
                        context.TextPrinter.Color = Color.Blue;
                        context.TextPrinter.Write(spriteBatch, (int)context.Enemies[i].Location.X, (int)context.Enemies[i].Location.Y + 20, "HP:" + context.Enemies[i].HP.ToString());

                        context.TextPrinter.Write(spriteBatch, (int)context.Enemies[i].Location.X, (int)context.Enemies[i].Location.Y + 40, "Loc:" + context.Enemies[i].Location.ToString());
                        context.TextPrinter.Write(spriteBatch, (int)context.Enemies[i].Location.X, (int)context.Enemies[i].Location.Y + 60, "SV:" + context.Enemies[i].ScrollValue.ToString());
                        context.TextPrinter.Color = Color.Black;

                        if (context.Enemies[i].Pattern != null)
                        {
                            //We must create a list of remaining point, not of all point in move pattern
                            MovePattern tmp = context.Enemies[i].Pattern;

                            if (tmp.Points.Count > 0)
                            {

                                Point firstPoint = tmp.GetCurrentPoint();

                                List<Point> currentList = new List<Point>();
                                currentList.Add(firstPoint);

                                for (int mp = tmp.PointIndex; mp < tmp.Points.Count; mp++)
                                {
                                    Point tmpPoint = tmp.Points[mp];
                                    currentList.Add(tmpPoint);
                                }

                                List<Node> nodeList = new List<Node>();

                                foreach (Point p in currentList)
                                {
                                    nodeList.Add(new Node(p));
                                }

                                MovePattern.DrawMovePattern(spriteBatch, context.Enemies[i], nodeList);

                            }
                        }

                        Bonus b = context.Enemies[i].BonusToDrop;
                        if (b != null)
                        {
                            Rectangle sRect = b.SrcRect;
                            Rectangle dRect = b.SrcRect;
                            dRect.X = (int)context.Enemies[i].Location.X;
                            dRect.Y = (int)context.Enemies[i].Location.Y + context.Enemies[i].DstRect.Height;
                            dRect.Width = (int)(((float)b.SrcRect.Width) / 5.0f);
                            dRect.Height = (int)(((float)b.SrcRect.Height) / 5.0f);

                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                            spriteBatch.Draw(context.BonusTex, dRect, sRect, Color.White);

                            spriteBatch.End();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Print Bonuses
        /// </summary>
        /// <param name="gameTime"></param>
        protected void DrawBonuses(GameTime gameTime)
        {
            for (int i = 0; i < context.Bonuses.Count; i++)
            {
                context.Bonuses[i].Draw(spriteBatch, context.BonusTex);

                if (context.Saver.OptionsData.DebugMode)
                {
                    context.Bonuses[i].Hitbox.Draw(spriteBatch);
                }

            }
        }

        /// <summary>
        /// Display context.Map with parallax backgrounds
        /// </summary>
        /// <param name="bg"></param>
        /// <param name="gameTime"></param>
        protected void DrawMap(ScrollingBackground bg, GameTime gameTime)
        {
            if (bg == null) return;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            bg.Draw(spriteBatch);
            spriteBatch.End();
        }

        #endregion

        #region Kill things
        /// <summary>
        /// Destroy player
        /// </summary>
        /// <returns>If player is dead or not</returns>
        protected bool KillPlayer(GameTime gameTime, PlayerIndex playerIndex)
        {
            Player player = context.GetPlayer(playerIndex);

            int indexToDisplay = (int)playerIndex + 1;

            if ((player.Invincible <= 0) && (!context.Cheatcodes.IsInvincible))
            {
                player.TodoOnDeath();
                player.Location = context.Map.InitialPlayerLocation;
                context.InputManager.SetVibrations(playerIndex, new Vector2(3.0f, 3.0f));

                Logger.Log(LogLevel.Info, "Player " + indexToDisplay + " died");
            }

            //Dead or Alive
            if (player.Lives < 0)
            {
                context.Map.Flags.SetFlag("player" + indexToDisplay + "die");

                context.InputManager.SetVibrations(playerIndex, new Vector2(0f, 0f));

                Logger.Log(LogLevel.Info, "Player " + indexToDisplay + " lost");

                return true;
            }

            return false;
        }

        protected void KillBonus(Bonus b, bool get)
        {
            KillBonus(b, get, PlayerIndex.Four);
        }

        /// <summary>
        /// Destroy bonus
        /// </summary>
        /// <param name="i">Bonus index in context.Bonuses array</param>
        /// <param name="get">Player get the bonus or it vanishes</param>
        protected void KillBonus(Bonus b, bool get, PlayerIndex playerIndex)
        {
            if (!(playerIndex == PlayerIndex.Four))
            {
                Player player = context.GetPlayer(playerIndex);

                if (get)
                {
                    int score = 200; //1000 pts per bonus

                    if (b.Type == BonusTypes.Weapon)
                    {

                        if (b.WeaponToDrop.GetType() == player.Weapon.GetType())
                        {
                            player.Weapon.UpgradeWeapon();

                            if (player.Weapon.UpgradeLevel == player.Weapon.MaxLevel)
                            {
                                //Get more points with full level
                                score = 2000;
                            }
                        }
                        else if (b.WeaponToDrop.GetType() == typeof(Megabomb))
                        {
                            player.Bomb.UpgradeWeapon();
                        }
                        else
                        {
                            int oldUpLevel = player.Weapon.UpgradeLevel;
                            player.Weapon = b.WeaponToDrop;

                            for (int i = 0; i < oldUpLevel; i++)
                            {
                                player.Weapon.UpgradeWeapon();
                            }

                            //Level up bonus if player take a SMG bonus
                            if (b.WeaponToDrop.GetType() == typeof(MachineGun))
                            {
                                player.Weapon.UpgradeWeapon();
                            }
                            context.Hud.ChangeWeaponIcon(player.Index, player.Weapon.GetType());
                        }
                    }
                    else
                    {
                        if (b.Type == BonusTypes.Life)
                        {
                            player.Lives++;
                        }
                    }

                    score = (int)((float)score * context.ScoreMulti);

                    //Display score
                    if (context.Options.ShowScore)
                    {
                        FlyingScores fScore = new FlyingScores(b.Location, "+" + score.ToString());
                        this.flyScores.Add(fScore);
                    }

                    player.Score += score;
                }

            }

            context.Bonuses.Remove(b);
        }

        /// <summary>
        /// Destroy enemy
        /// </summary>
        public bool KillEnemy(PlayerIndex playerIndex, BadGuy e, bool force, bool explosion, GameTime gameTime)
        {
            Player player = context.GetPlayer(playerIndex);

            if ((e.HP <= 0) || force)
            {
                //Score
                if (!force)
                {
                    e.TodoOnDeath();

                    int score = (int)((float)e.Points * context.ScoreMulti);

                    if (!(
                        (context.Cheatcodes.IsInvincible)
                        ||
                        (context.Cheatcodes.HasMaxPower)
                        ||
                        (context.Cheatcodes.IsGiantMode)))
                    {
                        player.Score += score;
                    }

                    //Draw score
                    if (context.Saver.OptionsData.ShowScore)
                    {
                        FlyingScores fs = new FlyingScores(e.Location, "+" + score);
                        flyScores.Add(fs);
                    }

                    //Drop bonus
                    if (e.BonusToDrop != null)
                    {
                        e.BonusToDrop.Location = Vectors.ConvertPointToVector2(e.DstRect.Center);
                        context.Bonuses.Add(e.BonusToDrop);
                    }

                    enemieskilled++;
                }

                //Explosion
                if (explosion)
                {
                    //context.ParticleManager.MakeExplosionWithoutQuake(Vectors.ConvertPointToVector2(e.DstRect.Center));
                }

                if (e.Removable)
                {
                    if (!enemiesToDelete.Contains(e))
                    {
                        enemiesToDelete.Add(e);
                    }
                }

                return true;
            }
            return false;
        }

        protected bool KillEnemy(PlayerIndex playerIndex, BadGuy e, bool force, GameTime gameTime)
        {
            return KillEnemy(playerIndex, e, force, true, gameTime);
        }

        /// <summary>
        /// Destroy shot
        /// </summary>
        /// <param name="i">Shot index in shot array</param>
        protected void KillShot(PlayerIndex playerIndex, Shot shot, bool force, GameTime gameTime)
        {
            bool delete = false;
            bool easy = context.Options.Difficulty == Difficulty.Easy;

            Player player = context.GetPlayer(playerIndex);

            if (shot.EnemyFire == false)
            {
                if (shot.Weapon.GetType() == typeof(Megabomb))
                {
                    BombExplosion(shot, gameTime);
                }

                delete = true;
            }
            else
            {
                delete = (((shot.Destructable == true) && (shot.Hp <= 0)) || (shot.Destructable == false));
            }

            if (delete || force || easy)
            {
                shot.TodoOnDeath();

                if (force == false)
                {
                    context.ParticleManager.MakeCircularExplosion(shot.Location, 10f, 10);

                    int score = 0;

                    if (easy == false)
                    {
                        score = (int)((float)shot.Points * context.ScoreMulti);
                    }

                    if (!(
                        (context.Cheatcodes.IsInvincible)
                        ||
                        (context.Cheatcodes.HasMaxPower)
                        ||
                        (context.Cheatcodes.IsGiantMode)))
                    {
                        player.Score += score;
                    }

                    //Draw score
                    if (context.Saver.OptionsData.ShowScore)
                    {
                        if (score > 0)
                        {
                            FlyingScores fs = new FlyingScores(shot.Location, "+" + score);
                            flyScores.Add(fs);
                        }
                    }
                }

                if (!shotsToDelete.Contains(shot))
                {
                    shotsToDelete.Add(shot);
                }
            }
        }

        /// <summary>
        /// Megabomb explosion ! Damage all context.Enemies on screen
        /// </summary>
        /// <param name="shot"></param>
        protected void BombExplosion(Shot shot, GameTime gameTime)
        {
            context.InputManager.SetVibrations(((Player)shot.Weapon.Owner).Index, new Vector2(1.0f, 1.0f));

            for (int i = 0; i < context.Enemies.Count; i++)
            {
                if (context.Enemies[i].IsOnScreen)
                {
                    context.Enemies[i].TodoOnBombing((int)((float)shot.Weapon.Damage * context.DamageMulti));
                }
            }

            //Megabomb destroy enemy context.Shots
            for (int i = 0; i < context.Shots.Count; i++)
            {
                if (context.Shots[i].EnemyFire)
                {
                    if (context.Shots[i].Destructable)
                    {
                        context.Shots[i].Hp = -1;
                    }

                    KillShot(context.Player1.Index, context.Shots[i], !context.Shots[i].Destructable, gameTime);
                }
            }

        }

        #endregion

        #region Public methods

        /// <summary>
        /// Show a rectangle on the screen with the color you want
        /// </summary>
        /// <param name="spriteB"></param>
        /// <param name="rect"></param>
        /// <param name="color"></param>
        public void PrintRectangle(Rectangle rect, Color color)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(context.NullTex, rect, color);
            spriteBatch.End();
        }
     

        #region Map loading

        /// <summary>
        /// Call load context.Map methods and initialize game
        /// </summary>
        /// <param name="mapName"></param>
        /// <param name="gd"></param>
        protected void LoadMap()
        {
            context.MapLoaded = false;

            MusicEngine.Instance.StopMusic();

            try
            {
                Logger.Log(LogLevel.Info, "Loading context.Map " + context.SelectedMap);

                context.Map = Map.BuildMapFromTGPAFile(context.SelectedMap);
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Map loading failed : " + e.Message);

                Exception newE = new Exception("Map loading failed : " + e.Message, e);
                throw newE;
            }
            context.Player1.Initialize();
            context.Player1.Lives = context.Map.InitialLivesCount;
            context.Player1.Score = context.Map.InitialScore;
            context.Player1.Weapon = Weapon.TypeToWeapon(context.Map.InitialWeapon.GetType());
            context.Player1.Bomb = new Megabomb();
            context.Player1.Location = context.Map.InitialPlayerLocation;
            context.Player1.Flip = context.Map.InitialFlip;

            context.Map.LoadMapContent(this);

            //NEW !
            //Apply difficlty
            switch (TGPAContext.Instance.Options.Difficulty)
            {
                case Difficulty.Easy:
                    context.ScoreMulti = 0.5f;
                    context.Player1.Lives = (int)Math.Floor((float)context.Map.InitialLivesCount * 1.5f);
                    context.DamageMulti = 1.25f;
                    break;

                case Difficulty.Normal:
                    context.ScoreMulti = 1f;
                    context.DamageMulti = 1f;
                    break;

                case Difficulty.Hard:
                    context.ScoreMulti = 2f;
                    context.DamageMulti = 0.75f;
                    context.Player1.Lives -= 1;
                    break;
            }

            //Initialize P2
            if (context.Player2 != null)
            {
                InitializeP2();

                //Divide lives :P
                int initLives = context.Player1.Lives;
                context.Player1.Lives = (initLives / 2) + (initLives % 2);
                context.Player2.Lives = context.Player1.Lives;
            }

            context.MapLoaded = true;
        }

        private void InitializeP2()
        {
            context.Player2.Initialize();
            context.Player2.Lives = context.Map.InitialLivesCount;
            context.Player2.Score = context.Map.InitialScore;

            Vector2 loc = context.Map.InitialPlayerLocation;
            loc.Y += context.Player1.DstRect.Height;
            if (loc.Y > context.ScreenHeight)
            {
                loc.Y = context.ScreenHeight - context.Player2.DstRect.Height;
            }

            context.Player2.Location = loc;
            context.Player2.Flip = context.Map.InitialFlip;

            //Need to copy weapon
            context.Player2.Weapon = Weapon.TypeToWeapon(context.Map.InitialWeapon.GetType());
            context.Player2.Bomb = new Megabomb();
        }

        /// <summary>
        /// Load context.Map and data
        /// </summary>
        public void PrepareGame()
        {
            this.GameInit();
            context.SelectedMap = levelSelectionScreen.World.GetMapFirstFile(levelSelectionScreen.SelectedLevelIndex);

            //Free memory
            if (context.Map != null)
            {
                context.Map.UnloadMapContent();
            }

            context.Map = null;

            loadingLayer = new LoadingScreen(Map.GetMapOverview(context.SelectedMap), this);
            this.context.CurrentGameState = GameState.Loading;

            LoadMap();
        }

        #endregion

        #region Gamestate change method

        /// <summary>
        /// Launch the game
        /// </summary>
        /// <param name="context.Map"></param>
        private void LaunchGame()
        {
            //Play music
            if (context.Map.Music != null)
            {
                MusicEngine.Instance.ChangeMusic(context.Map.Music);
            }
            else
            {
                MusicEngine.Instance.StopMusic();
            }

            //Update HUD weapon
            context.Hud.ChangeWeaponIcon(PlayerIndex.One, context.Player1.Weapon.GetType());

            if (context.Player2 != null)
            {
                context.Hud.ChangeWeaponIcon(PlayerIndex.Two, context.Player2.Weapon.GetType());
            }

            Logger.Log(LogLevel.Info, "Changing gamestate : " + this.context.CurrentGameState);
        }

        /// <summary>
        /// Display level selection screen
        /// </summary>
        private void LaunchLevelSelectionScreen()
        {
            levelSelectionScreen.Initialize();
            context.MapLoaded = false;

            Logger.Log(LogLevel.Info, "Changing gamestate : " + this.context.CurrentGameState);
        }

        /// <summary>
        /// Display Title screen
        /// </summary>
        private void LaunchTitleScreen(bool drawPushStart)
        {
#if WINDOWS
            if (context.Player1 == null)
                throw new Exception("No player WTF");
#endif
            context.Player2 = null; //Delete Player 2

            MusicEngine.Instance.ChangeMusic(MusicEngine.Instance.TitleSong);

            titleScreen.Initialize(drawPushStart);

            Logger.Log(LogLevel.Info, "Changing gamestate : " + this.context.CurrentGameState);
        }

        /// <summary>
        /// Display end level screen
        /// </summary>
        /// <param name="win"></param>
        private void LaunchEndLevel(bool win)
        {
            if (win)
                context.Map.Ended = Map.EndMode.Win;
            else
                context.Map.Ended = Map.EndMode.Lose;

            context.InputManager.SetVibrations(PlayerIndex.One, Vector2.Zero);

            //Disable scroll / collisions
            context.Player1.Hitbox = new EmptyHitbox(context.Player1);
            context.Player1.EnableCommands = false;

            if (context.Player2 != null)
            {
                context.InputManager.SetVibrations(PlayerIndex.Two, Vector2.Zero);
                context.Player2.Hitbox = new EmptyHitbox(context.Player2);
                context.Player2.EnableCommands = false;
            }

            context.Map.Background1.Speed = Vector2.Zero;
            context.Map.Background2.Speed = Vector2.Zero;
            //this.Map.Background3.Speed = Vector2.Zero;

            endLevelScreen.Initialize();

            int p1Lives = context.Player1.Lives >= 0 ? context.Player1.Lives : 0;
            int p2Lives = (context.Player2 != null && context.Player2.Lives >= 0) ? context.Player2.Lives : 0;

            endLevelScreen.SetDatas(win, context.Player1.Score, context.Player2 == null ? 0 : context.Player2.Score,
                enemieskilled, p1Lives + (context.Player2 == null ? 0 : p2Lives),
                context.Player1.Bomb.Ammo + (context.Player2 == null ? 0 : context.Player2.Bomb.Ammo), context.Map.StartTime, context.Map.OutDirection);

            Logger.Log(LogLevel.Info, "Changing gamestate : " + this.context.CurrentGameState);
        }

        private void LaunchEndDemoScreen()
        {
            MusicEngine.Instance.ChangeMusic(MusicEngine.Instance.TitleSong);

            demoEndScreen.Initialize();

            Logger.Log(LogLevel.Info, "Changing gamestate : " + this.context.CurrentGameState);
        }

        private void LaunchCreditsScreen()
        {
            MusicEngine.Instance.ChangeMusic(MusicEngine.Instance.CreditsSong);

            creditScreen.Initialize();

            Logger.Log(LogLevel.Info, "Changing gamestate : " + this.context.CurrentGameState);
        }

        /// <summary>
        /// Play next level or replay already loaded context.Map
        /// </summary>
        /// <param name="win"></param>
        public void NextLevel(bool win)
        {
            //Next context.Map or Menu if finish
            if (win)
            {
                if (levelSelectionScreen.World.CanGoNextLevel())
                {
                    context.SelectedMap = levelSelectionScreen.World.GetNextLevel();
                }
                else
                {
                    //End of the game
                    if (context.IsTrialMode)
                    {
                        TGPAContext.Instance.CurrentGameState = GameState.DemoEnd;
                    }
                    else
                    {
                        TGPAContext.Instance.CurrentGameState = GameState.Credits;
                    }

                    return;
                }
            }
            //Reload context.Map
            else
            {
                //Free memory
                if (context.Map != null)
                {
                    context.Map.UnloadMapContent();
                }

                context.Map = null;

                context.SelectedMap = levelSelectionScreen.World.GetCurrentLevel();
            }

            loadingLayer = new LoadingScreen(Map.GetMapOverview(context.SelectedMap), this);
            this.GameInit();
            this.context.CurrentGameState = GameState.Loading;

            new Thread(LoadMap).Start();
        }

        #endregion

        #endregion


    }


    /// <summary>
    /// Abstracted mouse in order to make it fully working with different resolutions.
    /// </summary>
    abstract class Mouse
    {
        public static MouseState GetState()
        {
            MouseState mouseState = Microsoft.Xna.Framework.Input.Mouse.GetState();
            Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
            mousePos = Vector2.Transform(mousePos - new Vector2(Resolution.ViewportX, Resolution.ViewportY), Matrix.Invert(Resolution.ResolutionMatrix));
            return new MouseState((int) mousePos.X, (int) mousePos.Y, mouseState.ScrollWheelValue, mouseState.LeftButton, mouseState.MiddleButton, mouseState.RightButton, mouseState.XButton1, mouseState.XButton2);
        }
    }
}