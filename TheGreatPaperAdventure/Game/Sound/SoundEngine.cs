//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;
using TGPA.Game.Weapons;
using TGPA.Game.BadGuys;
using TGPA.Game.Sound;
using System.Threading;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TGPA.Audio
{
    /// <summary>
    /// Simplify how to play sound effects. 
    /// Use XACT
    /// </summary>
    public class SoundEngine
    {
        /// <summary>
        /// Singleton
        /// </summary>
        private static SoundEngine instance;

        private AudioEngine audioEngine;
        private WaveBank waveBank;
        private SoundBank soundBank;
        /// <summary>
        /// Do not play a sound if it is already played in this frame
        /// </summary>
        private List<String> playedCues;

        private SoundEngine()
        {
            instance = this;
            this.Initialize();
        }

        /// <summary>
        /// Load XACT project
        /// </summary>
        private void Initialize()
        {
            this.audioEngine = new AudioEngine("Content\\sfx\\XACT\\tgpa.xgs");
            this.waveBank = new WaveBank(audioEngine, "Content\\sfx\\XACT\\TGPA Wave Bank.xwb");
            this.soundBank = new SoundBank(audioEngine, "Content\\sfx\\XACT\\TGPA Sound Bank.xsb");

            this.playedCues = new List<string>();
        }

        /// <summary>
        /// Play a sound using its cue name
        /// </summary>
        /// <param name="soundCueName"></param>
        public void PlaySound(String soundCueName)
        {
            if (soundCueName != null)
            {
                if (!this.playedCues.Contains(soundCueName))
                {
                    this.playedCues.Add(soundCueName);
                    Cue sound = soundBank.GetCue(soundCueName);
                    sound.Play();
                }
            }
        }

        /// <summary>
        /// Update audio engine 
        /// </summary>
        /// <param name="gameTime"></param>
        public void Update(GameTime gameTime)
        {
            this.audioEngine.Update();

            this.playedCues.Clear();
        }

        /// <summary>
        /// Creation of the instance, so you can load it without freezing the game
        /// </summary>
        public static void InitSoundEngineInstance()
        {
            new SoundEngine();
        }

        /// <summary>
        /// Singleton pattern
        /// </summary>
        /// <returns></returns>
        public static SoundEngine Instance
        {
            get
            {
                if (instance == null) instance = new SoundEngine();

                return instance;
            }
        }

        /// <summary>
        /// Change sounds volume (between 0.0f and 1.0f
        /// </summary>
        public float Volume
        {
            get
            {
                return TGPAContext.Instance.Options.SoundVolume;
            }
            set
            {
                this.audioEngine.GetCategory("Default").SetVolume(value);
            }
        }
    }
}
