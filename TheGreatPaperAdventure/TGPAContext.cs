using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGPA.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Game.Sound;
using TGPA.Game.Save;
using TGPA.Game;
using TGPA.Game.Other;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Content;
using TGPA.SpecialEffects;
using EasyStorage;

namespace TGPA
{
    /// <summary>
    /// Game model, encapsulating data and accessible in a static way
    /// </summary>
    public class TGPAContext
    {
        #region Init

        /// <summary>
        /// Context unique creation
        /// </summary>
        public TGPAContext()
        {
            Shots = new List<Shot>();
            Enemies = new List<BadGuy>();
            Bonuses = new List<Bonus>();

            Saver = new Saver();

            ParticleManager = new ParticleManager();
            ButtonPrinter = new ButtonPrinter();
            InputManager = new InputManager();
            SongInfo = new SongInfo();
            Cheatcodes = new Cheatcodes();
            TextPrinter = new TextPrinter();
            CurrentGameState = GameState.None;

#if WINDOWS
            IsTrialMode = false;
#else
            IsTrialMode = Guide.IsTrialMode;
#endif

            PaperRect = new Rectangle(180, 630, 640, 150);

            ScoreMulti = 1f;
            DamageMulti = 1f;
        }

        /// <summary>
        /// Global content loading
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            ParticleManager.LoadContent(Content);
            ButtonPrinter.LoadContent(Content);
            TextPrinter.LoadContent(Content);

            BonusTex = Content.Load<Texture2D>(@"gfx/bonus");
            ShotsTex = Content.Load<Texture2D>(@"gfx/tirs");
            HudTex = Content.Load<Texture2D>(@"gfx/hud");

            NullTex = Content.Load<Texture2D>(@"gfx/1x1");

            SongInfo.LoadContent(Content);
        }

        #endregion

        #region Singleton

        private static TGPAContext _instance;

        /// <summary>
        /// Singleton
        /// </summary>
        public static TGPAContext Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new TGPAContext();
                }

                return _instance;
            }
        }

        #endregion

        /// <summary>
        /// Screen size (width). CANNOT BE MODIFIED
        /// </summary>
        public int ScreenWidth { get { return 1024; } }

        /// <summary>
        /// Screen size (Height). CANNOT BE MODIFIED
        /// </summary>
        public int ScreenHeight { get { return 768; } }

        /// <summary>
        /// Fullscreen mode
        /// </summary>
        public bool Fullscreen { get; set; }

        #region Engines

        /// <summary>
        /// Game particule manager
        /// </summary>
        public ParticleManager ParticleManager { get; set; }

        /// <summary>
        /// Access to game input manager
        /// </summary>
        public InputManager InputManager { get; set; }

        /// <summary>
        /// Savegame manager
        /// </summary>
        public Saver Saver { get; set; }

#if XBOX
        /// <summary>
        /// X360 save device
        /// </summary>
        public IAsyncSaveDevice SaveDevice { get; set; }
