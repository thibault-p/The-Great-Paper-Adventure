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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TGPA.Audio;
using TGPA.Utils;
using TGPA.Localization;
using TGPA.SpecialEffects;
using Microsoft.Xna.Framework.GamerServices;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;
using TGPA.Screens.Controls;

namespace TGPA.Screens
{
    /// <summary>
    /// Available buttons on the menu
    /// </summary>
    public enum MenuButtons
    {
        None = 0,
        Play,
        Options,
        Exit
    }

    /// <summary>
    /// Menu modes
    /// </summary>
    public enum MenuMode
    {
        Titlescreen,
        Options
    }

    /// <summary>
    /// Display menu
    /// </summary>
    public class TitleScreen
    {
        /// <summary>
        /// Flying shadow in the sky
        /// </summary>
        private class FlyingStuff
        {
            private static Rectangle bnRect = new Rectangle(0, 400, 53, 53);
            private static Rectangle poulpiRect = new Rectangle(57, 395, 58, 53);
            private static Rectangle mcBernickRect = new Rectangle(120, 400, 57, 53);
            private static Rectangle cowRect = new Rectangle(190, 400, 69, 53);

            private Rectangle src, dst;
            private int direction, y;
            private int speed, ampl;

            public bool KillMe { get; set; }

            public FlyingStuff()
            {

                int rand = RandomMachine.GetRandomInt(0, 10);

                if (rand > 9)
                {
                    src = bnRect;
                }
                else if (rand > 7)
                {
                    src = mcBernickRect;
                }
                else if (rand > 5)
                {
                    src = cowRect;
                }
                else
                {
                    src = poulpiRect;
                }

                this.direction = RandomMachine.GetRandomInt(-1, 1);
                if (this.direction == 0) this.direction = 1;

                this.KillMe = false;

                this.dst = src;
                this.dst.X = (direction < 0) ? TGPAContext.Instance.ScreenWidth : -60;
                this.y = RandomMachine.GetRandomInt(100, 250);
                this.dst.Y = this.y;

                this.speed = RandomMachine.GetRandomInt(1, 6);
                this.ampl = RandomMachine.GetRandomInt(200, 400);
            }

            public void Update(GameTime gameTime)
            {
                this.dst.X += this.speed * this.direction;
                this.dst.Y = (int)(this.y + (float)Math.Cos((float)this.dst.X / TGPAContext.Instance.ScreenWidth) * this.ampl);

                if (direction < 0)
                {
                    KillMe = this.dst.X < -this.dst.Width - 1;
                }
                else
                {
                    KillMe = this.dst.X > TGPAContext.Instance.ScreenWidth + 1;
                }
            }

            public void Draw(TGPASpriteBatch spriteBatch, Texture2D texture)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(texture, dst, src, Color.White, 0.0f, Vector2.Zero, direction > 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 1.0f);
                spriteBatch.End();
            }
        }

        private MenuMode mode;

        private Texture2D bgForeTex, bgSignTex, bgBackTex;

        private Texture2D buttonsTex, optionsTex,parcheminTex;

        private Rectangle src, foreSrc, foreDst, dst;
        private Rectangle buttonGo;
        private Rectangle buttonOptions;
        private Rectangle buttonExit;
        private Rectangle buttonGoDst;
        private Rectangle buttonOptionsDst;
        private Rectangle buttonExitDst;

        private ParticleManager pmanager;

        //Random enemies flying in the sky
        private List<FlyingStuff> flyingStuff;
        private float stuffCooldown, stuffTimer;

        private MenuButtons focus;
        private bool confirm;
        private int anim = 0;
        private float time = 0, elapsed = 0;
        private MouseState previousMouseState;

        //Device management
        private bool startPressed;
        private float startPressAlpha, startPressAlphaDelta;
        private bool deviceOkP1;
        private Device player1device;
        private float fadeInOut;
        private bool signInRequested;

        //Options
        private float magicAlpha;
        private float magicAlphaDelta;
        private OptionSection optionsSection;
        private ListControl soundVolumeControl, musicVolumeControl, rumbleControl;
#if WINDOWS
        private ListControl fullscreenControl, p1DeviceListControl,screenResolutionListControl;
        private Textbox p1NameTextboxControl;
#endif

        private Rectangle buttonOptionsBackSrc, buttonOptionsBackDst, parcheminSrc;

        public TitleScreen()
        {
            Initialize(true);

            pmanager = new ParticleManager();
        }

        public void LoadContent(ContentManager Content)
        {
            bgForeTex = Content.Load<Texture2D>(@"gfx/Menu/home_fore");
            bgBackTex = Content.Load<Texture2D>(@"gfx/Menu/home_back");
            bgSignTex = Content.Load<Texture2D>(@"gfx/Menu/home_sign");
            buttonsTex = Content.Load<Texture2D>(@"gfx/Menu/sign");
            optionsTex = Content.Load<Texture2D>(@"gfx/Menu/options");
            parcheminTex = Content.Load<Texture2D>(@"gfx/Menu/parchemin");

            pmanager.LoadContent(Content);
        }

