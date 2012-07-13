//****************************************************************************************************
//                             The Great paper Adventure : 2009-2011
//                                   By The Great Paper Team
//©2009-2011 Valryon. All rights reserved. This may not be sold or distributed without permission.
//****************************************************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace TGPA.Utils
{
    public enum LogLevel
    {
        Info,
        Warning,
        Error,
        Fatal
    }

    /// <summary>
    /// Class managing a log file
    /// </summary>
    public class Logger
    {
        protected static bool _active;
        private static String _logPath;

        public static void Initialization(String logPath)
        {
            _logPath = logPath;
            _active = true;
#if WINDOWS

            StreamWriter textOut = new StreamWriter(new FileStream(logPath, FileMode.Create, FileAccess.Write));
            textOut.WriteLine("<h1>The Great Paper Adventure</h1>");
            textOut.WriteLine("<h2>- The Great Log File -</h2>");
            textOut.WriteLine("");
            textOut.WriteLine("<p style=\"font-family: &quot;Kootenay&quot;; color: #000000;\">");
            textOut.WriteLine("Log started at " + System.DateTime.Now.ToLongTimeString());
            textOut.WriteLine("</p><p>");
            textOut.WriteLine("This log file may help me tracking bug. If the game crashes, please send me this file at <a href=\"contact@thegreatpaperadventure.com\" >contact@thegreatpaperadventure.com</a>.");
            textOut.WriteLine("</p>");
            textOut.WriteLine("<p>");
            textOut.WriteLine("Thanks for playing and sorry for bugs !");
            textOut.WriteLine("</p>");
            textOut.WriteLine("<hr />");

            textOut.Close();


            Logger.Log(LogLevel.Info, "Starting log record");
#endif
        }

        /// <summary>
        /// Activate or Deactivate log
        /// </summary>
        public static bool Active
        {
            get { return _active; }
            set
            {
                if (!value)
                {
                    Logger.Log(LogLevel.Info, "Log disabled");
                }

                _active = value;
            }
        }

        /// <summary>
        /// Add a log entry
        /// </summary>
        /// <param name="level"></param>
        /// <param name="text"></param>
        public static void Log(LogLevel level, string text)
        {
#if WINDOWS
            if (!_active) return;

#if DEBUG
            Trace.WriteLine("TGPA " + level + " : " + text);
#endif

            string begin = "";
            switch (level)
            {
                case LogLevel.Error: begin = "<p style=\"color: #ff0000;\">"; break;
                case LogLevel.Info: begin = "<p style=\"color: #0008f0;\">"; break;
                case LogLevel.Warning: begin = "<p style=\"color: #ffb200;\">"; break;
                case LogLevel.Fatal: begin = "<p style=\"color: #ffff00;\">"; break;
            }
            text = begin + System.DateTime.Now.ToLongTimeString() + " : " + text + "</p>";

            Logger.Output(text);
#endif
        }

        /// <summary>
        /// Write text in the log file
        /// </summary>
        /// <param name="text"></param>
        private static void Output(string text)
        {
#if WINDOWS
            try
            {
                StreamWriter textOut = new StreamWriter(new FileStream(_logPath, FileMode.Append, FileAccess.Write));
                textOut.WriteLine(text);
                textOut.Close();
            }
            catch (Exception e)
            {
                string error = e.Message;
                Trace.WriteLine("TGPA Error : " + error);
            }
#endif
        }
    }
}
