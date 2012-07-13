//****************************************************************************************************
//                             The Great paper Adventure : 2009-2010
//                             By Valryon, LapinouFou & MinishLink !
//©2009-2010 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Xml;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;
using TGPA.Game.BadGuys;
using TGPA.Game.Sound;
using TGPA.Game.Weapons;
using TGPA.Maps;
using TGPA.Maps.Scripts;
using TGPA.Strings;
using System.Collections.Generic;
using TGPA.Maps.Commands;
using System.IO;
using Microsoft.Xna.Framework.Storage;

namespace TGPA.Game
{
    /// <summary>
    /// Define a level, with backgrounds, the original position of enemies, of bonuses, ...
    /// </summary>
    public class Map
    {
        public enum EndMode {
            None,Win,Lose
        }

        private ScrollingBackground background1;
        private ScrollingBackground background2;
        private ScrollingBackground background3;
        private int level;
        private string name;
        private string description;
        private MySong music;
        private MapFlags flags;
        private List<Event> events;
        private List<BadGuy> EnemyRessourcesToLoad;
        private List<MySong> MusicRessourcesToLoad;
        public int InitialScore { get; set; }
        public int InitialLivesCount { get; set; }
        public Weapon InitialWeapon { get; set; }
        public String GameVersion { get; set; }
        public EndMode Ended { get; set; }
        public double StartTime { get; set; }

        private Map()
        {
            level = 0;
            name = "<insert name here>";
            description = "<insert desc here>";
            flags = new MapFlags(128);

            InitialScore = 0;
            InitialLivesCount = 5;
            InitialWeapon = new MachineGun();

            events = new List<Event>();
            EnemyRessourcesToLoad = new List<BadGuy>();
            MusicRessourcesToLoad = new List<MySong>();

            Ended = EndMode.None;
            StartTime = 0.0f;
        }

        /// <summary>
        /// Load texture for background
        /// </summary>
        /// <param name="level"></param>
        public void LoadMapContent(ContentManager content, GraphicsDevice gd)
        {
            if (background1 != null)
                background1.LoadContent(content, gd.Viewport);
            if (background2 != null)
                background2.LoadContent(content, gd.Viewport);
            if (background3 != null)
                background3.LoadContent(content, gd.Viewport);

            if (music != null)
            {
                music.Song = content.Load<Song>("sfx/Music/" + music.FileName);
            }

            foreach (BadGuy b in EnemyRessourcesToLoad)
                b.LoadContent(content);

            foreach (MySong s in MusicRessourcesToLoad)
                s.Song = content.Load<Song>("sfx/Music/" + s.FileName);
        }

        /// <summary>
        /// Convert event into old version (0.8) enemy list.
        /// See EnemyEditor
        /// </summary>
        /// <returns></returns>
        public List<BadGuy> ExtractEnemiesFromEvents()
        {
            List<BadGuy> oldWayEnemies = new List<BadGuy>();

            foreach (Event e in events)
            {
                foreach (Command c in e.Commands)
                {
                    if (typeof(AddEnemyCommand) == c.GetType())
                    {
                        BadGuy enemy = ((AddEnemyCommand)c).Enemy;
                        enemy.ScrollValue = e.ScrollValue;
                        oldWayEnemies.Add(enemy);
                    }
                }
            }

            return oldWayEnemies;
        }

        /// <summary>
        /// Read .tgpa files
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Map BuildMapFromTGPAFile(String file)
        {
            return Map.BuildMapFromTGPAFile(file, new Vector2(TheGreatPaperGame.screenWidth, TheGreatPaperGame.screenHeight));
        }