        public void Initialize(bool drawPushStart)
        {
            mode = MenuMode.Titlescreen;

            magicAlpha = 1f;
            magicAlphaDelta = 0f;

            deviceOkP1 = false;
#if XBOX
            signInRequested = false;
            startPressed = drawPushStart ? false : true;
            startPressAlpha = 0.75f;
            startPressAlphaDelta = 0.01f;
#else
            startPressed = true;
            startPressAlpha = 0f;
            startPressAlphaDelta = 0f;
#endif
            this.fadeInOut = 1f;

            float ratio = TGPAContext.Instance.ScreenHeight / 768;
            src = new Rectangle(0, 0, 1024, 768);
            foreDst = new Rectangle(0, TGPAContext.Instance.ScreenHeight - (439 * (TGPAContext.Instance.ScreenHeight / 768)), 804 * (TGPAContext.Instance.ScreenWidth / 1024), 439 * (TGPAContext.Instance.ScreenHeight / 768));
            foreSrc = new Rectangle(0, 0, 804, 439);
            dst = new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight);

            focus = MenuButtons.Play;

            buttonGo = new Rectangle(0, 0, 286, 113);
            buttonGoDst = buttonGo;
            buttonGoDst.X = 670;
            buttonGoDst.Y = 380;


            buttonOptions = new Rectangle(0, 130, 166, 117);
            buttonOptionsDst = buttonOptions;
            buttonOptionsDst.X = 720;
            buttonOptionsDst.Y = 480;

            buttonExit = new Rectangle(0, 256, 219, 118);
            buttonExitDst = buttonExit;
            buttonExitDst.X = 730;
            buttonExitDst.Y = 580;

            confirm = false;

            flyingStuff = new List<FlyingStuff>();
            stuffCooldown = 1500f;
            stuffTimer = 0f;

