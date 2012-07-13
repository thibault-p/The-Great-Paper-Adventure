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
using TGPA.Utils;
using Microsoft.Xna.Framework.Input;
using TGPA.Game.Graphics;

namespace TGPA
{
    /// <summary>
    /// Informations to display on screen during game
    /// </summary>
    public class HUD
    {
        private static double p2PressStartBlinkTime = 1500f;

        private Rectangle score, lives, bombs;
        private Rectangle ammo, ammoLevel;

        #region Hud for boss
        private bool drawWarning;
        private bool drawBossLifebar;
        private Rectangle warningSrcRect;
        private Rectangle srcBarOut, srcBarIn;

        private double bossLifePurcent;
        private int initWidth;
        private float alpha;
        private double time;
        private static Texture2D warningSprite;

        #endregion

        /// <summary>
        /// Needle for ammo
        /// </summary>
        public double CurrentNeedleAngleP1 { get; set; }
        public double CurrentNeedleAngleP2 { get; set; }

        /// <summary>
        /// Icon for weapon ammo type
        /// </summary>
        public Rectangle WeaponP1 { get; set; }
        public Rectangle WeaponP2 { get; set; }

        private bool displayPressStartPlayer2;
        private double p2blinkCooldown;
        private Color displayedColor;

        /// <summary>
        /// Initialize informations
        /// </summary>
        public HUD()
        {
            score = new Rectangle(314, 181, 708, 100);
            lives = new Rectangle(25, 186, 109, 85);
            bombs = new Rectangle(174, 198, 109, 85);
            ammo = new Rectangle(0, 0, 100, 90);
            ammoLevel = new Rectangle(125, 30, 50, 25);
            CurrentNeedleAngleP1 = 0.0f;
            CurrentNeedleAngleP2 = 0.0f;

            warningSrcRect = new Rectangle(860, 327, 142, 210);
            srcBarOut = new Rectangle(0, 882, 695, 60);
            srcBarIn = new Rectangle(0, 942, 695, 60);
            initWidth = srcBarIn.Width;
            bossLifePurcent = 1;
            time = -1;
            alpha = 0f;

            displayPressStartPlayer2 = true;
            p2blinkCooldown = p2PressStartBlinkTime;
        }

        public void Update(GameTime gameTime)
        {
            if (time == -1f)
                time = gameTime.TotalGameTime.TotalMilliseconds;

            if (drawWarning)
            {
                if (gameTime.TotalGameTime.TotalMilliseconds - time < 1500f)
                {
                    alpha += 0.02f;
                    alpha = alpha > 1f ? 1f : alpha;
                }
                else
                {
                    alpha -= 0.04f;
                    alpha = alpha < 0f ? 0f : alpha;
                }
            }
            
            // only display "Press to Start" when there is a second or more game pad available. (not counting the first obviously)
            bool padConnected = (GamePad.GetState(PlayerIndex.One).IsConnected && TGPAContext.Instance.Player1.Index != PlayerIndex.One)
                || (GamePad.GetState(PlayerIndex.Two).IsConnected && TGPAContext.Instance.Player1.Index != PlayerIndex.Two)
                || (GamePad.GetState(PlayerIndex.Three).IsConnected && TGPAContext.Instance.Player1.Index != PlayerIndex.Three)
                || (GamePad.GetState(PlayerIndex.Four).IsConnected && TGPAContext.Instance.Player1.Index != PlayerIndex.Four);

            if ((p2blinkCooldown > 0f) && (padConnected))
            {
                p2blinkCooldown -= gameTime.ElapsedGameTime.TotalMilliseconds;

                if (p2blinkCooldown < 0)
                {
                    displayPressStartPlayer2 = !displayPressStartPlayer2;
                    p2blinkCooldown = p2PressStartBlinkTime;
                    displayedColor = new Color(RandomMachine.GetRandomFloat(0, 1.0f), RandomMachine.GetRandomFloat(0, 1.0f), RandomMachine.GetRandomFloat(0, 1.0f));
                }
            }
            else
            {
                displayPressStartPlayer2 = false;
            }
        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            if (drawBossLifebar)
            {
                //Draw lifebar : inside
                Rectangle dst = srcBarIn;
                dst.X = (TGPAContext.Instance.ScreenWidth - srcBarIn.Width) / 2;
#if XBOX
                dst.Y = TGPAContext.Instance.TitleSafeArea.Bottom - srcBarIn.Height;
#else
                dst.Y = TGPAContext.Instance.TitleSafeArea.Bottom - 2 * srcBarIn.Height;
#endif
                dst.Width = (int)((double)initWidth * bossLifePurcent);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.HudTex, dst, srcBarIn, Color.White);

                //Then a rectangle
                dst.Width = srcBarOut.Width;
                spriteBatch.Draw(TGPAContext.Instance.HudTex, dst, srcBarOut, Color.White);

                spriteBatch.End();
            }

