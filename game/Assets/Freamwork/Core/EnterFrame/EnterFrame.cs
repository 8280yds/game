using System;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// EnterFrame的Delegate
    /// </summary>
    public delegate void EnterFrameDelegate();

    /// <summary>
    /// EnterFrame是用来控制帧事件的 
    /// 【注意】一定要在帧频方法所在的实例释放之前中移除其侦听,否则容易造成内存泄露
    /// </summary>
    sealed public class EnterFrame
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private EnterFrame m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public EnterFrame instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new EnterFrame();
                }
                return m_instance;
            }
        }

        private EnterFrame()
        {
            if (m_instance != null)
            {
                throw new Exception("EnterFrame是单例，请使用EnterFrame.instance来获取其实例！");
            }

            m_instance = this;
            init();
        }

        private EnterFrameDelegate m_dele;
        private bool isEnterFrame;
        private GameObject enterFrameGameObject;

        private void init()
        {
            isEnterFrame = false;
        }

        /// <summary>
        /// 清除所有帧频事件
        /// </summary>
        public void clear()
        {
            if (isEnterFrame)
            {
                m_dele = null;
                stopEnterframe();
            }
        }

        //============================================================
        /// <summary>
        /// 添加一个帧频事件
        /// </summary>
        /// <param name="dele">帧频方法</param>
        public void addEnterFrame(EnterFrameDelegate dele)
        {
            if (isEnterFrame)
            {
                m_dele += dele;
            }
            else
            {
                m_dele = dele;
                beginEnterframe();
            }
        }

        /// <summary>
        /// 移除一个帧频事件
        /// </summary>
        /// <param name="dele">帧频方法</param>
        public void removeEnterFrame(EnterFrameDelegate dele)
        {
            if (isEnterFrame)
            {
                m_dele -= dele;
                if (m_dele == null)
                {
                    stopEnterframe();
                }
            }
        }

        /// <summary>
        /// 启动帧频
        /// </summary>
        private void beginEnterframe()
        {
            isEnterFrame = true;
            enterFrameGameObject = new GameObject("EnterFrame");
            enterFrameGameObject.AddComponent<EnterFrameComponent>();
        }

        /// <summary>
        /// 停止帧频
        /// </summary>
        private void stopEnterframe()
        {
            isEnterFrame = false;
            GameObject.Destroy(enterFrameGameObject);
        }

        /// <summary>
        /// 执行帧频事件
        /// </summary>
        public void doEnterFrame()
        {
            m_dele();
        }

        ///// <summary>
        ///// 报告错误释放
        ///// </summary>
        //public void destroyError()
        //{
        //    if(isEnterFrame)
        //    {
        //        throw new Exception("EnterFrame的GameObject非正常释放！");
        //    }
        //}

    }
}