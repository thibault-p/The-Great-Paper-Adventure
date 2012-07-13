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
using TGPA.Game.Graphics;

namespace TGPA.Game.Sound
{
    /// <summary>
    /// Display current music to player
    /// </summary>
    public class SongInfo 
    {
        protected String name;
        protected String artist;
        protected float alpha;

        private Color color;

        protected static Texture2D note;

        public SongInfo()
        {
            alpha = 0f;
            name = "";
            artist = "";
        }

        public SongInfo(String song_name, String song_artist)
        {
            alpha = 1.0f;
            name = song_name;
            artist = song_artist;
        }

        public void LoadContent(ContentManager cm)
        {
            note = cm.Load<Texture2D>(@"gfx/Messages/music");
        }

        public void Update()
        {
            alpha -= 0.003f;
            if (alpha < 0) alpha = 0f;

            color = Color.White * alpha;
        }

        public void Draw(TGPASpriteBatch spriteBatch,Rectangle src,Texture2D paperRect, GameState gameState)
        {

            if (alpha > 0f)
            {

                int width = 420;
                int height = 50;
                int x = (TGPAContext.Instance.ScreenWidth/2) - (width/2);
                int y = TGPAContext.Instance.TitleSafeArea.Top + 100;

                //Another location for title screen
                if ((gameState == GameState.TitleScreen) || (gameState == GameState.Loading) || (gameState == GameState.Credits))
                {
                    x = TGPAContext.Instance.TitleSafeArea.Left;
                    y = TGPAContext.Instance.TitleSafeArea.Top + 50;
                }

                Rectangle dst = src;
                dst.X = x;
                dst.Y = y - 20; ;
                dst.Width = width;
                dst.Height = height;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(paperRect,
                    dst,
                    src,
                    color,
                    0.0f,
                    Vector2.Zero,
                    SpriteEffects.None,
                    1.0f);
                spriteBatch.End();

                src = new Rectangle(0, 0, 32, 32);
                dst = src;
                dst.X = x + 60; ;
                dst.Y = y - 10; ;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(note, dst, src, color, 0.0f, Vector2.Zero, SpriteEffects.None, 1.0f);
                spriteBatch.End();

                TGPAContext.Instance.TextPrinter.Color =  Color.Black* alpha;
                TGPAContext.Instance.TextPrinter.Size = 0.8f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,x + 90, y - 5, artist + " - " + name, 128);
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1f;
            }
        }

        public String Name
        {
            get { return name; }
        }

        public String Artist
        {
            get { return artist; }
        }
    }
}
