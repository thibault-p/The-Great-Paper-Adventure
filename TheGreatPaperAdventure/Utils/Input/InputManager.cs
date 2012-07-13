//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using TGPA.Utils.Input;

namespace TGPA.Utils
{
    /// <summary>
    /// Gamepad buttons mapping
    /// </summary>
    public enum GamePadButtonsList
    {
        A,
        B,
        X,
        Y,
        LT,
        RT,
        LB,
        RB,
        Start,
        Back,
        Home,
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Manage input for players
    /// </summary>
    public class InputManager
    {

        private MouseState mouse, pmouse;
        private KeyboardState keyboard, pkeyboard;

        public JoystickManager JoystickManager { get; set; }

        /// <summary>
        /// GamePads
        /// </summary>
        private GamePadState pad1, ppad1, pad2, ppad2, pad3, ppad3, pad4, ppad4;

        private Dictionary<int, JoystickState> joyState;
        private Dictionary<int, JoystickState> previousJoyState;

        public InputManager()
        {
            JoystickManager = new JoystickManager();
            joyState = new Dictionary<int, JoystickState>();
            previousJoyState = new Dictionary<int, JoystickState>();
        }

        /// <summary>
        /// Get new input devices values
        /// </summary>
        public void StartUpdate()
        {
#if WINDOWS
            mouse = Mouse.GetState();
            keyboard = Keyboard.GetState();

            joyState.Clear();

            for (int i = 0; i < JoystickManager.ConnectedJoystick.Length; i++)
            {
                if (JoystickManager.ConnectedJoystick[i] == true)
                {
                    joyState.Add(i, JoystickManager.GetJoyState(i));
                }
            }
#endif
            pad1 = GamePad.GetState(PlayerIndex.One);
            pad2 = GamePad.GetState(PlayerIndex.Two);
            pad3 = GamePad.GetState(PlayerIndex.Three);
            pad4 = GamePad.GetState(PlayerIndex.Four);


        }

        /// <summary>
        /// Store old device input values
        /// </summary>
        public void EndUpdate()
        {
#if WINDOWS
            pmouse = mouse;
            pkeyboard = keyboard;

            previousJoyState.Clear();

            foreach (int k in joyState.Keys)
            {
                previousJoyState.Add(k, joyState[k]);
            }

            JoystickManager.Scan();
#endif

            ppad1 = pad1;
            ppad2 = pad2;
            ppad3 = pad3;
            ppad4 = pad4;
        }

        /// <summary>
        /// Set rumble value on a controller
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="power"></param>
        public void SetVibrations(PlayerIndex idx, Vector2 power)
        {
            if (TGPAContext.Instance.Options.Rumble)
            {
                Player p = TGPAContext.Instance.GetPlayer(idx);

                p.SetRumble(power);
            }
        }


        internal GamePadState GetPadState(int index)
        {
            switch ((PlayerIndex)index)
            {
                case PlayerIndex.One:
                    return pad1;

                case PlayerIndex.Two:
                    return pad2;

                case PlayerIndex.Three:
                    return pad3;

                case PlayerIndex.Four:
                    return pad4;
            }

            throw new Exception("Invalid pad index !!! " + index);
        }

        internal GamePadState GetPreviousPadState(int index)
        {
            switch ((PlayerIndex)index)
            {
                case PlayerIndex.One:
                    return ppad1;

                case PlayerIndex.Two:
                    return ppad2;

                case PlayerIndex.Three:
                    return ppad3;

                case PlayerIndex.Four:
                    return ppad4;
            }

            throw new Exception("Invalid pad index !!! " + index);
        }

        /// <summary>
        /// If the player use a gamepad, this will check that it is connected
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool CheckIfPadIsConnectedForPlayer(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.KeyboardMouse:
                    return true;

                case DeviceType.Gamepad:
                    return GetPadState(player.Device.Index).IsConnected;

#if WINDOWS
                case DeviceType.Joystick:
                    return JoystickManager.ConnectedJoystick[player.Device.Index];
#endif
            }

            return false;
        }


        #region Button press check

