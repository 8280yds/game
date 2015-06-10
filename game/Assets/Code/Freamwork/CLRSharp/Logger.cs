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

        public static void debug(params object[] objs)
        {
            Debug.Log(objs);
        }
    }
}