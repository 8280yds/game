using CLRSharp;
using System;

namespace Freamwork
{
    public class PackageUtil
    {
        //==============================================================================================
        /// <summary>
        /// 将实例转化为ByteBuffer
        /// </summary>
        public static ByteBuffer clrObjectToByteBuffer(CLRSharp_Instance inst)
        {
            ByteBuffer buff = new ByteBuffer();
            HashList<string, string> fieldInfos =
                CLRSharpManager.instance.Invoke(inst.type, "getFieldInfos", inst) as HashList<string, string>;
            string[] filedNames = fieldInfos.keys.ToArray();

            for (int i = 0, len = filedNames.Length; i < len; i++)
            {
                appendField(inst.Fields[filedNames[i]], ref buff);
            }
            return buff;
        }

        private static void appendField(object field, ref ByteBuffer buff)
        {
            if (field is CLRSharp_Instance)
            {
                buff.appendBuffer(clrObjectToByteBuffer(field as CLRSharp_Instance));
            }
            else
            {
                Type type = field.GetType();
                if (type.BaseType != null && type.BaseType.FullName == "System.Array")
                {
                    appendArray(field as Array, ref buff);
                }
                else
                {
                    append(field, ref buff);
                }
            }
        }

        private static void appendArray(Array array, ref ByteBuffer buff)
        {
            buff.appendUshort((ushort)(array.Length));
            for (int i = 0, len = array.Length; i < len; i++)
            {
                appendField(array.GetValue(i), ref buff);
            }
        }

        private static void append(object field, ref ByteBuffer buff)
        {
            Type type = field.GetType();
            switch (type.FullName)
            {
                case "System.Int16":
                    buff.appendShort((short)field);
                    break;
                case "System.Int32":
                    buff.appendInt((int)field);
                    break;
                case "System.Int64":
                    buff.appendLong((long)field);
                    break;
                case "System.UInt16":
                    buff.appendUshort((ushort)field);
                    break;
                case "System.UInt32":
                    buff.appendUint((uint)field);
                    break;
                //case "System.UInt64":
                //    buff.appendUlong((ulong)field);
                //    break;
                case "System.Byte":
                    buff.appendByte((byte)field);
                    break;
                case "System.Boolean":
                    buff.appendBool((bool)field);
                    break;
                case "System.Single":
                    buff.appendFloat((float)field);
                    break;
                case "System.Double":
                    buff.appendDouble((double)field);
                    break;
                case "System.String":
                    buff.appendString((string)field);
                    break;
                default:
                    throw new Exception("协议包含不可解析类型：" + type.FullName);
            }
        }

        //=========================================================================================
        /// <summary>
        /// 将ByteBuffer转化为实例
        /// </summary>
        public static object byteBufferToClrObject(ref ByteBuffer buff, ICLRType clrType)
        {
            CLRSharp_Instance inst = CLRSharpManager.instance.creatCLRInstance(clrType) as CLRSharp_Instance;

            HashList<string, string> fieldInfos =
                CLRSharpManager.instance.Invoke(inst.type, "getFieldInfos", inst) as HashList<string, string>;
            string[] filedNames = fieldInfos.keys.ToArray();

            for (int i = 0, len = filedNames.Length; i < len; i++)
            {
                inst.Fields[filedNames[i]] = getField(ref buff, fieldInfos.getValue(filedNames[i]));
            }
            return inst;
        }

        private static object getField(ref ByteBuffer buff, string typeName)
        {
            //数组
            int len = typeName.Length;
            if (len > 2 && typeName.Substring(len - 2) == "[]")
            {
                return getArray(ref buff, typeName.Substring(0, len - 2));
            }

            //C#类型
            Type type = Type.GetType(typeName, false);
            if (type != null)
            {
                return getValue(ref buff, type);
            }

            //L#类型
            ICLRType clrType = CLRSharpManager.instance.getCLRType(typeName);
            if (clrType != null)
            {
                return byteBufferToClrObject(ref buff, clrType);
            }

            throw new Exception("协议包含不可解析类型：" + typeName);
        }

        private static object getArray(ref ByteBuffer buff, string itemTypeName)
        {
            //数组长度
            int len = buff.removeUshort();

            //C#类型
            Type itemType = Type.GetType(itemTypeName, false);
            if (itemType != null)
            {
                object[] array = new object[len];
                for (int i = 0; i < len; i++)
                {
                    array[i] = getValue(ref buff, itemType);
                }
                return array;
            }

            //L#类型
            ICLRType clrType = CLRSharpManager.instance.getCLRType(itemTypeName);
            if (clrType != null)
            {
                object[] array = new object[len];
                for (int i = 0; i < len; i++)
                {
                    array[i] = byteBufferToClrObject(ref buff, clrType);
                }
                return array;
            }

            throw new Exception("协议包含不可解析类型：" + itemTypeName + "[]");
        }

        private static object getValue(ref ByteBuffer buff, Type type)
        {
            switch (type.FullName)
            {
                case "System.Int16":
                    return buff.removeShort();
                case "System.Int32":
                    return buff.removeInt();
                case "System.Int64":
                    return buff.removeLong();
                case "System.UInt16":
                    return buff.removeUshort();
                case "System.UInt32":
                    return buff.removeUint();
                //case "System.UInt64":
                //    return buff.removeUlong();
                case "System.Byte":
                    return buff.removeByte();
                case "System.Boolean":
                    return buff.removeBool();
                case "System.Single":
                    return buff.removeFloat();
                case "System.Double":
                    return buff.removeDouble();
                case "System.String":
                    return buff.removeString();
                default:
                    throw new Exception("协议包含不可解析类型：" + type.FullName);
            }
        }
        
    }
}