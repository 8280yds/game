using System;
using System.Collections;
using System.Collections.Generic;

namespace Freamwork.MVC
{
    sealed public class MVCCharge
    {
        static private MVCCharge m_instance;

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

        private Hashtable m_instanceHashtable;
        private List<string> m_listenerIdList;
        private Hashtable m_listenerHashtable;

        private bool m_listenerDisabled;
        private bool m_doLater;
        private List<string> m_disabledIdList;

        private void init()
        {
            m_instanceHashtable = new Hashtable();
            m_listenerIdList = new List<string>();
            m_listenerHashtable = new Hashtable();

            m_listenerDisabled = false;
            m_doLater = false;
            m_disabledIdList = new List<string>();
        }

        public void clear()
        {
            m_instanceHashtable.Clear();
            m_listenerIdList.Clear();
            m_listenerHashtable.Clear();

            m_listenerDisabled = false;
            m_doLater = false;
            m_disabledIdList.Clear();
        }

        //********************************* 发送命令 *************************************
        public void sendCommand<TCommand, TParam>(TParam param = default(TParam)) where TCommand : Command, new()
        {
            TCommand command = new TCommand();
            command.doExecute<TParam>(param);
        }

        //********************************* 更新管理 *************************************
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

        public void dispatch(string id)
        {
            if (!m_listenerIdList.Contains(id))
            {
                m_listenerIdList.Add(id);
            }
        }

        public void addDisabledId(string id)
        {
            if(!m_disabledIdList.Contains(id))
            {
                m_disabledIdList.Add(id);
            }
        }

        public void removeDisabledId(string id)
        {
            if (m_disabledIdList.Contains(id))
            {
                m_disabledIdList.Remove(id);
            }
        }

        public void addListener(string id, ListenerDelegate dele)
        {
            if (!m_listenerHashtable.Contains(id))
            {
                m_listenerHashtable.Add(id, dele);
            }
            else
            {
                ListenerDelegate delegates = m_listenerHashtable[id] as ListenerDelegate;
                delegates += dele;
            }
        }

        public void removeListener(string id, ListenerDelegate dele)
        {
            if (!m_listenerHashtable.Contains(id))
            {
                return;
            }
            ListenerDelegate delegates = m_listenerHashtable[id] as ListenerDelegate;
            delegates -= dele;

            if (delegates == null)
            {
                m_listenerHashtable.Remove(id);
            }
        }

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
                if (!m_disabledIdList.Contains(id) && m_listenerHashtable.Contains(id))
                {
                    ListenerDelegate dele = m_listenerHashtable[id] as ListenerDelegate;
                    dele();
                    m_listenerIdList.Remove(id);
                }
            }
        }

        //********************************* 单例管理 *************************************
        public T getModel<T>() where T : IModel, new()
        {
            return getInstance<T>();
        }

        public bool hasModel<T>() where T : IModel
        {
            return hasInstance<T>();
        }

        public bool delModel<T>() where T : IModel
        {
            return delInstance<T>();
        }

        public T getView<T>() where T : IView, new()
        {
            return getInstance<T>();
        }

        public bool hasView<T>() where T : IView
        {
            return hasInstance<T>();
        }

        public bool delView<T>() where T : IView
        {
            return delInstance<T>();
        }

        public T getInstance<T>() where T : IMVCObject, new()
        {
            string fullName = typeof(T).FullName;
            if (!m_instanceHashtable.ContainsKey(fullName))
            {
                m_instanceHashtable.Add(fullName, new T());
            }
            return (T)m_instanceHashtable[fullName];
        }

        public bool hasInstance<T>() where T : IMVCObject
        {
            string fullName = typeof(T).FullName;
            return m_instanceHashtable.ContainsKey(fullName);
        }

        public bool delInstance<T>() where T : IMVCObject
        {
            string fullName = typeof(T).FullName;
            if (m_instanceHashtable.ContainsKey(fullName))
            {
                m_instanceHashtable.Remove(fullName);
                return true;
            }
            return false;
        }

        public bool hasInstance(Type type)
        {
            string fullName = type.FullName;
            return m_instanceHashtable.ContainsKey(fullName);
        }

        public bool delInstance(Type type)
        {
            string fullName = type.FullName;
            if (m_instanceHashtable.ContainsKey(fullName))
            {
                m_instanceHashtable.Remove(fullName);
                return true;
            }
            return false;
        }

    }
}