        /// <summary>
        /// Player pressed pause button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonPause(Player player)
        {
            if (player.IsPlayingOnWindows())
            {
                bool pressed = ((keyboard.IsKeyUp(Keys.P) && pkeyboard.IsKeyDown(Keys.P)) || (keyboard.IsKeyUp(Keys.Pause) && pkeyboard.IsKeyDown(Keys.Pause)) || (keyboard.IsKeyUp(Keys.Escape) && pkeyboard.IsKeyDown(Keys.Escape)));

                if (player.IsPlayingWithAGamepad())
                {
                    pressed |= HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Start, GetPadState(player.Device.Index).Buttons.Start);
                }

                return pressed;
            }

            return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Start, GetPadState(player.Device.Index).Buttons.Start);
        }

        /// <summary>
        /// Player pressed retry level button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonRetryLevel(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.Joystick:
                case DeviceType.KeyboardMouse:
                    return (keyboard.IsKeyUp(Keys.R) && pkeyboard.IsKeyDown(Keys.R));
#endif
                case DeviceType.Gamepad:
                    return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Y, GetPadState(player.Device.Index).Buttons.Y);
            }

            return false;
        }

        /// <summary>
        /// Player pressed escape button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonEsc(Player player)
        {
            if (player.IsPlayingOnWindows())
            {
                bool pressed = (keyboard.IsKeyUp(Keys.Q) && pkeyboard.IsKeyDown(Keys.Q));

                if (player.IsPlayingWithAGamepad())
                {
                    pressed |= HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Back, GetPadState(player.Device.Index).Buttons.Back);
                }

                return pressed;
            }

            return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Back, GetPadState(player.Device.Index).Buttons.Back);
        }

        /// <summary>
        /// Player pressed Confirm button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonConfirm(Player player)
        {
            if (TGPAContext.Instance.IsActive)
            {
                if (player.IsPlayingOnWindows())
                {
                    bool pressed = false;
                    pressed = (HasBeenPressed(pmouse.LeftButton, mouse.LeftButton)) || (pkeyboard.IsKeyDown(Keys.Enter) && keyboard.IsKeyUp(Keys.Enter));

                    if (player.IsPlayingWithAGamepad())
                    {
                        pressed |= HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.A, GetPadState(player.Device.Index).Buttons.A);
                    }

                    if (player.IsPlayingWithAJoystick())
                    {
                        pressed |= (previousJoyState[player.Device.Index].Buttons[0] && !joyState[player.Device.Index].Buttons[0]);
                    }

                    return pressed;
                }
                else
                {
                    return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.A, GetPadState(player.Device.Index).Buttons.A);
                }


            }
            return false;
        }

        /// <summary>
        /// Player is pressing confirm button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerIsPressingButtonConfirm(Player player)
        {

            if (TGPAContext.Instance.IsActive)
            {
                if (player.IsPlayingOnWindows())
                {
                    bool pressed = false;
                    pressed = (mouse.LeftButton == ButtonState.Pressed) || keyboard.IsKeyDown(Keys.Enter);

                    if (player.IsPlayingWithAGamepad())
                    {
                        pressed |= GetPadState(player.Device.Index).Buttons.A == ButtonState.Pressed;
                    }

                    if (player.IsPlayingWithAJoystick())
                    {
                        pressed |= joyState[player.Device.Index].Buttons[0];
                    }

                    return pressed;
                }
                else
                {
                    return GetPadState(player.Device.Index).Buttons.A == ButtonState.Pressed;
                }


            }
            return false;
        }

        /// <summary>
        /// Player pressed Back button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonBack(Player player)
        {
            if (player.IsPlayingOnWindows())
            {
                bool pressed = (keyboard.IsKeyUp(Keys.Escape) && pkeyboard.IsKeyDown(Keys.Escape));

                if (player.IsPlayingWithAGamepad())
                {
                    pressed |= (HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.B, GetPadState(player.Device.Index).Buttons.B) || HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Back, GetPadState(player.Device.Index).Buttons.Back));
                }

                if (player.IsPlayingWithAJoystick())
                {
                    pressed |= (previousJoyState[player.Device.Index].Buttons[1] && !joyState[player.Device.Index].Buttons[1]);
                }

                return pressed;
            }

            return (HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.B, GetPadState(player.Device.Index).Buttons.B) || HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Back, GetPadState(player.Device.Index).Buttons.Back));
        }

        /// <summary>
        /// Player pressed Fire button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonFire(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    return (keyboard.IsKeyDown(Keys.RightControl) || keyboard.IsKeyDown(Keys.LeftControl) || (mouse.LeftButton == ButtonState.Pressed));
