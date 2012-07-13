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
using Microsoft.Xna.Framework.Graphics;
using TGPA.Audio;
using TGPA.Utils;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;

namespace TGPA.Game.Other
{
    /// <summary>
    /// Manage cheats
    /// </summary>
    public class Cheatcodes
    {
        private enum Cheat
        {
            Invincible,
            MaxPower,
            Kawaii,
            Gulliver,
            UnlockMap
        }

        private const float activationTime = 2000f;

        private Dictionary<Keys[], Cheat> keyboardCheats;
        private Dictionary<GamePadButtonsList[], Cheat> padCheats;

        public bool Active { get; set; }
        public bool IsInvincible { get; set; }
        public bool HasMaxPower { get; set; }
        public bool IsKawaii { get; set; }
        public bool IsGiantMode { get; set; }

        public float CheatActivationCooldown;

        private String message;
        private float alpha, alphaMessage, cooldown;

#if WINDOWS
        private List<Keys> currentKeyboardCombo;
#endif
        private List<GamePadButtonsList> currentPadCombo;

        public Cheatcodes()
        {
            //Fill cheats
            //Keyboard
            //****************************************************************************************************
            keyboardCheats = new Dictionary<Keys[], Cheat>();

            //Invincibility
            Keys[] invincibleKeys = new Keys[] { Keys.I, Keys.D, Keys.D, Keys.A, Keys.D };
            keyboardCheats.Add(invincibleKeys, Cheat.Invincible);

            //Maximum power
            Keys[] maxpowerKeys = new Keys[] { Keys.N, Keys.A, Keys.B, Keys.A, Keys.Z, Keys.T, Keys.A, Keys.G };
            keyboardCheats.Add(maxpowerKeys, Cheat.MaxPower);

            //Kawaii
            Keys[] kawaiKeys = new Keys[] { Keys.O, Keys.O, Keys.O, Keys.O, Keys.O, Keys.O, Keys.O, Keys.O, Keys.O, Keys.O };
            keyboardCheats.Add(kawaiKeys, Cheat.Kawaii);

            //Giant mode
            Keys[] giantKeys = new Keys[] { Keys.L, Keys.O, Keys.V, Keys.E };
            keyboardCheats.Add(giantKeys, Cheat.Gulliver);

            //Unlock map
            Keys[] mapKeys = new Keys[] { Keys.S, Keys.E, Keys.M, Keys.O, Keys.U, Keys.L, Keys.E };
            keyboardCheats.Add(mapKeys, Cheat.UnlockMap);

            //Gamepad
            //****************************************************************************************************
            padCheats = new Dictionary<GamePadButtonsList[], Cheat>();

            //Invincibility
            GamePadButtonsList[] invincibleButtons = new GamePadButtonsList[] { GamePadButtonsList.Up, GamePadButtonsList.Down, GamePadButtonsList.X, GamePadButtonsList.A, GamePadButtonsList.X, GamePadButtonsList.A };
            padCheats.Add(invincibleButtons, Cheat.Invincible);

            //Maximum power
            GamePadButtonsList[] maxpowerButtons = new GamePadButtonsList[] { GamePadButtonsList.Down, GamePadButtonsList.Up, GamePadButtonsList.Y, GamePadButtonsList.B, GamePadButtonsList.Y, GamePadButtonsList.B };
            padCheats.Add(maxpowerButtons, Cheat.MaxPower);

            //Kawaii
            GamePadButtonsList[] kawaiButtons = new GamePadButtonsList[] { GamePadButtonsList.Left, GamePadButtonsList.Left, GamePadButtonsList.Left, GamePadButtonsList.Right };
            padCheats.Add(kawaiButtons, Cheat.Kawaii);

            //Giant mode
            GamePadButtonsList[] giantButtons = new GamePadButtonsList[] { GamePadButtonsList.A, GamePadButtonsList.A, GamePadButtonsList.B, GamePadButtonsList.B, GamePadButtonsList.A, GamePadButtonsList.A, GamePadButtonsList.Y };
            padCheats.Add(giantButtons, Cheat.Gulliver);

            //Unlock map
            GamePadButtonsList[] mapButtons = new GamePadButtonsList[] { GamePadButtonsList.Y, GamePadButtonsList.A, GamePadButtonsList.B, GamePadButtonsList.X, GamePadButtonsList.Down };
            padCheats.Add(mapButtons, Cheat.UnlockMap);

            IsInvincible = false;
            HasMaxPower = false;
            IsKawaii = false;
            IsGiantMode = false;

            Initialize();
        }

        public void Initialize()
        {
            CheatActivationCooldown = activationTime;
            Active = false;
#if WINDOWS
            currentKeyboardCombo = null;
#endif
        }