#endif

        /// <summary>
        /// Text drawer
        /// </summary>
        public TextPrinter TextPrinter { get; set; }

        /// <summary>
        /// Display button on screen (like text machine)
        /// </summary>
        public ButtonPrinter ButtonPrinter { get; set; }

        /// <summary>
        /// Cheatcodes interface
        /// </summary>
        public Cheatcodes Cheatcodes { get; set; }

        #endregion

        #region Delegates

        /// <summary>
        /// Fired when the gamestate is modified
        /// </summary>
        public event Action GameStateChanged;

        /// <summary>
        /// Call this delegate to end the current level
        /// </summary>
        public Action<bool> EndLevel { get; set; }

        /// <summary>
        /// Call this delegate to go to the next level
        /// </summary>
        public Action<bool> NextLevel { get; set; }

        /// <summary>
        /// Load a new map
        /// </summary>
        /// <remarks>Use it carefully</remarks>
        public Action PrepareGame { get; set; }

        #endregion

        #region Game model

        /// <summary>
        /// Currently played song
        /// </summary>
        public SongInfo SongInfo { get; set; }

        private GameState _gamestate;

        /// <summary>
        /// Current state of the game
        /// </summary>
        public GameState CurrentGameState
        {
            get { return _gamestate; }
            set
            {
                _gamestate = value;

                if (GameStateChanged != null)
                {
                    GameStateChanged();
                }

            }
        }

        /// <summary>
        /// Options
        /// </summary>
        public OptionsData Options
        {
            get { return Saver.OptionsData; }
        }


        /// <summary>
        /// Player 1 object
        /// </summary>
        public Player Player1 { get; set; }

        /// <summary>
        /// Player 2 object
        /// </summary>
        public Player Player2 { get; set; }

        /// <summary>
        /// All shots on the screen
        /// </summary>
        public List<Shot> Shots { get; set; }

        /// <summary>
        /// All ennemies left on the level
        /// </summary>
        public List<BadGuy> Enemies { get; set; }

        /// <summary>
        /// All bonuses
        /// </summary>
        public List<Bonus> Bonuses { get; set; }

        /// <summary>
        /// Current map
        /// </summary>
        public Map Map { get; set; }

        /// <summary>
        /// Map selected on the level selection screen
        /// </summary>
        public String SelectedMap { get; set; }

        /// <summary>
        /// Current map filename
        /// </summary>
        public String CurrentMap
        {
            get
            {
                return SelectedMap.Remove(0, SelectedMap.IndexOf("level"));
            }
        }

        /// <summary>
        /// A map has been loaded and is ready to be played
        /// </summary>
        public Boolean MapLoaded { get; set; }

        /// <summary>
        /// Is the game in focus ?
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Is it the demo mode ?
        /// </summary>
        public bool IsTrialMode { get; set; }

        /// <summary>
        /// Is the 2P mode enable ?
        /// </summary>
        public bool IsTwoPlayerMode
        {
            get { return this.Player2 != null; }
        }

        /// <summary>
        /// Player who paused the game (index)
        /// </summary>
        public PlayerIndex PausingPlayerIndex { get; set; }

        
        /// <summary>
        /// Player who paused the game
        /// </summary>
        public Player PausingPlayer { 
            get {
                return GetPlayer(PausingPlayerIndex);
                }
        }

        /// <summary>
        /// Game is still loading
        /// </summary>
        public bool IsLoading { get; set; }

        /// <summary>
        /// Score coefficient
        /// </summary>
        public float ScoreMulti { get; set; }

        /// <summary>
        /// Damage coefficient
        /// </summary>
        public float DamageMulti { get; set; }

        #endregion

        #region Textures

        /// <summary>
        /// Blank 1x1 texture
        /// </summary>
        public Texture2D NullTex { get; set; }

        /// <summary>
        /// Texture for the Hud
        /// </summary>
        public Texture2D HudTex { get; set; }

        /// <summary>
        /// Texture for shots
        /// </summary>
        public Texture2D ShotsTex { get; set; }

        /// <summary>
        /// Texture for bonuses
        /// </summary>
        public Texture2D BonusTex { get; set; }

        /// <summary>
        /// Dimension of the GUI rect in the hud texture
        /// </summary>
        public Rectangle PaperRect { get; set; }

        /// <summary>
        /// Current game viewport TitleSafeArea
        /// </summary>
        public Rectangle TitleSafeArea { get; set; }
        //{
        //    get { return GraphicsDevice.Viewport.TitleSafeArea; }
        //}

        public GraphicsDeviceManager Graphics { get; set; }

        /// <summary>
        /// Graphical user interface
        /// </summary>
        public HUD Hud { get; set; }

        /// <summary>
        /// Mouse rectangle
        /// </summary>
        public Rectangle MouseDst { get; set; }

        /// <summary>
        /// Mouse rectangle
        /// </summary>
        public Point MousePoint
        {
            get
            {
                return new Point(MouseDst.X, MouseDst.Y);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Add an enemy to the list
        /// </summary>
        /// <param name="e"></param>
        public void AddEnemy(BadGuy e)
        {
            Enemies.Add(e);
        }

        /// <summary>
        /// Add a bonus to the list
        /// </summary>
        /// <param name="b"></param>
        public void AddBonus(Bonus b)
        {
            Bonuses.Add(b);
        }

        /// <summary>
        /// Kill every context.Enemies in the radius
        /// </summary>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        public void KillEnemiesInArea(Weapon weapon, Vector2 center, int radius)
        {
            foreach (BadGuy bg in Enemies)
            {
                if (!bg.IsOnScreen) continue;

                if (Math.Abs(bg.Location.X - center.X) < radius)
                {
                    if (Math.Abs(bg.Location.Y - center.Y) < radius)
                    {
                        bg.HP -= (int)((float)weapon.Damage * DamageMulti);
                    }
                }
            }
        }

        
        /// <summary>
        /// Return player object corresponding to the given playerindex
        /// </summary>
        /// <param name="playerIndex"></param>
        /// <returns></returns>
        public Player GetPlayer(PlayerIndex playerIndex)
        {
            Player player = null;

            switch (playerIndex)
            {
                case PlayerIndex.One:
                    player = Player1;
                    break;

                case PlayerIndex.Two:
                    player = Player2;
                    break;
            }
            return player;
        }

        #endregion
    }
}
