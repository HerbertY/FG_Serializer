using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using UnityEngine;

namespace FGame
{
    /// <summary>
    /// 游戏管理器         by: heyang   2017/12/9
    /// </summary>
    public class FGameApp : MonoBehaviour
    {
        private FGBaseNet _net; 

        void Start()
        {
            //_net = new FGBaseNet();
            //_net.Connect("119.23.16.144", 9409);
            //TestXml xml = FGXmlDocument.Deserialize<TestXml>("test.xml");
            //FGSerializer.Deserialize<TestConfig>(null);
            //HelperTool.Log("连接服务器成功！");
            //TestConfig a = new TestConfig();
            //a.id = 11;
            //a.len = 5;
            //a.name = "heyang";
            //a.data = new int[5];
            //a.data[0] = 0;
            //a.data[1] = 1;

            //byte[] bufferA;
            //using (MemoryStream stream = new MemoryStream())
            //{
            //    stream.Write(BitConverter.GetBytes(a.id), 0, 4);
            //    stream.Write(BitConverter.GetBytes(a.len), 0, 4);
            //    byte[] bytes = new byte[10];
            //    Encoding.UTF8.GetBytes(a.name).CopyTo(bytes,0);
            //    stream.Write(bytes, 0, bytes.Length);
            //    stream.Write(BitConverter.GetBytes(a.data[0]), 0, 4);
            //    stream.Write(BitConverter.GetBytes(a.data[1]), 0, 4);
            //    stream.Write(BitConverter.GetBytes(a.data[2]), 0, 4);
            //    stream.Write(BitConverter.GetBytes(a.data[3]), 0, 4);
            //    stream.Write(BitConverter.GetBytes(a.data[4]), 0, 4);
            //    bufferA = stream.ToArray();
            //}

            //byte[] bufferB = FGSerializer.Serialize(a);

            //TestConfig msg = FGSerializer.Deserialize<TestConfig>(bufferB);
            //Debug.Log(msg.ToString());
            //for (int i = 0; i < msg.data.Length; i++)
            //{
            //    Debug.Log("data["+i+"]"+" = "+msg.data[i]);
            //}

            //模拟服务器发送数据
            MSG_C2S_TEST msg = new MSG_C2S_TEST();
            msg.id = 10001;
            msg.name = "asdas";
            msg.gNum = 5;
            msg.qNum = 10;
            msg.array = new byte[msg.qNum * msg.gNum];
            msg.listInt.Add(10);
            msg.listInt.Add(100);
            msg.listInt.Add(145646);

            byte[] bufferC;
            using (MemoryStream stream = new MemoryStream())
            {
                //任务完成情况
                int index = 0;
                for (int i = 0; i < msg.gNum; i++)
                {
                    for (int j = 0; j < msg.qNum; j++)
                    {
                        msg.array[index++] = (byte)index;
                    }
                }

                FGSerializer.Serialize(stream, msg);
                bufferC = stream.ToArray();
            }

            //客户端收到消息
            MemoryStream stream1 = new MemoryStream(bufferC);
            MSG_C2S_BASE basemsg = FGSerializer.Deserialize<MSG_C2S_BASE>(stream1);
            stream1.Close();
            if (basemsg.type == CONST.MSG_C2S_TEST)
            {
                stream1 = new MemoryStream(bufferC);
                MSG_C2S_TEST testmsg = FGSerializer.Deserialize<MSG_C2S_TEST>(stream1);
                int id = testmsg.id;
                //string name = testmsg.name;
                Debug.Log("msg id = " + id + "   msg name = " + testmsg.name);
                if (basemsg.type == CONST.MSG_C2S_TEST)
                {
                    int index = 0;
                    for (int i = 0; i < testmsg.gNum; i++)
                    {
                        for (int j = 0; j < testmsg.qNum; j++)
                        {
                            Debug.Log("gid:" + i + " qid:" + j + " == " + testmsg.array[index++]);
                        }
                    }
                    foreach (var item in testmsg.listInt)
                    {
                        Debug.Log("Int = " + item);
                    }
                }
            }
            
               
           
            //using (MemoryStream stream = new MemoryStream(bufferC, 0, ))
            //{

            //}

            //TestConfig newOb = FGSerializer.Deserialize<TestConfig>(buffer);
            //if (newOb != null)
            //{
            //    Debug.Log(newOb.id + "  " + newOb.name + "  " + newOb.len);
            //}
        }

    }
}

