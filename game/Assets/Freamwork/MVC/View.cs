using System;
using UnityEngine;

namespace Freamwork.MVC
{
    public delegate void UpdateViewDelegate();

    public class View : MonoBehaviour, IView
    {
        protected bool disposed = false;






        public void sendCommand<TCommand, TParam>(TParam param = default(TParam)) where TCommand : Command, new()
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
            Type type = this.GetType();
            if (mvcCharge.hasInstance(type))
            {
                mvcCharge.delInstance(type);
            }
        }

    }
}
