using System;
using System.IO;

namespace Obonator.Library
{
    public class ObonLogger
    {
        private const string DEFAULT_DIR = @"C:\Obonator\Log";
        private const int MAX_TRY = 10;

        private string _CurrentDir;
        private string _CurrentFileName;
        private bool _IsLogOn;

        public ObonLogger()
        {
            _CurrentDir = DEFAULT_DIR;
            _CurrentFileName = "Obonator";
            _IsLogOn = true;
        }

        public ObonLogger(string LogName)
        {
            _CurrentDir = DEFAULT_DIR;
            _CurrentFileName = LogName;
            _IsLogOn = true;
        }

        public ObonLogger(string LogDir, string LogName)
        {
            _CurrentDir = LogDir;
            _CurrentFileName = LogName;
            _IsLogOn = true;
        }

        public ObonLogger(string LogDir, string LogName, bool IsLogOn)
        {
            _CurrentDir = LogDir;
            _CurrentFileName = LogName;
            _IsLogOn = IsLogOn;
        }

        public void Log(string Message)
        {
            WriteLog(Message, "");
        }

        public void Log(string Message, string StackTrace)
        {
            WriteLog(Message, StackTrace);
        }

        private void WriteLog(string Message, string StackTrace)
        {
            if (!_IsLogOn)
            {
                return;
            }
            string LogFileName;

            for (int i = 0; i < MAX_TRY; i++)
            {
                try
                {
                    if (Directory.Exists(this._CurrentDir) == false)
                        Directory.CreateDirectory(this._CurrentDir);

                    if (this._CurrentDir.Substring(this._CurrentDir.Length - 1, 1) == @"\")
                        LogFileName = this._CurrentDir + this._CurrentFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";
                    else
                        LogFileName = this._CurrentDir + @"\" + this._CurrentFileName + "_" + DateTime.Now.ToString("yyyy-MM-dd") + ".log";

                    using (StreamWriter fs = new StreamWriter(LogFileName, true))
                    {
                        fs.WriteLine(DateTime.Now.ToString("[HH:mm:ss] ") + Message);

                        if (StackTrace != "")
                        {
                            fs.WriteLine("{" + StackTrace + "}");
                            fs.WriteLine();
                        }

                        fs.Close();
                    }

                    //exit when success
                    break;
                }
                catch
                {
                    //try until success
                }
            }
        }

    }
}
