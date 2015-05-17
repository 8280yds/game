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
        //======================属性和字段===========================
        /// <summary>
        /// 加载优先级
        /// </summary>
        public LoadPriority priority
        {
            internal get
            {
                return m_priority;
            }
            set
            {
                m_priority = value;
            }
        }
        private LoadPriority m_priority = LoadPriority.two;

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
        public float progress
        {
            get
            {
                return m_progress;
            }
            internal set
            {
                m_progress = value;
            }
        }
        private float m_progress = 0f;

        /// <summary>
        /// 加载出来的数据
        /// </summary>
        public object assetBundle
        {
            get
            {
                return m_assetBundle;
            }
            internal set
            {
                m_assetBundle = value;
            }
        }
        private object m_assetBundle = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error
        {
            get
            {
                return m_error;
            }
            internal set
            {
                m_error = value;
            }
        }
        private string m_error = null;

        /// <summary>
        /// WWW
        /// </summary>
        internal WWW www;

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

        //======================方法和参数===========================
        /// <summary>
        /// 加载开始前执行的方法
        /// </summary>
        public LoadFunctionDele loadStart
        {
            internal get
            {
                return m_loadStart;
            }
            set
            {
                m_loadStart = value;
            }
        }
        private LoadFunctionDele m_loadStart = null;

        /// <summary>
        /// 加载开始后且在结束前每帧执行的方法
        /// </summary>
        public LoadFunctionDele loadProgress
        {
            internal get
            {
                return m_loadProgress;
            }
            set
            {
                m_loadProgress = value;
            }
        }
        private LoadFunctionDele m_loadProgress = null;

        /// <summary>
        /// 加载结束后执行的方法
        /// </summary>
        public LoadFunctionDele loadEnd
        {
            internal get
            {
                return m_loadEnd;
            }
            set
            {
                m_loadEnd = value;
            }
        }
        private LoadFunctionDele m_loadEnd = null;

        /// <summary>
        /// 加载失败后执行的方法
        /// </summary>
        public LoadFunctionDele loadFail
        {
            internal get
            {
                return m_loadFail;
            }
            set
            {
                m_loadFail = value;
            }
        }
        private LoadFunctionDele m_loadFail = null;

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
            newInfo.assetBundle = assetBundle;
            newInfo.error = error;

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
