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
using TGPA.Audio;
using TGPA.Utils;

using TGPA.Localization;
using TGPA.Screens;
using TGPA.Game;
using System.Threading;
using TGPA.Maps;
using TGPA.Game.Graphics;
using TGPA.Game.Save;

namespace TGPA.MenuItems
{
    /// <summary>
    /// Screen after the game : win or lose, stats, play again / next level
    /// </summary>
    public class EndLevelScreen
    {
        private Texture2D sprites;

        private Rectangle levelDst, scoreSrc, scoreDst, heartSrc, bombSrc;

        private bool victory;
        private int score;
        private int lives, totalLives, displayedLives;
        private int bombs, totalBombs, displayedBombs;
        private int scoreTotal;
        private int enemiesKilled;
        private double startGameTime;
        private double elapsedGameTime, delay;
        private ScrollDirection outDirection;

        private bool playersOut, scoreOK, highscoreOK, nextLevelOK;
        private int scoreRank = -1;
        private float fadeOut;
        private Rectangle outLimitRectangle;

        /// <summary>
        /// Where the score will be registered
        /// </summary>
        private ScoreType scoreTypeSave;
        /// <summary>
        /// Kind of scores to display
        /// </summary>
        private ScoreType scoreTypeDisplay;

        private bool cheat;

        private double timeout;

        public EndLevelScreen()
        {
            Initialize();
        }

        /// <summary>
        /// Fill data to show them to player if he wins / loses
        /// </summary>
        /// <param name="_score">Final score</param>
        /// <param name="_enemiesKilled">Number of killed enemies</param>
        /// <param name="_elapsedGameTime">Final gameTime</param>
        public void SetDatas(bool win, int _score1, int _score2, int _enemiesKilled, int remainingLives, int remainingBombs, double startTime, ScrollDirection outDirection)
        {
            this.cheat = false;

            this.victory = win;
            this.score = _score1 + _score2;
            this.enemiesKilled = _enemiesKilled;
            this.lives = remainingLives;
            this.bombs = remainingBombs;
            this.enemiesKilled = _enemiesKilled;
            this.startGameTime = startTime;
            this.elapsedGameTime = -1f;
            this.outDirection = outDirection;
            this.outLimitRectangle = Rectangle.Empty;

            //Level name
            this.levelDst = new Rectangle(0, 0, 385, 100);
            this.levelDst.X = (TGPAContext.Instance.ScreenWidth / 2) - (levelDst.Width / 2);
            this.levelDst.Y = 0;

            //Score count
            this.scoreSrc = new Rectangle(0, 200, 820, 560);
            this.scoreDst = scoreSrc;
            this.scoreDst.X = TGPAContext.Instance.ScreenWidth / 2 - this.scoreSrc.Width / 2;
            this.scoreDst.Y = 120;

            //Score items
            heartSrc = new Rectangle(517, 14, 50, 40);
            bombSrc = new Rectangle(567, 14, 36, 45);

            //Timeout security
            timeout = 2500f;
        }

        public void LoadContent(ContentManager Content)
        {
            sprites = Content.Load<Texture2D>(@"gfx/EndLevel/buttons");
        }

        public void Initialize()
        {
            this.scoreOK = false;
            this.highscoreOK = false;
            this.playersOut = false;
            this.nextLevelOK = false;

            this.scoreTotal = 0;
            this.totalLives = 0;
            this.totalBombs = 0;

            this.displayedBombs = 0;
            this.displayedLives = 0;

            this.fadeOut = 0f;
            this.scoreRank = -1;
            this.delay = 0f;

            this.scoreTypeSave = ScoreType.Single;
            if (TGPAContext.Instance.Player2 != null)
            {
                this.scoreTypeSave = ScoreType.Coop;
            }

            this.scoreTypeDisplay = this.scoreTypeSave;
        }

