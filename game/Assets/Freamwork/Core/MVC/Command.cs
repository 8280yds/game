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
        /// <param name="param">携带的数据</param>
        abstract public void execute<TParam>(TParam param);

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