            //Options
            //***********************************************************
            InitializeOptions();
        }

        private void InitializeOptionsValues(bool fromInitialization)
        {
            float soundLevel = (float)((int)(TGPAContext.Instance.Options.SoundVolume*10))/10;
            float musicLevel = (float)((int)(TGPAContext.Instance.Options.MusicVolume * 10)) / 10;
            soundVolumeControl.SetFocusedElementByValue(soundLevel);
            musicVolumeControl.SetFocusedElementByValue(musicLevel);
            rumbleControl.SetFocusedElementByValue(TGPAContext.Instance.Options.Rumble);

#if WINDOWS
            if (fromInitialization) //Avoid setting graphical parameters after a click on Options panel
            {
                foreach (ListElement element in this.screenResolutionListControl.Elements)
                {
                    if (((OptionResolution)element.Value).Width == TGPAContext.Instance.Options.Width)
                    {
                        if (((OptionResolution)element.Value).Height == TGPAContext.Instance.Options.Height)
                        {
                            this.screenResolutionListControl.FocusedElement = element;
                            break;
                        }
                    }
                }

                fullscreenControl.SetFocusedElementByValue(TGPAContext.Instance.Options.FullScreen);
            }
            if (TGPAContext.Instance.Player1 != null)
            {
                p1DeviceListControl.SetFocusedElementByValue(TGPAContext.Instance.Player1.Device);
            }

#endif
        }

        private void InitializeOptions()
        {
            parcheminSrc = new Rectangle(0, 0, 1024, 768);


#if WINDOWS
            this.optionsSection = new OptionSection("The Great Paper Options", 150, 7, 1);
#else
            this.optionsSection = new OptionSection("The Great Paper Options", 150, 5, 1);
#endif

            //******************************************************************************************************************
            soundVolumeControl = new ListControl("OptionsSoundVolume", new Vector2(250, 200), new List<ListElement>()
            {
                new ListElement(0f, new Rectangle(600, 600, 95, 15),  "0 %"), 
                new ListElement(0.1f, new Rectangle(600, 600, 95, 15),"10 %"), 
                new ListElement(0.2f, new Rectangle(600, 600, 95, 15),"20 %"),
                new ListElement(0.3f, new Rectangle(600, 600, 95, 15),"30 %"), 
                new ListElement(0.4f, new Rectangle(600, 600, 95, 15),"40 %"),
                new ListElement(0.5f, new Rectangle(600, 600, 95, 15),"50 %"), 
                new ListElement(0.6f, new Rectangle(600, 600, 95, 15),"60 %"),
                new ListElement(0.7f, new Rectangle(600, 600, 95, 15),"70 %"), 
                new ListElement(0.8f, new Rectangle(600, 600, 95, 15),"80 %"),
                new ListElement(0.9f, new Rectangle(600, 600, 95, 15),"90 %"),
                new ListElement(1f, new Rectangle(600, 600, 95, 15),  "100 %")
            });

            //******************************************************************************************************************
            musicVolumeControl = new ListControl("OptionsMusicVolume", new Vector2(250, 240), new List<ListElement>()
            {
                new ListElement(0f, new Rectangle(600, 600, 95, 15),  "0 %"), 
                new ListElement(0.1f, new Rectangle(600, 600, 95, 15),"10 %"), 
                new ListElement(0.2f, new Rectangle(600, 600, 95, 15),"20 %"),
                new ListElement(0.3f, new Rectangle(600, 600, 95, 15),"30 %"), 
                new ListElement(0.4f, new Rectangle(600, 600, 95, 15),"40 %"),
                new ListElement(0.5f, new Rectangle(600, 600, 95, 15),"50 %"), 
                new ListElement(0.6f, new Rectangle(600, 600, 95, 15),"60 %"),
                new ListElement(0.7f, new Rectangle(600, 600, 95, 15),"70 %"), 
                new ListElement(0.8f, new Rectangle(600, 600, 95, 15),"80 %"),
                new ListElement(0.9f, new Rectangle(600, 600, 95, 15),"90 %"),
                new ListElement(1f, new Rectangle(600, 600, 95, 15),  "100 %")
            });

            //******************************************************************************************************************
            rumbleControl = new ListControl("OptionsRumble", new Vector2(250, 280), new List<ListElement>()
            {
                new ListElement(true, new Rectangle(600, 600, 95, 15),  LocalizedStrings.GetString("Activated")), 
                new ListElement(false, new Rectangle(600, 600, 95, 15),  LocalizedStrings.GetString("Deactivated")), 
            });

            //******************************************************************************************************************
#if WINDOWS
            int[] graphicModeWidth = { 1920, 1680, 1600, 1440, 1366, 1280, 1152, 1024, 800, 640 };
            int[] graphicModeHeight = { 1200, 1080, 1050, 1024, 900, 960, 800, 768, 600, 480 };

            List<ListElement> resolutionsElements = new List<ListElement>();

            for (int i = graphicModeWidth.Length - 1; i >= 0; i--)
            {
                if (graphicModeWidth[i] <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width)
                {
                    for (int j = graphicModeHeight.Length - 1; j >= 0; j--)
                    {
                        if (graphicModeHeight[j] <= GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height)
                        {
                            resolutionsElements.Add(
                                new ListElement(new OptionResolution(graphicModeWidth[i], graphicModeHeight[j]), new Rectangle(600, 600, 95, 5))
                                );
                        }
                    }
                }
            }

            this.screenResolutionListControl = new ListControl("OptionsScreenResolution", new Vector2(250, 320), resolutionsElements, "Apply");
            this.screenResolutionListControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);

            //******************************************************************************************************************
            fullscreenControl = new ListControl("OptionsFullScreen", new Vector2(250, 360), new List<ListElement>()
            {
                new ListElement(true, new Rectangle(600, 600, 95, 15),  LocalizedStrings.GetString("Activated")), 
                new ListElement(false, new Rectangle(600, 600, 95, 15),  LocalizedStrings.GetString("Deactivated")), 
            });

            //******************************************************************************************************************
            this.p1NameTextboxControl = new Textbox("P1Name", new Vector2(250, 400));
            this.p1NameTextboxControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);
            this.p1NameTextboxControl.Text = TGPAContext.Instance.Options.Player1Name;

            //******************************************************************************************************************
            this.p1DeviceListControl = new ListControl("P1Device", new Vector2(250, 440),
                        new List<ListElement>() { 
                    new ListElement(new Device(),new Rectangle(600, 600, 135, 15),LocalizedStrings.GetString("KeyboardMouse"))
                });
            this.p1DeviceListControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);

            //Add connected pads
            if (GamePad.GetState(PlayerIndex.One).IsConnected)
            {
                this.p1DeviceListControl.Elements.Add(new ListElement(new Device(DeviceType.Gamepad, (int)PlayerIndex.One),new Rectangle(600, 600, 135, 15), LocalizedStrings.GetString("Gamepad") + " 1"));
            }
            if (GamePad.GetState(PlayerIndex.Two).IsConnected)
            {
                this.p1DeviceListControl.Elements.Add(new ListElement(new Device(DeviceType.Gamepad, (int)PlayerIndex.Two),new Rectangle(600, 600, 135, 15), LocalizedStrings.GetString("Gamepad") + " 2"));
            }
            if (GamePad.GetState(PlayerIndex.Three).IsConnected)
            {
                this.p1DeviceListControl.Elements.Add(new ListElement(new Device(DeviceType.Gamepad, (int)PlayerIndex.Three),new Rectangle(600, 600, 135, 15), LocalizedStrings.GetString("Gamepad") + " 3"));
            }
            if (GamePad.GetState(PlayerIndex.Four).IsConnected)
            {
                this.p1DeviceListControl.Elements.Add(new ListElement(new Device(DeviceType.Gamepad, (int)PlayerIndex.Four),new Rectangle(600, 600, 135, 15), LocalizedStrings.GetString("Gamepad") + " 4"));
            }

            //Add joysticks
            for (int i = 0; i < TGPAContext.Instance.InputManager.JoystickManager.ConnectedJoystick.Length; i++)
            {
                if (TGPAContext.Instance.InputManager.JoystickManager.ConnectedJoystick[i] == true)
                {
                    this.p1DeviceListControl.Elements.Add(new ListElement(new Device(DeviceType.Joystick, i),new Rectangle(600, 600, 135, 15), LocalizedStrings.GetString("Joystick") + " " + i));
                }

            }