#endif
                case DeviceType.Gamepad:
                    return IsTriggerPressed(GetPadState(player.Device.Index).Triggers.Right) || (GetPadState(player.Device.Index).Buttons.B == ButtonState.Pressed);

                case DeviceType.Joystick:
                    return joyState[player.Device.Index].Buttons[0];
            }

            return false;
        }

        /// <summary>
        /// Player pressed Bomb button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonBomb(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    return (keyboard.IsKeyDown(Keys.RightAlt) || keyboard.IsKeyDown(Keys.LeftAlt) || (mouse.RightButton == ButtonState.Pressed));
#endif
                case DeviceType.Gamepad:
                    return IsTriggerPressed(GetPadState(player.Device.Index).Triggers.Left) || HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.X, GetPadState(player.Device.Index).Buttons.X);

                case DeviceType.Joystick:
                    return joyState[player.Device.Index].Buttons[1];
            }

            return false;
        }

        /// <summary>
        /// Player press and can keep pressed Up Button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public float PlayerGoUp(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    if(keyboard.IsKeyDown(Keys.Up)) {
                        return 1.0f;
                    }

                    break;
#endif
                case DeviceType.Gamepad:

                    float stickValue = GetPadState(player.Device.Index).ThumbSticks.Left.Y;

                    if (stickValue > 0.25f)
                    {
                        return stickValue;
                    }
                    else if (GetPadState(player.Device.Index).DPad.Up == ButtonState.Pressed)
                    {
                        return 1.0f;
                    }

                    break;

                case DeviceType.Joystick:
                    if (joyState[player.Device.Index].YAxis < JoystickManager.MiddleAxisValue)
                    {
                        return 1.0f;
                    }
                    break;
            }

            return 0.0f;
        }

        /// <summary>
        /// Player press and can keep pressed Down Button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public float PlayerGoDown(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    if(keyboard.IsKeyDown(Keys.Down)) {
                        return 1.0f;
                    }

                    break;
#endif
                case DeviceType.Gamepad:

                    float stickValue = GetPadState(player.Device.Index).ThumbSticks.Left.Y;

                    if (stickValue < -0.25f)
                    {
                        return Math.Abs(stickValue);
                    }
                    else if (GetPadState(player.Device.Index).DPad.Down == ButtonState.Pressed)
                    {
                        return 1.0f;
                    }

                    break;

                case DeviceType.Joystick:
                    if (joyState[player.Device.Index].YAxis > JoystickManager.MiddleAxisValue)
                    {
                        return 1.0f;
                    }
                    break;

            }
            return 0.0f;
        }

        /// <summary>
        /// Player press and can keep pressed Up Button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public float PlayerGoRight(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    if(keyboard.IsKeyDown(Keys.Right)) {
                        return 1.0f;
                    }

                    break;
#endif
                case DeviceType.Gamepad:

                    float stickValue = GetPadState(player.Device.Index).ThumbSticks.Left.X;

                    if (stickValue > 0.25f)
                    {
                        return Math.Abs(stickValue);
                    }
                    else if (GetPadState(player.Device.Index).DPad.Right == ButtonState.Pressed)
                    {
                        return 1.0f;
                    }

                    break;

                case DeviceType.Joystick:
                    if (joyState[player.Device.Index].XAxis > JoystickManager.MiddleAxisValue)
                    {
                        return 1.0f;
                    }
                    break;

            }
            return 0.0f;
        }

        /// <summary>
        /// Player press and can keep pressed Down Button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public float PlayerGoLeft(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    if(keyboard.IsKeyDown(Keys.Left)) {
                        return 1.0f;
                    }

                    break;
