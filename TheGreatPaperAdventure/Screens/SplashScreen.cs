//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Audio;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;

namespace TGPA.Screens
{
    /// <summary>
    /// Small screen displaying developper info
    /// </summary>
    public class SplashScreen
    {
        private Texture2D background;
        private double timeLeft;
        private float fadeInOut;
        private bool readyToGoNext;
        private bool playSound;

        public SplashScreen()
        {
            this.Initialize();
        }

        public void Initialize()
        {
#if DEBUG
            this.timeLeft = 5000f;
#else
            this.timeLeft = 5000f;
#endif
            this.fadeInOut = 1f;
            this.readyToGoNext = false;
            this.playSound = false;
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>(@"gfx/SplashScreen/background");
        }

        public void Update(GameTime gameTime)
        {
            this.timeLeft -= gameTime.ElapsedGameTime.Milliseconds;

            if (this.timeLeft < 1000f)
            {
                this.fadeInOut += 0.02f;

                if (this.fadeInOut > 2f)
                {
                    this.fadeInOut = 2f;
                    this.readyToGoNext = true;
                }
            }
            else
            {
#if XBOX
                this.fadeInOut -= 0.005f;
#else
                this.fadeInOut -= 0.02f;
#endif
                this.fadeInOut -= 0.02f;

                if ((this.fadeInOut < 0.5f) && (!playSound))
                {
                    SoundEngine.Instance.PlaySound("gameboy");
                    playSound = true;
                }

                if (this.fadeInOut < 0f) this.fadeInOut = 0f;
            }

            if (readyToGoNext)
            {
#if WINDOWS
                //Instanciate player1 by looking for device in the savegame
                TGPAContext.Instance.Player1 = new Player(PlayerIndex.One,TGPAContext.Instance.Saver.OptionsData.Player1PlayingDevice);
                TGPAContext.Instance.Player1.Name = TGPAContext.Instance.Saver.OptionsData.Player1Name;

                if (TGPAContext.Instance.InputManager.CheckIfPadIsConnectedForPlayer(TGPAContext.Instance.Player1) == false)
                {
                    TGPAContext.Instance.Player1.Device = new Device();
                }
#endif

                TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;

            }

        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            //White background
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(TGPAContext.Instance.NullTex, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), Color.White);
            spriteBatch.End();

            //Draw 1024*768 image in center
            Rectangle dst = new Rectangle(0, 0, 1024, 768);
            dst.X = TGPAContext.Instance.ScreenWidth / 2 - dst.Width / 2;
            dst.Y = TGPAContext.Instance.ScreenHeight / 2 - dst.Height / 2;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(background, dst, Color.White);
            spriteBatch.End();

            //Draw fade in/out
            if (fadeInOut > 0f)
            {
                Color blackScreen = new Color(Color.Black.R, Color.Black.G, Color.Black.B, this.fadeInOut);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.NullTex, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), blackScreen);
                spriteBatch.End();
            }
        }
    }
}