        public void Update(GameTime gameTime)
        {
            if (!Active)
            {
                CheatActivationCooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (CheatActivationCooldown <= 0f)
                {
                    Active = true;
                    cooldown = 12500f;
                    alpha = 1f;
                    SoundEngine.Instance.PlaySound("gameboy");
                }
            }
            else
            {
                bool inactive = true;
                Cheat? cheat = null;

                //Pad recognition
                //******************************************************************************
                if (TGPAContext.Instance.PausingPlayer.Device.Type == DeviceType.Gamepad)
                {
                    GamePadButtonsList? button = null;

                    button = TGPAContext.Instance.InputManager.GetPressedButton(TGPAContext.Instance.PausingPlayer);

                    if (button != null)
                    {
                        if (currentPadCombo == null) currentPadCombo = new List<GamePadButtonsList>();
                        currentPadCombo.Add((GamePadButtonsList)button);

                    }

                    if (currentPadCombo != null)
                    {
                        bool atLeastOneComboMatch = false;
                        GamePadButtonsList[] imTheLastMatchingCombo = null;

                        foreach (GamePadButtonsList[] combo in padCheats.Keys)
                        {
                            if (currentPadCombo == null) break;

                            for (int i = 0; (i < combo.Length && i < currentPadCombo.Count); i++)
                            {
                                if (combo[i] == currentPadCombo[i])
                                {
                                    if (i == combo.Length - 1)
                                    {
                                        //Success !
                                        cheat = padCheats[combo];
                                        currentPadCombo = null;
                                        break;
                                    }

                                    atLeastOneComboMatch = true;
                                    imTheLastMatchingCombo = combo;
                                }
                                else
                                {
                                    if (atLeastOneComboMatch)
                                    {
                                        if (imTheLastMatchingCombo == combo)
                                        {
                                            //Stop combo
                                            currentPadCombo = null;
                                        }
                                    }

                                    break;
                                }

                                if (cheat != null)
                                {
                                    break;
                                }
                            }
                        }

                        inactive = atLeastOneComboMatch;

                        if (!atLeastOneComboMatch)
                        {
                            currentPadCombo = null;
                        }
                        else
                        {
                            cooldown += 2000f;
                        }

                    }
                }
                else
                {

                    //Keyboard recognition
#if WINDOWS
                    Keys? key = null;

                    key = TGPAContext.Instance.InputManager.GetPressedKey();

                    if (key != null)
                    {
                        if (currentKeyboardCombo == null) currentKeyboardCombo = new List<Keys>();
                        currentKeyboardCombo.Add((Keys)key);

                    }

                    if (currentKeyboardCombo != null)
                    {
                        bool atLeastOneComboMatch = false;
                        Keys[] imTheLastMatchingCombo = null;

                        foreach (Keys[] combo in keyboardCheats.Keys)
                        {
                            if (currentKeyboardCombo == null) break;

                            for (int i = 0; (i < combo.Length && i < currentKeyboardCombo.Count); i++)
                            {
                                if (combo[i] == currentKeyboardCombo[i])
                                {
                                    if (i == combo.Length - 1)
                                    {
                                        //Success !
                                        cheat = keyboardCheats[combo];
                                        currentKeyboardCombo = null;
                                        break;
                                    }

                                    atLeastOneComboMatch = true;
                                    imTheLastMatchingCombo = combo;
                                }
                                else
                                {
                                    if (atLeastOneComboMatch)
                                    {
                                        if (imTheLastMatchingCombo == combo)
                                        {
                                            //Stop combo
                                            currentKeyboardCombo = null;
                                        }
                                    }

                                    break;
                                }

                                if (cheat != null)
                                {
                                    break;
                                }
                            }
                        }

                        inactive = atLeastOneComboMatch;

                        if (!atLeastOneComboMatch)
                        {
                            currentKeyboardCombo = null;
                        }
                        else
                        {
                            cooldown += 2000f;
                        }

                    }
#endif
                }

                if (cheat != null)
                {
                    switch (cheat)
                    {
                        case Cheat.Invincible:
                            this.message = "Invincible !";
                            this.IsInvincible = !this.IsInvincible;
                            break;

                        case Cheat.Kawaii:
                            this.message = "<3 Kawaii <3";
                            this.IsKawaii = !this.IsKawaii;
                            break;

                        case Cheat.MaxPower:
                            this.message = "Maximum Power !";
                            this.HasMaxPower = !this.HasMaxPower;
                            break;

                        case Cheat.Gulliver:
                            this.message = "You look tiny...";
                            this.IsGiantMode = !this.IsGiantMode;
                            break;

                        case Cheat.UnlockMap:
                            this.message = "You lost";
                            TGPAContext.Instance.Saver.SaveData.LastLevel += 1;
                            break;
                    }

                    alphaMessage = 1.01f;

                    cheat = null;
                }

                if (inactive)
                {
                    cooldown -= (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                    if (cooldown <= 0)
                    {
                        Active = false;
                        CheatActivationCooldown = activationTime;
                    }
                    else if (cooldown <= 1000)
                    {
                        alpha -= 0.01f;
                    }
                }

                if (alphaMessage > 0f)
                {
                    alphaMessage -= 0.01f;
                    if (alphaMessage < 0f) alphaMessage = 0f;
                }
            }
        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            if (Active)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.White *alpha;
#if DEBUG
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,new Vector2(730, 650), "Pouet " + this.cooldown);
#endif

                if (alphaMessage > 0f)
                {
                    TGPAContext.Instance.TextPrinter.Color = Color.Chartreuse *alphaMessage;
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,new Vector2(730, 600), message);
                }
#if DEBUG
#if WINDOWS
                if (currentKeyboardCombo != null)
                {
                    int x = 0;
                    foreach (Keys k in currentKeyboardCombo)
                    {
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,new Vector2(730, 20 * x), k.ToString());
                        x++;
                    }
                }
#endif
                if (currentPadCombo != null)
                {
                    int x = 0;
                    foreach (GamePadButtonsList b in currentPadCombo)
                    {
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,new Vector2(730, 20 * x), b.ToString());
                        x++;
                    }
                }
                TGPAContext.Instance.TextPrinter.Color = Color.White;
#endif
            }
        }
    }
}
