using CLRSharp;
using System.Collections.Generic;

namespace Freamwork
{
    /// <summary>
    /// 通信方面的基类，继承它可以实现与后端的通信
    /// </summary>
    public abstract class Service : Model
    {
        private List<int> protocolList;

        public Service()
        {
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        virtual protected void init()
        {
            protocolList = new List<int>();
            addlisteners();
        }

        /// <summary>
        /// 向服务器发送请求
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="data">数据</param>
        protected void request(int protocol, ProtocolData data)
		{
            SocketManager.instance.request(protocol, data);
		}

        /// <summary>
        /// 添加回调侦听
        /// </summary>
        protected abstract void addlisteners();

        /// <summary>
        /// 添加回调侦听
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="fun">回调方法</param>
        /// <param name="clrType">数据的CLR类型</param>
        protected void addListener(int protocol, SocketListenerDele fun, ICLRType clrType)
		{
            protocolList.Add(protocol);
            SocketManager.instance.addListener(protocol, fun, clrType);
		}

        /// <summary>
        /// 移除回调侦听
        /// </summary>
        /// <param name="protocol">协议号</param>
        protected void removeListener(int protocol)
        {
            if (protocolList.Contains(protocol))
            {
                protocolList.Remove(protocol);
            }
            SocketManager.instance.removeListener(protocol);
        }

        public override void clearAll()
        {
            foreach(int protocol in protocolList)
            {
                removeListener(protocol);
            }
            base.clearAll();
        }

    }
}