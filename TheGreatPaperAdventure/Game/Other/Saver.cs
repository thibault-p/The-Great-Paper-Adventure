//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.IO;
using System.Xml.Serialization;
using EasyStorage;
using TGPA.MenuItems;
using System.Diagnostics;
using TGPA.Localization;
using System.Collections.Generic;
using TGPA.Maps;
using TGPA.Utils;
using System.Xml;
using TGPA.Screens;

namespace TGPA.Game.Save
{
    /// <summary>
    /// Instance of a saved game
    /// </summary>
    public class SaveData
    {
        public static int ScoreLineNumber = 20;
        /// <summary>
        /// Last level player has reached
        /// </summary>
        public int LastLevel { get; set; }
        /// <summary>
        /// Score management for each level
        /// </summary>
        public SerializableDictionary<ScoreType, SerializableDictionary<String, ScoreLine[]>> ScoresBylevel { get; set; }
        /// <summary>
        /// Game Options storage
        /// </summary>
        public OptionsData OptionsData { get; set; }
        /// <summary>
        /// Poulpis dead on the battle field
        /// </summary>
        public Int32 KilledPoulpis { get; set; }
        /// <summary>
        /// Display "Show credits" button on menu if the game has been ended
        /// </summary>
        public bool GameHasBeenEnded;

        /// <summary>
        /// If you need to get a clear savegame, use this method. But be careful, could erase other data
        /// </summary>
        public SaveData()
        {
            KilledPoulpis = 0;
            GameHasBeenEnded = false;

#if DEBUG
            LastLevel = LevelSelectionScreen.WorldCount;
#else
            LastLevel = 0;
#endif
            ScoresBylevel = new SerializableDictionary<ScoreType, SerializableDictionary<String, ScoreLine[]>>();
            ScoresBylevel.Add(ScoreType.Single, new SerializableDictionary<String, ScoreLine[]>());
            ScoresBylevel.Add(ScoreType.Coop, new SerializableDictionary<String, ScoreLine[]>());

            this.OptionsData = new OptionsData();
        }

        #region Score management

        /// <summary>
        /// Add a new score to the level scoreboard
        /// </summary>
        /// <param name="map">map name</param>
        /// <param name="scoreType"></param>
        /// <param name="newScoreLine"></param>
        /// <returns>The rank of the score</returns>
        public Int32 AddScore(String map, ScoreType scoreType, ScoreLine newScoreLine)
        {
            int rank = ScoreLineNumber + 1;

            bool replace = false;

            ScoreLine[] scoresForThisLevel = null;

            //Access to the score for this level and this mode
            SerializableDictionary<String, ScoreLine[]> score = null;

            if (ScoresBylevel.TryGetValue(scoreType, out score))
            {
                if (score.TryGetValue(map, out scoresForThisLevel))
                {
                    for (int scoreLineIndex = 0; scoreLineIndex < scoresForThisLevel.Length; scoreLineIndex++)
                    {
                        ScoreLine currentLine = scoresForThisLevel[scoreLineIndex];

                        //Look if scores in the top ScoreLineNumber lines
                        if (newScoreLine.Score > currentLine.Score)
                        {
                            replace = true;
                        }
                        else
                        {
                            if (newScoreLine.Score == currentLine.Score)
                            {
                                if (newScoreLine.Date > currentLine.Date)
                                {
                                    replace = true;
                                }
                            }
                        }

                        if (replace)
                        {
                            rank = currentLine.Rank;

                            //HACK Avoid corrupted savegames
                            if (rank == -1) rank = 20;

                            newScoreLine.Rank = rank;

                            ScoreLine last = null;
                            ScoreLine toReplace = newScoreLine;

                            //Move down new lines
                            for (int i = scoreLineIndex; i < scoresForThisLevel.Length - 1; i++)
                            {
                                last = scoresForThisLevel[i];
                                scoresForThisLevel[i] = toReplace;

                                toReplace = last;
                                toReplace.Rank += 1;
                            }

                            break;
                        }
                    }

                }
                else
                {
                    //Create scores data for this map
                    scoresForThisLevel = new ScoreLine[ScoreLineNumber];

                    for (int i = 0; i < scoresForThisLevel.Length; i++)
                    {
                        scoresForThisLevel[i] = ScoreLine.GetDefaultScoreLine(i + 1);
                    }

                    //Player has the best score
                    scoresForThisLevel[0] = newScoreLine;

                    ScoresBylevel[scoreType].Add(map, scoresForThisLevel);
                }
            }
            return rank;
        }

        #endregion
    }

    /// <summary>
    /// Player saved game management
    /// </summary>
    public class Saver
    {
        /// <summary>
        /// File where data will be stored for the player
        /// </summary>
        /// 
        private string filename = "save.tgps";

        private string containerName = "The Great Paper Adventure";

#if WINDOWS
        private string folderName;
        private string filePath;
#endif

