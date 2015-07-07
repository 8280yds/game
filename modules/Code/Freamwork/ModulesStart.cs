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
            ////测试GMB
            //var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //TestMB testMB = new TestMB();
            //testMB.init(cube);
            //var sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //TestMB2 testMB2 = MVCCharge.instance.getInstance(typeof(TestMB2) as ICLRType) as TestMB2;
            //testMB2.init(sphere);

            ////测试view
            //TestView testView = MVCCharge.instance.getInstance(typeof(TestView) as ICLRType) as TestView;
            //testView.show();

            //加载cell资源
            LoadManager.instance.addLoad("cell.assets", LoadPriority.two, LoadType.local,
                null, null, null, null, null, null, unZipEnd);
            LoadManager.instance.addLoad("node.assets", LoadPriority.two, LoadType.local,
                null, null, null, null, null, null, unZipEnd);
        }

        private static void unZipEnd(LoadData data)
        {
            GameObject go = data.assets[0] as GameObject;
            if (go.name == "cell")
            {
                CellWarManager.instance.cellPrefab = go;
            }
            else if (go.name == "node")
            {
                CellWarManager.instance.nodePrefab = go;
            }

            if (CellWarManager.instance.cellPrefab != null && CellWarManager.instance.nodePrefab != null)
            {
                CellWarView cellWarView = MVCCharge.instance.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
                cellWarView.show();
            }
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