        public void Update(GameTime gameTime)
        {
            if (elapsedGameTime < 0f)
            {
                this.elapsedGameTime = gameTime.TotalGameTime.TotalMilliseconds;
            }

            if ((TGPAContext.Instance.Cheatcodes.IsInvincible) || (TGPAContext.Instance.Cheatcodes.HasMaxPower) || (TGPAContext.Instance.Cheatcodes.IsGiantMode))
            {
                this.cheat = true;
            }
            
            //Victory
            if (victory)
            {
                if (!playersOut)
                {
                    //Timeout security
                    timeout -= gameTime.ElapsedGameTime.TotalMilliseconds;

                    if (timeout < 0)
                    {
                        playersOut = true;
                    }
                    else
                    {
                        #region Move player ship

                        playersOut = (MovePlayerShip(TGPAContext.Instance.Player1) && (TGPAContext.Instance.Player2 == null));

                        if (playersOut) SoundEngine.Instance.PlaySound("playerEndLevel");

                        if (TGPAContext.Instance.Player2 != null)
                        {
                            playersOut = MovePlayerShip(TGPAContext.Instance.Player2);

                            if (playersOut) SoundEngine.Instance.PlaySound("playerEndLevel");
                        }

                        #endregion
                    }
                    
                }
            }
            else
            {
                if (!playersOut)
                {
                    playersOut = true; //Player is out : he's dead :(
                    this.delay = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }

            #region Display Scores

            if (playersOut)
            {
                UpdateFadeOut();

                if ((!scoreOK) && (gameTime.TotalGameTime.TotalMilliseconds - delay > 1000f))
                {
                    scoreOK = true;
                }

                if (scoreOK && !highscoreOK)
                {
                    if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                    {
                        delay = 0f;
                    }

                    if (gameTime.TotalGameTime.TotalMilliseconds - delay > 500f)
                    {
                        this.delay = gameTime.TotalGameTime.TotalMilliseconds;

                        if ((lives > 0) && (victory))
                        {
                            this.delay = gameTime.TotalGameTime.TotalMilliseconds;

                            this.totalLives += 10000;
                            this.lives--;
                            this.displayedLives++;
                        }
                        else if ((bombs > 0) && (victory))
                        {
                            this.delay = gameTime.TotalGameTime.TotalMilliseconds;

                            this.totalBombs += 5000;
                            this.bombs--;
                            this.displayedBombs++;
                        }
                        else
                        {
                            this.scoreTotal += totalBombs;
                            this.scoreTotal += totalLives;
                            this.scoreTotal += score;

                            this.highscoreOK = true;
                        }
                    }
                }
            }

            #endregion

            #region Save score

            if (highscoreOK)
            {
                //Last part of a level
                if (TGPAContext.Instance.Map.LastPart)
                {
                    //Save progression
                    if (TGPAContext.Instance.Saver.SaveData.LastLevel < LevelSelectionScreen.WorldCount)
                    {
                        if (TGPAContext.Instance.Saver.SaveData.LastLevel == TGPAContext.Instance.Map.Level)
                        {
                            TGPAContext.Instance.Saver.SaveData.LastLevel = TGPAContext.Instance.Map.Level + 1;
                        }
                    }
                }

                //Add new score to scoreboard
                if (scoreRank == -1)
                {
                    String playerName = TGPAContext.Instance.Player1.Name;
                    if (TGPAContext.Instance.Player2 != null)
                    {
                        playerName += "+" + TGPAContext.Instance.Player2.Name;
                    }

                    if (!cheat)
                    {
                        scoreRank = TGPAContext.Instance.Saver.AddScore(TGPAContext.Instance.CurrentMap, this.scoreTypeSave, new ScoreLine(-1, DateTime.Now, playerName, this.scoreTotal, TGPAContext.Instance.Options.Difficulty));
                        TGPAContext.Instance.Saver.Save();
                    }
                    else
                    {
                        scoreRank = 40;
                    }
                    highscoreOK = true;
                    this.delay = gameTime.TotalGameTime.TotalMilliseconds;
                }
            }

            #endregion

            #region Go to next level

            if (highscoreOK)
            {
                if ((gameTime.TotalGameTime.TotalMilliseconds - delay > 500f) || (TGPAContext.Instance.InputManager.PlayerPressButtonBack(TGPAContext.Instance.Player1)))
                {
                    nextLevelOK = true;
                }
            }

            if (nextLevelOK)
            {
                //Hack : stop vibrations
                TGPAContext.Instance.Player1.SetRumble(Vector2.Zero);

                if (TGPAContext.Instance.Player2 != null)
                {
                    TGPAContext.Instance.Player2.SetRumble(Vector2.Zero);
                }

                if (TGPAContext.Instance.InputManager.PlayerPressButtonBack(TGPAContext.Instance.Player1))
                {
                    TGPAContext.Instance.Saver.Save();
                    TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
                }
                else if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                {
                    TGPAContext.Instance.Saver.Save();
                    TGPAContext.Instance.NextLevel(victory);
                }
                else if (TGPAContext.Instance.InputManager.PlayerPressButtonSwitch(TGPAContext.Instance.Player1))
                {
                    this.scoreTypeDisplay = (this.scoreTypeDisplay == ScoreType.Single ? ScoreType.Coop : ScoreType.Single);
                }
            }

            #endregion
        }

        public void Draw(TGPASpriteBatch spriteBatch)
        {
            if (playersOut)
            {
                if (fadeOut > 0f)
                {
                    Color backscreen;

                    if (victory)
                    {
                        backscreen = (Color.White *this.fadeOut);
                    }
                    else
                    {
                        backscreen = (Color.Black *this.fadeOut);
                    }
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(TGPAContext.Instance.NullTex, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), backscreen);
                    spriteBatch.End();
                }

                //Level 
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.HudTex, levelDst, TGPAContext.Instance.PaperRect,Color.White *(this.fadeOut + 0.1f));
                spriteBatch.End();

                String toPrint = "Level : " + TGPAContext.Instance.Map.Level;
                int w = 20 + toPrint.Length * 11;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,levelDst.X + (levelDst.Width / 2) - w / 2, levelDst.Y + (levelDst.Height / 4), toPrint, 512);

