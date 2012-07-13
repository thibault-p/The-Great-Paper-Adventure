//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TGPA.Audio;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Content;
using System.Threading;
using Microsoft.Xna.Framework.Audio;

namespace TGPA.Game.Sound
{
    /// <summary>
    /// My modification of the class "Song"
    /// </summary>
    public class MySong
    {
        public String Title, Artist;
        public Song Song;
        public String FileName;

        public MySong(String fileName_, String title_, String artist_)
            : this(fileName_, title_, artist_, null)
        {
        }

        public MySong(String fileName_, String title_, String artist_, Song song_)
        {
            FileName = fileName_;
            Title = title_;
            Artist = artist_;
            Song = song_;
        }

        public override string ToString()
        {
            return FileName + " \"" + Title + "\" \"" + Artist + "\"";
        }

        public void Dispose()
        {
            this.Song.Dispose();
        }
    }

    /// <summary>
    /// Music management
    /// </summary>
    public class MusicEngine
    {
        /// <summary>
        /// Singleton
        /// </summary>
        private static MusicEngine instance;

        /// <summary>
        /// Title Screen song
        /// </summary>
        private volatile MySong titleSong, creditsSong;

        private volatile Boolean loaded;
        private volatile MySong currentMusic;

        private MusicEngine()
        {
            loaded = false;
            titleSong = new MySong("titleMusic", "Turn turn around", "Spintronic");
            creditsSong = new MySong("creditsMusic", "Hypersmash", "Spintronic");

            instance = this;
        }

        public void LoadContent(ContentManager Content)
        {
            titleSong.Song = Content.Load<Song>("sfx/Music/" + titleSong.FileName);
            creditsSong.Song = Content.Load<Song>("sfx/Music/" + creditsSong.FileName);

            loaded = true;
        }

        /// <summary>
        /// Play music
        /// </summary>
        /// <param name="music"></param>
        /// <param name="level"></param>
        public void Resume()
        {
            if (!loaded) return;

            Thread t = new Thread(ResumeThread);
            t.Start();
        }

        public void ResumeThread()
        {
            MediaPlayer.Resume();
            MusicEngine.Instance.Volume = TGPAContext.Instance.Options.MusicVolume;
        }


        public void Pause()
        {
            if (!loaded) return;

            Thread t = new Thread(PauseThread);
            t.Start();
        }

        private static void PauseThread()
        {
            MusicEngine.Instance.Volume /= 3f;
            MediaPlayer.Pause();
        }

        /// <summary>
        /// Change current game music
        /// </summary>
        /// <param name="s"></param>
        public void ChangeMusic(MySong s)
        {
            if (s != currentMusic)
            {
                ThreadPool.QueueUserWorkItem(delegate(object data)
                { 
                    MediaPlayer.Play(s.Song);
                    MediaPlayer.IsRepeating = true;
                    MusicEngine.Instance.Volume = TGPAContext.Instance.Options.MusicVolume;

                    if (TGPAContext.Instance.Options.ShowMusic)
                    {
                        TGPAContext.Instance.SongInfo = new SongInfo(s.Title, s.Artist);
                    }

                    currentMusic = s;
                }); 
            }
        }

        private void StopMusicThread()
        {
            MediaPlayer.Stop();
            loaded = false;
        }

        public void StopMusic()
        {
            Thread t = new Thread(StopMusicThread);
            t.Start();
        }

        public static MusicEngine Instance
        {
            get
            {
                if (instance == null) instance = new MusicEngine();

                return instance;
            }
        }

        public float Volume
        {
            get { return MediaPlayer.Volume; }
            set { MediaPlayer.Volume = value; }
        }

        public MySong TitleSong
        {
            get { return titleSong; }
        }

        public MySong CreditsSong
        {
            get { return creditsSong; }
        }
    }
}
