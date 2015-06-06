using System;
using System.Xml;

namespace Freamwork
{
    /// <summary>
    /// db.xml数据的管理类
    /// </summary>
    sealed public class DBXMLManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private DBXMLManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public DBXMLManager instance()
        {
            if (m_instance == null)
            {
                m_instance = new DBXMLManager();
            }
            return m_instance;
        }

        private DBXMLManager()
        {
            if (m_instance != null)
            {
                throw new Exception("DBXMLManager是单例，请使用DBXMLManager.instance来获取其实例！");
            }
            m_instance = this;
        }

        //=================================================================
        /// <summary>
        /// 总数据xml
        /// </summary>
        private XmlNode xmlNode;

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="xmlStr"></param>
        private void init(string xmlStr)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlStr);
            xmlNode = xmlDocument.SelectSingleNode("root");
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            xmlNode = null;
        }

        /// <summary>
        /// 提取一条xml，提取后源数据不存在这条数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public XmlNode extractXmlNode(string name)
        {
            XmlNode node = xmlNode.SelectSingleNode(name);
            if (node != null)
            {
                xmlNode.RemoveChild(node);
            }
            return node;
        }

    }
}