        /// <summary>
        /// Read .tgpa files outside TGPA Game class
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static Map BuildMapFromTGPAFile(String file, Vector2 screenResolution)
        {
            Map map = new Map();

            StreamReader reader = new StreamReader(file);

            String line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Check version
            try
            {
                if (Convert.ToDouble((line.Split(' '))[1]) > Convert.ToDouble(TheGreatPaperGame.version))
                {
                    reader.Close();
                    throw new Exception("Insupported game version.");
                }
            }
            catch (FormatException) { throw new Exception("Invalid game version : " + line); }

            map.GameVersion = line.Split(' ')[1];

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Map informations
            #region Map Infos

            try
            {
                map.Level = Convert.ToInt32((line.Split(' '))[1]);
            }
            catch (FormatException) { reader.Close(); throw new Exception("Invalid map level number : " + line); }

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            map.Name = line.Replace("name ", "");

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            String filedesc = line.Replace("desc ", "");
            String s = null;
            try
            {
                s = Localization.GetString(filedesc);
            }
            catch (Exception) { /* Map Editor */}
            map.Description = s == null ? filedesc : s;

            #endregion

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Backgrounds
            #region Backgrounds

            for (int i = 1; i < 4; i++)
            {
                ScrollingBackground bg = null;

                if (i == 1)
                    bg = map.Background1;
                else if (i == 2)
                    bg = map.Background2;
                else if (i == 3)
                    bg = map.Background3;


                string[] tokens = line.Split(' ');

                ScrollingBackground.ScrollDirection direction = ScrollingBackground.ScrollDirection.Left;
                if (tokens[1].Equals(ScrollingBackground.ScrollDirection.Up.ToString()))
                {
                    direction = ScrollingBackground.ScrollDirection.Up;
                }
                else if (tokens[1].Equals(ScrollingBackground.ScrollDirection.Down.ToString()))
                {
                    direction = ScrollingBackground.ScrollDirection.Down;
                }
                else if (tokens[1].Equals(ScrollingBackground.ScrollDirection.Right.ToString()))
                {
                    direction = ScrollingBackground.ScrollDirection.Right;
                }
                else if (tokens[1].Equals(ScrollingBackground.ScrollDirection.Left.ToString()))
                {
                    direction = ScrollingBackground.ScrollDirection.Left;
                }
                else
                {
                    reader.Close();
                    throw new Exception("Invalid scrolling direction : " + line);
                }

                bool inf = false;
                try
                {
                    inf = Convert.ToBoolean(tokens[4]);
                }
                catch (FormatException) { reader.Close(); throw new Exception("Invalid boolean for Infinite scroll : " + line); }

                Vector2 speed = Vector2.Zero;
                try
                {
                    speed = new Vector2(Convert.ToInt32(tokens[2]), Convert.ToInt32(tokens[3]));
                }
                catch (FormatException) { reader.Close();  throw new Exception("Invalid Vector for scroll speed : " + line); }

                bg = new ScrollingBackground(direction, speed, inf);

                //Parts
                while ((line = reader.ReadLine()).Split(' ')[0].Equals("bgpart"))
                {
                    bg.AddBackground(line.Split(' ')[1]);
                }

                if (bg.BackgroundSprites.Count == 0) { reader.Close(); throw new Exception("No BGPart found for Background "+i); }

                if (i == 1)
                    map.Background1 = bg;
                else if (i == 2)
                    map.Background2 = bg;
                else if (i == 3)
                    map.Background3 = bg;

                line = reader.ReadLine();
                while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            }

            #endregion

            //Initialization
            #region Init

            if (!line.Split(' ')[0].Equals("init"))
            {
                reader.Close();
                throw new Exception("Invalid TGPA map : init section not found");
            }

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //level music
            string[] tokens2 = line.Split('\"');
            if (tokens2.Length < 4)
            {
                Console.WriteLine("No music loaded");
            }
            else
            {
                map.Music = new MySong(tokens2[0].Split(' ')[1], tokens2[1], tokens2[3]);
            }
            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Initial datas
            try
            {
                map.InitialScore = Convert.ToInt32(line.Split(' ')[1]);
            }
            catch (FormatException) { reader.Close(); throw new Exception("Invalid integer for score : " + line); }

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            try
            {
                map.InitialLivesCount = Convert.ToInt32(line.Split(' ')[1]);
            }
            catch (FormatException) { reader.Close(); throw new Exception("Invalid integer for lives : " + line); }

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            string weaponName = line.Split(' ')[1];
            map.InitialWeapon = Weapon.TypeToWeapon(weaponName);

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //End conditions
            Event endmap = new Event(Vector2.Zero, null);
            String winflag = line.Split(' ')[1];
            line = reader.ReadLine();
            String loseflag = line.Split(' ')[1];
            endmap.AddCommand(new EndLevelCommand(winflag, loseflag));

            map.Events.Add(endmap);

            #endregion

            //Script
            #region Script event

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            if (!line.Split(' ')[0].Equals("begin"))
            {
                reader.Close();
                throw new Exception("Invalid TGPA map : begin keyword not found");
            }

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Read all events
            while (!line.Split(' ')[0].Equals("end"))
            {
                tokens2 = line.Split(' ');

                if (!tokens2[0].Equals("event"))
                {
                    reader.Close();
                    throw new Exception("Invalid TGPA map : event section missing (line : "+line+")");
                }

                Vector2 vector = Vector2.Zero;
                try
                {
                    vector = new Vector2((float)Convert.ToDouble(tokens2[2]), (float)Convert.ToDouble(tokens2[3]));
                }
                catch (FormatException) { reader.Close(); throw new Exception("Invalid Vector for event scroll value : " + line); }

                line = reader.ReadLine();
                tokens2 = line.Split(' ');
                if (!tokens2[0].Equals("start"))
                {
                    reader.Close();
                    throw new Exception("Invalid TGPA map : start keyword missing");
                }

                String startFlag = null;
                IfCommand ifc = null;

                try
                {
                    startFlag = line.Split(' ')[1];
                    if (!startFlag.Equals("")) ifc = new IfCommand(startFlag);
                }
                catch (IndexOutOfRangeException) { }

                Event e = new Event(vector, ifc);

                line = reader.ReadLine();
                while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

                List<Command> commands = new List<Command>();

                //Add actions
                while (!line.Split(' ')[0].Equals("endevent"))
                {
                    Command c = null;

                    switch (line.Split(' ')[0])
                    {
                        case "addenemy":
                            c = new AddEnemyCommand(line, screenResolution);

                            AddEnemyRessourcesToLoadIfNecessary(map, (AddEnemyCommand)c);

                            break;

                        case "addbge":
                            c = new AddBackgroundElementCommand(line, screenResolution);
                            AddBGERessourcesToLoadIfNecessary(map, (AddBackgroundElementCommand)c);
                            break;

                        case "message":
                            c = new AddMessageCommand(line);
                            break;

                        case "if":
                            c = new IfCommand(line);
                            break;

                        case "whilenot":
                            c = new WhileNotCommand(line);

                            line = reader.ReadLine();
                            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

                            List<String> lines = new List<String>();
                            while (!line.Split(' ')[0].Equals("done"))
                            {
                                if (line.Split(' ')[0].Equals("addenemy"))
                                {
                                    AddEnemyRessourcesToLoadIfNecessary(map, new AddEnemyCommand(line, screenResolution));
                                }
                                lines.Add(line);

                                line = reader.ReadLine();
                                while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();
                            }

                            ((WhileNotCommand)c).AddCommands(lines);

                            break;

                        case "scrollspeedreset":
                            c = new ResetScrollingSpeedCommand(line);
                            break;

                        case "scrollspeed":
                            c = new NewScrollingSpeedCommand(line);
                            break;

                        case "changemusic":
                            c = new ChangeMusicCommand(line);

                            map.MusicRessourcesToLoad.Add(((ChangeMusicCommand)c).Song);

                            break;

                        case "autogen":
                            c = new EnemyAutoGenerationCommand(line);
                            break;

                        case "addrandombonus":
                            c = new AddRandomBonusCommand(line);
                            break;

                        case "changemusicstate":
                            c = new ChangeMusicStateCommand(line);
                            break;

                        default:
                            throw new Exception("Unknown TGPA script command " + line);
                    }

                    commands.Add(c);

                    line = reader.ReadLine();
                    while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();
                } //commands

                e.AddCommands(commands);

                map.Events.Add(e);

                line = reader.ReadLine();
                while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            } //events

            #endregion

            reader.Close();

            return map;
        }

