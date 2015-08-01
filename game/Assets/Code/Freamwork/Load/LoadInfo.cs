using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载信息的集合类
    /// </summary>
    public class LoadInfo : LoadData
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
        /// request
        /// </summary>
        public AssetBundleRequest request = null;

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
        /// 解压开始后且在结束前每帧执行的方法
        /// </summary>
        public LoadFunctionDele unZipProgress = null;

        /// <summary>
        /// 解压完毕后执行的方法
        /// </summary>
        public LoadFunctionDele unZipEnd = null;

        /// <summary>
        /// 解压完毕后执行的方法
        /// </summary>
        public LoadFunctionDele unZipFail = null;

        /// <summary>
        /// 克隆（浅克隆）
        /// </summary>
        /// <returns>一个新的LoadInfo实例</returns>
        public LoadInfo clone()
        {
            LoadInfo newInfo = new LoadInfo();
            newInfo.fullName = fullName;

            newInfo.loadProgressNum = loadProgressNum;
            newInfo.unZipProgressNum = unZipProgressNum;
            newInfo.error = error;
            newInfo.assetBundle = assetBundle;

            newInfo.loadType = loadType;
            newInfo.priority = priority;
            newInfo.request = request;
            newInfo.assets = assets;

            newInfo.loadStart = loadStart;
            newInfo.loadProgress = loadProgress;
            newInfo.loadEnd = loadEnd;
            newInfo.loadFail = loadFail;
            newInfo.unZipStart = unZipStart;
            newInfo.unZipProgress = unZipProgress;
            newInfo.unZipEnd = unZipEnd;
            newInfo.unZipFail = unZipFail;
            return newInfo;
        }

        /// <summary>
        /// 根据本实例获取一个BundleLoadInfo实例
        /// </summary>
        /// <returns></returns>
        public BundleLoadInfo getBundleLoadInfo()
        {
            BundleLoadInfo newInfo = new BundleLoadInfo();
            newInfo.fullName = fullName;

            newInfo.loadProgressNum = loadProgressNum;
            newInfo.unZipProgressNum = unZipProgressNum;
            newInfo.error = error;
            newInfo.assetBundle = assetBundle;

            newInfo.priority = priority;
            newInfo.loadType = loadType;
            newInfo.assets = assets;

            newInfo.loadStart = loadStart;
            newInfo.loadProgress = loadProgress;
            newInfo.loadEnd = loadEnd;
            newInfo.loadFail = loadFail;
            return newInfo;
        }

    }
}