#endif
            //******************************************************************************************************************
            this.soundVolumeControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);
            this.musicVolumeControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);
            this.rumbleControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);

            //******************************************************************************************************************
            this.optionsSection.AddControl(0, 0, this.soundVolumeControl);
            this.optionsSection.AddControl(1, 0, this.musicVolumeControl);
            this.optionsSection.AddControl(2, 0, this.rumbleControl);


#if WINDOWS
            this.screenResolutionListControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);
            this.fullscreenControl.ValueChanged += new ValueChangedEventhandler(TodoOnValueChanging);

            this.optionsSection.AddControl(3, 0, this.screenResolutionListControl);
            this.optionsSection.AddControl(4, 0, this.fullscreenControl);
            this.optionsSection.AddControl(5, 0, this.p1NameTextboxControl);
            this.optionsSection.AddControl(6, 0, this.p1DeviceListControl);
#endif
            //******************************************************************************************************************
            buttonOptionsBackSrc = new Rectangle(0, 0, 53, 46);
            buttonOptionsBackDst = buttonOptionsBackSrc;
            buttonOptionsBackDst.X = 200;
            buttonOptionsBackDst.Y = 570;

            optionsSection.SetFocus(soundVolumeControl);

            InitializeOptionsValues(true);
        }

        /// <summary>
        /// Update menu/title screen (buttons)
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            this.fadeInOut -= 0.05f;
            if (this.fadeInOut < 0) { this.fadeInOut = 0f; }

            #region Titlescreen
            if (mode == MenuMode.Titlescreen)
            {

                if (startPressed == false)
                {
                    startPressAlpha += startPressAlphaDelta;
                    if (startPressAlpha > 1f || startPressAlpha < 0f) startPressAlphaDelta = -startPressAlphaDelta;

                    //Wait for a player to press start
                    if (deviceOkP1 == false)
                    {
                        for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
                        {
                            if (GamePad.GetState(index).Buttons.Start == ButtonState.Pressed)
                            {
                                //It will become Player 1
                                player1device = new Device(DeviceType.Gamepad, (int)index);
                                deviceOkP1 = true;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (TGPAContext.Instance.Player1 == null)
                        {
                            if (deviceOkP1)
                            {
                                //Ask for sign in
                                if (Device.DeviceHasProfile(player1device) == false)
                                {
                                    if (Guide.IsVisible == false)
                                    {
                                        signInRequested = true;
                                        Guide.ShowSignIn(1, false);
                                    }
                                    else
                                    {
                                        if (signInRequested)
                                        {
                                            signInRequested = false;
                                            deviceOkP1 = false;
                                            player1device = null;
                                        }
                                    }
                                }

                                else
                                {
                                    TGPAContext.Instance.Player1 = new Player(PlayerIndex.One, player1device);
#if XBOX
                                    TGPAContext.Instance.Player1.Name = TGPAContext.Instance.Player1.XboxProfile.Gamertag;
#endif
                                }
                            }
                        }
                        else
                        {
                            //Assign eventually a new controller for P1
                            if (deviceOkP1)
                            {
                                TGPAContext.Instance.Player1.Device = player1device;
                            }

                            startPressed = true;
                        }
                    }
                }
                else
                {
                    if (TGPAContext.Instance.Player1 == null)
                        throw new Exception("No player WTF");

                    if (TGPAContext.Instance.InputManager.PlayerPressButtonBack(TGPAContext.Instance.Player1))
                    {
                        TGPAContext.Instance.CurrentGameState = GameState.Credits;
                    }
                    else
                    {
                        //Buy from title screen
                        if (TGPAContext.Instance.IsTrialMode)
                        {
                            if (TGPAContext.Instance.InputManager.PlayerPressYButton(TGPAContext.Instance.Player1))
                            {
                                if (Guide.IsVisible == false)
                                {
                                    //Player need rights
                                    SignedInGamer xboxProfile = Device.DeviceProfile(TGPAContext.Instance.Player1.Device);

                                    if (xboxProfile.Privileges.AllowPurchaseContent)
                                    {
                                        Guide.ShowMarketplace((PlayerIndex)TGPAContext.Instance.Player1.Device.Index);
                                    }
                                    else
                                    {
                                        Guide.BeginShowMessageBox(LocalizedStrings.GetString("BuyFailTitle"), LocalizedStrings.GetString("BuyFailContent"), new List<string> { "OK" }, 0, MessageBoxIcon.Warning, null, null);
                                    }
                                }
                            }
                        }

                    }

                    KeyboardState keyboard = Keyboard.GetState();

                    //Mouse and menus
                    Rectangle startRect = new Rectangle(
                        StartButton.X, StartButton.Y,
                        StartButton.X + StartButton.Width,
                        StartButton.Y + StartButton.Height);

                    Rectangle optionsRect = new Rectangle(
                         OptionsButton.X, OptionsButton.Y,
                         OptionsButton.X + OptionsButton.Width,
                         OptionsButton.Y + OptionsButton.Height);

                    Rectangle exitRect = new Rectangle(
                                         QuitButton.X, QuitButton.Y,
                         QuitButton.X + QuitButton.Width,
                         QuitButton.Y + QuitButton.Height);

                    //PC Management
                    #region Mouse input
                    if (TGPAContext.Instance.Player1.IsPlayingOnWindows() && TGPAContext.Instance.IsActive)
                    {
                        if (buttonGoDst.Contains(TGPAContext.Instance.MousePoint))
                        {
                            Focus = MenuButtons.Play;

                            if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                                TGPAContext.Instance.CurrentGameState = GameState.LevelSelectionScreen;

                        }
                        else if (buttonOptionsDst.Contains(TGPAContext.Instance.MousePoint))
                        {
                            Focus = MenuButtons.Options;

                            if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                            {
                                changeModeAfterAlphaBlending(MenuMode.Options);
                                InitializeOptionsValues(false);
                            }

                        }
                        else if (buttonExitDst.Contains(TGPAContext.Instance.MousePoint))
                        {
                            Focus = MenuButtons.Exit;

                            if ((InputManager.IsClic(previousMouseState, Mouse.GetState())) || (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1)))
                                TGPAContext.Instance.CurrentGameState = GameState.Exit;

                        }
                        else
                        {
                            Focus = MenuButtons.None;
                        }
                    }
                    #endregion

                    switch (focus)
                    {
                        case MenuButtons.Play:

                            if ((TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1)))
                                TGPAContext.Instance.CurrentGameState = GameState.LevelSelectionScreen;

                            if (TGPAContext.Instance.InputManager.PlayerPressDown(TGPAContext.Instance.Player1))
                                focus = MenuButtons.Options;

                            if (TGPAContext.Instance.InputManager.PlayerPressUp(TGPAContext.Instance.Player1))
                                focus = MenuButtons.Exit;

                            break;

                        case MenuButtons.Options:

                            if ((TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1)))
                            {
                                changeModeAfterAlphaBlending(MenuMode.Options);
                                InitializeOptionsValues(false);
                            }

                            if (TGPAContext.Instance.InputManager.PlayerPressDown(TGPAContext.Instance.Player1))
                                focus = MenuButtons.Exit;

                            if (TGPAContext.Instance.InputManager.PlayerPressUp(TGPAContext.Instance.Player1))
                                focus = MenuButtons.Play;

                            break;

                        case MenuButtons.Exit:

                            if ((TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1)))
                                TGPAContext.Instance.CurrentGameState = GameState.Exit;

                            if (TGPAContext.Instance.InputManager.PlayerPressDown(TGPAContext.Instance.Player1))
                                focus = MenuButtons.Play;

                            if (TGPAContext.Instance.InputManager.PlayerPressUp(TGPAContext.Instance.Player1))
                                focus = MenuButtons.Options;

                            break;
#if XBOX
                    case MenuButtons.None:
                        focus = MenuButtons.Play;
                        break;
#endif
                    }
                    previousMouseState = Mouse.GetState();
                }

            }
            #endregion
            #region Option screen
            else if (mode == MenuMode.Options)
            {
                this.optionsSection.Update(gameTime);

                //Set focus
                if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false)
                {
                    #region Pad Management

                    TGPAControl control = null;

                    if (TGPAContext.Instance.InputManager.PlayerPressDown(TGPAContext.Instance.Player1))
                    {
                        if (optionsSection.CurrentLine + 1 < optionsSection.Lines)
                        {
                            control = optionsSection.GetControl(optionsSection.CurrentLine + 1, optionsSection.CurrentColumn);
                        }

                        if (control == null)
                        {
                            control = optionsSection.GetControl(0, optionsSection.CurrentColumn);
                        }

                        if (control != null)
                        {
                            optionsSection.UnsetFocus();
                            optionsSection.SetFocus(control);
                        }
                    }
                    else if (TGPAContext.Instance.InputManager.PlayerPressUp(TGPAContext.Instance.Player1))
                    {
                        if (optionsSection.CurrentLine - 1 < optionsSection.Lines)
                        {
                            control = optionsSection.GetControl(optionsSection.CurrentLine - 1, optionsSection.CurrentColumn);
                        }

                        if (control == null)
                        {
                            control = optionsSection.GetControl(optionsSection.Lines - 1, optionsSection.CurrentColumn);
                        }

                        if (control != null)
                        {
                            optionsSection.UnsetFocus();
                            optionsSection.SetFocus(control);
                        }
                    }
                    #endregion
                }
