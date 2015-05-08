using System;
using UnityEngine;

namespace Freamwork.MVC
{
    /// <summary>
    /// MVCObject是Model、View、Command等MVC类的基类，继承它可以实现由MVCCharge来托管实例
    /// </summary>
    public class MVCObject : IMVCObject
    {
        /// <summary>
        /// 是否已经执行了释放
        /// </summary>
        protected bool disposed = false;

        public MVCObject()
        {
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        virtual protected void init()
        {

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
            MVCCharge.instance.sendCommand<TCommand>(param);
        }

        /// <summary>
        /// 释放
        /// </summary>
        virtual public void dispose()
        {
            if (disposed)
            {
                Debug.Log(this.GetType().FullName + "对象重复释放！");
                return;
            }
            disposed = true;
            MVCCharge.instance.delInstance(this.GetType());
        }
    }
}
