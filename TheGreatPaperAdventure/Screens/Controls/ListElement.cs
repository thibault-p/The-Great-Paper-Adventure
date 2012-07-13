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
using TGPA.Localization;
using TGPA.Game.Graphics;

namespace TGPA.Screens.Controls
{
    /// <summary>
    /// Displayable element for GUI list control
    /// </summary>
    public class ListElement
    {
        private static Texture2D sprite;

        private Rectangle sRect;
        private Object value;
        private string label;

        public static void LoadContent(ContentManager Content)
        {
            sprite = Content.Load<Texture2D>(@"gfx/Controls/elementsSprites");
        }

        public ListElement(Object value, Rectangle sRect) :
            this(value, sRect, null)
        {
        }

        public ListElement(Object value, String label) :
            this(value, Rectangle.Empty, label)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="sRect"></param>
        /// <param name="label">Localization ID for the element</param>
        public ListElement(Object value, Rectangle sRect, String label)
        {
            this.value = value;
            this.sRect = sRect;
            this.label = label;
        }

        public void Update(GameTime gameTimee)
        {

        }

        public void Draw(Rectangle dRect, TGPASpriteBatch spriteBatch)
        {
            //Draw element
            if (sRect != Rectangle.Empty)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(sprite, dRect, sRect, Color.White);
                spriteBatch.End();
            }

            Vector2 textLoc = new Vector2(dRect.X, dRect.Y);

            string text;

            if (label == null)
            {
                text = value.ToString();
            }
            else
            {
                text = LocalizedStrings.GetString(label);
            }

            dRect.Width = text.Length + 20;
            dRect.Height = 20;

            TGPAContext.Instance.TextPrinter.Write(spriteBatch,textLoc.X, textLoc.Y, text, 1024);
        }

        /// <summary>
        /// 
        /// </summary>
        public Rectangle SrcRect
        {
            get { return this.sRect; }
            set { this.sRect = value; }
        }

        public Object Value
        {
            get { return this.value; }
            set { this.value = value; }
        }
    }
}
