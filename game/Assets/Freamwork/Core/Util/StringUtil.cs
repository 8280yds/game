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



    }
}
