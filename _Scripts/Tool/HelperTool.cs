using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace FGame
{
    /// <summary>
    /// 帮助工具类       by: heyang   2017/12/9
    /// </summary>
    public class HelperTool
    {

        #region 日志信息打印
        public static bool IsPrintLog = true;

        private enum LogType
        {
            Normal,
            Error
        };

        private static string _logFilePath = string.Empty;      //log文件路径

        /// <summary>
        /// 获取log文件的路径
        /// </summary>
        private static string GetLogFilePath(string fileName)
        {
            //只有windows的编辑器模式才能输出log
            if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                return Application.dataPath + @"\_Logs\" + fileName;
            }
            return string.Empty;
        }

        private static void OutputLogs(LogType type, string log)
        {
            if (_logFilePath == string.Empty)
            {
                _logFilePath = GetLogFilePath("FGLogs.log");
            }

            //输出到unity 控制台
            if (type == LogType.Normal)
            {
                Debug.Log(log);
            }
            else
            {
                Debug.LogError(log);
            }

            using (StreamWriter w = File.AppendText(_logFilePath))
            {
                w.WriteLine("{0} [{1}] : {2}", type.ToString(), DateTime.Now.ToString(), log);
            }
        }

        /// <summary>
        /// 输出普通日志信息
        /// </summary>
        public static void Log(string log)
        {
            if (IsPrintLog)
            {
                OutputLogs(LogType.Normal, log);
            }
        }
        /// <summary>
        /// 输出错误日志信息
        /// </summary>
        public static void LogError(string log)
        {
            if (IsPrintLog)
            {
                OutputLogs(LogType.Error, log);
            }
        }

        #endregion

    }

}
