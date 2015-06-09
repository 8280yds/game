using CLRSharp;
using System;
using System.Collections.Generic;
using System.Xml;

namespace Freamwork
{
    /// <summary>
    /// 配置表DBModel的基类
    /// </summary>
    public abstract class DBModel : Model
    {
        private XmlNode xmlNode;
        private Dictionary<int, object> m_dataDic;
        private int[] m_ids;

        private IMethod m_method_xmlToVo;
        private IMethod m_method_creatVo;

        public DBModel()
        {
            initDBModel();
        }

        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="voCLRType">vo的L#类型</param>
        /// <param name="name">数据表名称，不带后缀</param>
        /// <param name="order">数据是否有顺序性</param>
        virtual protected void initDBModel(ICLRType voCLRType = null, string sheet = "", bool order = false)
        {
            object dbvoCLRType = typeof(DBVO);
            if (!CLRSharpManager.instance.isExtend(voCLRType as Type_Common_CLRSharp, dbvoCLRType as Type_Common_CLRSharp))
            {
                throw new Exception(this.GetType().FullName + "初始化时VO的类型并非是DBVO的子类");
            }
            this.voCLRType = voCLRType;
            this.order = order;
            xmlNode = DBXMLManager.instance().extractXmlNode(sheet);
            count = xmlNode.ChildNodes.Count;
            m_dataDic = new Dictionary<int, object>();
            if (order)
            {
                m_ids = new int[count];
            }
        }

        /// <summary>
        /// vo的xmlToVo方法
        /// </summary>
        public IMethod method_xmlToVo
        {
            get
            {
                if (m_method_xmlToVo == null)
                {
                    m_method_xmlToVo = CLRSharpManager.instance.GetMethod(voCLRType,
                        "xmlToVo", CLRSharpManager.instance.getParamTypeList(typeof(XmlNode)));
                }
                return m_method_xmlToVo;
            }
        }

        /// <summary>
        /// vo的构造方法
        /// </summary>
        public IMethod method_creatVo
        {
            get
            {
                if (m_method_creatVo == null)
                {
                    m_method_creatVo = CLRSharpManager.instance.GetMethod(voCLRType, CLRSharpConstant.METHOD_CTOR);
                }
                return m_method_creatVo;
            }
        }

        /// <summary>
        /// vo的L#类型
        /// </summary>
        public ICLRType voCLRType
        {
            get;
            private set;
        }

        /// <summary>
        /// 数据是否有顺序
        /// </summary>
        protected bool order
        {
            get;
            private set;
        }

        /// <summary>
        /// 表中数据的总条数
        /// </summary>
        protected int count
        {
            get;
            private set;
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
        /// 存储所有数据vo的dictionary(返回的是浅克隆的列表)
        /// </summary>
        protected Dictionary<int, object> dataDic
        {
            get
            {
                return new Dictionary<int, object>(m_dataDic);
            }
        }

        /// <summary>
        /// 当数据具有顺序性时用来存储数据排序的数组，当数据不具有顺序性时，其值为null,
        /// <param>数组中存储的是Id，可以通过dataDic[ids[index]]来获取数据VO</param>
        /// <param>(返回的是克隆列表)</param>
        /// </summary>
        protected int[] ids
        {
            get
            {
                int[] arr = new int[count];
                m_ids.CopyTo(arr, 0);
                return arr;
            }
        }

        /// <summary>
        /// 解析全表，调用此方法后表中所有的数据将都会被加载到列表中，
        /// xmlNodeList和xmlNode将被置为null
        /// </summary>
        protected void analysis()
        {
            if (!analysised)
            {
                object vo;
                XmlNode node;

                if (order)
                {
                    for (int i = 0, len = m_ids.Length; i < len; i++)
                    {
                        if (m_ids[i] == 0)
                        {
                            //创建实例，调用构造函数
                            vo = method_creatVo.Invoke(CLRSharpManager.instance.context, null, null);

                            //调用xmlToVo方法
                            node = xmlNode.FirstChild;
                            method_xmlToVo.Invoke(CLRSharpManager.instance.context, vo, new object[] { node });

                            xmlNode.RemoveChild(node);
                            m_ids[i] = int.Parse(((XmlElement)node).GetAttribute("id"));
                            m_dataDic.Add(m_ids[i], vo);

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
                        node = xmlNode.FirstChild;

                        //创建实例，调用构造函数
                        vo = method_creatVo.Invoke(CLRSharpManager.instance.context, null, null);
                        //调用xmlToVo方法
                        method_xmlToVo.Invoke(CLRSharpManager.instance.context, vo, new object[] { node });

                        m_dataDic.Add(int.Parse(((XmlElement)node).GetAttribute("id")), vo);
                        xmlNode.RemoveChild(node);
                    }
                }
            }
            xmlNode = null;
        }

        /// <summary>
        /// 根据id获取一条数据，不会解析全表，只会取想要的数据
        /// </summary>
        /// <param name="id">表的索引id</param>
        /// <returns></returns>
        public object getVoById(int id)
        {
            if (m_dataDic.ContainsKey(id))
            {
                return m_dataDic[id];
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

            object vo = method_creatVo.Invoke(CLRSharpManager.instance.context, null, null);
            method_xmlToVo.Invoke(CLRSharpManager.instance.context, vo, new object[] { node });

            int _id = int.Parse(((XmlElement)node).GetAttribute("id"));
            m_dataDic.Add(_id, vo);
            if (order)
            {
                m_ids[findIndex(node)] = _id;
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
