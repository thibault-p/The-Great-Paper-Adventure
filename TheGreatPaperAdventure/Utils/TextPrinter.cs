//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Graphics;
using System.Diagnostics;

namespace TGPA.Utils
{
    /// <summary>
    /// Display Text on screen. 
    /// Convert some keyword into button icons. See ButtonPrinter
    /// </summary>
    public class TextPrinter
    {

        private float size = 1f;
        private Color color = Color.White;
        private SpriteFont defaultFont, font, optionFont;
        private float alphaColor;

        public TextPrinter()
        {

        }

        public void LoadContent(ContentManager Content)
        {
            this.defaultFont = Content.Load<SpriteFont>(@"fonts/gameFont");
            this.optionFont = Content.Load<SpriteFont>(@"fonts/optionsFont");
            this.SetDefaultFont();
        }

        /// <summary>
        /// Text color
        /// </summary>
        public Color Color
        {
            get { return color; }
            set { color = value; }
        }

        public float AlphaColor
        {
            get { return alphaColor; }
            set { alphaColor = value; }
        }

        /// <summary>
        /// Text size
        /// </summary>
        public float Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Text font
        /// </summary>
        public SpriteFont Font
        {
            get { return this.font; }
            set { this.font = value; }
        }

        public void SetDefaultFont()
        {
            this.font = defaultFont;
        }

        public void SetOptionFont()
        {
            this.font = optionFont;
        }

        /// <summary>
        /// Write text at the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="s"></param>
        public void Write(TGPASpriteBatch spriteBatch,float x, float y, String s)
        {
            this.Write(spriteBatch,x, y, s, 1024);
        }

        /// <summary>
        /// Write text at the specified location.
        /// </summary>
        /// <param name="location"></param>
        /// <param name="s"></param>
        public void Write(TGPASpriteBatch spriteBatch, Vector2 location, String s)
        {
            this.Write(spriteBatch, location.X, location.Y, s, 1024);
        }

        /// <summary>
        /// Write text at the specified location but with custom layout settings
        /// </summary>
        /// <param name="location"></param>
        /// <param name="s"></param>
        /// <param name="lineSize"></param>
        public void Write(TGPASpriteBatch spriteBatch, float x, float y, String s, int lineSize)
        {
            if (font == null) return; //HACK

            string[] words = s.Split(' ');
            int jumpSize = (int)((float)(font.LineSpacing * Size));
            int currentLineSpacesleft = lineSize;
            int lineNumber = 0;

            StringBuilder linePart = new StringBuilder();
            bool draw = false;
            int xButtonPosition = 0;
            Vector2 buttonLocation = Vector2.Zero;
            String missingPart = "";

            foreach (string w in words)
            {
                //Draw button
                //if (ButtonPrinter.Keywords.Contains(w))
                //{
                //    if (TGPAContext.Instance.Player1 != null)
                //    {
                //        buttonLocation.X = x + xButtonPosition * 2;
                //        buttonLocation.Y = y + (lineNumber * jumpSize);
                //        buttonPrinter.Draw(spriteBatch, w, TGPAContext.Instance.Player1.Device, buttonLocation, new Color(Color.White,alphaColor));
                //    }

                //    for (int i = 0; i < w.Length; i++)
                //    {
                //        linePart.Append("  ");
                //    }
                //}
                //else
                //{

                xButtonPosition += w.Length;

                if (currentLineSpacesleft - w.Length <= 0)
                {
                    currentLineSpacesleft = lineSize;
                    draw = true;
                    lineNumber++;
                    missingPart = w;
                }
                else
                {
                    currentLineSpacesleft -= w.Length;
                    linePart.Append(w + " ");
                }

                if (draw)
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.DrawString(font, linePart.ToString(), new Vector2(x, y + (lineNumber * jumpSize)), color, 0f, new Vector2(), size, SpriteEffects.None, 1f);
                    spriteBatch.End();

                    draw = false;
                    linePart.Remove(0, linePart.Length - 1); //Clear buffer
                    linePart.Append(missingPart + " ");
                    missingPart = "";

                    xButtonPosition = 0;
                }
            }

            if (lineNumber > 0)
            {
                lineNumber++; //If just 1 line
            }

            //Last line
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.DrawString(font, linePart.ToString(), new Vector2(x, y + (lineNumber * jumpSize)), color, 0f, new Vector2(), size, SpriteEffects.None, 1f);
            spriteBatch.End();
        }




    }
}
