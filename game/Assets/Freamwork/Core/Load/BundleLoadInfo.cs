using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载Bundle信息的集合类
    /// </summary>
    public class BundleLoadInfo : LoadData
    {
        //======================属性===========================
        /// <summary>
        /// 加载优先级
        /// </summary>
        public LoadPriority priority = LoadPriority.two;

        /// <summary>
        /// 加载类型
        /// </summary>
        public LoadType loadType = LoadType.local;

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
        /// 克隆（浅克隆）
        /// </summary>
        /// <returns>一个新的BundleLoadInfo实例</returns>
        public BundleLoadInfo clone()
        {
            BundleLoadInfo newInfo = new BundleLoadInfo();
            newInfo.fullName = fullName;

            newInfo.loadProgressNum = loadProgressNum;
            newInfo.unZipProgressNum = unZipProgressNum;
            newInfo.error = error;
            newInfo.assetBundle = assetBundle;

            newInfo.priority = priority;
            newInfo.loadType = loadType;
            newInfo.www = www;
            newInfo.assets = assets;

            newInfo.loadStart = loadStart;
            newInfo.loadProgress = loadProgress;
            newInfo.loadEnd = loadEnd;
            newInfo.loadFail = loadFail;
            return newInfo;
        }

    }
}