#endif
                case DeviceType.Gamepad:

                    float stickValue = GetPadState(player.Device.Index).ThumbSticks.Left.X;

                    if (stickValue < -0.25f)
                    {
                        return Math.Abs(stickValue);
                    }
                    else if (GetPadState(player.Device.Index).DPad.Left == ButtonState.Pressed)
                    {
                        return 1.0f;
                    }

                    break;

                case DeviceType.Joystick:
                    if (joyState[player.Device.Index].XAxis < JoystickManager.MiddleAxisValue)
                    {
                        return 1.0f;
                    }
                    break;

            }
            return 0.0f;
        }

        /// <summary>
        /// Player pressed Up button 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressUp(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    return IsPadUp(GetPreviousPadState(player.Device.Index), GetPadState(player.Device.Index));
            }

            return false;
        }

        /// <summary>
        /// Player pressed Down button (for menus)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressDown(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    return IsPadDown(GetPreviousPadState(player.Device.Index), GetPadState(player.Device.Index));
            }

            return false;
        }

        /// <summary>
        /// Player pressed Left button (for menus)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressLeft(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    return IsPadLeft(GetPreviousPadState(player.Device.Index), GetPadState(player.Device.Index));
            }

            return false;
        }

        /// <summary>
        /// Player pressed Right button (for menus)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressRight(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    return IsPadRight(GetPreviousPadState(player.Device.Index), GetPadState(player.Device.Index));
            }

            return false;
        }

#if DEBUG
        /// <summary>
        /// Player pressed Debug button (D)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressDebugButton(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    return (pkeyboard.IsKeyDown(Keys.D) && keyboard.IsKeyUp(Keys.D));
#endif
                case DeviceType.Gamepad:
                    return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.LeftShoulder, GetPadState(player.Device.Index).Buttons.LeftShoulder);
            }

            return false;
        }
#endif
        /// <summary>
        /// Player pressed X button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressXButton(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    return (pkeyboard.IsKeyDown(Keys.L) && keyboard.IsKeyUp(Keys.L));
