using CLRSharp;
using Freamwork;
using UnityEngine;

public class TestService : Service
{
    private const int TestProtocol = 11111111;

    protected override void addlisteners()
    {
        addListener(TestProtocol, receiveMessage, typeof(TestData) as ICLRType);
    }

    private void receiveMessage(Package package)
    {
        

    }

    public void sendMessage()
    {
        TestData data = new TestData();
        data._byte = 1;
        data._bool = true;
        data._short = -111;
        data._int = -1111111;
        data._long = -1111111111;
        data._ushort = 111;
        data._uint = 1111111111;
        data._float = 11.11f;
        data._double = 111111111.111111111;
        data._string = "111111111";
        data._strings = new string[] { "123", "sss", "AAA" };
        data.test2Data = new Test2Data();
        data.test2Datas = new Test2Data[] { new Test2Data(), new Test2Data(), new Test2Data()};
        request(TestProtocol, data);
    }

    public class TestData : ProtocolData
    {
        public byte _byte;
        public bool _bool;
        public short _short;
        public int _int;
        public long _long;
        public ushort _ushort;
        public uint _uint;
        public float _float;
        public double _double;
        public string _string;

        public string[] _strings;

        public Test2Data test2Data;
        public Test2Data[] test2Datas;

        public override HashList<string, string> getFieldInfos()
        {
            HashList<string, string> hash = new HashList<string, string>();
            hash.add("_byte", typeName(typeof(byte)));
            hash.add("_bool", typeName(typeof(bool)));
            hash.add("_short", typeName(typeof(short)));
            hash.add("_int", typeName(typeof(int)));
            hash.add("_long", typeName(typeof(long)));
            hash.add("_ushort", typeName(typeof(ushort)));
            hash.add("_uint", typeName(typeof(uint)));
            hash.add("_float", typeName(typeof(float)));
            hash.add("_double", typeName(typeof(double)));
            hash.add("_string", typeName(typeof(string)));

            hash.add("_strings", typeName(typeof(string[])));
            hash.add("test2Data", typeName(typeof(Test2Data)));
            hash.add("test2Datas", typeName(typeof(Test2Data)) + "[]");

            return hash;
        }
    }

    public class Test2Data : ProtocolData
    {
        public bool _bool = true;
        public int _int = 888;

        public override HashList<string, string> getFieldInfos()
        {
            HashList<string, string> hash = new HashList<string, string>();
            hash.add("_bool", typeName(typeof(bool)));
            hash.add("_int", typeName(typeof(int)));
            return hash;
        }
    }

}
