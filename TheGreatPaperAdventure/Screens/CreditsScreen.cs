//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using TGPA.Localization;
using Microsoft.Xna.Framework.Media;
using TGPA.MenuItems;
using TGPA.Game.Graphics;

namespace TGPA.Screens
{
    /// <summary>
    /// End of the game :)
    /// </summary>
    public class CreditsScreen 
    {
        #region Page template

        /// <summary>
        /// A page of the credits sequence
        /// </summary>
        private abstract class CreditPage
        {
            /// <summary>
            /// Time to live of the page. When over, go to next page
            /// </summary>
            public int TTL { get; set; }
            public float alphaColor { get; set; }

            public CreditPage()
            {
                alphaColor = 0f;
            }

            public void Update(GameTime gameTime)
            {
                this.TTL -= (int)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (TTL < 1500f)
                {
                    //Fade out
                    alphaColor -= 0.02f;
                }
                else
                {
                    //Fade in
                    if (alphaColor < 1)
                    {
                        alphaColor += 0.02f;
                        if (alphaColor > 1) alphaColor = 1;
                    }
                }
            }

            public abstract void Draw(TGPASpriteBatch spritbatch);
        }

        #endregion

        #region Pages

        private class CreditPageOne : CreditPage
        {
            public CreditPageOne()
                : base()
            {
                this.TTL = 7500;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.5f;

                TGPAContext.Instance.TextPrinter.Write(spriteBatch,325, TGPAContext.Instance.TitleSafeArea.Top + 275, "The Great Paper Adventure", 24);

                TGPAContext.Instance.TextPrinter.Size = 1.2f;

                TGPAContext.Instance.TextPrinter.Write(spriteBatch,405, TGPAContext.Instance.TitleSafeArea.Top + 315, "Thanks for playing !", 56);

                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private class CreditPageTwo : CreditPage
        {
            public CreditPageTwo()
                : base()
            {
                this.TTL = 10000;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 275, "Project leader", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,550, TGPAContext.Instance.TitleSafeArea.Top + 275, "Main Developpers", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 425, "2D \"Graphist\"", 52);

                TGPAContext.Instance.TextPrinter.Color = Color.Red;
                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 325, "Damien MAYANCE (Valryon)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,550, TGPAContext.Instance.TitleSafeArea.Top + 325, "Damien MAYANCE (Valryon)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,550, TGPAContext.Instance.TitleSafeArea.Top + 365, "Thibault PERSON (LapinouFou)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 475, "Thibault PERSON (LapinouFou)", 52);

                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private class CreditPageThree : CreditPage
        {
            public CreditPageThree()
                : base()
            {
                this.TTL = 10000;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.1f;

                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 175, "iPad developper", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 275, "Website developper", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,550, TGPAContext.Instance.TitleSafeArea.Top + 175, "Artistic manager", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 550, TGPAContext.Instance.TitleSafeArea.Top + 275, "Xbox360 developper", 52);

                TGPAContext.Instance.TextPrinter.Color = (Color.Red);
                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 225, "Aymeric DE ABREU (Aymarick)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, TGPAContext.Instance.TitleSafeArea.Top + 325, "Matthieu OGER (Ashen)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,550, TGPAContext.Instance.TitleSafeArea.Top + 225, "Anaïs NOBLANC (Semoule)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 550, TGPAContext.Instance.TitleSafeArea.Top + 325, "Louis LAGRANGE (Minishlink)", 52);

                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private class CreditPageThreeBis : CreditPage
        {
            public CreditPageThreeBis()
                : base()
            {
                this.TTL = 10000;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,160, TGPAContext.Instance.TitleSafeArea.Top + 275, "Cooking Mama + Graphist Tormentor", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,580, TGPAContext.Instance.TitleSafeArea.Top + 275, "Music composer", 52);

                TGPAContext.Instance.TextPrinter.Color = Color.Red;
                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,160, TGPAContext.Instance.TitleSafeArea.Top + 325, "Anaïs NOBLANC (Semoule)", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,580, TGPAContext.Instance.TitleSafeArea.Top + 325, "Cyril BROUILLARD (Spintronic)", 52);

                TGPAContext.Instance.TextPrinter.Size = 0.8f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 580, TGPAContext.Instance.TitleSafeArea.Top + 360, "(Download the game OST for free !", 52);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 580, TGPAContext.Instance.TitleSafeArea.Top + 395, "Check our website)", 52);
                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private class CreditPageFour : CreditPage
        {
            public CreditPageFour()
                : base()
            {
                this.TTL = 15000;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,180, TGPAContext.Instance.TitleSafeArea.Top + 75, "Official Testers", 52);

                TGPAContext.Instance.TextPrinter.Color = Color.Red;
                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 110, "Matthieu OGER (Ashen)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 100, TGPAContext.Instance.TitleSafeArea.Top + 145, "Anaïs NOBLANC (Semoule)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 100, TGPAContext.Instance.TitleSafeArea.Top + 180, "Vincent ROZEC (Paihel)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 215, "Cédric COPY (Boris)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 250, "Amandine ROCH (Diosadidine)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 100, TGPAContext.Instance.TitleSafeArea.Top + 285, " Cédric LERCH (Cedange)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 320, "Gabriel CORBEL (yaki) !!!", 152);

                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private class CreditPageFive : CreditPage
        {
            public CreditPageFive()
                : base()
            {
                this.TTL = 15000;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.1f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,240, TGPAContext.Instance.TitleSafeArea.Top + 255, "Special thanks", 52);

                //XNA reviewers
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,150, TGPAContext.Instance.TitleSafeArea.Top + 100, "XNA Peer-reviewers and playtesters", 152);
                
                TGPAContext.Instance.TextPrinter.Color = Color.DarkGreen;
                TGPAContext.Instance.TextPrinter.Size = 0.8f;                
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 150, "Mr Helmut, Irregular Games, Stinky Badger, Bounding Box Games LLC ", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 180, "DaleCantwel, Quebarium inc., Blau, laurent goethals ", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 210, "ggaler, Eclipse Games", 152);

