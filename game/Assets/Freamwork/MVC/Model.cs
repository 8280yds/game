using UnityEngine;

namespace Freamwork.MVC
{
    public class Model<TClass> : MVCObject<TClass>, IModel where TClass : Model<TClass>
    {
        public Model()
            : base()
        {

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
