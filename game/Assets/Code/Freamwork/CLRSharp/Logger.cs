using CLRSharp;
using UnityEngine;

namespace Freamwork
{
    public class Logger : ICLRSharp_Logger
    {
        public void Log(string str)
        {
            Debug.Log(str);
        }

        public void Log_Error(string str)
        {
            Debug.LogError(str);
        }

        public void Log_Warning(string str)
        {
            Debug.LogWarning(str);
        }

        //======================静态方法======================
        public static void log(string message)
        {
            Debug.Log(message);
        }

        public static void log(string message, Object context)
        {
            Debug.Log(message, context);
        }

        public static void logWarning(string message)
        {
            Debug.LogWarning(message);
        }

        public static void logWarning(string message, Object context)
        {
            Debug.LogWarning(message, context);
        }

        public static void logError(string message)
        {
            Debug.LogError(message);
        }

        public static void logError(string message, Object context)
        {
            Debug.LogError(message, context);
        }

    }
}