                toPrint = TGPAContext.Instance.Map.Name;
                w = 20 + toPrint.Length * 11;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,levelDst.X + (levelDst.Width / 2) - w / 2, levelDst.Y + (levelDst.Height / 2), TGPAContext.Instance.Map.Name, 512);

                //Score 
                #region Score compute
                if (playersOut)
                {
                    scoreDst.X = TGPAContext.Instance.ScreenWidth / 2 - scoreDst.Width / 2;

                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                    spriteBatch.Draw(sprites, scoreDst, scoreSrc, Color.White);
                    spriteBatch.End();

                    double duree = (elapsedGameTime - startGameTime);
                    int h = (int)((duree / 1000) / 3600);
                    int m = (int)(((duree / 1000) / 60) % 60);
                    int s = (int)((duree / 1000) % 60);

                    if (!victory)
                    {
                        TGPAContext.Instance.TextPrinter.Color = Color.Red;
                    }

                    if (cheat)
                    {
                        TGPAContext.Instance.TextPrinter.Color = Color.Silver;
                    }


                    int x = 137;
                    int xbis = 337;
                    int y = 212;

                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,x, y, LocalizedStrings.GetString("EndlevelScreenElapsedTime") + " : ");

                    TGPAContext.Instance.TextPrinter.Color = Color.Black;

                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,xbis, 240, h.ToString("00") + "h" + m.ToString("00") + "m" + s.ToString("00") + "s");

                    if ((victory) && (!cheat))
                    {
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,x, scoreDst.Y + 150, LocalizedStrings.GetString("EndlevelScreenRemainingLives") + " :");

                        //Display heart icons or just a heart and a number
                        if (displayedLives < 6)
                        {
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

                            for (int i = 0; i < displayedLives; i++)
                            {
                                Rectangle heartDst = new Rectangle(x + i * (heartSrc.Width / 2), scoreDst.Y + 180, heartSrc.Width, heartSrc.Height);
                                spriteBatch.Draw(sprites, heartDst, heartSrc, Color.White);
                            }

                            spriteBatch.End();
                        }
                        else
                        {
                            
                            Rectangle heartDst = new Rectangle(x, scoreDst.Y + 180, heartSrc.Width, heartSrc.Height);
                            
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                            spriteBatch.Draw(sprites, heartDst, heartSrc, Color.White);
                            spriteBatch.End();

                            TGPAContext.Instance.TextPrinter.Color = Color.DarkMagenta;
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,heartDst.Right + 10, heartDst.Center.Y, " x " + displayedLives,128);
                            TGPAContext.Instance.TextPrinter.Color = Color.Black;
                        }
                       

                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,xbis, scoreDst.Y + 179, totalLives.ToString("0000000 pts"));

                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,x, scoreDst.Y + 238, LocalizedStrings.GetString("EndlevelScreenRemainingBombs") + " :");

                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                        for (int i = 0; i < displayedBombs; i++)
                        {
                            Rectangle bombDst = new Rectangle(x + i * (bombSrc.Width / 2), scoreDst.Y + 265, bombSrc.Width, bombSrc.Height);
                            spriteBatch.Draw(sprites, bombDst, bombSrc, Color.White);
                        }
                        spriteBatch.End();

                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,xbis, scoreDst.Y + 270, totalBombs.ToString("0000000 pts"));

                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,x, scoreDst.Y + 325, LocalizedStrings.GetString("EndlevelScreenLevelPoints") + " : ");

                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,xbis, scoreDst.Y + 355, score.ToString("0000000 pts"));

                        if (scoreOK)
                        {
                            TGPAContext.Instance.TextPrinter.Color = Color.Blue;
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,x, scoreDst.Y + 380, "Supertotal : ");
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,xbis - 40, scoreDst.Y + 410, scoreTotal.ToString("0000000000 pts"));
                            TGPAContext.Instance.TextPrinter.Color = Color.Black;
                        }

                    }
                }
                #endregion

                #region Highscore
                if (highscoreOK)
                {
                    int xHighscoreOK = 480;

                    TGPAContext.Instance.TextPrinter.Size = 0.9f;

                    String mode = this.scoreTypeDisplay == ScoreType.Single ? "Mode1P" : "Mode2P";
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,xHighscoreOK + 65, scoreDst.Y + 90, "(" + LocalizedStrings.GetString(mode) + ")");

                    if (cheat)
                    {
                        TGPAContext.Instance.TextPrinter.Color = Color.Red;
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,xHighscoreOK + 65, scoreDst.Y + 120, "Cheats ON (no score)");
                        TGPAContext.Instance.TextPrinter.Color = Color.Black;
                    }

                    for (int i = 0; i < 7; i++)
                    {
                        if ((scoreRank == (i + 1)) && (this.scoreTypeSave == this.scoreTypeDisplay))
                        {
                            TGPAContext.Instance.TextPrinter.Color = Color.Green;
                        }

                        SerializableDictionary<string, ScoreLine[]> scoreDictionnary;

                        if (TGPAContext.Instance.Saver.SaveData.ScoresBylevel.TryGetValue(this.scoreTypeDisplay, out scoreDictionnary))
                        {
                            ScoreLine[] scoreLines;

                            if (scoreDictionnary.TryGetValue(TGPAContext.Instance.CurrentMap, out scoreLines))
                            {
                                TGPAContext.Instance.TextPrinter.Write(spriteBatch,xHighscoreOK + 65, scoreDst.Y + 150 + (i * 30), (i + 1) + ". " + scoreLines[i].ToString());
                            }
                        }
                        else
                        {
                            TGPAContext.Instance.Saver.AddScore(TGPAContext.Instance.CurrentMap, this.scoreTypeDisplay, ScoreLine.GetDefaultScoreLine(1));
                        }

                        if (scoreRank == (i + 1))
                        {
                            TGPAContext.Instance.TextPrinter.Color = Color.Black;
                        }

                    }

                    TGPAContext.Instance.TextPrinter.Size = 1f;

                    //Buttons 
                    //***************************************************************************
                    if (highscoreOK)
                    {
                        TGPAContext.Instance.TextPrinter.Color = Color.LightSalmon;
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,600, this.scoreDst.Bottom - 65, LocalizedStrings.GetString(this.scoreTypeDisplay == ScoreType.Single ? "SwitchToScoreTypeCoop" : "SwitchToScoreTypeSingle"));
                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Plus", TGPAContext.Instance.Player1.Device.Type, 550, this.scoreDst.Bottom - 70);

                        if (victory)
                        {
                            TGPAContext.Instance.TextPrinter.Color = Color.Black;
                        }
                        else
                        {
                            TGPAContext.Instance.TextPrinter.Color = Color.White;
                        }

                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Back", TGPAContext.Instance.Player1.Device.Type, 150, 700);
                        if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false)
                        {
                            TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Cancel", TGPAContext.Instance.Player1.Device.Type, 200, 700);
                        }
                        TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Confirm", TGPAContext.Instance.Player1.Device.Type, 550, 700);

                        if (victory)
                        {
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,600, 700, LocalizedStrings.GetString("EndlevelScreenPressAcontinue"));
                        }
                        else
                        {
                            TGPAContext.Instance.TextPrinter.Write(spriteBatch,600, 700, LocalizedStrings.GetString("EndlevelScreenPressAretry"));
                        }
                        TGPAContext.Instance.TextPrinter.Write(spriteBatch,245, 700, LocalizedStrings.GetString("EndlevelScreenPressB"));

                        TGPAContext.Instance.TextPrinter.Color = Color.Black;
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// Fade out in black screen
        /// </summary>
        private void UpdateFadeOut()
        {
            if (fadeOut < 0.9f)
            {
                fadeOut += 0.03f;
                if (fadeOut > 0.9f) fadeOut = 0.9f;
            }
        }

        /// <summary>
        /// Move player's ship at the end of the level
        /// </summary>
        /// <param name="player"></param>
        /// <returns>Player is out</returns>
        private bool MovePlayerShip(Player player)
        {
            Vector2 trajectory;

            switch (this.outDirection)
            {
                case ScrollDirection.Right:
                    if (outLimitRectangle == Rectangle.Empty)
                    {
                        outLimitRectangle = new Rectangle(0, (TGPAContext.Instance.ScreenHeight / 2), TGPAContext.Instance.ScreenWidth * 3, 1);
                    }

                    //Move Player on Y-axis
                    trajectory = player.Trajectory;

                    if (!(player.DstRect.Intersects(outLimitRectangle)))
                    {
                        if (player.Location.Y < (TGPAContext.Instance.ScreenHeight / 2))
                        {
                            trajectory.Y += 75f;
                        }
                        else if (player.Location.Y > (TGPAContext.Instance.ScreenHeight / 2))
                        {
                            trajectory.Y -= 75f;
                        }
                    }
                    else
                    {
                        //Move Player on X-axis
                        if (player.DstRect.X < (TGPAContext.Instance.ScreenWidth * 3))
                        {
                            trajectory.X += 80f;
                            trajectory.Y = 0f;
                        }
                    }

                    player.Trajectory = trajectory;

                    return (player.DstRect.X > TGPAContext.Instance.ScreenWidth);

                case ScrollDirection.Left:
                    if (outLimitRectangle == Rectangle.Empty)
                    {
                        outLimitRectangle = new Rectangle(0, (TGPAContext.Instance.ScreenHeight / 2), TGPAContext.Instance.ScreenWidth * 3, 1);
                    }

                    //Move Player on Y-axis
                    trajectory = player.Trajectory;

                    if (!(player.DstRect.Intersects(outLimitRectangle)))
                    {
                        if (player.Location.Y < (TGPAContext.Instance.ScreenHeight / 2))
                        {
                            trajectory.Y += 75f;
                        }
                        else if (player.Location.Y > (TGPAContext.Instance.ScreenHeight / 2))
                        {
                            trajectory.Y -= 75f;
                        }
                    }
                    else
                    {
                        //Move Player on X-axis
                        if (player.DstRect.X > (-player.DstRect.Width * 3))
                        {
                            trajectory.X -= 80f;
                            trajectory.Y = 0f;
                        }
                    }

                    player.Trajectory = trajectory;
                    return (player.DstRect.X < (-player.DstRect.Width));

                case ScrollDirection.Up:
                    if (outLimitRectangle == Rectangle.Empty)
                    {
                        outLimitRectangle = new Rectangle((TGPAContext.Instance.ScreenWidth / 2), 0, 1, TGPAContext.Instance.ScreenHeight);
                    }

                    //Move Player on X-axis
                    trajectory = player.Trajectory;

                    if (!(player.DstRect.Intersects(outLimitRectangle)))
                    {
                        if (player.Location.X < (TGPAContext.Instance.ScreenWidth / 2))
                        {
                            trajectory.X += 75f;
                        }
                        else if (player.Location.X > (TGPAContext.Instance.ScreenWidth / 2))
                        {
                            trajectory.X -= 75f;
                        }
                    }
                    else
                    {
                        //Move Player on Y-axis
                        if (player.DstRect.Y > (-player.DstRect.Height * 3))
                        {
                            trajectory.Y -= 80f;
                            trajectory.X = 0f;
                        }
                    }

                    player.Trajectory = trajectory;
                    return (player.DstRect.Y < (-player.DstRect.Height));

                case ScrollDirection.Down:
                    if (outLimitRectangle == Rectangle.Empty)
                    {
                        outLimitRectangle = new Rectangle((TGPAContext.Instance.ScreenWidth / 2), 0, 1, TGPAContext.Instance.ScreenHeight);
                    }

                    //Move Player on X-axis
                    trajectory = player.Trajectory;

                    if (!(player.DstRect.Intersects(outLimitRectangle)))
                    {
                        if (player.Location.X < (TGPAContext.Instance.ScreenWidth / 2))
                        {
                            trajectory.X += 75f;
                        }
                        else if (player.Location.X > (TGPAContext.Instance.ScreenWidth / 2))
                        {
                            trajectory.X -= 75f;
                        }
                    }
                    else
                    {
                        //Move Player on Y-axis
                        if (player.DstRect.Y < (TGPAContext.Instance.ScreenHeight * 3))
                        {
                            trajectory.Y += 80f;
                            trajectory.X = 0f;
                        }
                    }

                    player.Trajectory = trajectory;
                    return (player.DstRect.Y > (TGPAContext.Instance.ScreenHeight));

                default:
                    return true;
            }


        }

        #region Properties

        public int ScoreTotal
        {
            get { return scoreTotal; }
        }

        public bool Victory
        {
            get { return victory; }
            set { victory = value; }
        }

        #endregion
    }

}
