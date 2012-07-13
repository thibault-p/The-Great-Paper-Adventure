//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGPA.Game;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using TGPA.Localization;
using System.Threading;
using TGPA.Maps;
using TGPA.Game.Graphics;

namespace TGPA.Screens
{
    /// <summary>
    /// Loading screen that can be displayed over another screen.
    /// Load a map and display its overview.
    /// </summary>
    public class LoadingScreen
    {
        private static Texture2D texture;
        private MapOverview overview;
        private TheGreatPaperGame Game;
        private Rectangle currentloading, loading1, loading2, loading3, background, backgroundDst;
        private double frameTime;
        private BackgroundPart previewBG1, previewBG2;
        private float previewBG1Fadeout, previewBG2Fadeout;
        public bool IsLoaded { get; set; }

        public LoadingScreen(MapOverview overview, TheGreatPaperGame Game)
        {
            this.Initialize();
            this.Game = Game;

            loading1 = new Rectangle(0, 356, 270, 178);
            loading2 = new Rectangle(0, 178, 270, 178);
            loading3 = new Rectangle(0, 0, 270, 178);
            currentloading = loading1;

            background = new Rectangle(300, 0, 1024, 768);
            backgroundDst = background;
            backgroundDst.X = 0;
            backgroundDst.Y = 0;
            backgroundDst.Width = TGPAContext.Instance.ScreenWidth;
            backgroundDst.Height = TGPAContext.Instance.ScreenHeight;

            this.overview = overview;
            this.previewBG1 = null;
            this.previewBG1Fadeout = 0.0f;
            this.previewBG2 = null;
            this.previewBG2Fadeout = 0.0f;
            this.IsLoaded = false;
        }

        public void Initialize()
        {
            this.previewBG1 = null;
            this.previewBG1Fadeout = 0.0f;
            this.previewBG2 = null;
            this.previewBG2Fadeout = 0.0f;
            this.IsLoaded = false;
        }

        public static void LoadContent(ContentManager Content)
        {
            texture = Content.Load<Texture2D>(@"gfx/loading");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            if (this.previewBG1 == null)
            {
                if ((TGPAContext.Instance.Map != null) && (TGPAContext.Instance.Map.Ended == Map.EndMode.None))
                {
                    if (TGPAContext.Instance.Map.Background1.BackgroundSprites.Count > 0)
                    {
                        this.previewBG1 = TGPAContext.Instance.Map.Background1.BackgroundSprites[0];
                    }
                }
            }
            else
            {
                previewBG1Fadeout += 0.01f;
                if (previewBG1Fadeout > 1f)
                {
                    previewBG1Fadeout = 1f;

                    if (this.previewBG2 == null)
                    {
                        if (TGPAContext.Instance.Map.Background2.BackgroundSprites.Count > 0)
                        {
                            this.previewBG2 = TGPAContext.Instance.Map.Background2.BackgroundSprites[0];
                        }

                    }
                    else
                    {
                        previewBG2Fadeout += 0.01f;
                        if (previewBG2Fadeout > 1f) previewBG2Fadeout = 1f;
                    }
                }
            }

            if (gameTime.TotalGameTime.TotalMilliseconds - frameTime > 500f)
            {
                frameTime = gameTime.TotalGameTime.TotalMilliseconds;

                if (currentloading == loading1)
                    currentloading = loading2;
                else if (currentloading == loading2)
                    currentloading = loading3;
                else if (currentloading == loading3)
                    currentloading = loading1;
            }

            if (TGPAContext.Instance.MapLoaded)
            {
                if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                {
                    TGPAContext.Instance.CurrentGameState = GameState.Game;
                }
            }

            this.IsLoaded = TGPAContext.Instance.MapLoaded;
        }
        /// <summary>
        /// Draw a sleeping cat + map information
        /// </summary>
        /// <param name="spriteBatch"></param>
        public void Draw(TGPASpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(TGPAContext.Instance.NullTex, backgroundDst, null, Color.White);
            spriteBatch.End();

            //Draw background
            if ((this.previewBG1 != null) && (this.previewBG1Fadeout > 0.0f))
            {
                if (this.previewBG1.SpriteTexture != null)
                {
                    Color c = Color.White * previewBG1Fadeout;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(this.previewBG1.SpriteTexture, backgroundDst, null, c);
                    spriteBatch.End();
                }
            }

            if ((this.previewBG2 != null) && (this.previewBG2Fadeout > 0.0f))
            {
                if (this.previewBG2.SpriteTexture != null)
                {
                    Color c = (Color.White * previewBG2Fadeout);

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(this.previewBG2.SpriteTexture, backgroundDst, null, c);
                    spriteBatch.End();
                }
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(texture, backgroundDst, background, Color.White);
            spriteBatch.End();

            Rectangle rect2 = new Rectangle(170, 195, 685, 220);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(TGPAContext.Instance.NullTex, rect2, Color.White * 0.55f);
            spriteBatch.End();

            //display name and description
            Vector2 textPosition = new Vector2(500, 85);
            textPosition.X -= (overview.Name.Length / 2) * 10;

            TGPAContext.Instance.TextPrinter.Color = Color.Black;
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, textPosition.X, textPosition.Y, overview.Name, 50);

            textPosition = new Vector2(185, 180);
            TGPAContext.Instance.TextPrinter.Size = 1f;
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, textPosition.X, textPosition.Y, overview.Description, 45);


            Rectangle dst = currentloading;
            dst.X = TGPAContext.Instance.ScreenWidth / 2 - dst.Width / 2;
            dst.Y = (2 * TGPAContext.Instance.ScreenHeight / 3 - dst.Height / 2);

            if (!TGPAContext.Instance.MapLoaded)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(texture, dst, currentloading, Color.White);
                spriteBatch.End();
            }
            else
            {
                Rectangle rect3 = new Rectangle(410, 470, 190, 55);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.NullTex, rect3, Color.White * 0.45f);
                spriteBatch.End();

                dst.Y -= dst.Height;

                TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Confirm", TGPAContext.Instance.Player1.Device.Type, new Vector2((TGPAContext.Instance.ScreenWidth / 2) - 100, dst.Y + dst.Height + 50), Color.White);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, TGPAContext.Instance.ScreenWidth / 2 - 50, dst.Y + dst.Height + 50, LocalizedStrings.GetString("LevelSelectionScreenPressStart"), 64);
            }

        }

        /// <summary>
        /// Launch the map loading thread
        /// </summary>
        public void LoadMap()
        {
            //Loading map
            Thread t = new Thread(new ThreadStart(
                                delegate()
                                {
                                    TGPAContext.Instance.PrepareGame();
                                }));
            t.Start();
        }
    }
}