            if (drawWarning)
            {
                Rectangle dst = new Rectangle((TGPAContext.Instance.ScreenWidth / 2) - (warningSprite.Width / 2),
                                           (TGPAContext.Instance.ScreenHeight / 2) - (warningSprite.Height / 2),
                                           warningSprite.Width,
                                           warningSprite.Height);

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(warningSprite, dst, Color.White * alpha);
                spriteBatch.End();
            }

            //***************************************
            // Player 1 HUD
            //***************************************

            #region Player 1 HUD

            Rectangle sRect;
            Rectangle dRect;
            Color cCheat = Color.White;

            if (TGPAContext.Instance.Player1.Lives >= 0)
            {
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
                TGPAContext.Instance.TextPrinter.Size = 1;

                sRect = this.Lives;
                dRect = this.Lives;
                dRect.X = TGPAContext.Instance.TitleSafeArea.Left + 10;
                dRect.Y = TGPAContext.Instance.TitleSafeArea.Top;
                dRect.Width = (int)(((float)this.Lives.Width) / 2.0f);
                dRect.Height = (int)(((float)this.Lives.Height) / 2.0f);

                int leftX = dRect.X;

                if (TGPAContext.Instance.Cheatcodes.IsInvincible) cCheat = (new Color(Color.Silver.R, Color.Silver.G, Color.Silver.B, 0.75f));

                if (TGPAContext.Instance.Player1.Lives < 6)
                {
                    for (int i = 0; i < TGPAContext.Instance.Player1.Lives; i++)
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                        spriteBatch.End();
                        dRect.X += 20;
                    }
                }
                else
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                    spriteBatch.End();
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,dRect.X + dRect.Width + 5, 20 + dRect.Y, "x " + TGPAContext.Instance.Player1.Lives, 128);
                }

                sRect = this.Bombs;
                dRect = this.Bombs;
                dRect.X = TGPAContext.Instance.TitleSafeArea.Left + 10;
                dRect.Y = TGPAContext.Instance.TitleSafeArea.Top + 50;
                dRect.Width = (int)(((float)this.Lives.Width) / 2.0f);
                dRect.Height = (int)(((float)this.Lives.Height) / 2.0f);

                if (TGPAContext.Instance.Player1.Bomb.Ammo < 6)
                {
                    for (int i = 0; i < TGPAContext.Instance.Player1.Bomb.Ammo; i++)
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                        spriteBatch.End();
                        dRect.X += 20;
                    }
                }
                else
                {
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, Color.White);
                    spriteBatch.End();
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,dRect.X + dRect.Width + 5, 65 + TGPAContext.Instance.TitleSafeArea.Top, "x " + TGPAContext.Instance.Player1.Bomb.Ammo);
                }

                //Ammo left
                sRect = this.Ammo;
                dRect = this.Ammo;
                dRect.X = TGPAContext.Instance.TitleSafeArea.Left + 10;
                dRect.Y = TGPAContext.Instance.TitleSafeArea.Top + 90;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                spriteBatch.End();

                sRect = this.AmmoLevel;
                dRect = this.AmmoLevel;
                dRect.X = TGPAContext.Instance.TitleSafeArea.Left + 58;
                dRect.Y = TGPAContext.Instance.TitleSafeArea.Top + 90 + 59;

                double purcentAmmo = (float)TGPAContext.Instance.Player1.Weapon.Ammo / (float)TGPAContext.Instance.Player1.Weapon.MaxAmmo;
                double rotation = purcentAmmo * Math.PI;
                Color c = Color.White;

                if (rotation > this.CurrentNeedleAngleP1)
                {
                    this.CurrentNeedleAngleP1 += 0.01;
                    if (this.CurrentNeedleAngleP1 > rotation) this.CurrentNeedleAngleP1 = rotation;
                }
                else if (rotation < this.CurrentNeedleAngleP1)
                {
                    this.CurrentNeedleAngleP1 -= 0.01;
                    if (this.CurrentNeedleAngleP1 < rotation) this.CurrentNeedleAngleP1 = rotation;
                }

                if (this.CurrentNeedleAngleP1 > Math.PI)
                {
                    this.CurrentNeedleAngleP1 = Math.PI;
                }

                if (this.CurrentNeedleAngleP1 > 3 * Math.PI / 4)
                    c = new Color(Color.Green.R, Color.Green.G, Color.Green.B, 0.8f);
                else if ((this.CurrentNeedleAngleP1 < 3 * Math.PI / 4) && (rotation > 2 * Math.PI / 4))
                    c = new Color(Color.Blue.R, Color.Blue.G, Color.Blue.B, 0.8f);
                else if (this.CurrentNeedleAngleP1 < 2 * Math.PI / 4)
                {
                    c = (new Color(Color.Red.R, Color.Red.G, Color.Red.B, 0.8f));
                    if (this.CurrentNeedleAngleP1 < Math.PI / 5)
                    {
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,leftX, dRect.Y + dRect.Height, LocalizedStrings.GetString("LowAmmo"), 128);
                    }
                }

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, c, (float)this.CurrentNeedleAngleP1, new Vector2(34, 12), SpriteEffects.None, 1.0f);
                spriteBatch.End();

                //Ammo type ( = bonus to get)
                sRect = this.WeaponP1;
                dRect = this.WeaponP1;
                dRect.Width /= 6;
                dRect.Height /= 6;
                dRect.X = TGPAContext.Instance.TitleSafeArea.Left + 160;
                dRect.Y = TGPAContext.Instance.TitleSafeArea.Top;

                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.BonusTex, dRect, sRect, Color.White);
                spriteBatch.End();


                //more precise debug infos
                if (TGPAContext.Instance.Saver.OptionsData.DebugMode)
                {
                    TGPAContext.Instance.TextPrinter.Color = Color.Chartreuse;
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,200, 0, "" + TGPAContext.Instance.Map.Scroll + " invincible : " + TGPAContext.Instance.Player1.Invincible);
                    TGPAContext.Instance.TextPrinter.Color = Color.Black;
                }
            }

            #endregion

            //***************************************
            // Player 2 HUD
            //***************************************

            #region Player 2 HUD

                if ((TGPAContext.Instance.Player2 != null) && (TGPAContext.Instance.Player2.Lives >= 0))
                {
                    cCheat = Color.White;

                    TGPAContext.Instance.TextPrinter.Color = Color.Black;
                    TGPAContext.Instance.TextPrinter.Size = 1;

                    sRect = this.Lives;
                    dRect = this.Lives;
                    dRect.X = TGPAContext.Instance.ScreenWidth - 10 - TGPAContext.Instance.TitleSafeArea.Left;
                    dRect.Y = TGPAContext.Instance.TitleSafeArea.Top;
                    dRect.Width = (int)(((float)this.Lives.Width) / 2.0f);
                    dRect.Height = (int)(((float)this.Lives.Height) / 2.0f);
                    dRect.X -= dRect.Width;

                    if (TGPAContext.Instance.Player2.Lives < 6)
                    {
                        for (int i = 0; i < TGPAContext.Instance.Player2.Lives; i++)
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                            spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                            spriteBatch.End();
                            dRect.X -= 20;
                        }
                    }
                    else
                    {
                        dRect.X = TGPAContext.Instance.ScreenWidth - TGPAContext.Instance.TitleSafeArea.Left - 125;

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                        spriteBatch.End();
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,dRect.X + dRect.Width + 5, dRect.Y + 20, "x " + TGPAContext.Instance.Player2.Lives, 128);
                    }

                    sRect = this.Bombs;
                    dRect = this.Bombs;
                    dRect.X = TGPAContext.Instance.ScreenWidth - 10 - TGPAContext.Instance.TitleSafeArea.Left;
                    dRect.Y = 50 + TGPAContext.Instance.TitleSafeArea.Top;
                    dRect.Width = (int)(((float)this.Lives.Width) / 2.0f);
                    dRect.Height = (int)(((float)this.Lives.Height) / 2.0f);
                    dRect.X -= dRect.Width;

                    if (TGPAContext.Instance.Player2.Bomb.Ammo < 6)
                    {
                        for (int i = 0; i < TGPAContext.Instance.Player2.Bomb.Ammo; i++)
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                            spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                            spriteBatch.End();
                            dRect.X -= 20;
                        }
                    }
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                        spriteBatch.End();
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,dRect.X - dRect.Width + 20, 65 + TGPAContext.Instance.TitleSafeArea.Top, "x " + TGPAContext.Instance.Player2.Bomb.Ammo, 128);
                    }

                    //Ammo left
                    sRect = this.Ammo;
                    dRect = this.Ammo;
                    dRect.X = TGPAContext.Instance.ScreenWidth - dRect.Width - 10 - TGPAContext.Instance.TitleSafeArea.Left;
                    dRect.Y = 90 + TGPAContext.Instance.TitleSafeArea.Top;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
                    spriteBatch.End();

                    sRect = this.AmmoLevel;
                    dRect = this.AmmoLevel;
                    dRect.X = TGPAContext.Instance.ScreenWidth - 58 - TGPAContext.Instance.TitleSafeArea.Left;
                    dRect.Y = TGPAContext.Instance.TitleSafeArea.Top + 90 + 59;

                    double rotation = ((float)TGPAContext.Instance.Player2.Weapon.Ammo / (float)TGPAContext.Instance.Player2.Weapon.MaxAmmo) * Math.PI;
                    Color c = Color.White;

                    if (rotation > this.CurrentNeedleAngleP2)
                    {
                        this.CurrentNeedleAngleP2 += 0.01;
                        if (this.CurrentNeedleAngleP2 > rotation) this.CurrentNeedleAngleP2 = rotation;
                    }
                    else if (rotation < this.CurrentNeedleAngleP2)
                    {
                        this.CurrentNeedleAngleP2 -= 0.01;
                        if (this.CurrentNeedleAngleP2 < rotation) this.CurrentNeedleAngleP2 = rotation;
                    }

                    if (this.CurrentNeedleAngleP2 > Math.PI)
                    {
                        this.CurrentNeedleAngleP2 = Math.PI;
                    }

                    if (this.CurrentNeedleAngleP2 > 3 * Math.PI / 4)
                        c = (Color.Green * 0.8f);
                    else if ((this.CurrentNeedleAngleP2 < 3 * Math.PI / 4) && (rotation > 2 * Math.PI / 4))
                        c = (Color.Blue * 0.8f);
                    else if (this.CurrentNeedleAngleP2 < 2 * Math.PI / 4)
                    {
                        c = (Color.Red * 0.8f);

                        if (this.CurrentNeedleAngleP2 < Math.PI / 5)
                        {
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,TGPAContext.Instance.TitleSafeArea.Right - 200, dRect.Y + dRect.Height, LocalizedStrings.GetString("LowAmmo"), 100);
                        }
                    }

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, c, (float)this.CurrentNeedleAngleP2, new Vector2(34, 12), SpriteEffects.None, 1.0f);
                    spriteBatch.End();

                    //Ammo type ( = bonus to get)
                    sRect = this.WeaponP2;
                    dRect = this.WeaponP2;
                    dRect.Width /= 6;
                    dRect.Height /= 6;
                    dRect.X = TGPAContext.Instance.ScreenWidth - TGPAContext.Instance.TitleSafeArea.Left - 160 - dRect.Width;
                    dRect.Y = TGPAContext.Instance.TitleSafeArea.Top;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(TGPAContext.Instance.BonusTex, dRect, sRect, Color.White);
                    spriteBatch.End();

                }
                else
                {
                    if (TGPAContext.Instance.Player2 == null)
                    {
                        if (displayPressStartPlayer2)
                        {
                            String text = null;
                            int x = 0;
#if WINDOWS
                            if (TGPAContext.Instance.Player1.Device.Type == Utils.Input.DeviceType.KeyboardMouse)
                            {
                                text = LocalizedStrings.GetString("P2PressStartXbox"); //Start
                            }
                            else
                            {
                                text = LocalizedStrings.GetString("P2PressStartPC"); //Start/Enter
                            }
                            x = TGPAContext.Instance.ScreenWidth - TGPAContext.Instance.TitleSafeArea.Left - 305;
#else
                            text = LocalizedStrings.GetString("P2PressStartXbox");
                            x = TGPAContext.Instance.ScreenWidth - TGPAContext.Instance.TitleSafeArea.Left - 225;
#endif

                            TGPAContext.Instance.TextPrinter.Color = displayedColor;
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,x, TGPAContext.Instance.TitleSafeArea.Top + 15, text, 512);
                            TGPAContext.Instance.TextPrinter.Color = Color.Black;
                        }
                    }
                }

            //Score
            sRect = this.Score;
            dRect = this.Score;

            dRect.Width = (int)(((float)this.Score.Width) / 2.0f);
            dRect.Height = (int)(((float)this.Score.Height) / 2.0f);



            int scoreX = TGPAContext.Instance.TitleSafeArea.Top;
            scoreX = (TGPAContext.Instance.ScreenWidth / 2) - (dRect.Width / 2);
            dRect.X = scoreX;

            int scoreY = TGPAContext.Instance.TitleSafeArea.Top;
            dRect.Y = scoreY;

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
            spriteBatch.Draw(TGPAContext.Instance.HudTex, dRect, sRect, cCheat);
            spriteBatch.End();

            int total = TGPAContext.Instance.Player1.Score;
            if (TGPAContext.Instance.Player2 != null)
            {
                total += TGPAContext.Instance.Player2.Score;
            }

            TGPAContext.Instance.TextPrinter.Write(spriteBatch,scoreX + dRect.Width/3, scoreY + 20, "" + total, 56);

            #endregion
        }

        public void ChangeWeaponIcon(PlayerIndex player, Type weaponType)
        {
            Bonus b = new Bonus(weaponType.Name.ToString());
            switch (player)
            {
                case PlayerIndex.One:
                    WeaponP1 = b.SrcRect;
                    break;

                case PlayerIndex.Two:
                    WeaponP2 = b.SrcRect;
                    break;
            }

        }

        public static void LoadWarningContent(ContentManager Content)
        {
            warningSprite = Content.Load<Texture2D>(@"gfx/warning");
        }

        public Rectangle Score
        {
            get { return score; }
        }
        public Rectangle Lives
        {
            get { return lives; }
        }
        public Rectangle Bombs
        {
            get { return bombs; }
        }
        public Rectangle Ammo
        {
            get { return ammo; }
        }
        public Rectangle AmmoLevel
        {
            get { return ammoLevel; }
        }

        public double BossLifePurcent
        {
            set { this.bossLifePurcent = value; }
        }

        public bool DrawBossLifebar
        {
            set { this.drawBossLifebar = value; }
        }

        public bool DrawWarning
        {
            set
            {
                if (this.drawWarning != value)
                {
                    this.drawWarning = value;
                    this.time = -1f;
                }
            }
        }
    }
}
