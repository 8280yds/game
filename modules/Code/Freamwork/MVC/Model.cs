using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// Model将作为具体模块里的Model的基类，将用来处理和存储数据，
    /// 继承它将可以实现通过dispatch()方法发送更新请求
    /// </summary>
    public abstract class Model : MVCObject, IModel
    {
        public Model()
        {
            
        }

        /// <summary>
        /// 发送更新请求
        /// </summary>
        /// <param name="id">更新的名称</param>
        protected void dispatch(string id)
        {
            mvcCharge.dispatch(id);
        }

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
            clearAll();

            base.dispose();
        }

        /// <summary>
        /// 清除数据
        /// </summary>
        virtual public void clearAll()
        {

        }

    }
}
