using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载中相关的触发方法
    /// </summary>
    public delegate void LoadFunctionDele(LoadInfo loadData);

    /// <summary>
    /// 加载时事件方法的集合类
    /// </summary>
    public class LoadInfo
    {
        //======================属性===========================
        /// <summary>
        /// 加载优先级
        /// </summary>
        public LoadPriority priority = LoadPriority.two;

        /// <summary>
        /// 加载路径
        /// </summary>
        public string path = "/";

        /// <summary>
        /// 加载文件的名称(带后缀)
        /// </summary>
        public string fileName;

        /// <summary>
        /// 加载物件的版本号
        /// </summary>
        public int version = 0;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float progress = 0f;

        /// <summary>
        /// 加载出来的数据
        /// </summary>
        public AssetBundle assetBundle = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error = null;

        /// <summary>
        /// WWW
        /// </summary>
        public WWW www = null;

        /// <summary>
        /// 全名（路径+文件名）
        /// </summary>
        public string fullName
        {
            get
            {
                return path + fileName;
            }
        }

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
        /// <returns>一个新的LoadInfo实例</returns>
        public LoadInfo clone()
        {
            LoadInfo newInfo = new LoadInfo();
            //事件中必然需要返回的数据
            newInfo.path = path;
            newInfo.fileName = fileName;
            newInfo.version = version;

            //事件中可能需要返回的数据
            newInfo.progress = progress;
            newInfo.error = error;
            newInfo.assetBundle = assetBundle;

            //事件中不需要返回的数据
            newInfo.priority = priority;
            newInfo.www = www;
            newInfo.loadStart = loadStart;
            newInfo.loadProgress = loadProgress;
            newInfo.loadEnd = loadEnd;
            newInfo.loadFail = loadFail;
            return newInfo;
        }

    }
}
