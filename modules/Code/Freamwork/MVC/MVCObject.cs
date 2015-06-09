using CLRSharp;
using System;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// MVCObject是Model、View、Command等MVC类的基类，继承它可以实现由MVCCharge来托管实例
    /// </summary>
    public abstract class MVCObject : IMVCObject
    {
        /// <summary>
        /// 是否已经执行了释放
        /// </summary>
        protected bool disposed = false;

        public MVCObject()
        {

        }

        protected MVCCharge mvcCharge
        {
            get
            {
                return MVCCharge.instance;
            }
        }

        public ICLRType getCLRType
        {
            get
            {
                return CLRSharpManager.instance.getCLRTypeByInst(this);
            }
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
        /// 释放
        /// </summary>
        virtual public void dispose()
        {
            if (disposed)
            {
                Debug.Log(getCLRType.FullName + "对象重复释放！");
                return;
            }
            disposed = true;
            mvcCharge.delInstance(getCLRType);
        }
    }
}
