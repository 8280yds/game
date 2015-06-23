using CLRSharp;
using System;

namespace Freamwork
{
    /// <summary>
    /// socket回调方法委托
    /// </summary>
    /// <param name="package"></param>
    public delegate void SocketListenerDele(Package package);

    public class Package
    {
        /// <summary>
        /// 时间戳
        /// </summary>
        public int timeStamp;

        /// <summary>
        /// 协议号
        /// </summary>
        public int protocol;

        /// <summary>
        /// 内容长度
        /// </summary>
        public ushort len;

        /// <summary>
        /// 内容结构体
        /// </summary>
        public object data;

        /// <summary>
        /// 打印时使用的详细信息
        /// </summary>
        /// <returns></returns>
        public string toString()
        {
            string str = "";
            str += TimeUtil.StampToDateTime(timeStamp).ToString();
            str += " 协议:" + protocol;
            str += " 长度:" + len;
            str += "\n";

            if (data == null)
            {
                str += "null";
            }
            else
            {
                str += getDataString(data as CLRSharp_Instance);
            }

            return str;
        }

        private string getDataString(CLRSharp_Instance inst)
        {
            string str = "{";
            ICLRType type = CLRSharpManager.instance.getCLRType("Freamwork.ProtocolData");
            bool boo = CLRSharpManager.instance.isExtend(inst.type as Type_Common_CLRSharp, type as Type_Common_CLRSharp);

            if(boo)
            {
                HashList<string, string> fieldInfos = 
                    CLRSharpManager.instance.Invoke(inst.type, "getFieldInfos", inst) as HashList<string, string>;
                string[] filedNames = fieldInfos.keys.ToArray();

                for (int i = 0, len = filedNames.Length; i < len; i++)
                {
                    str += getFieldString(inst.Fields[filedNames[i]], filedNames[i] + ":");
                    if (i < len - 1)
                    {
                        str += ", ";
                    }
                }
            }
            else
            {
                str += inst.ToString();
            }

            str += "}";
            return str;
        }

        private string getArrayString(Array array)
        {
            string str = "[";

            for (int i = 0, len = array.Length; i < len; i++)
            {
                str += getFieldString(array.GetValue(i), "");
                if (i < len - 1)
                {
                    str += ", ";
                }
            }

            str += "]";
            return str;
        }

        private string getFieldString(object field, string filedName)
        {
            string str = filedName;
            Type t = field.GetType();

            if (field is CLRSharp_Instance)
            {
                str += getDataString(field as CLRSharp_Instance);
            }
            else if (t.BaseType != null && t.BaseType.FullName == "System.Array")
            {
                str += getArrayString(field as Array);
            }
            else if (field is string)
            {
                str += "\"" + field + "\"";
            }
            else
            {
                str += field.ToString();
            }

            return str;
        }

    }
}