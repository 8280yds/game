using UnityEngine;

namespace Freamwork.MVC
{
    public class Model : MVCObject, IModel
    {
        public Model()
            : base()
        {

        }

        protected void updateViewById(string updateId)
        {
            MVCCharge.instance.addUpdateViewId(updateId);
        }

        protected void removeUpdateViewById(string updateId)
        {
            MVCCharge.instance.addUpdateViewId(updateId);
        }

        override public void dispose()
        {
            if (disposed)
            {
                Debug.Log(this.GetType().FullName + "对象重复释放！");
                return;
            }
            disposed = true;
            clearAll();

            base.dispose();
        }

        virtual public void clearAll()
        {

        }

    }
}
