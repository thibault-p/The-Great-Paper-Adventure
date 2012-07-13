//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game;
using TGPA.Maps;
using TGPA.Localization;
using TGPA.Screens.Controls;
using TGPA.Utils.Input;
using TGPA.Game.Graphics;
using TGPA.Game.Save;
using System.Diagnostics;

namespace TGPA.Screens
{
    /// <summary>
    /// Direction types
    /// </summary>
    public enum ShipDirection
    {
        Up,
        Down,
        Left,
        Right
    }

    /// <summary>
    /// Direction action
    /// </summary>
    public enum ShipAction
    {
        None,
        GoBack,
        GoNext
    }

    /// <summary>
    /// Define directions that the ship can take form a point, and what to do
    /// </summary>
    public class AvailableShipDirection
    {
        public AvailableShipDirection(int levelindex)
        {
            this.LevelIndex = levelindex;
            this.Directions = new Dictionary<ShipDirection,ShipAction>();
        }

        /// <summary>
        /// Index of the level
        /// </summary>
        public int LevelIndex { get; set; }

        /// <summary>
        /// Direction for this level
        /// </summary>
        public Dictionary<ShipDirection, ShipAction> Directions { get; set; }
    }

    /// <summary>
    /// Level selection & Highscore Screen
    /// </summary>
    public class LevelSelectionScreen
    {

        public static int LevelCount = 10;
        public static int WorldCount = 7;
        public static int bezierDuration = 900; //minimal duration of the journey between 2 point

        private enum LevelSelectionScreenButton
        {
            Back,
            Play,
            Level
        }

        private World maps;
        private int selectedIndex, pointedIndex;
        private LevelSelectionScreenButton focus;

        private Texture2D background;
        private Texture2D buttons;

        private Rectangle levelSrc, shipSrc, shipDst, waySrc;
        private Rectangle backSrc, backDst;
        private Rectangle playSrc, playDst;
        private Rectangle arrowSrc;
        private Rectangle postitSrc, postitDst; //post tits !
        private Dictionary<int, Vector2> levelPositionOnMap;

        private float alphaColorForLevelBlinking, blinkingDelta;

        //Ship attribute
        private bool shipIsMoving;
        private float shipAngle = 0;
        private SpriteEffects shipFlip;

        //Post it
        private MapOverview overview;
        private int overviewIndex;
        private String[] levelParts;
        private float alphaPostIt;

        //Bezier
        private float bezier_time = 0;
        private float duration;
        private int aimedlevel = -1, delta = 1;
        private Dictionary<int, List<Vector2>> pointsBezier;

        private ScoreType scoreType;
        private float fadeoutAlpha;
        private bool difficultyChoosen;
        private ListControl difficultyListControl;
        private bool launchLevel, loadingLaunched;
        private Rectangle rectDst;

        //Available movements for each level for the ship
        private List<AvailableShipDirection> availableShipDirectionByLevel;

