using UnityEngine;

namespace Freamwork.MVC
{
    public class Model : MVCObject, IModel
    {
        public Model()
            : base()
        {

        }

        protected void dispatch(string id)
        {
            MVCCharge.instance.dispatch(id);
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
