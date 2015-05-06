﻿using System;
using UnityEngine;

namespace Freamwork.MVC
{
    public class MVCObject<TClass> : IMVCObject where TClass : MVCObject<TClass>
    {
        protected bool disposed = false;

        public MVCObject()
        {
            init();
        }

        virtual protected void init()
        {

        }

        public void sendCommand<TCommand, TParam>(TParam param) where TCommand : Command<TCommand>, new()
        {
            if (disposed)
            {
                throw new Exception(this.GetType().FullName + "对象已经销毁，sendCommand失败");
            }
            MVCCharge.instance.sendCommand<TCommand, TParam>(param);
        }

        virtual public void dispose()
        {
            if (disposed)
            {
                Debug.Log(this.GetType().FullName + "对象重复释放！");
                return;
            }
            disposed = true;

            MVCCharge mvcCharge = MVCCharge.instance;
            if (mvcCharge.hasInstance<TClass>())
            {
                mvcCharge.delInstance<TClass>();
            }
        }
    }
}