        public LevelSelectionScreen()
        {
            selectedIndex = 0;

            arrowSrc = new Rectangle(410, 10, 60, 50);

            postitSrc = new Rectangle(86, 4, 245, 382);
            postitDst = postitSrc;
            postitDst.X = 60;
            postitDst.Y = 140;
            postitDst.Width += (postitDst.Width / 3); //Enlarge post-it to display more text
            postitDst.Height += (postitDst.Width / 3);

            playSrc = new Rectangle(857, 0, 90, 120);
            playDst = arrowSrc;
            playDst.X = postitDst.X + (postitDst.Width / 2) + (arrowSrc.Width / 2);
            playDst.Y = (postitDst.Y + postitDst.Height) - arrowSrc.Height;

            backSrc = new Rectangle(677, 0, 120, 120);
            backDst = arrowSrc;
            backDst.X = postitDst.X;
            backDst.Y = 650;

            levelSrc = new Rectangle(0, 0, 32, 32);

            shipSrc = new Rectangle(0, 260, 70, 55);
            waySrc = new Rectangle(4, 46, 25, 15);

            //Place levels
            levelPositionOnMap = new Dictionary<int, Vector2>();
            levelPositionOnMap.Add(0, new Vector2(895, 272));
            levelPositionOnMap.Add(1, new Vector2(796, 167));
            levelPositionOnMap.Add(2, new Vector2(796, 475));
            levelPositionOnMap.Add(3, new Vector2(210, 600));
            levelPositionOnMap.Add(4, new Vector2(586, 354));
            levelPositionOnMap.Add(5, new Vector2(640, 145));
            levelPositionOnMap.Add(6, new Vector2(410, 230));

            //list of bezier's points
            //2 waypoints per road
            Vector2[] tmp;
            pointsBezier = new Dictionary<int, List<Vector2>>();
            tmp = new Vector2[] { new Vector2(895, 236), new Vector2(942, 210) }; //between 0 - 1
            pointsBezier.Add(0, new List<Vector2>(tmp));
            tmp = new Vector2[] { new Vector2(850, 256), new Vector2(870, 220) }; //between 1 - 2
            pointsBezier.Add(1, new List<Vector2>(tmp));
            tmp = new Vector2[] { new Vector2(672, 610), new Vector2(300, 490) }; //between 2 - 3
            pointsBezier.Add(2, new List<Vector2>(tmp));
            tmp = new Vector2[] { new Vector2(236, 386), new Vector2(616, 461) }; //between 3 - 4
            pointsBezier.Add(3, new List<Vector2>(tmp));
            tmp = new Vector2[] { new Vector2(675, 250), new Vector2(600, 200) };
            pointsBezier.Add(4, new List<Vector2>(tmp));
            tmp = new Vector2[] { new Vector2(560, 265), new Vector2(575, 175) };
            pointsBezier.Add(5, new List<Vector2>(tmp));
            tmp = new Vector2[] { new Vector2(485, 165), new Vector2(460, 250) };
            pointsBezier.Add(6, new List<Vector2>(tmp));

            //Set direction for GamePad users to move on the map
            availableShipDirectionByLevel = new List<AvailableShipDirection>();

            AvailableShipDirection dir0 = new AvailableShipDirection(0);
            dir0.Directions.Add(ShipDirection.Up, ShipAction.GoNext);
            dir0.Directions.Add(ShipDirection.Left, ShipAction.GoNext);
            dir0.Directions.Add(ShipDirection.Right, ShipAction.GoBack);
            dir0.Directions.Add(ShipDirection.Down, ShipAction.GoBack);

            AvailableShipDirection dir1 = new AvailableShipDirection(1);
            dir1.Directions.Add(ShipDirection.Left, ShipAction.GoNext);
            dir1.Directions.Add(ShipDirection.Down, ShipAction.GoNext);
            dir1.Directions.Add(ShipDirection.Right, ShipAction.GoBack);
            dir1.Directions.Add(ShipDirection.Up, ShipAction.GoBack);

            AvailableShipDirection dir2 = new AvailableShipDirection(2);
            dir2.Directions.Add(ShipDirection.Left, ShipAction.GoNext);
            dir2.Directions.Add(ShipDirection.Down, ShipAction.GoNext);
            dir2.Directions.Add(ShipDirection.Right, ShipAction.GoBack);
            dir2.Directions.Add(ShipDirection.Up, ShipAction.GoBack);

            AvailableShipDirection dir3 = new AvailableShipDirection(3);
            dir3.Directions.Add(ShipDirection.Left, ShipAction.GoNext);
            dir3.Directions.Add(ShipDirection.Right, ShipAction.GoBack);
            dir3.Directions.Add(ShipDirection.Up, ShipAction.GoNext);
            dir3.Directions.Add(ShipDirection.Down, ShipAction.GoBack);

            AvailableShipDirection dir4 = new AvailableShipDirection(4);
            dir4.Directions.Add(ShipDirection.Left, ShipAction.GoBack);
            dir4.Directions.Add(ShipDirection.Down, ShipAction.GoBack);
            dir4.Directions.Add(ShipDirection.Right, ShipAction.GoNext);
            dir4.Directions.Add(ShipDirection.Up, ShipAction.GoNext);

            AvailableShipDirection dir5 = new AvailableShipDirection(5);
            dir5.Directions.Add(ShipDirection.Left, ShipAction.GoBack);
            dir5.Directions.Add(ShipDirection.Down, ShipAction.GoBack);
            dir5.Directions.Add(ShipDirection.Right, ShipAction.GoNext);
            dir5.Directions.Add(ShipDirection.Up, ShipAction.GoNext);

            AvailableShipDirection dir6 = new AvailableShipDirection(6);
            dir6.Directions.Add(ShipDirection.Left, ShipAction.GoNext);
            dir6.Directions.Add(ShipDirection.Down, ShipAction.GoNext);
            dir6.Directions.Add(ShipDirection.Right, ShipAction.GoBack);
            dir6.Directions.Add(ShipDirection.Up, ShipAction.GoBack);

            availableShipDirectionByLevel.Add(dir0);
            availableShipDirectionByLevel.Add(dir1);
            availableShipDirectionByLevel.Add(dir2);
            availableShipDirectionByLevel.Add(dir3);
            availableShipDirectionByLevel.Add(dir4);
            availableShipDirectionByLevel.Add(dir5);
            availableShipDirectionByLevel.Add(dir6);

            shipDst = shipSrc;
            shipDst.X = (int)levelPositionOnMap[0].X;
            shipDst.Y = (int)levelPositionOnMap[0].Y;

            Vector2 controlLoc = new Vector2(105, 350);

            difficultyListControl = new ListControl("", controlLoc, new List<ListElement>()
            {
                new ListElement(Difficulty.Easy, new Rectangle(600, 600, 95, 5),"Easy"), 
                new ListElement(Difficulty.Normal, new Rectangle(600, 600, 95, 5),"Normal"), 
                new ListElement(Difficulty.Hard, new Rectangle(600, 600, 95, 5),"Hard") 

            }, "StartLevel");
            difficultyListControl.Focus = true;

            foreach (ListElement le in difficultyListControl.Elements)
            {
                if (TGPAContext.Instance.Options.Difficulty == (Difficulty)le.Value)
                {
                    difficultyListControl.FocusedElement = le;
                }
            }
            difficultyListControl.ValueChanged += new ValueChangedEventhandler(this.DifficultyValueChanged);

            rectDst = new Rectangle(150, 250, 774, 275);

            Initialize();
        }

