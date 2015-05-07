using UnityEngine;

namespace Freamwork.MVC
{
    public class Command : MVCObject, ICommand
    {
        internal void doExecute<TParam>(TParam param = default(TParam))
        {
            execute<TParam>(param);
            dispose();
        }

        virtual public void execute<TParam>(TParam param = default(TParam))
        {

        }

        sealed override public void dispose()
        {
            if (disposed)
            {
                Debug.Log(this.GetType().FullName + "对象重复释放！");
                return;
            }
            disposed = true;
        }

    }
}
