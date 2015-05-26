using UnityEngine;
using System.Collections;

namespace Freamwork
{
    public static class StringUtil
    {
        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns></returns>
        public static string getFileName(string fullName)
        {
            int lastIndexOf = fullName.LastIndexOf("\\");
            return fullName.Substring(lastIndexOf + 1, fullName.Length - 1 - lastIndexOf);
        }

        /// <summary>
        /// 将相对路径转化成绝对路径
        /// </summary>
        /// <param name="relaPath">相对路径</param>
        /// <returns>绝对路径</returns>
        public static string relaToAbs(string relaPath)
        {
            relaPath = relaPath.Trim();
            string absPath = Application.dataPath;
            while (relaPath[0].ToString() == ".")
            {
                relaPath = relaPath.Substring(1);
                absPath = absPath.Substring(0, absPath.LastIndexOf("/"));
            }
            absPath += relaPath;
            return absPath;
        }

        /// <summary>
        /// 获取最后一个“/”后面的字符
        /// </summary>
        /// <param name="targetStr"></param>
        /// <returns></returns>
        public static string getLastStr(string targetStr)
        {
            string str = targetStr;
            int index = targetStr.LastIndexOf("/");
            if (index >= 0)
            {
                str = targetStr.Substring(index + 1);
            }
            return str;
        }

    }
}