        public void Initialize()
        {
            //Fill game maps : can change during execution
            //************************************************************************************************
            //LevelCount = 7; //Do not forget !!!
            //WorldCount = 5;

            if (TGPAContext.Instance.IsTrialMode == false)
            {
                //Level bonus test
                //maps = new World(new String[] { "level0.tgpa", "level1.tgpa", "level2_1.tgpa", "level2_2.tgpa", "level3_1.tgpa", "level3_2.tgpa", "level4_1.tgpa", "level4_2.tgpa", "level5.tgpa", "level_b1_2.tgpa", "level_b1_1.tgpa" },
                //                 new int[] { 0, 1, 2, 2, 3, 3, 4, 4, 5, 6, 6 });

                //LevelCount = 10;
                //WorldCount = 7;

                //Classic game config
                maps = new World(new String[] { "level0.tgpa", "level1.tgpa", "level2_1.tgpa", "level2_2.tgpa", "level3_1.tgpa", "level3_2.tgpa", "level4_1.tgpa", "level4_2.tgpa", "level5.tgpa" },
                 new int[] { 0, 1, 2, 2, 3, 3, 4, 4, 5 });

                LevelCount = 9;
                WorldCount = 6;
            }
            else //Demo mode
            {
                maps = new World(new String[] { "level0.tgpa", "level1.tgpa" },
                                 new int[] { 0, 1 });
                WorldCount = 2;
                LevelCount = 2;
            }

            alphaColorForLevelBlinking = 0.5f;
            blinkingDelta = 0.01f;

            aimedlevel = -1;
            pointedIndex = -1; //Level pointed by mouse or next mevem
            focus = LevelSelectionScreenButton.Level;
            LoadOverview(selectedIndex);

            shipIsMoving = false;
            shipAngle = 0.0f;
            shipFlip = SpriteEffects.None;

            this.scoreType = ScoreType.Single;
            if (TGPAContext.Instance.Player2 != null)
            {
                this.scoreType = ScoreType.Coop;
            }

            //Difficulty
            launchLevel = false;
            difficultyChoosen = false;
            fadeoutAlpha = 0.9f;
            loadingLaunched = false;
        }

        public void LoadContent(ContentManager Content)
        {
            background = Content.Load<Texture2D>(@"gfx/LevelSelection/background");
            buttons = Content.Load<Texture2D>(@"gfx/LevelSelection/buttons");
        }

