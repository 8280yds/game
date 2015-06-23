using CLRSharp;
using System;

namespace Freamwork
{
    public abstract class ProtocolData
    {
        public abstract HashList<string, string> getFieldInfos();

        protected string typeName(object type)
        {
            Type_Common_CLRSharp clrType = type as Type_Common_CLRSharp;
            if (clrType != null)
            {
                return clrType.FullName;
            }

            Type t = type as Type;
            if (t != null)
            {
                return t.FullName;
            }

            throw new Exception("协议数据类型名称获取失败");
        }
    }
}