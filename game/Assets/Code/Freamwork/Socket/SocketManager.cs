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
            IAsyncResult result = socket.BeginConnect(ipEndPoint, new AsyncCallback(connectCallback), socket);
            //判断连接超时
            if (!result.AsyncWaitHandle.WaitOne(5000, true))
            {
                closed();
                Debug.LogError("服务器连接超时……");
            }
            else
            {
                //socket连接成功，开启线程接受服务端数据。
                listenDic = new Dictionary<int, SocketListenerVO>();
                bytesList = new List<byte[]>();

                EnterFrame.instance.addEnterFrame(onEnterFrame);

                Thread thread = new Thread(receiveSorket);
                thread.IsBackground = true;
                thread.Start();
            }
        }

        /// <summary>
        /// 连接成功
        /// </summary>
        /// <param name="asyncConnect"></param>
        private void connectCallback(IAsyncResult asyncConnect)
        {
            Debug.Log("服务器连接成功……");
        }

        /// <summary>
        /// 在死循环中接受服务端数据
        /// </summary>
        private void receiveSorket()
        {
            while (true)
            {
                if (!socket.Connected)
                {
                    //与服务器断开连接跳出循环
                    Debug.LogError("服务器已断开……");
                    socket.Close();
                    EnterFrame.instance.addEnterFrame(onEnterFrame);
                    break;
                }

                try
                {
                    byte[] bytes = new byte[4096];

                    //Receive方法会一直等待，直到服务端返回数据
                    int i = socket.Receive(bytes);
                    if (i <= 0)
                    {
                        socket.Close();
                        EnterFrame.instance.addEnterFrame(onEnterFrame);
                        break;
                    }

                    //此处根据实际需要判断
                    if (bytes.Length <= 2)
                    {
                        Debug.LogError("Socket数据包长度小于或等于包头长度……");
                    }
                    else
                    {
                        byte[] bytes2 = new byte[i];
                        Array.Copy(bytes, bytes2, i);
                        bytesList.Add(bytes2);
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError("Socket出错：" + e);
                    socket.Close();
                    EnterFrame.instance.addEnterFrame(onEnterFrame);
                    break;
                }
            }
        }

        private void onEnterFrame()
        {
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
            if (!socket.Connected)
            {
                socket.Close();
                return;
            }

            try
            {
                ByteBuffer buff = packet(protocol, data);

                //向服务端异步发送这个字节数组
                //IAsyncResult asyncSend = socket.BeginSend(buff.buffer, 0, buff.length, SocketFlags.None, 
                //    new AsyncCallback(sendCallback), socket);
                IAsyncResult asyncSend = socket.BeginSend(buff.buffer, 0, buff.length, SocketFlags.None, null, socket);
                //监测超时
                bool success = asyncSend.AsyncWaitHandle.WaitOne(5000, true);
                if (!success)
                {
                    socket.Close();
                    Debug.LogError("发送请求超时……");
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Socket出错：" + e);
            }
        }
        //private void sendCallback(IAsyncResult asyncSend)
        //{
        //    Debug.Log("发送请求成功……");
        //}

        /// <summary>
        /// 关闭Socket
        /// </summary>
        public void closed()
        {
            if (socket != null && socket.Connected)
            {
                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
            }
            socket = null;
            EnterFrame.instance.addEnterFrame(onEnterFrame);
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
            //try
            //{
                Package package = new Package();
                package.timeStamp = buff.removeInt();
                package.protocol = buff.removeInt();
                package.len = buff.removeUshort();
                package.data = PackageUtil.byteBufferToClrObject(ref buff, listenDic[package.protocol].clrType);

                Debug.Log("[接收] " + package.toString());
                return package;
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError("协议解析出错:" + e);
            //}
            //return null;
        }

    }
}