        /// <summary>
        /// Level selection, launch game, come back, etc
        /// </summary>
        /// <param name="gameTime"></param>
        /// <param name="Game"></param>
        public void Update(GameTime gameTime)
        {
            if (loadingLaunched == true) return;

            float elapsedTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            alphaColorForLevelBlinking += blinkingDelta;

            if (alphaColorForLevelBlinking >= 1f)
            {
                alphaColorForLevelBlinking = 1f;
                blinkingDelta = -blinkingDelta;
            }
            else if (alphaColorForLevelBlinking <= 0f)
            {
                alphaColorForLevelBlinking = 0f;
                blinkingDelta = -blinkingDelta;
            }

            if (TGPAContext.Instance.InputManager.PlayerPressButtonBack(TGPAContext.Instance.Player1))
            {
                if (launchLevel)
                {
                    launchLevel = false;
                }
                else
                {
                    TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
                    return;
                }
            }

            #region Level launch

            if (launchLevel)
            {
                fadeoutAlpha += 0.01f;
                if (fadeoutAlpha > 0.75f) fadeoutAlpha = 0.75f;

                if (difficultyChoosen)
                {
                    Thread t = new Thread(new ThreadStart(
                    delegate()
                    {
                        TGPAContext.Instance.PrepareGame();
                    }));
                    t.Start();
                    loadingLaunched = true;
                }
                else
                {
                    difficultyListControl.Update(gameTime);
                }
            }
            else
            {
                fadeoutAlpha -= 0.01f;
                if (fadeoutAlpha < 0f) fadeoutAlpha = 0f;
            }

            #endregion

            #region Input management

            if ((!launchLevel) & (!shipIsMoving)) // controls are enabled only if the ship is on a point otherwise you have to wait
            {
                //PC Management
                if (TGPAContext.Instance.Player1.IsPlayingOnWindows())
                {
                    Rectangle mouseEnhancedDst = TGPAContext.Instance.MouseDst;
                    mouseEnhancedDst.Inflate(10, 10);

                    if (playDst.Intersects(mouseEnhancedDst))
                    {
                        focus = LevelSelectionScreenButton.Play;

                        if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                        {
                            launchLevel = true;
                        }
                    }
                    else if (backDst.Intersects(mouseEnhancedDst))
                    {
                        focus = LevelSelectionScreenButton.Back;

                        if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                        {
                            TGPAContext.Instance.CurrentGameState = GameState.TitleScreen;
                        }
                    }
                    else
                    {
                        //Click on a level = change selected index

                        for (int i = 0; i < WorldCount; i++)
                        {
                            Rectangle levelDst = Rectangle.Empty;

                            //The player need to have unlocked the level to display it
                            if ((i <= TGPAContext.Instance.Saver.SaveData.LastLevel) && (i < WorldCount))
                            {
                                Vector2 loc = levelPositionOnMap[i];
                                levelDst = new Rectangle(
                                    (int)loc.X - levelSrc.Width / 2,
                                    (int)loc.Y - levelSrc.Height / 2,
                                    levelSrc.Width,
                                    levelSrc.Height
                                    );
                            }

                            pointedIndex = -1;
                            if (levelDst.Intersects(mouseEnhancedDst))
                            {
                                //if (Math.Abs(i - selectedIndex) > 1) break; //not allow player to select a too far point  
                                pointedIndex = i;
                                if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                                {
                                    aimedlevel = i;
                                }
                                break;
                            }


                        }
                        focus = LevelSelectionScreenButton.Level;
                    }
                }

                #region Move with gamepad / Joystick

                bool goBack = false;
                bool goNext = false;
                Dictionary<ShipDirection,ShipAction> directionAndAction = availableShipDirectionByLevel[selectedIndex].Directions;

                if (TGPAContext.Instance.InputManager.PlayerGoLeft(TGPAContext.Instance.Player1) > 0f)
                {
                    goBack = (directionAndAction[ShipDirection.Left] == ShipAction.GoBack);
                    goNext = (directionAndAction[ShipDirection.Left] == ShipAction.GoNext);
                }

                if (TGPAContext.Instance.InputManager.PlayerGoRight(TGPAContext.Instance.Player1) > 0f)
                {
                    goBack = (directionAndAction[ShipDirection.Right] == ShipAction.GoBack);
                    goNext = (directionAndAction[ShipDirection.Right] == ShipAction.GoNext);
                }

                if (TGPAContext.Instance.InputManager.PlayerGoUp(TGPAContext.Instance.Player1) > 0f)
                {
                    goBack = (directionAndAction[ShipDirection.Up] == ShipAction.GoBack);
                    goNext = (directionAndAction[ShipDirection.Up] == ShipAction.GoNext);
                }

                if (TGPAContext.Instance.InputManager.PlayerGoDown(TGPAContext.Instance.Player1) > 0f)
                {
                    goBack = (directionAndAction[ShipDirection.Down] == ShipAction.GoBack);
                    goNext = (directionAndAction[ShipDirection.Down] == ShipAction.GoNext);
                }

                if (goNext)
                {
                    //Go to next level
                    if ((selectedIndex + 1 > TGPAContext.Instance.Saver.SaveData.LastLevel) || (selectedIndex + 1 >= WorldCount))
                    {
                        aimedlevel = 0;
                    }
                    else
                    {
                        aimedlevel = selectedIndex + 1;
                    }
                }
                else if (goBack)
                {
                    if (selectedIndex - 1 < 0)
                    {
                        aimedlevel = TGPAContext.Instance.Saver.SaveData.LastLevel;

                        if (aimedlevel >= WorldCount)
                        {
                            aimedlevel = WorldCount - 1;
                        }
                    }
                    else
                    {
                        aimedlevel = selectedIndex - 1;
                    }
                }

                #endregion

                if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false)
                {
                    pointedIndex = aimedlevel;

                    if (TGPAContext.Instance.InputManager.PlayerPressButtonConfirm(TGPAContext.Instance.Player1))
                    {
                        launchLevel = true;
                    }
                }

                //Both
                if (TGPAContext.Instance.InputManager.PlayerPressButtonSwitch(TGPAContext.Instance.Player1))
                {
                    this.scoreType = (this.scoreType == ScoreType.Single ? ScoreType.Coop : ScoreType.Single);
                }
            }

