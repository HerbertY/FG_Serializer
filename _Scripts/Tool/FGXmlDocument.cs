using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;


namespace FGame
{
    /// <summary>
    /// XML 序列化工具
    /// </summary>
    public class FGXmlDocument
    {
        public static T Deserialize<T>(string xmlName)
        {
            if (xmlName == string.Empty)
            {
                HelperTool.LogError("FGXmlDocument.Deserialize : xml文件名为空");
                return default(T);
            }

            string xmlPath = Application.dataPath + @"\_Xmls\" + xmlName;
            if (!File.Exists(xmlPath))
            {
                HelperTool.LogError("FGXmlDocument.Deserialize : " + xmlName +"路径不正确");
                return default(T);
            }
            try
            {
                using (StreamReader r = new StreamReader(xmlPath))
                {
                    XmlSerializer xs = new XmlSerializer(typeof(TestXml));
                    return (T)xs.Deserialize(r);
                }
            }
            catch (System.Exception)
            {
                HelperTool.LogError("FGXmlDocument.Deserialize : " + xmlName + "序列化失败");
                throw;
            }

        }
    }
}

