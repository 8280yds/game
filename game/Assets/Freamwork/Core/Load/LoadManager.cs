using System;
using System.Collections.Generic;

namespace Freamwork
{
    /// <summary>
    /// 加载管理类，具有排队，优先级，超时检测等功能，
    /// 并能根绝情况确定是否需要到网络加载
    /// </summary>
    sealed public class LoadManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private LoadManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public LoadManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new LoadManager();
                }
                return m_instance;
            }
        }

        private LoadManager()
        {
            if (m_instance != null)
            {
                throw new Exception("LoadManager是单例，请使用LoadManager.instance来获取其实例！");
            }
            m_instance = this;
            init();
        }

        /// <summary>
        /// 已加载资源的缓存
        /// </summary>
        private Dictionary<string, LoadInfo> cacheDic;

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            
        }

        //=====================================================================

        public void load()
        {




        }





    }
}