#if WINDOWS
                else
                {
                    bool leave = false;

                    for (int i = 0; i < optionsSection.Lines; i++)
                    {
                        for (int j = 0; j < optionsSection.Columns; j++)
                        {
                            TGPAControl control = optionsSection.GetControl(i, j);

                            if (control != null)
                            {
                                if (control.DstRect.Intersects(TGPAContext.Instance.MouseDst))
                                {
                                    if (control != optionsSection.FocusedControl)
                                    {
                                        optionsSection.UnsetFocus();
                                        optionsSection.SetFocus(control);
                                    }
                                    leave = true;
                                    break;
                                }
                            }

                            if (leave) break; //As beautiful as a GOTO
                        }
                        if (leave) break;
                    }
                }
                if (TGPAContext.Instance.MouseDst.Intersects(buttonOptionsBackDst))
                {
                    buttonOptionsBackSrc.Y = 50;
                    bool exit = TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1);

                    if (exit)
                    {
                        changeModeAfterAlphaBlending(MenuMode.Titlescreen);
                        TGPAContext.Instance.Saver.Save();
                    }
                }
                else
                {
                    buttonOptionsBackSrc.Y = 0;
                }
#endif
                if (TGPAContext.Instance.InputManager.PlayerPressButtonBack(TGPAContext.Instance.Player1))
                {
                    changeModeAfterAlphaBlending(MenuMode.Titlescreen);
                    TGPAContext.Instance.Saver.Save();
                }

            }
            #endregion

            #region Animation

            stuffTimer += gameTime.ElapsedGameTime.Milliseconds;

            //Update stuff
            List<FlyingStuff> fsToDelete = new List<FlyingStuff>();
            foreach (FlyingStuff fs in flyingStuff)
            {
                fs.Update(gameTime);

                if (fs.KillMe)
                {
                    fsToDelete.Add(fs);
                }
            }

            foreach (FlyingStuff deadfs in fsToDelete)
            {
                flyingStuff.Remove(deadfs);
            }

            if (stuffTimer - stuffCooldown > 0f)
            {
                flyingStuff.Add(new FlyingStuff());
                stuffTimer = 0f;
                stuffCooldown = RandomMachine.GetRandomFloat(750, 6000);
            }

            for (int i = 0; i < 2; i++)
            {
                Vector2 loc = new Vector2(610, 625);
                loc.X += RandomMachine.GetRandomInt(-35, 35);
                loc.Y += RandomMachine.GetRandomInt(-35, 35);

                loc.Y = (loc.Y / 768) * TGPAContext.Instance.ScreenHeight;

                Smoke s = new Smoke(loc, RandomMachine.GetRandomVector2(30f, 150f, -40f, -120f), 0.03f, 0.04f, 0.14f, 1,
                                1f, RandomMachine.GetRandomInt(0, 4));

                pmanager.AddParticle(s, true);
            }

            //anim
            elapsed += gameTime.ElapsedGameTime.Milliseconds;

            if (elapsed > time)
            {
                anim = (anim == 0) ? 1 : 0;
                foreSrc.Y = anim * 440;
                elapsed = 0;
                time = 200 + (float)(RandomMachine.GetRandomFloat(-100, 300));
            }

            pmanager.UpdateParticles((float)gameTime.ElapsedGameTime.TotalSeconds);

            #endregion

            this.magicAlpha -= magicAlphaDelta;

            if (this.magicAlpha < 0) { 
                this.magicAlpha = 0f; 
                this.magicAlphaDelta = -magicAlphaDelta; 

                //Change mode
                if (nextMode != mode)
                {
                    mode = nextMode;
                }
            }
            if (this.magicAlpha > 1) { this.magicAlpha = 1f; }
        }

        private MenuMode nextMode;

        private void changeModeAfterAlphaBlending(MenuMode newMode)
        {
            magicAlphaDelta = 0.1f;
            nextMode = newMode;
        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.bgBackTex, dst, src, Color.White);

            //draw stuff
            spriteBatch.End();

            foreach (FlyingStuff fs in flyingStuff)
            {
                fs.Draw(spriteBatch, this.buttonsTex);
            }

            //draw particle
            pmanager.DrawParticles(spriteBatch, true);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(this.bgForeTex, foreDst, foreSrc, Color.White);
            spriteBatch.End();

            if (mode == MenuMode.Titlescreen)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (!startPressed)
                {
                    spriteBatch.End();

                    TGPAContext.Instance.TextPrinter.Color = new Color(Color.White.R, Color.White.G, Color.White.B, startPressAlpha);

                    TGPAContext.Instance.TextPrinter.Size = 3f;

                    String s = LocalizedStrings.GetString("P2PressStartXbox");

                    int x = 512 - ((s.Length * 10) + 50);
                    int y = 370;

                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, x, y, s, 128);

                    TGPAContext.Instance.TextPrinter.Size = 1f;

                    TGPAContext.Instance.TextPrinter.Color = Color.Black;
                }
                else
                {

                    Rectangle tmp_src;

                    spriteBatch.Draw(this.bgSignTex, dst, src, Color.White*magicAlpha);

                    //option
                    tmp_src = buttonOptions;
                    if (this.Focus == MenuButtons.Options)
                        tmp_src.X += 289;

                    spriteBatch.Draw(this.Buttons, buttonOptionsDst, tmp_src, Color.White * magicAlpha);


                    //play
                    tmp_src = buttonGo;
                    if (this.Focus == MenuButtons.Play)
                        tmp_src.X = 289;

                    spriteBatch.Draw(this.Buttons, buttonGoDst, tmp_src, Color.White * magicAlpha);

                    //quit
                    tmp_src = buttonExit;
                    if (this.Focus == MenuButtons.Exit)
                        tmp_src.X += 289;


                    spriteBatch.Draw(this.Buttons, buttonExitDst, tmp_src, Color.White * magicAlpha);
                    spriteBatch.End();

                    TGPAContext.Instance.TextPrinter.Color = Color.White * magicAlpha;

                    //Show Player name
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, TGPAContext.Instance.TitleSafeArea.Left + 10, TGPAContext.Instance.TitleSafeArea.Bottom - 2 * TGPAContext.Instance.TextPrinter.Size * TGPAContext.Instance.TextPrinter.Font.LineSpacing, TGPAContext.Instance.Player1.Name);

                    //Show version
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, TGPAContext.Instance.TitleSafeArea.Left + 10, TGPAContext.Instance.TitleSafeArea.Bottom - TGPAContext.Instance.TextPrinter.Size * TGPAContext.Instance.TextPrinter.Font.LineSpacing, "v" + TheGreatPaperGame.Version);

                    //Fade in
                    if (fadeInOut > 0f)
                    {
                        Color blackScreen = new Color(Color.Black.R, Color.Black.G, Color.Black.B, this.fadeInOut);

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        spriteBatch.Draw(TGPAContext.Instance.NullTex, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), blackScreen);
                        spriteBatch.End();
                    }


                    if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false)
                    {
                        //Buttons
                        //***************************************************************************
                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Move", TGPAContext.Instance.Player1.Device.Type, 250, 570);
                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Confirm", TGPAContext.Instance.Player1.Device.Type, 250, 620);

                        TGPAContext.Instance.TextPrinter.Write(spriteBatch, 300, 580, LocalizedStrings.GetString("TitleScreenMove"));
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch, 300, 630, LocalizedStrings.GetString("TitleScreenButtonA"));
                    }

                    if (TGPAContext.Instance.IsTrialMode)
                    {
                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Plus", TGPAContext.Instance.Player1.Device.Type, 250, 520);
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch, 300, 530, LocalizedStrings.GetString("Buy"));
                    }
                    TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Back", TGPAContext.Instance.Player1.Device.Type, 250, 670);
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, 300, 680, LocalizedStrings.GetString("ShowCreditsScreen"));

                    TGPAContext.Instance.TextPrinter.Color = Color.Black;
                }

            }
            else
            {
                spriteBatch.Begin();
                spriteBatch.Draw(this.parcheminTex, parcheminSrc, Color.White * magicAlpha);
                spriteBatch.End();


                TGPAContext.Instance.TextPrinter.Color = Color.Black * magicAlpha;

                this.optionsSection.Draw(spriteBatch);

                //Back
#if WINDOWS
                spriteBatch.Begin();
                spriteBatch.Draw(this.optionsTex, buttonOptionsBackDst, buttonOptionsBackSrc, Color.White * magicAlpha);
                spriteBatch.End();

                TGPAContext.Instance.TextPrinter.Write(spriteBatch, buttonOptionsBackDst.X + buttonOptionsBackDst.Width + 25, buttonOptionsBackDst.Y + buttonOptionsBackDst.Height / 4, "Retour");
#else
                TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Cancel", TGPAContext.Instance.Player1.Device.Type, 200, 600);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 250, 610, LocalizedStrings.GetString("OptionScreenBack"));

#endif



                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private void TodoOnValueChanging(object sender)
        {
            if (sender is TGPAControl)
            {
                if (sender == this.soundVolumeControl)
                {
                    TGPAContext.Instance.Options.SoundVolume = (float)this.soundVolumeControl.FocusedElement.Value;
                }
                else if (sender == this.musicVolumeControl)
                {
                    TGPAContext.Instance.Options.MusicVolume = (float)this.musicVolumeControl.FocusedElement.Value;
                }
#if WINDOWS
                else if (sender == this.fullscreenControl)
                {
                    TGPAContext.Instance.Options.FullScreen = (bool)this.fullscreenControl.FocusedElement.Value;

                    Resolution.SetResolution(TGPAContext.Instance.Options.Width, TGPAContext.Instance.Options.Height, TGPAContext.Instance.Options.FullScreen);
                }
                else if (sender == this.rumbleControl)
                {
                    TGPAContext.Instance.Options.Rumble = (bool)this.rumbleControl.FocusedElement.Value;
                }
                else if (sender == this.p1NameTextboxControl)
                {
                    TGPAContext.Instance.Options.Player1Name = this.p1NameTextboxControl.Text;

                    TGPAContext.Instance.Player1.Name = TGPAContext.Instance.Options.Player1Name;
                }
                else if (sender == this.p1DeviceListControl)
                {
                    TGPAContext.Instance.Options.Player1PlayingDevice = (Device)(this.p1DeviceListControl.FocusedElement.Value);

                    TGPAContext.Instance.Player1.Device = TGPAContext.Instance.Options.Player1PlayingDevice;
                }
                else if (sender == this.screenResolutionListControl)
                {
                    TGPAContext.Instance.Options.Width = ((OptionResolution)this.screenResolutionListControl.FocusedElement.Value).Width;
                    TGPAContext.Instance.Options.Height = ((OptionResolution)this.screenResolutionListControl.FocusedElement.Value).Height;

                    Resolution.SetResolution(TGPAContext.Instance.Options.Width, TGPAContext.Instance.Options.Height, TGPAContext.Instance.Options.FullScreen);
                }
#endif
            }
        }

        public bool Confirmed
        {
            get { return confirm; }
            set { confirm = value; }
        }

        public Texture2D Buttons
        {
            get { return buttonsTex; }
        }

        public Rectangle StartButton
        {
            get { return buttonGoDst; }
        }

        public Rectangle OptionsButton
        {
            get { return buttonOptionsDst; }
        }

        public Rectangle QuitButton
        {
            get { return buttonExitDst; }
        }

        public MenuButtons Focus
        {
            set { focus = value; }
            get { return focus; }
        }
    }



    #region Option resolution

