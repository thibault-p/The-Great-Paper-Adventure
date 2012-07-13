//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;

namespace TGPA.Utils
{
    /// <summary>
    /// Display controller buttons icons on the screen
    /// </summary>
    public class ButtonPrinter
    {
        /// <summary>
        /// Display buttons to press
        /// </summary>
        private Texture2D buttonsSprite;

        /// <summary>
        /// Recognized keywords
        /// </summary>
        public static String[] Keywords = { "#Pause", "#Move", "#Back", "#Confirm", "#Fire", "#Cancel", "#Bomb", "#Minus", "#Plus", "#Retry" };

        public void LoadContent(ContentManager Content)
        {
            buttonsSprite = Content.Load<Texture2D>(@"gfx/buttons/buttonSprites");
        }

        /// <summary>
        /// Translate a button (#Pause or #Fire for example) to a picture
        /// </summary>
        /// <param name="button">#Move #Back #Cancel #Confirm #Fire #Bomb #Pause</param>
        /// <returns></returns>
        private Rectangle TransformButtonToSprite(String button, DeviceType device)
        {
            Rectangle dst = Rectangle.Empty;

            switch (button)
            {
                case "#Move":
                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(128, 128, 360, 225);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(2, 399, 149, 165);
                            break;
                    }

                    break;

                case "#Pause":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(128, 0, 128, 115);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(713, 415, 81, 93);
                            break;
                    }

                    break;

                case "#ExitFromPause":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(0, 128, 128, 128);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(491, 425, 80, 82);
                            break;
                    }

                    break;

                case "#Fire":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(256, 0, 128, 115);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(380, 580, 83, 167);
                            break;
                    }

                    break;

                case "#Back":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(128, 0, 128, 115);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(491, 425, 80, 82);
                            break;
                    }

                    break;

                case "#Bomb":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(384, 0, 128, 128);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(468, 580, 83, 167);
                            break;
                    }

                    break;

                case "#Confirm":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(0, 0, 128, 128);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(878, 437, 76, 83);
                            break;

                    }

                    break;

                case "#Cancel":

                    switch (device)
                    {
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(128, 0, 128, 115);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(82, 625, 76, 82);
                            break;

                    }

                    break;

                case "#Plus":

                    switch (device)
                    {
                        //For switch
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(512, 16, 87, 82);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(2, 625, 76, 82);
                            break;

                    }

                    break;

                case "#Minus":

                    switch (device)
                    {
                        case DeviceType.Gamepad:
                            dst = new Rectangle(798, 437, 76, 83);
                            break;

                    }

                    break;

                case "#Retry":

                    switch (device)
                    {
                        //For switch
                        case DeviceType.Joystick:
                        case DeviceType.KeyboardMouse:
                            dst = new Rectangle(512, 128, 128, 128);
                            break;

                        case DeviceType.Gamepad:
                            dst = new Rectangle(2, 625, 76, 82);
                            break;

                    }

                    break;

                default:
                    dst = Rectangle.Empty;
                    break;
            }

            return dst;
        }

        /// <summary>
        /// Display a button on the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="button">#Move #Back #Cancel #Confirm #Move #Fire #Bomb #Pause</param>
        /// <param name="device"></param>
        public void Draw(TGPASpriteBatch spriteBatch, String button, DeviceType device, int locX, int locY)
        {
            this.Draw(spriteBatch, button, device, new Vector2(locX, locY));
        }

        /// <summary>
        /// Display a button on the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="button">#Move #Back #Cancel #Confirm #Move #Fire #Bomb #Pause</param>
        /// <param name="device"></param>
        /// <param name="location"></param>
        public void Draw(TGPASpriteBatch spriteBatch, String button, DeviceType device, Vector2 location)
        {
            this.Draw(spriteBatch, button, device, location, Color.White);
        }

        /// <summary>
        /// Display a button on the screen
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="button">#Move #Back #Cancel #Confirm #Move #Fire #Bomb #Pause</param>
        /// <param name="device"></param>
        /// <param name="location"></param>
        /// <param name="color">Special color</param>
        public void Draw(TGPASpriteBatch spriteBatch, String button, DeviceType device, Vector2 location, Color color)
        {
            if (buttonsSprite == null) return; //HACK

            Rectangle src = TransformButtonToSprite(button, device);
            Rectangle dst = src;
            dst.X = (int)location.X;
            dst.Y = (int)location.Y;
            dst.Width /= 2;
            dst.Height /= 2;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(buttonsSprite, dst, src, color);
            spriteBatch.End();
        }
    }
}
