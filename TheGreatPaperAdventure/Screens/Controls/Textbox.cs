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
using Microsoft.Xna.Framework.Input;
using System.Diagnostics;
using System.Text.RegularExpressions;
using TGPA.Game.Graphics;

namespace TGPA.Screens.Controls
{
    /// <summary>
    /// Simple TextBox where player can type in
    /// </summary>
    public class Textbox : TGPAControl
    {
        private static Rectangle boxSrc = new Rectangle(10, 285, 500, 60);
        private static int MaxTextLength = 45;

        private String text;
        private bool blink;
        private double blinkCooldown;

        public Textbox(String name, Vector2 location)
            : base(name, location)
        {
            this.text = "";
            this.blink = false;
            this.blinkCooldown = 750f;
        }

        public override void Update(GameTime gameTime)
        {
            if (blinkCooldown > 0.0f)
            {
                blinkCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (blinkCooldown < 0)
                {
                    blink = !blink;
                    blinkCooldown = 750f;
                }
            }
#if WINDOWS
            if (this.Focus)
            {
                //Typing text
                Keys[] currentKeys = TGPAContext.Instance.InputManager.GetKeyboardPressKeys();
                Keys[] lastKeys = TGPAContext.Instance.InputManager.GetPreviousKeyboardPressKeys();
                bool found = false;

                //Find pressed keys
                for (int i = 0; i < currentKeys.Length; i++)
                {
                    found = false;

                    for (int y = 0; y < lastKeys.Length; y++)
                    {
                        if (currentKeys[i] == lastKeys[y])
                        {
                            found = true;
                            break;
                        }
                    }

                    if (!found)
                    {
                        bool upper = currentKeys.Contains(Keys.RightShift) || currentKeys.Contains(Keys.LeftShift);

                        //Add new key
                        PressKey(currentKeys[i], upper);
                    }
                }
            }
#endif

            this.DstRect = new Rectangle((int)location.X, (int)location.Y, 700, 70);


            base.Update(gameTime);
        }

        /// <summary>
        /// A key has been pressed and need to be added to textbox
        /// </summary>
        /// <param name="key"></param>
        private void PressKey(Keys key, bool upper)
        {
            if (key == Keys.Back)
            {
                if (text.Length > 0)
                {
                    text = text.Substring(0, text.Length - 1);
                }
            }
            else if (key == Keys.Space)
            {
                if (text.Length < MaxTextLength)
                {
                    text += " ";
                }
            }
            else
            {
                if (text.Length < MaxTextLength)
                {
                    String toAdd;
                    String c = ((char)key).ToString();

                    Regex alphaPattern = new Regex("[^a-zA-Z0-9]");
                    if (!alphaPattern.IsMatch(c))
                    {
                        if (upper)
                        {
                            toAdd = ("" + c).ToUpper();
                        }
                        else
                        {
                            toAdd = ("" + c).ToLower();
                        }
                        text = text + toAdd;
                    }
                }
            }

            this.Changed = true;
        }

        public override void Draw(TGPASpriteBatch spriteBatch)
        {
             //Text in box
            //************************************************************
            Vector2 textLocation = this.location;
            textLocation.X += 245;

            try
            {
                if (Focus)
                {
                    if (blink)
                    {
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,textLocation.X,textLocation.Y, this.text + "_",512);
                    }
                    else
                    {
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,textLocation.X, textLocation.Y, this.text, 512);
                    }
                }
                else
                {
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,textLocation.X, textLocation.Y, this.text, 512);
                }
            }
            catch (ArgumentException)
            {
                this.PressKey(Keys.Back, false);
            }

            //Box name
            //************************************************************
            TGPAContext.Instance.TextPrinter.Color = Color.Navy;
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, this.location.X, this.location.Y, this.name, 512);
            TGPAContext.Instance.TextPrinter.Color = Color.Black;
        }

        public String Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
    }
}
