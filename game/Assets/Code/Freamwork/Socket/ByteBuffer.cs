using System;
using System.Collections.Generic;
using System.Text;

namespace Freamwork
{
    /// <summary>
    /// 字节缓冲区
    /// </summary>
    public class ByteBuffer
    {
        public ByteBuffer()
        {
            this.buffer = new byte[0];
        }

 		public ByteBuffer(byte[] buffer)
        {
            this.buffer = buffer;
		}

        /// <summary>
        /// 字节数组
        /// </summary>
        public byte[] buffer
        {
            get
            {
                return m_buffer.ToArray();
            }
            set
            {
                m_buffer = new List<byte>(value);
            }
        }
        private List<byte> m_buffer;

        /// <summary>
        /// 字节长度
        /// </summary>
        public int length
        {
            get
            {
                return m_buffer.Count;
            }
		}

        //============================添加==============================
        public void appendByte(byte value)
        {
            m_buffer.Add(value);
        }

        public void appendBytes(byte[] value)
        {
            m_buffer.AddRange(value);
		}

        public void appendBuffer(ByteBuffer buf)
        {
            appendBytes(buf.buffer);
        }

        public void appendBool(bool value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void appendShort(short value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void appendInt(int value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
		}

        public void appendLong(long value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void appendUshort(ushort value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void appendUint(uint value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        //public void appendUlong(ulong value)
        //{
        //    m_buffer.AddRange(BitConverter.GetBytes(value));
        //}

        public void appendFloat(float value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void appendDouble(double value)
        {
            m_buffer.AddRange(BitConverter.GetBytes(value));
        }

        public void appendString(string value)
		{
            appendShort((short)value.Length);
            m_buffer.AddRange(Encoding.UTF8.GetBytes(value));
		}

        //============================提取==============================
        public byte removeByte()
        {
            byte result = m_buffer[0];
            m_buffer.RemoveAt(0);
            return result;
		}
		
		public byte[] removeBytes(int count)
        {
            byte[] result = m_buffer.GetRange(0, count).ToArray();
            m_buffer.RemoveRange(0, count);
            return result;
		}

        public bool removeBool()
        {
            bool result = BitConverter.ToBoolean(buffer, 0);
            m_buffer.RemoveRange(0, 1);
            return result;
        }

        public short removeShort()
        {
            short result = BitConverter.ToInt16(buffer, 0);
            m_buffer.RemoveRange(0, 2);
            return result;
        }

        public int removeInt()
        {
            int result = BitConverter.ToInt32(buffer, 0);
            m_buffer.RemoveRange(0, 4);
            return result;
        }

        public long removeLong()
        {
            long result = BitConverter.ToInt64(buffer, 0);
            m_buffer.RemoveRange(0, 8);
            return result;
        }

        public ushort removeUshort()
        {
            ushort result = BitConverter.ToUInt16(buffer, 0);
            m_buffer.RemoveRange(0, 2);
            return result;
        }

        public uint removeUint()
        {
            uint result = BitConverter.ToUInt32(buffer, 0);
            m_buffer.RemoveRange(0, 4);
            return result;
        }

        //public ulong removeUlong()
        //{
        //    ulong result = BitConverter.ToUInt64(buffer, 0);
        //    m_buffer.RemoveRange(0, 8);
        //    return result;
        //}

        public float removeFloat()
        {
            float result = BitConverter.ToSingle(buffer, 0);
            m_buffer.RemoveRange(0, 4);
            return result;
        }
		
		public double removeDouble()
        {
            double result = BitConverter.ToDouble(buffer, 0);
            m_buffer.RemoveRange(0, 8);
            return result;
		}
		
		public string removeString()
		{
            short len = removeShort();
            string result = Encoding.UTF8.GetString(buffer, 0, len);
            m_buffer.RemoveRange(0, len);
            return result;
		}
		
        /// <summary>
        /// 销毁
        /// </summary>
		public void dispose()
		{
			m_buffer.Clear();
			m_buffer = null;
		}

    }
}