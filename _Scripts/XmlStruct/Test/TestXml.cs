using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml;
using System.Xml.Serialization;

namespace FGame
{
    /// <summary>
    /// 测试用xml序列化类
    /// </summary>
    [XmlRootAttribute("Test")]
    public class TestXml
    {
        [XmlAttribute("Id")]
        public int id;
        [XmlAttribute("Name")]
        public string name;

        [XmlElementAttribute("Item")]
        public List<TestItem> items;
        //public void Serialize(CSerialize)
        //{

        //}
    }

    [XmlRootAttribute("Item")]
    public class TestItem
    {
        [XmlAttribute("Index")]
        public int index;
    }
}

