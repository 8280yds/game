using System;
using System.Collections;
using System.Collections.Generic;

namespace Freamwork.MVC
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
        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="param">需要传递的数据</param>
        public void sendCommand<TCommand>(object param = null) where TCommand : Command, new()
        {
            TCommand command = new TCommand();
            command.doExecute(param);
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
            if (!m_listenerHashtable.Contains(id))
            {
                m_listenerHashtable.Add(id, dele);
            }
            else
            {
                ListenerDelegate delegates = m_listenerHashtable[id] as ListenerDelegate;
                delegates += dele;
                m_listenerHashtable[id] = delegates;
            }
        }

        /// <summary>
        /// 移除更新的侦听
        /// </summary>
        /// <param name="id">更新的名称</param>
        /// <param name="dele">更新的方法</param>
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
            else
            {
                m_listenerHashtable[id] = delegates;
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
                if (!m_disabledIdList.Contains(id) && m_listenerHashtable.Contains(id))
                {
                    ListenerDelegate dele = m_listenerHashtable[id] as ListenerDelegate;
                    dele();
                    m_listenerIdList.Remove(id);
                }
            }
        }

        //********************************* 单例管理 *************************************
        /// <summary>
        /// 获取Model实例，如果实例不存在将会创建
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <returns>IModel实例</returns>
        public T getModel<T>() where T : IModel, new()
        {
            return getInstance<T>();
        }

        /// <summary>
        /// 是否存在Model实例
        /// </summary>
        /// <typeparam name="T">Model类型</typeparam>
        /// <returns>bool</returns>
        public bool hasModel<T>() where T : IModel
        {
            return hasInstance<T>();
        }

        /// <summary>
        /// 删除存储的Model实例
        /// </summary>
        /// <typeparam name="T">Model的类型</typeparam>
        /// <returns>bool</returns>
        public bool delModel<T>() where T : IModel
        {
            return delInstance<T>();
        }

        /// <summary>
        /// 获取View实例，如果实例不存在将会创建
        /// </summary>
        /// <typeparam name="T">View类型</typeparam>
        /// <returns>IView实例</returns>
        public T getView<T>() where T : IView, new()
        {
            return getInstance<T>();
        }

        /// <summary>
        /// 是否存在View实例
        /// </summary>
        /// <typeparam name="T">View类型</typeparam>
        /// <returns>bool</returns>
        public bool hasView<T>() where T : IView
        {
            return hasInstance<T>();
        }

        /// <summary>
        /// 删除存储的View实例
        /// </summary>
        /// <typeparam name="T">View的类型</typeparam>
        /// <returns>bool</returns>
        public bool delView<T>() where T : IView
        {
            return delInstance<T>();
        }

        /// <summary>
        /// 获取MVCObject实例，如果实例不存在将会创建
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>IMVCObject实例</returns>
        public T getInstance<T>() where T : IMVCObject, new()
        {
            string fullName = typeof(T).FullName;
            if (!m_instanceHashtable.ContainsKey(fullName))
            {
                m_instanceHashtable.Add(fullName, new T());
            }
            return (T)m_instanceHashtable[fullName];
        }

        /// <summary>
        /// 是否存在实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>bool</returns>
        public bool hasInstance<T>() where T : IMVCObject
        {
            string fullName = typeof(T).FullName;
            return m_instanceHashtable.ContainsKey(fullName);
        }

        /// <summary>
        /// 删除存储的实例
        /// </summary>
        /// <typeparam name="T">类型</typeparam>
        /// <returns>bool</returns>
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

        /// <summary>
        /// 获取MVCObject实例，如果实例不存在将会创建
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>IMVCObject实例</returns>
        public IMVCObject getInstance(Type type)
        {
            string fullName = type.FullName;
            if (!type.IsAssignableFrom(typeof(IMVCObject)))
            {
                throw new Exception(fullName + "并未实现" + typeof(IMVCObject).FullName +
                    "接口，无法通过MVCCharge.instance.getInstance方法存储和获取其实例");
            }
            if (!m_instanceHashtable.ContainsKey(fullName))
            {
                m_instanceHashtable.Add(fullName, Activator.CreateInstance(type));
            }
            return m_instanceHashtable[fullName] as IMVCObject;
        }

        /// <summary>
        /// 是否存在实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>bool</returns>
        public bool hasInstance(Type type)
        {
            string fullName = type.FullName;
            return m_instanceHashtable.ContainsKey(fullName);
        }

        /// <summary>
        /// 删除存储的实例
        /// </summary>
        /// <param name="type">类型</param>
        /// <returns>bool</returns>
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

        /// <summary>
        /// 存储实例，为那些并非通过MVCCharge.instance.getInstance()方法创建的实例实现手动存储，
        /// 主要是为了解决那些在编辑器中拖拽上去的View的继承类无法通过MVCCharge管理的问题
        /// </summary>
        /// <param name="instance">实例</param>
        internal void saveInstance(IMVCObject instance)
        {
            string fullName = instance.GetType().FullName;
            if (!m_instanceHashtable.ContainsKey(fullName))
            {
                m_instanceHashtable.Add(fullName, instance);
            }
            else
            {
                throw new Exception(fullName + "的实例已经存在，无法再次存储");
            }
        }

    }
}