            #endregion

            #region Shipanimation Bezier

            if ((aimedlevel != -1) && (aimedlevel != selectedIndex) && (!shipIsMoving)) //if we have to move to the next destination
            {
                bezier_time = 0;
                shipIsMoving = true;
                duration = bezierDuration + (int)(Math.Sqrt(Math.Pow(levelPositionOnMap[selectedIndex].X - levelPositionOnMap[aimedlevel].X, 2) + Math.Pow(levelPositionOnMap[selectedIndex].Y - levelPositionOnMap[aimedlevel].Y, 2))) - (Math.Abs(aimedlevel - selectedIndex) - 1) * 200;
            }


            if ((shipIsMoving) && (aimedlevel != -1))
            {
                bezier_time += gameTime.ElapsedGameTime.Milliseconds;
                Vector2 p1, p2, p3, p4;
                if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false)
                {
                    if (selectedIndex < aimedlevel)  //waypoints depend of the direction
                    {
                        p1 = levelPositionOnMap[selectedIndex];
                        p2 = pointsBezier[selectedIndex][0];
                        p3 = pointsBezier[selectedIndex][1];
                        p4 = levelPositionOnMap[selectedIndex + 1];
                        delta = 1;
                    }
                    else
                    {
                        p1 = levelPositionOnMap[selectedIndex];
                        p3 = pointsBezier[selectedIndex - 1][0];
                        p2 = pointsBezier[selectedIndex - 1][1];
                        p4 = levelPositionOnMap[selectedIndex - 1];
                        delta = -1;
                    }

                }
                else
                {
                    delta = aimedlevel - selectedIndex;
                    p1 = levelPositionOnMap[selectedIndex];

                    p4 = levelPositionOnMap[aimedlevel];
                    if (selectedIndex < aimedlevel)
                    {
                        p2 = pointsBezier[selectedIndex + Math.Min(delta, 0)][0];
                        p3 = pointsBezier[selectedIndex + Math.Min(delta, 0)][1];

                    }
                    else
                    {
                        p2 = pointsBezier[selectedIndex + Math.Min(delta, 0)][1];
                        p3 = pointsBezier[selectedIndex + Math.Min(delta, 0)][0];
                    }
                }

