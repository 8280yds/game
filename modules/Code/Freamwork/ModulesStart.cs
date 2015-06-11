using CLRSharp;
using UnityEngine;
using UnityEngine.UI;

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
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            TestMB testMB = new TestMB();
            testMB.init(cube);
            var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            TestMB2 testMB2 = MVCCharge.instance.getInstance(typeof(TestMB2) as ICLRType) as TestMB2;
            testMB2.init(sphere);

            //测试view
            GameObject panel = GameObject.Find("Panel");
            TestView testView = MVCCharge.instance.getInstance(typeof(TestView) as ICLRType) as TestView;
            testView.init(panel);

            //测试enterframe
            txt = GameObject.Find("TestText2").GetComponent<Text>();
            EnterFrame.instance.addEnterFrame(doEnterframe);
        }

        /// <summary>
        /// 清除单例等，主要是用于项目重启时的清理
        /// </summary>
        public static void clear()
        {
            MVCCharge.instance.clear();
        }

        private static void doEnterframe()
        {
            count++;
            txt.text = "" + count;
            if (count >= 500)
            {
                EnterFrame.instance.removeEnterFrame(doEnterframe);
            }
        }
        private static int count = 0;
        private static Text txt;
    }
}