                TGPAContext.Instance.TextPrinter.Size = 1f;
                TGPAContext.Instance.TextPrinter.Color = Color.Red;

                //Other
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 305, "Aymeric DE ABREU (Aymarick), Louis LAGRANGE (Minishlink)", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch, 100, TGPAContext.Instance.TitleSafeArea.Top + 350, "Morgane BERTHOU (tsu) and 3Hitcombo", 152);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 420, "Canard PC, Dev-FR, IndieDB", 72);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,100, TGPAContext.Instance.TitleSafeArea.Top + 480, "Friends and family (We love you <3)", 72);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,150, TGPAContext.Instance.TitleSafeArea.Top + 530, "Every TGPA Player !!!", 72);
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        private class CreditPageSix : CreditPage
        {
            public CreditPageSix()
                : base()
            {
                this.TTL = 60000;
            }
            public override void Draw(TGPASpriteBatch spriteBatch)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1.2f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,390, TGPAContext.Instance.TitleSafeArea.Top + 550, " Poulpis killed : " + TGPAContext.Instance.Saver.SaveData.KilledPoulpis, 72);
                TGPAContext.Instance.TextPrinter.Size = 1f;
            }
        }

        #endregion

        private Texture2D videoBackground;
        private List<Texture2D> backgrounds;
        private float transitionAlpha, transitionDelta;
        private Video creditsVideo;
        private VideoPlayer videoPlayer;

        private List<CreditPage> pages;
        private int currentPageIndex;

        private Rectangle videoDst, bgDst;

        private bool playCredits;
        private bool playVideo;

        public CreditsScreen()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            //Update GameHasBeenEnded boolean for old savegames
            if (TGPAContext.Instance.Saver.SaveData.GameHasBeenEnded == false)
            {
                if (TGPAContext.Instance.Saver.SaveData.LastLevel >= LevelSelectionScreen.WorldCount)
                {
                    TGPAContext.Instance.Saver.SaveData.GameHasBeenEnded = true;
                    TGPAContext.Instance.Saver.Save();
                }
            }

            //Play and replay the video when the game is officially over
            playVideo = TGPAContext.Instance.Saver.SaveData.GameHasBeenEnded;

            if (playVideo == false)
            {
                playCredits = true;
            }
            else
            {
                playCredits = false;
            }

            this.videoPlayer = new VideoPlayer();

            this.currentPageIndex = 0;
            this.pages = new List<CreditPage>();
            this.pages.Add(new CreditPageOne());
            this.pages.Add(new CreditPageTwo());
            this.pages.Add(new CreditPageThree());
            this.pages.Add(new CreditPageThreeBis()); //I'm too lazy to change every page name after 3
            this.pages.Add(new CreditPageFour());
            this.pages.Add(new CreditPageFive());
            this.pages.Add(new CreditPageSix());

            this.videoDst = new Rectangle(0, 0, 720, 460);
            this.videoDst.X = (TGPAContext.Instance.ScreenWidth / 2) - (this.videoDst.Width / 2);
            this.videoDst.Y = (TGPAContext.Instance.ScreenHeight / 2) - (this.videoDst.Height / 2);

            this.bgDst = new Rectangle(0, 0, 1024, 768);
            this.bgDst.X = (TGPAContext.Instance.ScreenWidth / 2) - (this.bgDst.Width / 2);
            this.bgDst.Y = (TGPAContext.Instance.ScreenHeight / 2) - (this.bgDst.Height / 2);

            this.transitionAlpha = 0f;
            this.transitionDelta = 0.01f;
        }

        public void LoadContent(ContentManager Content)
        {
            this.videoBackground = Content.Load<Texture2D>(@"gfx/CreditsScreen/background");

            this.backgrounds = new List<Texture2D>();
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg1"));
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg2"));
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg3"));
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg4"));
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg5"));
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg6"));
            this.backgrounds.Add(Content.Load<Texture2D>(@"gfx/CreditsScreen/bg7"));

            this.creditsVideo = Content.Load<Video>(@"vids/credits");
        }

        public void Update(GameTime gameTime)
        {
            if (playVideo)
            {
                if (this.videoPlayer.State == MediaState.Stopped)
                {
                    this.videoPlayer.IsLooped = false;
                    this.videoPlayer.Play(creditsVideo);
                }

                if (this.videoPlayer.PlayPosition.Seconds == 39)
                {
                    this.videoPlayer.Pause();
                    this.playCredits = true;
                    this.playVideo = false;
                }
            }

            if (this.playCredits)
            {
                this.pages[currentPageIndex].Update(gameTime);

                this.transitionAlpha += this.transitionDelta;

                if (this.transitionAlpha > 1f)
                {
                    this.transitionAlpha = 1f;
                }
            }

            if (TGPAContext.Instance.InputManager.PlayerPressButtonBack(TGPAContext.Instance.Player1))
            {
                this.videoPlayer.Stop();
                this.videoPlayer.Dispose();

                TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
            }
            else if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
            {
                if (this.playCredits)
                {
                    if (this.pages[currentPageIndex].TTL > 1000)
                    {
                        this.pages[currentPageIndex].TTL = 1000;
                    }
                    else
                    {
                        this.pages[currentPageIndex].TTL -= 500;
                    }
                }
                else
                {
                    this.videoPlayer.Pause();
                    this.playCredits = true;
                    this.playVideo = false;
                }
            }

            if (this.pages[currentPageIndex].TTL < 0)
            {
                if (currentPageIndex + 1 < pages.Count)
                {
                    this.currentPageIndex++;
                    this.transitionAlpha = 0f;
                }
                else
                {
                    this.videoPlayer.Stop();
                    this.videoPlayer.Dispose();

                    TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
                }
            }
        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(videoBackground, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), Color.White);
            spriteBatch.End();

            if (playVideo)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.White;

                if (this.videoPlayer.State == MediaState.Playing)
                {
                    //Display video
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(this.videoPlayer.GetTexture(), this.videoDst, Color.White);
                    spriteBatch.End();
                }
            }
            else if (playCredits)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                if (currentPageIndex > 0)
                {
                    spriteBatch.Draw(backgrounds[currentPageIndex - 1], bgDst, Color.White);
                }

                spriteBatch.Draw(backgrounds[currentPageIndex], bgDst, new Color(Color.White.R, Color.White.G, Color.White.B, transitionAlpha));

                spriteBatch.End();

                this.pages[currentPageIndex].Draw(spriteBatch);
            }

            if (playVideo)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.White;
            }
            else
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Navy;
            }
            TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Back", TGPAContext.Instance.Player1.Device.Type, new Vector2(750, TGPAContext.Instance.ScreenHeight - 110));
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, new Vector2(800, TGPAContext.Instance.ScreenHeight - 100), LocalizedStrings.GetString("CreditsScreenLeave"));

            TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Confirm", TGPAContext.Instance.Player1.Device.Type, new Vector2(750, TGPAContext.Instance.ScreenHeight - 55));
            TGPAContext.Instance.TextPrinter.Write(spriteBatch, new Vector2(800, TGPAContext.Instance.ScreenHeight - 45), LocalizedStrings.GetString("CreditsScreenNext"));

            TGPAContext.Instance.TextPrinter.Color = Color.Black;
        }
    }
}
