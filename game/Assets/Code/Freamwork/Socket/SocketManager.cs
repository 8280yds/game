using CLRSharp;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using UnityEngine;

namespace Freamwork
{
    class SocketListenerVO
    {
        public ICLRType clrType;
        public SocketListenerDele fun;

        public SocketListenerVO(ICLRType clrType, SocketListenerDele fun)
        {
            this.clrType = clrType;
            this.fun = fun;
        }
    }

    sealed public class SocketManager
    {
        public static SocketManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new SocketManager();
                }
                return m_instance;
            }
        }
        private static SocketManager m_instance;

        private SocketManager()
        {
            if (m_instance != null)
            {
                throw new Exception("SocketManager是单例，请使用SocketManager.instance来获取其实例！");
            }
            m_instance = this;
            init();
        }

        private Socket socket;
        private Dictionary<int, SocketListenerVO> listenDic;
        private List<byte[]> bytesList;

        private void init()
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAdress = IPAddress.Parse(SocketConstant.IPAddress);
            IPEndPoint ipEndPoint = new IPEndPoint(ipAdress, SocketConstant.IPEndPoint);

            //创建异步连接，在成功后执行connectCallback方法
            IAsyncResult result = socket.BeginConnect(ipEndPoint, null, socket);
            if (!result.AsyncWaitHandle.WaitOne(5000, true))
            {
                close();
                Debug.LogWarning("服务器连接超时……");
            }
            else
            {
                Debug.Log("服务器连接成功……");
                EnterFrame.instance.addEnterFrame(onEnterFrame);

                listenDic = new Dictionary<int, SocketListenerVO>();
                bytesList = new List<byte[]>();

                Thread thread = new Thread(receiveSorket);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        /// <summary>
        /// 在死循环中接受服务端数据
        /// </summary>
        private void receiveSorket()
        {
            while (true)
            {
                if (socket == null || !socket.Connected)
                {
                    break;
                }

                try
                {
                    byte[] bytes = new byte[1024];

                    //Receive方法会一直等待，直到服务端返回数据
                    int len = socket.Receive(bytes);
                    if (len <= 0)
                    {
                        socket.Close();
                        break;
                    }

                    if (bytes.Length <= 10)
                    {
                        Debug.LogWarning("Socket数据包长度小于或等于包头长度……");
                    }
                    else
                    {
                        byte[] bytes2 = new byte[len];
                        Array.Copy(bytes, 0, bytes2, 0, len);
                        bytesList.Add(bytes2);
                    }
                }
                catch (Exception e)
                {
                    socket.Close();
                    Debug.LogError("Socket出错：" + e);
                    break;
                }
            }
        }

        private void onEnterFrame()
        {
            if (socket == null || !socket.Connected)
            {
                close();
            }
            while(bytesList.Count > 0)
            {
                ByteBuffer buff = new ByteBuffer(bytesList[0]);
                bytesList.RemoveAt(0);

                Package package = unpack(buff);
                if (listenDic.ContainsKey(package.protocol))
                {
                    listenDic[package.protocol].fun(package);
                }
            }
        }

        /// <summary>
        /// 向服务器发送请求
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="data">协议数据</param>
        public void request(int protocol, object data)
        {
            if (socket == null || !socket.Connected)
            {
                close();
                return;
            }

            try
            {
                ByteBuffer buff = packet(protocol, data);

                //向服务端异步发送这个字节数组
                IAsyncResult asyncSend = socket.BeginSend(buff.buffer, 0, buff.length, SocketFlags.None, null, socket);
                bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
                if (!success)
                {
                    close();
                    Debug.LogWarning("发送" + protocol + "协议超时……");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Socket出错：" + e);
            }
        }

        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void close()
        {
            if (socket != null)
            {
                if (socket.Connected)
                {
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
                socket = null;
            }
            EnterFrame.instance.removeEnterFrame(onEnterFrame);
        }

        /// <summary>
        /// 添加回调侦听
        /// </summary>
        /// <param name="protocol">协议号</param>
        /// <param name="fun">回调方法</param>
        /// <param name="clrType">数据的CLR类型</param>
        public void addListener(int protocol, SocketListenerDele fun, ICLRType clrType)
        {
            if (!listenDic.ContainsKey(protocol))
            {
                listenDic.Add(protocol, new SocketListenerVO(clrType, fun));
            }
        }

        /// <summary>
        /// 移除回调侦听
        /// </summary>
        /// <param name="protocol">协议号</param>
        public void removeListener(int protocol)
        {
            if (listenDic.ContainsKey(protocol))
            {
                listenDic.Remove(protocol);
            }
        }

        /// <summary>
        /// 封包，时间戳 + 协议号 + 内容长度 + 内容
        /// </summary>
        /// <param name="protocol"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private ByteBuffer packet(int protocol, object data)
        {
            ByteBuffer buff = PackageUtil.clrObjectToByteBuffer(data as CLRSharp_Instance);

            Package package = new Package();
            package.timeStamp = TimeUtil.getTimeStamp();
            package.protocol = protocol;
            package.len = (ushort)buff.length;
            package.data = data;
            Debug.Log("[发送] " + package.toString());

            ByteBuffer buffer = new ByteBuffer();
            buffer.appendInt(package.timeStamp);
            buffer.appendInt(package.protocol);
            buffer.appendUshort(package.len);
            buffer.appendBuffer(buff);
            return buffer;
        }

        /// <summary>
        /// 拆包，时间戳 + 协议号 + 内容长度 + 内容
        /// </summary>
        /// <param name="buff"></param>
        /// <returns></returns>
        private Package unpack(ByteBuffer buff)
        {
            Package package = new Package();
            package.timeStamp = buff.removeInt();
            package.protocol = buff.removeInt();
            package.len = buff.removeUshort();

            if (buff.length < package.len)
            {
                throw new Exception("协议" + package.protocol + 
                    "包体字节长度不对，应是" + package.len + "，但当前是" + buff.length);
            }

            if (listenDic.ContainsKey(package.protocol))
            {
                package.data = PackageUtil.byteBufferToClrObject(ref buff, listenDic[package.protocol].clrType);
                Debug.Log("[接收] " + package.toString());
            }
            else
            {
                Debug.Log("[接收，未处理] " + package.toString());
            }
            return package;
        }

    }
}