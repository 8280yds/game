using System.Collections.Generic;
using System.Xml;

namespace Freamwork
{
    /// <summary>
    /// 配置表DBModel的基类
    /// </summary>
    /// <typeparam name="TDBVO">数据VO</typeparam>
    public abstract class DBModel<TDBVO> : Model where TDBVO : DBVO, new()
    {
        protected bool m_order;
        private int m_count;

        private XmlNode xmlNode;
        private Dictionary<int, TDBVO> m_dataDic;
        private int[] m_ids;

        public DBModel()
        {
            initDBModel();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="name">数据表名称，不带后缀</param>
        /// <param name="order">数据是否有顺序性</param>
        virtual protected void initDBModel(string name = "", bool order = false)
        {
            m_order = order;
            xmlNode = DBXMLManager.instance.extractXmlNode(name);
            m_count = xmlNode.ChildNodes.Count;
            m_dataDic = new Dictionary<int, TDBVO>();
            if (order)
            {
                m_ids = new int[count];
            }
        }

        /// <summary>
        /// 数据是否有顺序
        /// </summary>
        protected bool order
        {
            get
            {
                return m_order;
            }
        }

        /// <summary>
        /// 表中数据的总条数
        /// </summary>
        protected int count
        {
            get
            {
                return m_count;
            }
        }

        /// <summary>
        /// 数据是否已经全部解析完毕
        /// </summary>
        protected bool analysised
        {
            get
            {
                return m_dataDic.Count == count;
            }
        }

        /// <summary>
        /// 存储所有数据vo的dictionary
        /// </summary>
        protected Dictionary<int, TDBVO> dataDic
        {
            get
            {
                return m_dataDic;
            }
        }

        /// <summary>
        /// 当数据具有顺序性时用来存储数据排序的数组，当数据不具有顺序性时，其值为null,
        /// 数组中存储的是Id，可以通过dataDic[ids[index]]来获取数据VO
        /// </summary>
        protected int[] ids
        {
            get
            {
                return m_ids;
            }
        }

        /// <summary>
        /// 解析全表，调用此方法后表中所有的数据将都会被加载到dataDic中，
        /// xmlNodeList和xmlNode将被置为null
        /// </summary>
        protected void analysis()
        {
            if (!analysised)
            {
                TDBVO vo;
                XmlNode node;

                if (order)
                {
                    for (int i = 0, len = ids.Length; i < len; i++)
                    {
                        if (ids[i] == 0)
                        {
                            vo = new TDBVO();
                            node = xmlNode.FirstChild;
                            vo.xmlToVo(node);
                            xmlNode.RemoveChild(node);
                            ids[i] = vo.id;
                            dataDic.Add(vo.id, vo);

                            if (!xmlNode.HasChildNodes)
                            {
                                break;
                            }
                        }
                    }
                }
                else
                {
                    while (xmlNode.HasChildNodes)
                    {
                        vo = new TDBVO();
                        node = xmlNode.FirstChild;
                        vo.xmlToVo(node);
                        xmlNode.RemoveChild(node);
                        dataDic.Add(vo.id, vo);
                    }
                }
            }
            xmlNode = null;
        }

        /// <summary>
        /// 根据id获取一条数据，不会解析全表，只会取想要的数据
        /// </summary>
        /// <param name="id">表的索引id</param>
        /// <returns>TDBVO</returns>
        public TDBVO getVoById(int id)
        {
            if (dataDic.ContainsKey(id))
            {
                return dataDic[id];
            }

            if (analysised)
            {
                return null;
            }

            XmlNode node = xmlNode.SelectSingleNode("item[@id=" + id + "]");
            if (node == null)
            {
                return null;
            }

            TDBVO vo = new TDBVO();
            vo.xmlToVo(node);
            dataDic.Add(vo.id, vo);
            if (order)
            {
                ids[findIndex(node)] = vo.id;
            }
            xmlNode.RemoveChild(node);
            return vo;
        }

        private int findIndex(XmlNode node)
        {
            XmlNodeList list = xmlNode.ChildNodes;
            for (int i = 0, len = list.Count; i < len; i++)
            {
                if (list.Item(i) == node)
                {
                    for (int j = 0, t = 0; j < count; j++)
                    {
                        if (m_ids[j] == 0)
                        {
                            if (t == i)
                            {
                                return j;
                            }
                            t++;
                        }
                    }
                    break;
                }
            }
            return -1;
        }

        /// <summary>
        /// 清除数据，在dispose中会调用
        /// </summary>
        override public void clearAll()
        {
            xmlNode = null;
            m_dataDic = null;
            m_ids = null;
        }

    }
}