#endif

                case DeviceType.Gamepad:
                    return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.X, GetPadState(player.Device.Index).Buttons.X);

                case DeviceType.Joystick:
                    return (previousJoyState[player.Device.Index].Buttons[2] && !joyState[player.Device.Index].Buttons[2]);
            }

            return false;
        }

        /// <summary>
        /// Player pressed Y button 
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressYButton(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    return (pkeyboard.IsKeyDown(Keys.M) && keyboard.IsKeyUp(Keys.M));
#endif

                case DeviceType.Gamepad:
                    return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Y, GetPadState(player.Device.Index).Buttons.Y);

                case DeviceType.Joystick:
                    return (previousJoyState[player.Device.Index].Buttons[3] && !joyState[player.Device.Index].Buttons[3]);
            }

            return false;
        }

        /// <summary>
        /// Player pressed switch mode button (Y/ TAB)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonSwitch(Player player)
        {
            switch (player.Device.Type)
            {
#if WINDOWS
                case DeviceType.KeyboardMouse:
                    return (pkeyboard.IsKeyDown(Keys.Tab) && keyboard.IsKeyUp(Keys.Tab));
#endif

                case DeviceType.Gamepad:
                    return HasBeenPressed(GetPreviousPadState(player.Device.Index).Buttons.Y, GetPadState(player.Device.Index).Buttons.Y);

                case DeviceType.Joystick:
                    return (previousJoyState[player.Device.Index].Buttons[3] && !joyState[player.Device.Index].Buttons[3]);
            }

            return false;
        }

        /// <summary>
        /// Player pressed Left mouse clic (for menus)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressLeftClic(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.KeyboardMouse:
                    return ((pmouse.LeftButton == ButtonState.Pressed) && (mouse.LeftButton == ButtonState.Released));
            }
            return false;
        }

        /// <summary>
        /// Player pressed Right mouse button (for menus)
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressRightClic(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.KeyboardMouse:
                    return ((pmouse.RightButton == ButtonState.Pressed) && (mouse.RightButton == ButtonState.Released));
            }
            return false;
        }


        #endregion

        /// <summary>
        /// Determine if player 2 joined the game, and with wich device
        /// </summary>
        /// <param name="player1"></param>
        /// <returns></returns>
        public Device Player2JoinedTheGame(Player player1)
        {
            for (PlayerIndex index = PlayerIndex.One; index <= PlayerIndex.Four; index++)
            {
                bool pressed = false;
                switch (index)
                {
                    case PlayerIndex.One:
                        pressed = HasBeenPressed(ppad1.Buttons.Start, pad1.Buttons.Start);
                        break;

                    case PlayerIndex.Two:
                        pressed = HasBeenPressed(ppad2.Buttons.Start, pad2.Buttons.Start);
                        break;

                    case PlayerIndex.Three:
                        pressed = HasBeenPressed(ppad3.Buttons.Start, pad3.Buttons.Start);
                        break;

                    case PlayerIndex.Four:
                        pressed = HasBeenPressed(ppad4.Buttons.Start, pad4.Buttons.Start);
                        break;
                }

                if (pressed)
                {
                    Device playerDevice = new Device(DeviceType.Gamepad, (int)index);

                    //Not player 1 ?
                    if (player1.Device.Type != DeviceType.KeyboardMouse)
                    {
                        if (player1.Device.Type == playerDevice.Type)
                        {
                            if (player1.Device.Index == playerDevice.Index) return null;
                        }
                    }

                    return playerDevice;
                }
            }

            if (player1.Device.Type != DeviceType.KeyboardMouse)
            {
                if (pkeyboard.IsKeyDown(Keys.Enter) && keyboard.IsKeyUp(Keys.Enter)) //On PC Player 2 has to hit Enter touch to join
                {
                    return new Device();
                }
            }

            //Joystick join
            //HACK : Disabled cause this cause me too many problems
            //#if WINDOWS
            //            for (int i = 0; i < JoystickManager.MaxJoystickNumber; i++)
            //            {
            //                if ((player1.Device.Type == DeviceType.Joystick) && (player1.Device.Index == i)) continue;

            //                if (JoystickManager.ConnectedJoystick[i])
            //                {
            //                    foreach (bool b in joyState[i].Buttons)
            //                    {
            //                        if (b == true)
            //                        {
            //                            return new Device(DeviceType.Joystick,i);
            //                        }
            //                    }
            //                }
            //            }
            //#endif

            return null;
        }

        /// <summary>
        /// Player pressed Y button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerIsPressingButtonPlus(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    return GetPadState(player.Device.Index).Buttons.Y == ButtonState.Pressed;
            }

            return false;
        }

        /// <summary>
        /// Player pressed X button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerIsPressingButtonMinus(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    return GetPadState(player.Device.Index).Buttons.X == ButtonState.Pressed;
            }

            return false;
        }

        /// <summary>
        /// Player pressed cheat button
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public bool PlayerPressButtonCheat(Player player)
        {
            switch (player.Device.Type)
            {
                case DeviceType.Joystick:
                case DeviceType.KeyboardMouse:
                    return keyboard.IsKeyDown(Keys.Space);

                case DeviceType.Gamepad:
                    return GetPadState(player.Device.Index).Buttons.RightShoulder == ButtonState.Pressed;
            }

            return false;
        }


        #region "Generic" code

        public static bool HasBeenPressed(ButtonState previousButtonState, ButtonState buttonState)
        {
            return previousButtonState == ButtonState.Released && buttonState == ButtonState.Pressed;
        }

        /// <summary>
        /// Return the last key typed the keyboard, or null if it don't exists
        /// </summary>
        /// <returns></returns>
        public Keys? GetPressedKey()
        {
            if (pkeyboard.GetPressedKeys().Length > 0)
            {
                Keys firstkey = pkeyboard.GetPressedKeys()[0];
                if (pkeyboard.IsKeyDown(firstkey) && keyboard.IsKeyUp(firstkey))
                {
                    return firstkey;
                }
            }

            return null;
        }

        /// <summary>
        /// Return the eventual last pressed button on a controller
        /// </summary>
        /// <returns></returns>
        public GamePadButtonsList? GetPressedButton(Player player)
        {
            GamePadState ppad, pad;
            switch (player.Device.Type)
            {
                case DeviceType.Gamepad:
                    ppad = GetPreviousPadState(player.Device.Index);
                    pad = GetPadState(player.Device.Index);
                    break;

                default:
                    throw new Exception("You requested buttons for keyboard/joystick");
            }

            if (HasBeenPressed(ppad.Buttons.A, pad.Buttons.A)) return GamePadButtonsList.A;
            if (HasBeenPressed(ppad.Buttons.B, pad.Buttons.B)) return GamePadButtonsList.B;
            if (HasBeenPressed(ppad.Buttons.X, pad.Buttons.X)) return GamePadButtonsList.X;
            if (HasBeenPressed(ppad.Buttons.Y, pad.Buttons.Y)) return GamePadButtonsList.Y;
            if (HasBeenPressed(ppad.Buttons.Back, pad.Buttons.Back)) return GamePadButtonsList.Back;
            if (HasBeenPressed(ppad.Buttons.Start, pad.Buttons.Start)) return GamePadButtonsList.Start;
            if (HasBeenPressed(ppad.Buttons.BigButton, pad.Buttons.BigButton)) return GamePadButtonsList.Home;
            if (HasBeenPressed(ppad.Buttons.RightShoulder, pad.Buttons.RightShoulder)) return GamePadButtonsList.RB;
            if (HasBeenPressed(ppad.Buttons.LeftShoulder, pad.Buttons.LeftShoulder)) return GamePadButtonsList.LB;
            if (IsTriggerPressed(pad.Triggers.Left)) return GamePadButtonsList.LT;
            if (IsTriggerPressed(pad.Triggers.Right)) return GamePadButtonsList.RT;
            if (PlayerPressUp(player)) return GamePadButtonsList.Up;
            if (PlayerPressDown(player)) return GamePadButtonsList.Down;
            if (PlayerPressLeft(player)) return GamePadButtonsList.Left;
            if (PlayerPressRight(player)) return GamePadButtonsList.Right;

            return null;
        }

        public static bool IsTriggerPressed(float trigger)
        {
            return trigger > 0.2f;
        }

        public static bool IsPadUp(GamePadState ppad, GamePadState pad)
        {
            return ((ppad.DPad.Up == ButtonState.Pressed) && (pad.DPad.Up == ButtonState.Released))
            || ((ppad.ThumbSticks.Left.Y > 0.0f) && (pad.ThumbSticks.Left.Y == 0.0f));
        }

        public static bool IsPadDown(GamePadState ppad, GamePadState pad)
        {
            return ((ppad.DPad.Down == ButtonState.Pressed) && (pad.DPad.Down == ButtonState.Released))
            || ((ppad.ThumbSticks.Left.Y < 0.0f) && (pad.ThumbSticks.Left.Y == 0.0f));
        }

        public static bool IsPadLeft(GamePadState ppad, GamePadState pad)
        {
            return ((ppad.DPad.Left == ButtonState.Pressed) && (pad.DPad.Left == ButtonState.Released))
            || ((ppad.ThumbSticks.Left.X < 0.0f) && (pad.ThumbSticks.Left.X == 0.0f));
        }

        public static bool IsPadRight(GamePadState ppad, GamePadState pad)
        {
            return ((ppad.DPad.Right == ButtonState.Pressed) && (pad.DPad.Right == ButtonState.Released))
            || ((ppad.ThumbSticks.Left.X > 0.0f) && (pad.ThumbSticks.Left.X == 0.0f));
        }

        /// <summary>
        /// User has clicked (Button A on Pad)
        /// </summary>
        public static bool IsClic(MouseState previousMouse, MouseState mouse)
        {
            if ((previousMouse.LeftButton == ButtonState.Pressed) && (mouse.LeftButton == ButtonState.Released))
                return true;

            return false;
        }

        #endregion

        /// <summary>
        /// User is clicking
        /// </summary>
        public bool IsLeftClicking()
        {
            return (this.mouse.LeftButton == ButtonState.Pressed);
        }


        /// <summary>
        /// Returns pressed keys of keyboard
        /// </summary>
        /// <returns></returns>
        public Keys[] GetKeyboardPressKeys()
        {
            return keyboard.GetPressedKeys();
        }

        /// <summary>
        /// Returns previouisly pressed keys of keyboard
        /// </summary>
        /// <returns></returns>
        public Keys[] GetPreviousKeyboardPressKeys()
        {
            return pkeyboard.GetPressedKeys();
        }

    }
}