                float t = bezier_time / duration; //quadratic bezier equation need a parameter t, with 0<=t<=1 
                if (t <= 1)
                {
                    shipDst.X = (int)((Math.Pow(1 - t, 3)) * p1.X + 3 * (Math.Pow(1 - t, 2)) * t * p2.X + 3 * (1 - t) * t * t * p3.X + t * t * t * p4.X) - shipSrc.Width / 2;
                    shipDst.Y = (int)((Math.Pow(1 - t, 3)) * p1.Y + 3 * (Math.Pow(1 - t, 2)) * t * p2.Y + 3 * (1 - t) * t * t * p3.Y + t * t * t * p4.Y) - shipSrc.Height / 2;
                }
                else //if the journey is over
                {
                    selectedIndex += delta;
                    shipIsMoving = false;

                    if (aimedlevel == selectedIndex)
                    {
                        aimedlevel = -1;
                        LoadOverview(selectedIndex);
                    }
                }

                //Post-it fade out
                alphaPostIt -= 0.05f;
                if (alphaPostIt < 0) alphaPostIt = 0f;
            }

            //spinning ship
            if (!shipIsMoving)
            {
                shipAngle += (gameTime.ElapsedGameTime.Milliseconds * 0.001f) % MathHelper.Pi * 2;
                shipDst.X = (int)(levelPositionOnMap[selectedIndex].X + 10 * Math.Cos(shipAngle) - shipSrc.Width / 2);
                shipDst.Y = (int)(levelPositionOnMap[selectedIndex].Y + 10 * Math.Sin(shipAngle) - shipSrc.Height / 2);

                //Post-it fade in
                if (aimedlevel == -1)
                {
                    alphaPostIt += 0.1f;
                    if (alphaPostIt > 1) alphaPostIt = 1f;
                }
            }

