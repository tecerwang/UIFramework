using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UIFramework
{
    public static class Utility
    {
        public enum LogLevel
        {
            Internal,
            Normal
        }

        public static LogLevel logLevel = LogLevel.Normal;

        #region Debug Log
        public static void LogDebug(params object[] args)
        {
            Debug.Log(CombineLogParams(args));
        }

        public static void LogDebug(string prefix, params object[] args)
        {
            Debug.Log($"<color=#00ff00ff>[{prefix}] </color>" + CombineLogParams(args));
        }

        /// <summary>
        /// 内部需要打印的消息
        /// </summary>
        /// <param name="args"></param>
        public static void LogInternalDebug(params object[] args)
        {
            if (logLevel == LogLevel.Internal)
            {
                Debug.Log("<color=#888888ff>LogInternal </color>" + CombineLogParams(args));
            }
        }

        public static void LogWarning(params object[] args)
        {
            Debug.LogWarning(CombineLogParams(args));
        }

        public static void LogExpection(params object[] args)
        {
            Debug.Log("<color=#bbbb00ff>Expection </color>" + CombineLogParams(args));
        }

        public static void LogError(params object[] args)
        {
            Debug.Log("<color=#ff0000ff>Error </color>" + CombineLogParams(args));
        }

        private static string CombineLogParams(params object[] args)
        {
            string result = string.Empty;
            if (args.Length > 0)
            {
                foreach (var arg in args)
                {
                    result += arg.ToString() + " ";
                }
                result = result.Substring(0, result.Length - 1);
            }
            return result;
        }
        #endregion

        public static string GetDeviceId()
        {
#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
            return SystemInfo.deviceUniqueIdentifier;
#else
        return Guid.NewGuid().ToString();
#endif
        }

        #region UTCTime
        public static DateTime GetCurrentUTC()
        {
            return DateTime.UtcNow;
        }

        public static long UTCNowMilliseconds()
        {
            TimeSpan timeSpan = new TimeSpan(System.DateTime.UtcNow.Ticks);
            return (long)timeSpan.TotalMilliseconds;
        }

        public static long UTCNowSeconds()
        {
            TimeSpan timeSpan = new TimeSpan(System.DateTime.UtcNow.Ticks);
            return (long)timeSpan.TotalSeconds;
        }

        /// <summary>
        /// </summary>
        /// <param name="utcTimeString">like: 03:40:20</param>
        /// <param name="toLocalTime"></param>
        /// <returns></returns>
        public static DateTime? ParseUTCTimeString(string utcTimeString, DateTimeKind dateTimeKind = DateTimeKind.Local)
        {
            if (string.IsNullOrEmpty(utcTimeString))
            {
                return null;
            }

            string[] segments = utcTimeString.Split(':');
            if (segments == null || segments.Length != 3)
            {
                return null;
            }

            int hour, mins, secs;
            if (!int.TryParse(segments[0], out hour) ||
                !int.TryParse(segments[1], out mins) ||
                !int.TryParse(segments[2], out secs) ||
                hour < 0 || hour > 23 ||
                mins < 0 || mins > 59 ||
                secs < 0 || secs > 59)
            {
                return null;
            }

            // date is meaningless
            return new DateTime(2000, 1, 1, hour, mins, secs, dateTimeKind);
        }
        #endregion
    }
}
