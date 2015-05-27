using System;
using System.Collections.Generic;

namespace Freamwork
{
    /// <summary>
    /// PanelView管理类
    /// </summary>
    sealed public class PanelViewManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private PanelViewManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public PanelViewManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new PanelViewManager();
                }
                return m_instance;
            }
        }

        private PanelViewManager()
        {
            if (m_instance != null)
            {
                throw new Exception("PanelViewManager是单例，请使用PanelViewManager.instance来获取其实例！");
            }

            m_instance = this;
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            
        }

        /// <summary>
        /// 清除所有帧频事件
        /// </summary>
        public void clear()
        {
            
        }

        //============================================================
        /// <summary>
        /// 当前已经打开的面板的列表，返回的是浅克隆的列表
        /// </summary>
        public List<PanelView> showPanelViewList
        {
            get
            {
                return new List<PanelView>(m_showPanelViewList);
            }
        }
        private List<PanelView> m_showPanelViewList;








    }
    
}