#if WINDOWS
    public class OptionResolution
    {
        public int Width { get; set; }
        public int Height { get; set; }

        public OptionResolution(int w, int h)
        {
            this.Width = w;
            this.Height = h;
        }

        public override string ToString()
        {
            return Width + "x" + Height;
        }
    }
#endif
    #endregion

    #region Option Section

    /// <summary>
    /// 
    /// </summary>
    public class OptionSection
    {
        public String LocalizedName { get; set; }
        public String SectionName { get; set; }
        public int Y { get; set; }
        public int CurrentLine { get; set; }
        public int CurrentColumn { get; set; }
        public int Lines { get; set; }
        public int Columns { get; set; }

        private Rectangle dstRect;
        private TGPAControl[][] controls;
        private TGPAControl focus;

        public OptionSection(String localizedName, int y, int lines, int columns)
        {
            this.LocalizedName = localizedName;
            this.SectionName = LocalizedStrings.GetString(this.LocalizedName);
            this.Y = y;
            this.Lines = lines;
            this.Columns = columns;

            this.controls = new TGPAControl[lines][];

            for (int i = 0; i < this.controls.Length; i++)
            {
                this.controls[i] = new TGPAControl[columns];

                for (int j = 0; j < this.controls[i].Length; j++)
                {
                    this.controls[i][j] = null;
                }
            }

            this.focus = null;
        }

        public void AddControl(int line, int column, TGPAControl control)
        {
            this.controls[line][column] = control;
        }

        public TGPAControl GetControl(int line, int column)
        {

            if ((line >= Lines) || (column >= Columns))
            {
                throw new ArgumentException("Array are to smalls.");
            }

            return controls[line][column];
        }

        public void UnsetFocus()
        {
            this.focus.Focus = false;
        }

        public void SetFocus(TGPAControl control)
        {
            for (int i = 0; i < this.controls.Length; i++)
            {
                for (int j = 0; j < this.controls[i].Length; j++)
                {
                    if (this.controls[i][j] == control)
                    {
                        this.focus = control;
                        control.Focus = true;
                        this.CurrentLine = i;
                        this.CurrentColumn = j;
                        return;
                    }
                }
            }

            throw new ArgumentException("Control " + control + " not in the section");
        }

        public void Update(GameTime gameTime)
        {
            this.SectionName = LocalizedStrings.GetString(this.LocalizedName);

            int height = 0;

            foreach (TGPAControl[] controlList in controls)
            {
                foreach (TGPAControl control in controlList)
                {
                    if (control != null)
                    {
                        control.Update(gameTime);

                        height = Math.Max(height, control.DstRect.Y + control.DstRect.Height);
                    }
                }
            }

            this.dstRect = new Rectangle();
            this.dstRect.X = 90;
            this.dstRect.Y = this.Y;
            this.dstRect.Width = 1024 - 90;
            this.dstRect.Height = height;

        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, 110, this.Y, this.SectionName);

            foreach (TGPAControl[] controlList in controls)
            {
                foreach (TGPAControl control in controlList)
                {
                    if (control != null)
                    {
                        control.Draw(spriteBatch);
                    }
                }
            }

            TGPAContext.Instance.TextPrinter.SetDefaultFont();
        }

        /// <summary>
        /// Read only access to the focused control
        /// </summary>
        public TGPAControl FocusedControl
        {
            get { return this.focus; }
        }
    }

    #endregion
}
