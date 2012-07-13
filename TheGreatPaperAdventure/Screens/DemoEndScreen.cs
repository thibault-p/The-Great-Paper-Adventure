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
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.GamerServices;
using TGPA.Localization;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;

namespace TGPA.Screens
{
    /// <summary>
    /// End of the demo :)
    /// </summary>
    public class DemoEndScreen
    {
        private Texture2D background;

        private double remainingTime;
        private float alpha;

        public DemoEndScreen()
        {
            this.Initialize();
        }

        public void Initialize()
        {
            this.remainingTime = 30000f;
            this.alpha = 0f;
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>(@"gfx/DemoEndScreen/background");
        }

        public void Update(GameTime gameTime)
        {
            this.remainingTime -= gameTime.ElapsedGameTime.TotalMilliseconds;

#if XBOX
            if (TGPAContext.Instance.InputManager.PlayerPressYButton(TGPAContext.Instance.Player1))
            {
                if (Guide.IsVisible == false)
                {
                    //Player need rights
                    SignedInGamer xboxProfile = Device.DeviceProfile(TGPAContext.Instance.Player1.Device);

                    if (xboxProfile.Privileges.AllowPurchaseContent)
                    {
                        Guide.ShowMarketplace((PlayerIndex)TGPAContext.Instance.Player1.Device.Index);
                    }
                    else
                    {
                        Guide.BeginShowMessageBox(LocalizedStrings.GetString("BuyFailTitle"), LocalizedStrings.GetString("BuyFailContent"), new List<string> { "OK" }, 0, MessageBoxIcon.Warning, null, null);
                    }
                }
            }
#endif
            //Leave the game with fade out
            if (alpha < 1f)
            {
                if ((TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1)) 
                    || (this.remainingTime < 0f) 
                    || (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    || (TGPAContext.Instance.IsTrialMode == false)
                    )
                {
                    alpha += 0.05f;
                    this.remainingTime = -1f;
                }
            }
            else
            {
                if (TGPAContext.Instance.IsTrialMode == false)
                {
                    TGPAContext.Instance.CurrentGameState = GameState.LevelSelectionScreen;
                }
                else
                {
                    TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
                }
            }
        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            //Display ad
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(TGPAContext.Instance.NullTex, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), Color.White);

            bool specialCenter = false;
            Rectangle inboundsDst = TGPAContext.Instance.TitleSafeArea;
            if ((inboundsDst.Width > 1024) || (inboundsDst.Height > 768))
            {
                specialCenter = true;
            }

            if (specialCenter)
            {
                Rectangle dst = new Rectangle(0, 0, 1024, 768);
                dst.X = TGPAContext.Instance.ScreenWidth / 2 - dst.Width / 2;
                dst.Y = TGPAContext.Instance.ScreenHeight / 2 - dst.Height / 2;

                spriteBatch.Draw(background, dst, Color.White);
            }
            else
            {
                spriteBatch.Draw(background, inboundsDst, Color.White);
            }

            if (alpha > 0f)
            {
                spriteBatch.Draw(TGPAContext.Instance.NullTex, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), Color.Black * alpha);
            }

            spriteBatch.End();

#if XBOX
            TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Plus", TGPAContext.Instance.Player1.Device.Type, new Vector2(250, 700));
            TGPAContext.Instance.TextPrinter.Write(spriteBatch,new Vector2(300, 700), LocalizedStrings.GetString("Buy"));
#endif
        }
    }
}