        private static void AddEnemyRessourcesToLoadIfNecessary(Map map, AddEnemyCommand c)
        {
            //Enemy ressources to load with the map
            bool mustAdd = true;

            //Get one enemy of each type used in the map
            foreach (BadGuy b in map.EnemyRessourcesToLoad)
            {
                if (b.GetType() == c.Enemy.GetType())
                    mustAdd = false;
            }

            if (mustAdd)
                map.EnemyRessourcesToLoad.Add(c.Enemy);
        }

        private static void AddBGERessourcesToLoadIfNecessary(Map map, AddBackgroundElementCommand c)
        {
            //Enemy ressources to load with the map
            bool mustAdd = true;

            //Get one enemy of each type used in the map
            foreach (BadGuy b in map.EnemyRessourcesToLoad)
            {
                if (b.GetType() == c.Enemy.GetType())
                    mustAdd = false;
            }

            if (mustAdd)
                map.EnemyRessourcesToLoad.Add(c.Enemy);
        }

        /// <summary>
        /// Get map informations (.tgpa files)
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public static MapOverview GetMapOverview(String file)
        {
            MapOverview overview = new MapOverview();
            overview.Filename = file;

            StreamReader reader = new StreamReader(file);

            String line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Check version
            if (Convert.ToDouble((line.Split(' '))[1]) > Convert.ToDouble(TheGreatPaperGame.version))
                throw new Exception("Insupported game version.");

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            //Map informations
            overview.Level = Convert.ToInt32((line.Split(' '))[1]);

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            overview.Name = line.Replace("name ", "");

            line = reader.ReadLine();
            while (line.Equals("") || line.StartsWith("//")) line = reader.ReadLine();

            String filedesc = line.Replace("desc ", "");
            String s = Localization.GetString(filedesc);
            overview.Description = s == null ? filedesc : s;

            reader.Close();

            //Load scores for this map
           // overview.Scores = Highscores.LoadScores(file.Replace(".tgpa", ".esd"));

            return overview;
        }

