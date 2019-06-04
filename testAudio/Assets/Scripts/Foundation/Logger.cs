#define LOG_ENABLED_1

#if UNITY_ANDROID || UNITY_IPHONE
#undef LOG_ENABLED_1
#endif
#if UNITY_EDITOR
#define LOG_ENABLED_1
#endif

#if !DEBUG
#undef LOG_ENABLED_1
#else
#define LOG_ENABLED_1
#endif

#if UNITY_ANDROID || UNITY_IPHONE
#define MOBILE
#endif

using UnityEngine;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace COMMON
{
    /// <summary>
    /// 原则上必须使用这个类来打印日志和错误，不允许直接使用Debug.Log
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 是否开始日志
        /// </summary>
        public static bool startLog = true;
        public static int isCurrentLogError = 10;
        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="info"></param>
        /// <param name="args"></param>

        public static void LogError(string info, params object[] args)
        {


#if CLOSE_LOG
            return;
#endif

            info = string.Format(info, args);
            LogError(info);

        }
        /// <summary>
        /// 打印错误
        /// </summary>
        /// <param name="info"></param>

        public static void LogError(string info)
        {

#if CLOSE_LOG
            return;
#endif

            info = "PLD:  " + System.DateTime.Now.Millisecond + "       " + info;
            UnityEngine.Debug.LogError(info);



        }
        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="info"></param>
        /// <param name="args"></param>

        public static void LogWarning(string info, params object[] args)
        {

#if CLOSE_LOG
            return;
#endif
            info = string.Format(info, args);
            LogWarning(info);
        }
        /// <summary>
        /// 打印警告
        /// </summary>
        /// <param name="info"></param>

        public static void LogWarning(string info)
        {

#if CLOSE_LOG
            return;
#endif

            info = "PLD:  " + System.DateTime.Now.Millisecond + "       " + info;
            UnityEngine.Debug.LogWarning(info);


        }
        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="info"></param>

        public static void Log(string info)
        {

#if CLOSE_LOG
            return;
#endif

            info = "PLD:  " + System.DateTime.Now + "       " + info;
            UnityEngine.Debug.Log(info);


        }


        public static void LogGreen(string info, params object[] param)
        {
            LogError(string.Format("<color=#00ff00ff>{0}</color>", string.Format(info, param)));
        }


        public static void LogBlue(string info, params object[] param)
        {
            LogError(string.Format("<color=#009BC3FF>{0}</color>", string.Format(info, param)));
        }

        /// <summary>
        /// 打印日志
        /// </summary>
        /// <param name="info"></param>
        /// <param name="args"></param>

        public static void Log(string info, params object[] args)
        {

#if CLOSE_LOG
            return;
#endif
            info = string.Format(info, args);
            Log(info);
        }

        /// <summary>
        /// 画线
        /// 会将坐标的Y设为10
        /// </summary>
        public static void DrawLine(Vector3 start, Vector3 end, Color color)
        {
            start.y = 10;
            end.y = 10;

            UnityEngine.Debug.DrawLine(start, end, color);
        }

        /// <summary>
        /// 画线
        /// 会将坐标的Y设为10
        /// </summary>
        public static void DrawLine(Vector3 start, Vector3 end)
        {
            DrawLine(start, end, Color.white);
        }
    }//end class
}//end namesapce


