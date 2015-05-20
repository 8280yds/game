using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载时事件方法的集合类
    /// </summary>
    public class LoadInfo : LoadData
    {
        //======================属性===========================
        /// <summary>
        /// 加载优先级
        /// </summary>
        public LoadPriority priority = LoadPriority.two;

        /// <summary>
        /// WWW
        /// </summary>
        public WWW www = null;

        //======================方法===========================
        /// <summary>
        /// 加载开始前执行的方法
        /// </summary>
        public LoadFunctionDele loadStart = null;

        /// <summary>
        /// 加载开始后且在结束前每帧执行的方法
        /// </summary>
        public LoadFunctionDele loadProgress = null;

        /// <summary>
        /// 加载结束后执行的方法
        /// </summary>
        public LoadFunctionDele loadEnd = null;

        /// <summary>
        /// 加载失败后执行的方法
        /// </summary>
        public LoadFunctionDele loadFail = null;

        /// <summary>
        /// 解压开始前执行的方法
        /// </summary>
        public LoadFunctionDele unZipStart = null;

        /// <summary>
        /// 解压完毕后执行的方法
        /// </summary>
        public LoadFunctionDele unZipEnd = null;

        /// <summary>
        /// 克隆（浅克隆）
        /// </summary>
        /// <returns>一个新的LoadInfo实例</returns>
        public LoadInfo clone()
        {
            LoadInfo newInfo = new LoadInfo();
            newInfo.path = path;
            newInfo.fileName = fileName;
            newInfo.version = version;
            newInfo.progress = progress;
            newInfo.error = error;
            newInfo.assetBundle = assetBundle;
            newInfo.priority = priority;
            newInfo.www = www;
            newInfo.loadStart = loadStart;
            newInfo.loadProgress = loadProgress;
            newInfo.loadEnd = loadEnd;
            newInfo.loadFail = loadFail;
            newInfo.unZipStart = unZipStart;
            newInfo.unZipEnd = unZipEnd;
            return newInfo;
        }

    }
}
