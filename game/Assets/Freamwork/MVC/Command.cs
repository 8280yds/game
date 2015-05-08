using UnityEngine;

namespace Freamwork.MVC
{
    /// <summary>
    /// Command将作为具体的命令类的基类
    /// </summary>
    public class Command : MVCObject, ICommand
    {
        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="param">携带的数据</param>
        internal void doExecute(object param = null)
        {
            execute(param);
            dispose();
        }

        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="param">携带的数据</param>
        virtual public void execute(object param)
        {

        }

        /// <summary>
        /// 释放
        /// </summary>
        override public void dispose()
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
