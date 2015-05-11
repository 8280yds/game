using System;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork.MVC
{
    /// <summary>
    /// 更新侦听的Delegate
    /// </summary>
    public delegate void ListenerDelegate();

    /// <summary>
    /// View是显示物体（特别是UI）的基类，继承它可以实现由MVCCharge托管实例，
    /// 并且可以通过侦听来实现自动更新
    /// </summary>
    public abstract class View : MonoBehaviour, IView
    {
        /// <summary>
        /// 是否已经执行了释放
        /// </summary>
        protected bool disposed = false;

        /// <summary>
        /// 是否已经执行了销毁
        /// </summary>
        protected bool isDestroyed = false;

        /// <summary>
        /// 为释放侦听暂存的数据
        /// </summary>
        private Dictionary<string, ListenerDelegate> listenerDic;

        public MVCCharge mvcCharge
        {
            get
            {
                return MVCCharge.instance;
            }
        }

        /// <summary>
        /// Awake事件，在此处将实例交给MVCCharge托管
        /// </summary>
        virtual protected void Awake()
        {
            mvcCharge.saveInstance(this);
        }

        /// <summary>
        /// Update事件
        /// </summary>
        virtual protected void Update()
        {
            mvcCharge.doLater = true;
        }

        /// <summary>
        /// LateUpdate事件，在此处会具体执行更新的方法
        /// 一般重写时主要带上base.LateUpdate()
        /// </summary>
        virtual protected void LateUpdate()
        {
            mvcCharge.doListenerDelegate();
        }

        /// <summary>
        /// OnEnable事件，一般在此处添加所有更新的侦听
        /// </summary>
        virtual protected void OnEnable()
        {
            addListeners();
        }

        /// <summary>
        /// OnDisable事件，一般在此处移除所有更新的侦听
        /// 默认会自动移除，一般重写时要注意带上base.OnDisable()，否则不会自动移除侦听
        /// </summary>
        virtual protected void OnDisable()
        {
            removeListeners();
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <typeparam name="TCommand">命令类型</typeparam>
        /// <param name="param">需要传递的数据</param>
        public void sendCommand<TCommand>(object param = null) where TCommand : Command, new()
        {
            if (disposed)
            {
                throw new Exception(this.GetType().FullName + "对象已经销毁，sendCommand失败");
            }
            mvcCharge.sendCommand<TCommand>(param);
        }

        /// <summary>
        /// 添加更新的侦听
        /// </summary>
        virtual protected void addListeners()
        {

        }

        /// <summary>
        /// 添加单个更新的侦听，一般在addListeners方法中使用
        /// </summary>
        /// <param name="id">更新的名称</param>
        /// <param name="dele">更新的委托</param>
        protected void addListener(string id, ListenerDelegate dele)
        {
            if (listenerDic == null)
            {
                listenerDic = new Dictionary<string, ListenerDelegate>();
            }

            if (listenerDic.ContainsKey(id))
            {
                listenerDic[id] += dele;
            }
            else
            {
                listenerDic.Add(id, dele);
                mvcCharge.addListener(id, dele);
            }
        }

        /// <summary>
        /// 移除更新的侦听
        /// </summary>
        private void removeListeners()
        {
            if (listenerDic != null)
            {
                foreach (string key in listenerDic.Keys)
                {
                    mvcCharge.removeListener(key, listenerDic[key]);
                }
                listenerDic = null;
            }
        }

        /// <summary>
        /// 销毁，会调用dispose方法，销毁View实例
        /// </summary>
        virtual protected void OnDestroy()
        {
            isDestroyed = true;
            dispose();
        }

        /// <summary>
        /// 释放，调用此方法会彻底销毁View的实例
        /// </summary>
        virtual public void dispose()
        {
            if (!isDestroyed)
            {
                Destroy(this);
            }
            if (disposed)
            {
                Debug.Log(this.GetType().FullName + "对象重复释放！");
                return;
            }
            disposed = true;

            mvcCharge.delInstance(this.GetType());
        }

    }
}