        /// <summary>
        /// Deeper background (where the enemies & player are)
        /// </summary>
        public ScrollingBackground Background1
        {
            get { return background1; }
            set { background1 = value; }
        }

        /// <summary>
        /// Middle Background
        /// </summary>
        public ScrollingBackground Background2
        {
            get { return background2; }
            set { background2 = value; }
        }

        /// <summary>
        /// Nearest background
        /// </summary>
        public ScrollingBackground Background3
        {
            get { return background3; }
            set { background3 = value; }
        }

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public String Description
        {
            get { return description; }
            set { description = value; }
        }

        /// <summary>
        /// Level number
        /// </summary>
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public MySong Music
        {
            set { music = value; }
            get { return music; }
        }

        public Vector2 Scroll
        {
            get { return Background1.Scroll; }
        }

        public TGPA.Maps.ScrollingBackground.ScrollDirection Direction
        {
            get { return Background2.Direction; }
        }

        public MapFlags Flags { get { return flags; } set { flags = value; } }
        public List<Event> Events { get { return events; } set { events = value; } }
    }

    /// <summary>
    /// Get a thumbnail, map info and map score but don't load enemy or backgrounds
    /// </summary>
    public class MapOverview
    {
        /// <summary>
        /// Load 64x64 thumbnails
        /// </summary>
        /// <param name="Content"></param>
        public void LoadContent(ContentManager Content)
        {
            try
            {
                thumb = Content.Load<Texture2D>(filename.Replace(".tgpa", "").Replace("Content/maps", ""));
            }
            catch (Exception)
            {
                thumb = Content.Load<Texture2D>(StorageContainer.TitleLocation + "\\" + "Content\\maps\\nothumb");
            }

            Rect = new Rectangle(0, 0, 96, 64);
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set { filename = value; }
        }

        private Texture2D thumb;
        public Texture2D Thumb
        {
            get { return thumb; }
            set { thumb = value; }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private string desc;
        public string Description
        {
            get { return desc; }
            set { desc = value; }
        }

        private ScoreLine[] scores;
        public ScoreLine[] Scores
        {
            get { return scores; }
            set { scores = value; }
        }

        private int level;
        public int Level
        {
            get { return level; }
            set { level = value; }
        }

        public Rectangle Rect;
    }
}
