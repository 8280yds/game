using System;
using UnityEngine;

namespace Freamwork.MVC
{
    public delegate void UpdateViewDelegate();

    public class View : MonoBehaviour, IView
    {
        protected bool disposed = false;

        protected bool isDestroyed = false;

        virtual protected void Update()
        {
            MVCCharge.instance.updateViewDoLater = true;
        }

        virtual protected void LateUpdate()
        {
            MVCCharge.instance.doUpdateViewDelegate();
        }

        virtual protected void OnEnable()
        {
            addUpdateViewDelegate();
        }

        virtual protected void OnDisable()
        {
            removeUpdateViewDelegate();
        }

        public void sendCommand<TCommand, TParam>(TParam param = default(TParam)) where TCommand : Command, new()
        {
            if (disposed)
            {
                throw new Exception(this.GetType().FullName + "对象已经销毁，sendCommand失败");
            }
            MVCCharge.instance.sendCommand<TCommand, TParam>(param);
        }

        virtual protected void addUpdateViewDelegate()
        {

        }

        virtual protected void removeUpdateViewDelegate()
        {

        }

        virtual protected void OnDestroy()
        {
            isDestroyed = true;
            dispose();
        }

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

            MVCCharge.instance.delInstance(this.GetType());
        }

    }
}
