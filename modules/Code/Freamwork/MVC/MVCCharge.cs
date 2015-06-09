using CLRSharp;
using System;
using System.Collections.Generic;

namespace Freamwork
{
    /// <summary>
    /// MVCCharge是单例，请使用MVCCharge.instance来获取其实例，
    /// MVCCharge总管MVC的所有实例，被其托管的实例都不是严格意义上的单例，但是可以确保通过MVCCharge获取的实例都是同一个，
    /// MVCCharge总管更新，所有更新方法都将在LateUpdate事件中执行，而且确保每帧每个方法至多执行一次，
    /// </summary>
    sealed public class MVCCharge
    {
        /// <summary>
        /// MVCCharge的实例
        /// </summary>
        static private MVCCharge m_instance;

        /// <summary>
        /// 获取MVCCharge的实例
        /// </summary>
        static public MVCCharge instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new MVCCharge();
                }
                return m_instance;
            }
        }

        private MVCCharge()
        {
            if (m_instance != null)
            {
                throw new Exception("MVCCharge是单例，请使用MVCCharge.instance来获取其实例！");
            }

            m_instance = this;
            init();
        }

        private Dictionary<string, object> m_instanceDic;
        private List<string> m_listenerIdList;
        private Dictionary<string, ListenerDelegate> m_listenerDic;

        private bool m_listenerDisabled;
        private bool m_doLater;
        private List<string> m_disabledIdList;

        private void init()
        {
            m_instanceDic = new Dictionary<string, object>();
            m_listenerIdList = new List<string>();
            m_listenerDic = new Dictionary<string, ListenerDelegate>();

            m_listenerDisabled = false;
            m_doLater = false;
            m_disabledIdList = new List<string>();
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void clear()
        {
            List<string> keys = new List<string>(m_instanceDic.Keys);
            for (int i = 0, len = keys.Count; i < len; i++)
            {
                delInstance(CLRSharpManager.instance.getCLRType(keys[i]));
            }
            m_instanceDic.Clear();
            m_listenerIdList.Clear();
            m_listenerDic.Clear();

            m_listenerDisabled = false;
            m_doLater = false;
            m_disabledIdList.Clear();
        }

        //********************************* 更新管理 *************************************
        /// <summary>
        /// 是否在之后的LateUpdate事件中执行所有更新
        /// </summary>
        public bool doLater
        {
            get
            {
                return m_doLater;
            }
            set
            {
                m_doLater = value;
            }
        }

        /// <summary>
        /// 是否禁止所有更新
        /// </summary>
        public bool listenerDisabled
        {
            get
            {
                return m_listenerDisabled;
            }
            set
            {
                m_listenerDisabled = value;
            }
        }

        /// <summary>
        /// 发送更新，其具体更新要在下一个LateUpdate事件中才会执行
        /// </summary>
        /// <param name="id">更新的名称</param>
        public void dispatch(string id)
        {
            if (!m_listenerIdList.Contains(id))
            {
                m_listenerIdList.Add(id);
            }
        }

        /// <summary>
        /// 添加禁止更新
        /// </summary>
        /// <param name="id">更新的名称</param>
        public void addDisabledId(string id)
        {
            if(!m_disabledIdList.Contains(id))
            {
                m_disabledIdList.Add(id);
            }
        }

        /// <summary>
        /// 取消禁止更新
        /// </summary>
        /// <param name="id">更新的名称</param>
        public void removeDisabledId(string id)
        {
            if (m_disabledIdList.Contains(id))
            {
                m_disabledIdList.Remove(id);
            }
        }

        /// <summary>
        /// 添加更新的侦听
        /// </summary>
        /// <param name="id">更新的名称</param>
        /// <param name="dele">更新的方法</param>
        public void addListener(string id, ListenerDelegate dele)
        {
            if (m_listenerDic.ContainsKey(id))
            {
                m_listenerDic[id] += dele;
            }
            else
            {
                m_listenerDic.Add(id, dele);
            }
        }

        /// <summary>
        /// 移除更新的侦听
        /// </summary>
        /// <param name="id">更新的名称</param>
        /// <param name="dele">更新的方法</param>
        public void removeListener(string id, ListenerDelegate dele)
        {
            if (!m_listenerDic.ContainsKey(id))
            {
                return;
            }
            ListenerDelegate delegates = m_listenerDic[id] - dele;
            if (delegates == null)
            {
                m_listenerDic.Remove(id);
            }
            else
            {
                m_listenerDic[id] = delegates;
            }
        }

        /// <summary>
        /// 执行更新
        /// </summary>
        public void doListenerDelegate()
        {
            if (m_listenerDisabled || !doLater)
            {
                return;
            }
            doLater = false;

            for (int i = 0, len = m_listenerIdList.Count; i < len; i++)
            {
                string id = m_listenerIdList[i];
                if (!m_disabledIdList.Contains(id) && m_listenerDic.ContainsKey(id))
                {
                    m_listenerDic[id]();
                    m_listenerIdList.Remove(id);
                    //?????????????????????????????????????????????????????????????????????????
                    //
                    //?????????????????????????????????????????????????????????????????????????
                }
            }
        }

        //********************************* 发送命令 *************************************
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="type">L#类型</param>
        /// <param name="param">命令携带的参数</param>
        public void sendCommand(ICLRType clrType, object param)
        {
            CLRSharpManager mana =  CLRSharpManager.instance;
            object command = mana.creatCLRInstance(clrType);
            MethodParamList paramTypes = mana.getParamTypeList(typeof(object));
            object[] paramList = new object[] { param };
            mana.Invoke(clrType, "execute", command, paramTypes, paramList);
        }

        //********************************* 单例管理 *************************************
        /// <summary>
        /// 获取MVCObject实例，如果实例不存在将会创建
        /// </summary>
        /// <param name="type">L#类型</param>
        /// <returns>IMVCObject</returns>
        public object getInstance(ICLRType clrType)
        {
            string fullName = clrType.FullName;
            if (!m_instanceDic.ContainsKey(fullName))
            {
                List<ICLRType> list = CLRSharpManager.instance.getInterfaces(clrType as Type_Common_CLRSharp);
                if (!list.Contains(typeof(IMVCObject) as ICLRType))
                {
                    throw new Exception(fullName + "并非IMVCObject的实现者，无法通过MVCCharge来管理");
                }
                object mvcObject = CLRSharpManager.instance.creatCLRInstance(clrType);
                m_instanceDic.Add(fullName, mvcObject);
            }
            return m_instanceDic[fullName];
        }

        /// <summary>
        /// 是否存在实例
        /// </summary>
        /// <param name="type">L#类型</param>
        /// <returns>bool</returns>
        public bool hasInstance(ICLRType clrType)
        {
            return m_instanceDic.ContainsKey(clrType.FullName);
        }

        /// <summary>
        /// 删除存储的实例
        /// </summary>
        /// <param name="type">L#类型</param>
        /// <returns>bool</returns>
        public bool delInstance(ICLRType clrType)
        {
            if (m_instanceDic.ContainsKey(clrType.FullName))
            {
                object mvcObject = m_instanceDic[clrType.FullName];
                m_instanceDic.Remove(clrType.FullName);
                CLRSharpManager.instance.Invoke(clrType, "dispose", mvcObject);
                return true;
            }
            return false;
        }

    }
}