        private SaveData save;
        private readonly XmlSerializer serializer = new XmlSerializer(typeof(SaveData));

        public Saver()
        {
#if WINDOWS
            folderName = System.IO.Path.Combine(System.IO.Path.Combine(Environment.GetEnvironmentVariable("USERPROFILE"), "Saved Games"), containerName);
            filePath = System.IO.Path.Combine(folderName, filename);
#endif
            save = new SaveData();
        }

        /// <summary>
        /// Add a new score to the level scoreboard
        /// </summary>
        /// <param name="scoreLine"></param>
        /// <returns>The rank of the score</returns>
        public Int32 AddScore(String map, ScoreType scoreType, ScoreLine scoreLine)
        {
            return save.AddScore(map, scoreType, scoreLine);
        }

        /// <summary>
        /// Return the highscore for the required level
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public ScoreLine GetBestScoreForLevel(String map, ScoreType scoreType)
        {
            try
            {
                ScoreLine[] result = null;
                if (save.ScoresBylevel[scoreType].TryGetValue(map, out result))
                {
                    return result[0];
                }
                else
                {
                    save.AddScore(map, scoreType, ScoreLine.GetDefaultScoreLine(1));

                    return GetBestScoreForLevel(map, scoreType);
                }
            }
            catch (IndexOutOfRangeException)
            {
                this.save = new SaveData();
                this.Save();

                //This exception is throwed only if you have an old savegame
                //For now I just reset scores and options
                throw new Exception(LocalizedStrings.GetString("CorruptedSavegame"));
            }
        }

        /// <summary>
        /// Saving the save
        /// </summary>
        public void Save()
        {
#if WINDOWS
            Stream stream = null;
#endif
            try
            {
                
#if XBOX
                if (TGPAContext.Instance.SaveDevice != null)
                {
                    // make sure the device is ready
                    if (TGPAContext.Instance.SaveDevice.IsReady)
                    {
                        // save a file asynchronously. this will trigger IsBusy to return true
                        // for the duration of the save process.
                        TGPAContext.Instance.SaveDevice.SaveAsync(
                            containerName,
                            filename,
                            stream =>
                            {
                                using (XmlWriter writer = XmlWriter.Create(stream))
                                {
                                    SerializeSave(writer);
                                }
                                
                            });
                    }
                }
#else
                    //HACK : Delete the file to avoid syntax error..
                    if(File.Exists(filePath)) {
                        File.Delete(filePath);
                    }

                    XmlWriterSettings settings = new XmlWriterSettings();
                    settings.Indent = true;

                    stream = File.OpenWrite(filePath);
                    XmlWriter writer = XmlWriter.Create(stream, settings);
                    SerializeSave(writer);
                    writer.Close();
                    stream.Close();
#endif
                
            }
            catch (Exception e)
            {
                Logger.Log(LogLevel.Error, "Exception when loading savegame : " + e.Message);
            }
            finally
            {
#if WINDOWS
                if(stream != null)
                    stream.Close();
#endif
            }
            Logger.Log(LogLevel.Info, "Game Saved");
        }

        /// <summary>
        /// Load a saved game
        /// </summary>
        public void Load()
        {
#if WINDOWS
            Stream stream = null;
#endif
            try
            {
#if XBOX
            if (TGPAContext.Instance.SaveDevice.FileExists(containerName, filename) == false)
            {
                this.Save();
            }
                // make sure the device is ready
                if (TGPAContext.Instance.SaveDevice.IsReady)
                {
                    // load a file asynchronously. this will trigger IsBusy to return true
                    // for the duration of the save process.
                    TGPAContext.Instance.SaveDevice.LoadAsync(
                         containerName,
                            filename,
                            stream =>
                            {
                                using (XmlReader reader = XmlReader.Create(stream))
                                {
                                    DeserializeSave(reader);
                                }
                            }
                        );
                }

#else
                stream = File.OpenRead(filePath);
                XmlReader reader = XmlReader.Create(stream);
                DeserializeSave(reader);
                stream.Close();
#endif
            }
            catch (Exception e)
            {
#if WINDOWS
                if (stream != null)
                {
                    stream.Close();
                }
#endif
                //New savegame
                save = new SaveData();

                Logger.Log(LogLevel.Error, "Exception when loading savegame : " + e.Message);

                this.Save();
            }

            Logger.Log(LogLevel.Info, "Savedgame loaded");
        }

        private void SerializeSave(XmlWriter writer)
        {
            serializer.Serialize(writer, save);
        }

        private void DeserializeSave(XmlReader reader)
        {
            save = serializer.Deserialize(reader) as SaveData;
        }

        public OptionsData OptionsData
        {
            get
            {
                return this.save.OptionsData;
            }
        }

        public SaveData SaveData
        {
            get
            {
                return this.save;
            }
        }
    }
}