            #endregion
        }

        /// <summary>
        /// Draw a map with points for each level, and way between them
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="Game"></param>
        public void Draw(TGPASpriteBatch spriteBatch)
        {
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            //Background
            spriteBatch.Draw(background, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), null, Color.White);

            Rectangle LastLevelDst = Rectangle.Empty;
            //Draw levels and a way between them
            for (int i = 0; i < WorldCount; i++)
            {
                //The player need to have unlocked the level to display it
                Rectangle levelDst = Rectangle.Empty;
                if ((i <= TGPAContext.Instance.Saver.SaveData.LastLevel) && (i < WorldCount))
                {
                    Vector2 loc = levelPositionOnMap[i];
                    levelDst = new Rectangle(
                        (int)loc.X - levelSrc.Width / 2,
                        (int)loc.Y - levelSrc.Height / 2,
                        levelSrc.Width,
                        levelSrc.Height
                        );

                    Color levelColor = Color.White;
                    Color blinkColor = Color.Gray * alphaColorForLevelBlinking;

                    if ((selectedIndex == i) & (!shipIsMoving))
                    {
                        levelColor = Color.Red;
                        blinkColor = Color.OrangeRed * 0.0f;
                    }
                    else if (pointedIndex == i)
                    {
                        levelColor = Color.SpringGreen;
                        blinkColor = Color.White * 0.0f;
                    }
                    else if (i == TGPAContext.Instance.Saver.SaveData.LastLevel)
                    {
                        blinkColor = Color.PaleVioletRed * alphaColorForLevelBlinking;
                    }
                    spriteBatch.Draw(buttons, levelDst, levelSrc, levelColor);
                    spriteBatch.Draw(buttons, levelDst, levelSrc, blinkColor);

                    //Draw the way
                    if (i > 0)
                    {

                        Vector2 p1 = levelPositionOnMap[i - 1];
                        Vector2 p2 = pointsBezier[i - 1][0];
                        Vector2 p3 = pointsBezier[i - 1][1];
                        Vector2 p4 = levelPositionOnMap[i];
                        int requiredPoints = (int)(Math.Sqrt(Math.Pow(p4.X - p1.X, 2) + Math.Pow(p4.Y - p1.Y, 2)) / 50) + 2; //evaluate number of point to show depending of the distance between p1 an p4

                        Rectangle wayDst = waySrc;

                        for (int x = 1; x < requiredPoints; x++)
                        {
                            float t = (float)x / (float)(requiredPoints); //quadratic bezier equation need a parameter t, with 0<=t<=1 
                            if (t <= 1)
                            {
                                wayDst.X = (int)((Math.Pow(1 - t, 3)) * p1.X + 3 * (Math.Pow(1 - t, 2)) * t * p2.X + 3 * (1 - t) * t * t * p3.X + t * t * t * p4.X) - wayDst.Width / 2;
                                wayDst.Y = (int)((Math.Pow(1 - t, 3)) * p1.Y + 3 * (Math.Pow(1 - t, 2)) * t * p2.Y + 3 * (1 - t) * t * t * p3.Y + t * t * t * p4.Y) - wayDst.Height / 2;

                                spriteBatch.Draw(buttons, wayDst, waySrc, Color.White, 0.0f, new Vector2(waySrc.Width / 2, waySrc.Height / 2), SpriteEffects.None, 1.0f);
                            }

                        }
                    }


                }
            }

            //Draw little ship
            spriteBatch.Draw(buttons, shipDst, shipSrc, Color.White, 0.0f, Vector2.Zero, shipFlip, 1.0f);

            //Draw selected world information

            Vector2 v = levelPositionOnMap[selectedIndex];

            Color whiteColor = Color.White * alphaPostIt;

            spriteBatch.Draw(buttons, postitDst, postitSrc, whiteColor);

            //Play and back Buttonfor PC
            if (TGPAContext.Instance.Player1.IsPlayingOnWindows())
            {
                //Launch game
                if (focus == LevelSelectionScreenButton.Play)
                {
                    Rectangle srcBis = arrowSrc;
                    srcBis.Y += srcBis.Height;
                    spriteBatch.Draw(buttons, playDst, srcBis, whiteColor,0.0f,Vector2.Zero,SpriteEffects.FlipHorizontally,1.0f);
                }
                else
                {
                    spriteBatch.Draw(buttons, playDst, arrowSrc, whiteColor, 0.0f, Vector2.Zero, SpriteEffects.FlipHorizontally, 1.0f);
                }

                //Back
                if (focus == LevelSelectionScreenButton.Back)
                {
                    Rectangle srcBis = arrowSrc;
                    srcBis.Y += srcBis.Height;
                    spriteBatch.Draw(buttons, backDst, srcBis, whiteColor);
                }
                else
                {
                    spriteBatch.Draw(buttons, backDst, arrowSrc, whiteColor);
                }
            }

            spriteBatch.End();

            //Play and back text for PC
            if (TGPAContext.Instance.Player1.IsPlayingOnWindows())
            {
                if (alphaPostIt > 0.8f)
                {
                    playDst.X = 290;
                    playDst.Y = 485;

                    backDst.X = 90;
                    backDst.Y = 555;

                    string playText = "Play";
                    string backText = "Back to title";

                    TGPAContext.Instance.TextPrinter.Color = Color.Navy;
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, playDst.X - (playText.Length * 12), playDst.Y + 5, playText);
                    TGPAContext.Instance.TextPrinter.Color = Color.Red;
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, backDst.X + backDst.Width, backDst.Y + 5, backText);
                    TGPAContext.Instance.TextPrinter.Color = Color.Black;
                }
            }

            TGPAContext.Instance.TextPrinter.Color = (Color.Black * alphaPostIt);

            TGPAContext.Instance.TextPrinter.Write(spriteBatch,postitDst.X + 20, postitDst.Y + 100, "Level " + overview.Level);
            TGPAContext.Instance.TextPrinter.Write(spriteBatch,postitDst.X + 20, postitDst.Y + 130, overview.Name);

            TGPAContext.Instance.TextPrinter.Size = 0.8f;

            for (int i = 0; i < this.levelParts.Length; i++)
            {
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,postitDst.X + 25, postitDst.Y + 170 + (80 * i), LocalizedStrings.GetString("LevelSelectionScreenPart") + " " + i + " - " + LocalizedStrings.GetString("LevelSelectionScreenBestScore"));

                ScoreLine bestScore = TGPAContext.Instance.Saver.GetBestScoreForLevel(this.levelParts[i], this.scoreType);
                if (bestScore.Score > 0)
                {
                    TGPAContext.Instance.TextPrinter.Color = (Color.Blue * alphaPostIt);
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,postitDst.X + 25, postitDst.Y + 190 + (80 * i), bestScore.ToString());
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch, postitDst.X + 105, postitDst.Y + 210 + (80 * i), bestScore.GetDifficultyString());
                }
                else
                {
                    TGPAContext.Instance.TextPrinter.Color = (Color.Red * alphaPostIt);
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,postitDst.X + 25, postitDst.Y + 190 + (80 * i), LocalizedStrings.GetString("LevelSelectionScreenNoHighscore"));
                }

                TGPAContext.Instance.TextPrinter.Color = (Color.Black * alphaPostIt);
            }

            TGPAContext.Instance.TextPrinter.Color = Color.Black;
            TGPAContext.Instance.TextPrinter.Size = 1f;


            if (launchLevel == false)
            {
                //Buttons 
                //***************************************************************************
                if (TGPAContext.Instance.Player1.IsPlayingOnWindows() == false)
                {
                    TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Move", TGPAContext.Instance.Player1.Device.Type, 535, 630);
                    TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Confirm", TGPAContext.Instance.Player1.Device.Type, 535, 700);
                    TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Cancel", TGPAContext.Instance.Player1.Device.Type, 150, 630);

                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,595, 650, LocalizedStrings.GetString("LevelSelectionScreenMove"));
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,595, 700, LocalizedStrings.GetString("LevelSelectionScreenPressA"));
                    TGPAContext.Instance.TextPrinter.Write(spriteBatch,205, 650, LocalizedStrings.GetString("LevelSelectionScreenPressB"));
                }

                TGPAContext.Instance.TextPrinter.Write(spriteBatch,170, 700, LocalizedStrings.GetString(this.scoreType == ScoreType.Single ? "SwitchToScoreTypeCoop" : "SwitchToScoreTypeSingle"));
                TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Plus", TGPAContext.Instance.Player1.Device.Type, 115, 690);
            }

            if (fadeoutAlpha > 0f)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(background, new Rectangle(0, 0, TGPAContext.Instance.ScreenWidth, TGPAContext.Instance.ScreenHeight), null, (Color.Black * fadeoutAlpha));
                spriteBatch.End();
            }

            if (launchLevel)
            {
                spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                spriteBatch.Draw(TGPAContext.Instance.HudTex, rectDst, TGPAContext.Instance.PaperRect, Color.White);
                spriteBatch.End();


                TGPAContext.Instance.TextPrinter.Write(spriteBatch,rectDst.X + 50, rectDst.Y + 60, LocalizedStrings.GetString("Difficulty"));

                difficultyListControl.Draw(spriteBatch);

                TGPAContext.Instance.TextPrinter.Size = 0.8f;
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,rectDst.X + 40, rectDst.Y + 125, LocalizedStrings.GetString((Difficulty)this.difficultyListControl.FocusedElement.Value + "Desc"), 50);
                TGPAContext.Instance.TextPrinter.Size = 1f;

                TGPAContext.Instance.TextPrinter.Color = Color.White;
                TGPAContext.Instance.ButtonPrinter.Draw(spriteBatch, "#Back", TGPAContext.Instance.Player1.Device.Type, 350, 550);
                TGPAContext.Instance.TextPrinter.Write(spriteBatch,400, 560, LocalizedStrings.GetString("CancelLaunchLevel"));
                TGPAContext.Instance.TextPrinter.Color = Color.Black;
            }
        }

        public void DifficultyValueChanged(object sender)
        {
            TGPAContext.Instance.Options.Difficulty = (Difficulty)this.difficultyListControl.FocusedElement.Value;

            //Register difficluty
            TGPAContext.Instance.Saver.Save();
            difficultyChoosen = true;
        }

        /// <summary>
        /// Reload map overviews
        /// </summary>
        private MapOverview LoadOverview(int level)
        {
            String mapFile = World.GetMapFirstFile(level);
            overviewIndex = level;
            overview = Map.GetMapOverview(mapFile);
            levelParts = World.GetMaps(level);

            return overview;
        }

        public int SelectedLevelIndex
        {
            get { return selectedIndex; }
        }

        public World World
        {
            get { return maps; }
        }
    }
}
