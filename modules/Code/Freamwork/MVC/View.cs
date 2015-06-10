using CLRSharp;
using System;
using System.Collections.Generic;

namespace Freamwork
{
    /// <summary>
    /// View是显示物体（特别是UI）的基类，继承它可以实现由MVCCharge托管实例，
    /// 并且可以通过侦听来实现自动更新
    /// </summary>
    public abstract class View : GMB, IView
    {
        /// <summary>
        /// 为释放侦听暂存的数据
        /// </summary>
        private Dictionary<string, ListenerDelegate> listenerDic;

        protected MVCCharge mvcCharge
        {
            get
            {
                return MVCCharge.instance;
            }
        }

        /// <summary>
        /// Update事件
        /// </summary>
        override protected void Update()
        {
            mvcCharge.doLater = true;
            base.Update();
        }

        /// <summary>
        /// LateUpdate事件，在此处会具体执行更新的方法
        /// 一般重写时主要带上base.LateUpdate()
        /// </summary>
        override protected void LateUpdate()
        {
            mvcCharge.doListenerDelegate();
            base.LateUpdate();
        }

        /// <summary>
        /// OnEnable事件，一般在此处添加所有更新的侦听
        /// </summary>
        override protected void OnEnable()
        {
            addListeners();
            base.OnEnable();
        }

        /// <summary>
        /// OnDisable事件，一般在此处移除所有更新的侦听
        /// 默认会自动移除，一般重写时要注意带上base.OnDisable()，否则不会自动移除侦听
        /// </summary>
        override protected void OnDisable()
        {
            base.OnDisable();
            removeListeners();
        }

        /// <summary>
        /// 发送命令
        /// </summary>
        /// <param name="type">L#类型，必须是ICLRType的实现者</param>
        /// <param name="param">命令携带的参数</param>
        public void sendCommand(ICLRType clrType, object param)
        {
            if (disposed)
            {
                throw new Exception(getCLRType.FullName + "对象已经销毁，sendCommand失败");
            }
            mvcCharge.sendCommand(clrType, param);
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
        /// 释放，调用此方法会彻底销毁View的实例
        /// </summary>
        override public void dispose()
        {
            base.dispose();
            mvcCharge.delInstance(getCLRType);
        }

    }
}