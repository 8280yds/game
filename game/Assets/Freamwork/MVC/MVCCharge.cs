using System;
using System.Collections;

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

        private void init()
        {
            m_instanceHashtable = new Hashtable();
        }

        public void clear()
        {
            m_instanceHashtable.Clear();
        }

        //********************************* 发送命令 *************************************
        public void sendCommand<TCommand, TParam>(TParam param) where TCommand : Command<TCommand>, new()
        {
            TCommand command = new TCommand();
            command.doExecute<TParam>(param);
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

    }
}

