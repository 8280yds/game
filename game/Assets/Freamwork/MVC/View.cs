using System;
using System.Collections;
using UnityEngine;

namespace Freamwork.MVC
{
    public delegate void ListenerDelegate();

    public class View : MonoBehaviour, IView
    {
        protected bool disposed = false;

        protected bool isDestroyed = false;

        private Hashtable listenerHashtable;

        virtual protected void Update()
        {
            MVCCharge.instance.doLater = true;
        }

        virtual protected void LateUpdate()
        {
            MVCCharge.instance.doListenerDelegate();
        }

        virtual protected void OnEnable()
        {
            addListeners();
        }

        virtual protected void OnDisable()
        {
            removeListeners();
        }

        public void sendCommand<TCommand, TParam>(TParam param = default(TParam)) where TCommand : Command, new()
        {
            if (disposed)
            {
                throw new Exception(this.GetType().FullName + "对象已经销毁，sendCommand失败");
            }
            MVCCharge.instance.sendCommand<TCommand, TParam>(param);
        }

        virtual protected void addListeners()
        {

        }

        protected void addListener(string id, ListenerDelegate dele)
        {
            if (listenerHashtable == null)
            {
                listenerHashtable = new Hashtable();
            }

            if (listenerHashtable.Contains(id))
            {
                ListenerDelegate listenerDele = listenerHashtable[id] as ListenerDelegate;
                listenerDele += dele;
            }
            else
            {
                listenerHashtable.Add(id, dele);
                MVCCharge.instance.addListener(id, dele);
            }
        }

        private void removeListeners()
        {
            if (listenerHashtable != null)
            {
                foreach (string key in listenerHashtable.Keys)
                {
                    MVCCharge.instance.removeListener(key, listenerHashtable[key] as ListenerDelegate);
                }
                listenerHashtable = null;
            }
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
