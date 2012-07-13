//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using Microsoft.Xna.Framework.Media;
using TGPA.Audio;
using TGPA.Localization;
using Microsoft.Xna.Framework.Graphics;
using TGPA.Utils.Input;

namespace TGPA.Game.Save
{
    /// <summary>
    /// Game options data storage.
    /// Data is stored in the savegame file
    /// </summary>
    public class OptionsData
    {
       // private String version;
        private bool showScores;
        private bool showMusic;
        private bool debugMode;
        private float musicVolume;
        private float soundVolume;
        private bool rumble;
        private Difficulty difficulty;

#if WINDOWS
        private int screenWidth;
        private int screenHeight;
        public string Player1Name { get; set; }
        public Device Player1PlayingDevice { get; set; }
        private bool fullScreen;
        private bool log;
#endif


        public OptionsData()
        {
            //version = TGPAContext.Instance.Version;

            showScores = false;
            showMusic = true;
            debugMode = false;
            difficulty = Difficulty.Normal;

#if WINDOWS
            screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width; //Use best resolution
            screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            Player1Name = "windowsPlayer";
            Player1PlayingDevice = new Device();
            fullScreen = true;
            log = true;
#endif

            musicVolume = 0.9f;
            soundVolume = 0.5f;
            rumble = true;
        }

        public bool ShowScore
        {
            set { showScores = value; }
            get { return showScores; }
        }

        public bool ShowMusic
        {
            set { showMusic = value; }
            get { return showMusic; }
        }

        public bool DebugMode
        {
            get { return debugMode; }
            set { debugMode = value; }
        }

        public float MusicVolume
        {
            get { return musicVolume; }
            set
            {
                musicVolume = value;
                MediaPlayer.Volume = value;
            }
        }

        public float SoundVolume
        {
            get { return soundVolume; }
            set
            {
                soundVolume = value;
                SoundEngine.Instance.Volume = value;
            }
        }

        //public String Version
        //{
        //    get { return version; }
        //    set { version = value; }
        //}

        public bool Rumble
        {
            get { return rumble; }
            set { rumble = value; }
        }

        public Difficulty Difficulty
        {
            get { return difficulty; }
            set { difficulty = value; }
        }

#if WINDOWS
        public int Width
        {
            get { return screenWidth; }
            set { screenWidth = value; }
        }

        public int Height
        {
            get { return screenHeight; }
            set { screenHeight = value; }
        }

        public bool FullScreen
        {
            get { return fullScreen; }
            set { fullScreen = value; }
        }

        public bool EnableLog
        {
            get { return log; }
            set { log = value; }
        }
#endif
    }
}
