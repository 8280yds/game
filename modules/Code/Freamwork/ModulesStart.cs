using CLRSharp;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork
{
    public class ModulesStart
    {
        public static void start()
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            TestMB testMB = new TestMB();
            testMB.init(cube);

            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            TestMB2 testMB2 = MVCCharge.instance.getInstance(typeof(TestMB2) as ICLRType) as TestMB2;
            testMB2.init(sphere);

            TestView testView = MVCCharge.instance.getInstance(typeof(TestView) as ICLRType) as TestView;
            testView.init(GameObject.Find("Panel"));
        }

        //public static void start()
        //{
        //    object type = typeof(Test);
        //    ICLRType clrType = type as ICLRType;
        //    Test test = CLRSharpManager.instance.creatCLRInstance(clrType) as Test;
        //    Debug.Log(test.getValue("dd"));
        //}

    }
    public class Test
    {
        public Dictionary<string, object> dic = new Dictionary<string, object>();
        public Test()
        {
            dic.Add("aa", "AA");
            dic.Add("bb", "BB");
            dic.Add("cc", "CC");
            dic.Add("dd", "DD");
        }

        public object getValue(string key)
        {
            return dic[key];
        }
    }

}
