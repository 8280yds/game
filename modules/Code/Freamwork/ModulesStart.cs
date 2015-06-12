using CLRSharp;
using UnityEngine;

namespace Freamwork
{
    public class ModulesStart
    {
        /// <summary>
        /// L#模块总启动
        /// </summary>
        public static void start()
        {
            //测试GMB
            //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //TestMB testMB = new TestMB();
            //testMB.init(cube);
            //var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //TestMB2 testMB2 = MVCCharge.instance.getInstance(typeof(TestMB2) as ICLRType) as TestMB2;
            //testMB2.init(sphere);

            //测试view
            TestView testView = MVCCharge.instance.getInstance(typeof(TestView) as ICLRType) as TestView;
            testView.show();
        }

        /// <summary>
        /// 清除单例等，主要是用于项目重启时的清理
        /// </summary>
        public static void clear()
        {
            MVCCharge.instance.clear();
            UIManager.instance.clear();
        }

    }
}
