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
        private List<string> updateViewIdList;
        private Hashtable delegateHashtable;

        private bool m_updateViewDisabled;
        private List<string> updateViewDisabledIdList;

        private void init()
        {
            m_instanceHashtable = new Hashtable();
            updateViewIdList = new List<string>();
            delegateHashtable = new Hashtable();

            m_updateViewDisabled = false;
            updateViewDisabledIdList = new List<string>();
        }

        public void clear()
        {
            m_instanceHashtable.Clear();
            updateViewIdList.Clear();
            delegateHashtable.Clear();

            m_updateViewDisabled = false;
            updateViewDisabledIdList.Clear();
        }

        //********************************* 发送命令 *************************************
        public void sendCommand<TCommand, TParam>(TParam param = default(TParam)) where TCommand : Command, new()
        {
            TCommand command = new TCommand();
            command.doExecute<TParam>(param);
        }

        //********************************* 更新管理 *************************************
        public bool updateViewDisabled
        {
            get
            {
                return m_updateViewDisabled;
            }
            set
            {
                m_updateViewDisabled = value;
            }
        }

        public void addUpdateViewId(string id)
        {
            if (!updateViewIdList.Contains(id))
            {
                updateViewIdList.Add(id);
            }
        }

        public void removeUpdateViewId(string id)
        {
            if (updateViewIdList.Contains(id))
            {
                updateViewIdList.Remove(id);
            }
        }

        public void addUpdateViewDisabledId(string id)
        {
            if(!updateViewDisabledIdList.Contains(id))
            {
                updateViewDisabledIdList.Add(id);
            }
        }

        public void removeUpdateViewDisabledId(string id)
        {
            if (updateViewDisabledIdList.Contains(id))
            {
                updateViewDisabledIdList.Remove(id);
            }
        }

        public void addUpdateViewDelegate(string updateId, UpdateViewDelegate dele)
        {
            if(!delegateHashtable.Contains(updateId))
            {
                delegateHashtable.Add(updateId, dele);
            }
            else
            {
                UpdateViewDelegate delegates = delegateHashtable[updateId] as UpdateViewDelegate;
                delegates += dele;
            }
        }

        public void removeUpdateViewDelegate(string updateId, UpdateViewDelegate dele)
        {
            if (!delegateHashtable.Contains(updateId))
            {
                return;
            }
            UpdateViewDelegate delegates = delegateHashtable[updateId] as UpdateViewDelegate;
            delegates -= dele;

            if (delegates == null)
            {
                delegateHashtable.Remove(updateId);
            }
        }

        public void doUpdateViewDelegate()
        {
            if (m_updateViewDisabled)
            {
                return;
            }
            for (int i = 0, len = updateViewIdList.Count; i < len; i++)
            {
                string id = updateViewIdList[i];
                if (!updateViewDisabledIdList.Contains(id) && delegateHashtable.Contains(id))
                {
                    UpdateViewDelegate dele = delegateHashtable[id] as UpdateViewDelegate;
                    dele();
                    updateViewIdList.Remove(id);
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

