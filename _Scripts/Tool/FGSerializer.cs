using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace FGame
{
    /// <summary>
    /// 自定义消息序列化类  by : heyang  2017/12/11
    /// </summary>
    public class FGSerializer
    {
        #region 序列化

        public static byte[] Serialize(Object ob)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                Serialize(stream, ob);
                return stream.ToArray();
            }
        }

        public static void Serialize(MemoryStream stream, object ob)
        {
            if (stream == null || ob == null)
            {
                return;
            }

            FGWrite write = new FGWrite(stream);
            Serialize(write, ob, ob.GetType());
        }

        /// <summary>
        /// 核心序列化代码
        /// </summary>
        private static void Serialize(FGWrite write, object ob, Type type)
        {
            // 基本类型
            if (type.IsPrimitive)
            {
                if (type.Equals(typeof(int))) write.Write((int)ob);
                else if (type.Equals(typeof(float))) write.Write((float)ob);
                else if (type.Equals(typeof(byte))) write.Write((byte)ob);
                else if (type.Equals(typeof(long))) write.Write((long)ob);
                else HelperTool.LogError("FGSerializer.Deserialize : 序列化类型不支持");
                return;
            }

            // 字符串
            if (type.Equals(typeof(string)))
            {
                string str = (string)ob;
                write.Write(str.Length);
                write.WriteString(str);
                return;
            }

            // 数组
            if (type.IsArray)
            {
                Array arr = (Array)ob;
                if (arr != null)
                {
                    if (arr.Length > 0)
                    {
                        write.Write(arr.Length);
                        Type at = arr.GetValue(0).GetType();
                        if (at.Equals(typeof(byte)) || at.Equals(typeof(sbyte)))
                        {
                            write.Write((byte[])ob);
                        }
                        else
                        {
                            for (int i = 0; i < arr.Length; i++)
                            {
                                Serialize(write, arr.GetValue(i), at);
                            }
                        }
                    }
                }
                return;
            }

            // 泛型List
            if (type.IsGenericType)
            {
                Type listType = type.GetGenericArguments()[0];
                Type[] types = type.GetInterfaces();
                if (Array.IndexOf(types, typeof(IEnumerable)) > -1)
                {
                    IEnumerable en = ob as IEnumerable;
                    var enumerator = en.GetEnumerator();
                    int length = 0;
                    while (enumerator.MoveNext())
                    {
                        length++;
                    }
                    write.Write(length);
                    enumerator = en.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        Serialize(write, enumerator.Current, listType);
                    }
                }
                return;
            }

            // 类
            FGReflection reflection = new FGReflection(type);
            foreach (FGSerializeInfo item in reflection.fieldList)
            {
                FieldInfo info = item.info;
                Type t = info.FieldType;
                
                Serialize(write, info.GetValue(ob), t);
            }
        }

        #endregion

        #region 反序列化

        public static T Deserialize<T>(byte[] buffer)
        {
            return Deserialize<T>(buffer, 0, buffer.Length);
        }

        public static T Deserialize<T>(byte[] buffer, int start, int size)
        {
            //写入内存
            using (MemoryStream ms = new MemoryStream(buffer, start, size))
            {          
                return Deserialize<T>(ms);
            }
        }

        public static T Deserialize<T>(MemoryStream stream)
        {
            FGReader reader = new FGReader(stream);
            return (T)Deserialize(reader, typeof(T));
        }

        /// <summary>
        /// 反序列化基元类型数据
        /// </summary>
        private static object DeserializePrimitive(FGReader reader, Type type)
        {
            if (type.Equals(typeof(int)))
            {
                return reader.ReadInt();
            }
            else if (type.Equals(typeof(byte)))
            {
                return reader.ReadByte();
            }
            else if (type.Equals(typeof(sbyte)))
            {
                return reader.ReadSByte();
            }
            else if (type.Equals(typeof(float)))
            {
                return reader.ReadFloat();
            }
            else if (type.Equals(typeof(long)))
            {
                return reader.ReadLong();
            }
            else if (type.Equals(typeof(uint)))
            {
                return reader.ReadUint();
            }
            return null;
        }

        private static object Deserialize(FGReader reader, Type type)
        {
            // 基本类型
            if (type.IsPrimitive)
            {
                return DeserializePrimitive(reader, type);
            }
            else if(type.Equals(typeof(string)))
            {
                return reader.ReadString();
            }

            /* 自定义特性类反序列化 */
            //Dictionary<string, int> lenDict = new Dictionary<string, int>(); //保存 数组长度标志 和 数组 的关系
            object ob = Activator.CreateInstance(type, true);     
            FGReflection reflection = new FGReflection(type);
            foreach (var item in reflection.fieldList)
            {
                FieldInfo info = item.info;
                Type t = info.FieldType;

                if (t.Equals(typeof(string)))
                {
                    int length = reader.ReadInt();
                    if (length > 0)
                    {
                        info.SetValue(ob, reader.ReadString(length));
                    }
                    else
                    {
                        info.SetValue(ob, reader.ReadString());
                    }
                }
                else
                {
                    // 数组
                    if (t.IsArray)
                    {
                        //根据长度记录，序列化
                        int length = reader.ReadInt();
                        if (length > 0)
                        {
                            Type at = t.GetElementType();
                            if (at.Equals(typeof(byte)) || at.Equals(typeof(sbyte)))
                            {
                                byte[] array = reader.ReadBytes(length);
                                info.SetValue(ob, array);
                            }
                            else
                            {
                                Array array = Array.CreateInstance(t.GetElementType(), length);
                                for (int i = 0; i < length; i++)
                                {
                                    array.SetValue(Deserialize(reader, at), i);
                                }
                                info.SetValue(ob, array);
                            }
                        }
                        else
                        {
                            HelperTool.LogError("FGSerializer.Deserialize : 反序列化" + type.Name + "失败");
                            continue;
                        }
                    }
                    // 泛型List
                    else if (t.IsGenericType)
                    {
                        Type listType = t.GetGenericArguments()[0];  
                        Type[] types = t.GetInterfaces();
                        if (Array.IndexOf(types, typeof(IEnumerable)) > -1)
                        {
                            int length = reader.ReadInt();
                            Type lsType = typeof(List<>);
                            var makeme = lsType.MakeGenericType(listType);
                            object o = Activator.CreateInstance(makeme);
                            MethodInfo method = o.GetType().GetMethod("Add");
                            object[] insts = new object[length];
                            for (int i = 0; i < length; i++)
                            {
                                Object inst = Deserialize(reader, listType);
                                method.Invoke(o, new object[] { inst });
                            }
                            info.SetValue(ob, o);
                        }
                    }
                    else
                    {
                        //非数组结构直接序列化,如果字段为数组的长度标志，记录下来
                        info.SetValue(ob, DeserializePrimitive(reader, t));
                    }

                    
                }
            }   
            return ob;
        }

        public static void Read()
        {

        }

        #endregion
    }

    /// <summary>
    /// 字节流格式化读取类     by: heyang    2017/12/11
    /// </summary>
    public class FGReader
    {
        private MemoryStream _dataStream;

        //内部数据缓冲区,重复利用
        private byte[] _buffer = new byte[8];

        public FGReader(MemoryStream data)
        {
            _dataStream = data;
        }

        /// <summary>
        /// 将数据写入到缓冲区
        /// </summary>
        private void ReadToBuffer(int start, int length)
        {
            try
            {
                _dataStream.Read(_buffer, start, length);
            }
            catch (Exception)
            {
                HelperTool.LogError("FGSerializer.ReadToBuffer : 内存流数据读取错误");
                throw;
            }
        }

        public int ReadInt()
        {
            ReadToBuffer(0, 4);
            return BitConverter.ToInt32(_buffer, 0);
        }

        public float ReadFloat()
        {
            ReadToBuffer(0, 4);
            return BitConverter.ToSingle(_buffer, 4);
        }

        public byte ReadByte()
        {
            return (byte)_dataStream.ReadByte();
        }

        public byte[] ReadBytes(int len)
        {
            byte[] bytes = new byte[len];
            for (int i = 0; i < len; i++)
            {
                bytes[i] = (byte)_dataStream.ReadByte();
            }
            return bytes;
        }

        public sbyte ReadSByte()
        {
            return (sbyte)_dataStream.ReadByte();
        }

        public uint ReadUint()
        {
            ReadToBuffer(0, 4);
            return BitConverter.ToUInt32(_buffer, 0);
        }

        public long ReadLong()
        {
            ReadToBuffer(0, 8);
            return BitConverter.ToInt64(_buffer, 0);
        }

        public double ReadDouble()
        {
            ReadToBuffer(0, 8);
            return BitConverter.ToDouble(_buffer, 8);
        }

        public string ReadString()
        {
            byte[] bytes = new byte[_dataStream.Length - _dataStream.Position];
            _dataStream.Read(bytes, 0, bytes.Length);
            return Encoding.UTF8.GetString(bytes);
        }

        public string ReadString(int length)
        {
            byte[] bytes = new byte[length];
            _dataStream.Read(bytes, 0, length);
            return Encoding.UTF8.GetString(bytes);
        }
    }

    /// <summary>
    /// 字节流格式化写入类     by: heyang    2017/12/13
    /// </summary>
    public class FGWrite
    {
        private MemoryStream _dataStream;

        public FGWrite(MemoryStream stream)
        {
            _dataStream = stream;
        }

        public void Write(int value)
        {
            _dataStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Write(long value)
        {
            _dataStream.Write(BitConverter.GetBytes(value), 0, 8);
        }

        public void Write(float value)
        {
            _dataStream.Write(BitConverter.GetBytes(value), 0, 4);
        }

        public void Write(byte value)
        {
            _dataStream.WriteByte(value);
        }

        public void Write(byte[] values)
        {
            _dataStream.Write(values, 0, values.Length);
        }

        public void WriteString(string str, int length)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(str);
            byte[] buffer = new byte[length];
            bytes.CopyTo(buffer, 0);
            _dataStream.Write(buffer, 0, buffer.Length);
        }

        public void WriteString(string str)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(str);
            _dataStream.Write(buffer, 0, buffer.Length);
        }
    }
    /// <summary>
    /// 序列化信息类 : 用于 FGReflection 
    /// </summary>
    public struct FGSerializeInfo
    {
        public FieldInfo info;
        public int order;

        /// <summary>
        /// 如果是字符串表示 字符串长度， 如果是数组 表示数组长度
        /// </summary>
        public int length;

        /// <summary>
        /// 是否为数组长度变量
        /// </summary>
        public bool isLenOfArray;

        public string arrayName;
    }

    /// <summary>
    /// 反序列化反射类        by: heyang   2017/12/11
    /// </summary>
    public class FGReflection
    {
        private const BindingFlags _flag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public List<FGSerializeInfo> fieldList = new List<FGSerializeInfo>();

        public FGReflection(Type type)
        {
            //获取类中所有字段
            FieldInfo[] infos = type.GetFields(_flag);

            FGSerializeInfo sinfo = new FGSerializeInfo();
            for (int i = 0; i < infos.Length; i++)
            {
                //获取所有描述该字段的某个特性
                var attrs = infos[i].GetCustomAttributes(typeof(MemberAttribute), false);

                //var enumerator = attrs.GetEnumerator();
                //while (enumerator.MoveNext())
                //{
                //    enumerator.Current
                //}
                foreach (MemberAttribute attr in attrs)
                {
                    sinfo.info = infos[i];
                    //sinfo.order = attr.Order;
                    sinfo.order = attr.Length + GetOffsetOrder(infos[i], type);
                    sinfo.length = attr.Length;
                    sinfo.isLenOfArray = attr.IsLenOfArray;
                    sinfo.arrayName = attr.ArrayName;
                    fieldList.Add(sinfo);
                    break;
                }
            }

            fieldList.Sort(CompareByOrder);
        }

        private int CompareByOrder(FGSerializeInfo n1, FGSerializeInfo n2)
        {
            if (n1.order == n2.order)
            {
                return 0;
            }
            else
            {
                if (n1.order > n2.order)
                {
                    return 1;
                }
                else
                {
                    return -1;
                }
            }
        }

        private const int MAX_HIERARCHY = 30;
        private const int MAX_FIELDS = 200;
        /// <summary>
        /// 判断 当前特性成员变量 是属于父类定义的变量 还是自身定义的变量
        /// 如果自身的返回0，父类的依次向上判断，直到Object，返回一个 小于等于0 的数
        /// </summary>
        private int GetOffsetOrder(FieldInfo field, Type type)
        {
            int index = 0;
            //field.DeclaringType 表示声明这个字段的类的类型
            if (field.DeclaringType != type)
            {
                Type t = field.DeclaringType;
                while (t != typeof(System.Object))
                {
                    index++;
                    t = t.BaseType;
                }
                return MAX_FIELDS * (index - MAX_HIERARCHY);   //为了避免父类定义的字段 Member的order 与 自身的order冲突 
            }
            else
            {
                return 0;
            }
        }
    }


    /// <summary>
    /// 类成员变量序列化特性   by: heyang   2017/12/11
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class MemberAttribute : Attribute
    {
        public MemberAttribute(int order)
        {
            Order = order;
            ArrayName = string.Empty;
            Length = 0;
            IsLenOfArray = false;
        }

        public MemberAttribute(int order, int length)
        {
            ArrayName = string.Empty;
            Order = order;
            Length = length;
            IsLenOfArray = false;
        }

        public MemberAttribute(int order, string array)
        {
            Order = order;
            ArrayName = array;
            Length = 0;
            IsLenOfArray = true;
        }

        public int Order;
        public string ArrayName;

        /// <summary>
        /// 如果是字符串表示 字符串长度， 如果是数组 表示数组长度
        /// </summary>
        public int Length;

        /// <summary>
        /// 是否为数组长度变量
        /// </summary>
        public bool IsLenOfArray;
    }
}

