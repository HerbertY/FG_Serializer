using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


namespace FGame
{
    /// <summary>
    /// 基础通信类      by : heyang    2017/12/9
    /// </summary>
    public class FGBaseNet
    {
        #region 成员变量
        private const int MAXBUFFLEN = 0x20000;                 //缓冲区最大长度
        private const int HEADLEN = 4;                          //头长度

        private Socket _clientSocket;
        private IPEndPoint _serverInfo;

        private string _ip = "119.23.16.144";
        private int _port = 9409;

        private ulong _msgTatalLength = 0;

        /// <summary>
        /// 每次从网络层接收的数据
        /// </summary>
        private byte[] _recvBuffer = new byte[64 * 1024 - 1];

        /// <summary>
        /// 获取数据的缓冲区
        /// </summary>
        private byte[] _recvCache = new byte[MAXBUFFLEN];
        private int _cacheLength = 0;                           //当前缓冲区中实际数据长度

        private byte[] _dataBuffer = new byte[2 * MAXBUFFLEN];  //解密后的数据
        private int _dataLength = 0;

        #endregion

        #region 内部方法
       

        /// <summary>
        /// 连接回调
        /// </summary>
        private void ConnectCallback(IAsyncResult ar)
        {
            try
            {
                _clientSocket.EndConnect(ar);
                if (IsConnect())
                {
                    //开始接受消息
                    ReceiveAsync();
                }
                else
                {
                    HelperTool.LogError("FGBaseNet.ConnectCallback : Socket 连接失败...");
                }
            }
            catch (Exception e)
            {
                HelperTool.LogError("FGBaseNet.ConnectCallback : " + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// 接收消息回调
        /// </summary>
        private void ReceiveCallback(IAsyncResult ar)
        {
            try
            {
                if (_clientSocket == null)
                {
                    HelperTool.LogError("FGBaseNet.ReceiveCallback : Socket == null");
                    return;
                }
                if (!_clientSocket.Connected)
                {
                    HelperTool.LogError("FGBaseNet.ReceiveCallback : Socket未连接");   
                    return;
                }

                int length = _clientSocket.EndReceive(ar);
                if (length > 0)
                {
                    _msgTatalLength += (ulong)length;
                    //读取字节流
                    ReadData(length);

                    //继续异步接收
                    _clientSocket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clientSocket);
                }
                else
                {
                    //关闭连接
                    //进入发送消息和接受消息
                    //关闭套接字，不允许重用
                    Quit();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 异步接收消息
        /// </summary>
        private void ReceiveAsync()
        {
            try
            {
                _clientSocket.BeginReceive(_recvBuffer, 0, _recvBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), _clientSocket);
            }
            catch (Exception e)
            {
                HelperTool.LogError("FGBaseNet.ReceiveAsync : " + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// 读取数据
        /// </summary>
        private void ReadData(int length)
        {
            if (_cacheLength + length > _recvCache.Length)
            {
                HelperTool.LogError("FGBaseNet.ReadData : 缓冲区溢出");
                return;
            }
            Array.Copy(_recvBuffer, 0, _recvCache, _cacheLength, length);  //把数据写入缓冲区
            _cacheLength += length;
            if (_cacheLength < 8)
            {
                HelperTool.LogError("FGBaseNet.ReadData : 缓冲区数据不足8字节，则继续等待接收");
                return;
            }

            //TODO :解密数据到_dataBuffer
            Array.Copy(_recvCache, 0, _dataBuffer, _dataLength, _cacheLength);
            _dataLength += _cacheLength;

            if (_cacheLength - _dataLength > 0)
            {
                //数据未接收完，下次再接收
                return;
            }
            else
            {
                _cacheLength = 0;
            }

            //测试 : 输出消息
            Debug.Log(Encoding.UTF8.GetString(_dataBuffer, 0, _dataLength));
        }

        /// <summary>
        /// 退出 : 断开连接等
        /// </summary>
        private void Quit()
        {

        }

        #endregion

        #region 外部接口

        /// <summary>
        /// 连接服务器
        /// </summary>
        public void Connect(string ip, int port)
        {
            _ip = ip;
            _port = port;

            Connect();
        }
        public void Connect()
        {
            try
            {
                //转换ip地址
                IPAddress ipa;
                if (IPAddress.TryParse(_ip, out ipa))
                {
                    _serverInfo = new IPEndPoint(ipa, _port);
                }
                else
                {
                    //转换失败，根据ip获取相关ip地址或ip地址的dns服务器
                    IPHostEntry iph = Dns.GetHostEntry(_ip);
                    foreach (var item in iph.AddressList)
                    {
                        _serverInfo = new IPEndPoint(item, _port);
                        break;
                    }
                }

                _clientSocket = new Socket(_serverInfo.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
                _clientSocket.BeginConnect(_serverInfo, new System.AsyncCallback(ConnectCallback), _clientSocket);
            }
            catch (Exception e)
            {
                //断开连接
                Quit();
                //打印错误日志
                HelperTool.LogError("FGBaseNet.Connect : " + e.ToString());
                throw;
            }
        }

        /// <summary>
        /// 是否已连接
        /// </summary>
        private bool IsConnect()
        {
            if (_clientSocket != null)
            {
                return _clientSocket.Connected;
            }
            return false;
        }
        #endregion
    }
}

