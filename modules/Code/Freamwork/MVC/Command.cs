using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// Command将作为具体的命令类的基类
    /// </summary>
    public abstract class Command : MVCObject, ICommand
    {
        /// <summary>
        /// 执行命令
        /// </summary>
        /// <param name="param">传递过来的参数</param>
        abstract public void execute(object param);

        /// <summary>
        /// 释放
        /// </summary>
        override public void dispose()
        {
            if (disposed)
            {
                Debug.Log(getCLRType.FullName + "对象重复释放！");
                return;
            }
            disposed = true;
        }

    }
}
