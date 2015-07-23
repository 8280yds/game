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
            LoadManager.instance.addLoad("cell_war_scene.assets", LoadPriority.two, LoadType.local,
                null, null, null, null, null, null, unZipSceneEnd);
        }

        private static void unZipSceneEnd(LoadData data)
        {
            Application.LoadLevel("cell_war_scene");

            LevelView view = MVCCharge.instance.getInstance(typeof(LevelView) as ICLRType) as LevelView;
            view.show();

            //LoadManager.instance.addLoad("cell.assets", LoadPriority.two, LoadType.local,
            //    null, null, null, null, null, null, unZipEnd);
            //LoadManager.instance.addLoad("node.assets", LoadPriority.two, LoadType.local,
            //    null, null, null, null, null, null, unZipEnd);
            //LoadManager.instance.addLoad("popuptext.assets", LoadPriority.two, LoadType.local,
            //    null, null, null, null, null, null, unZipEnd);
        }

        //private static void unZipEnd(LoadData data)
        //{
        //    GameObject go = data.assets[0] as GameObject;
        //    if (go.name == "cell")
        //    {
        //        CellWarManager.instance.cellPrefab = go;
        //    }
        //    else if (go.name == "node")
        //    {
        //        CellWarManager.instance.nodePrefab = go;
        //    }
        //    else if (go.name == "popuptext")
        //    {
        //        PopUpTextManager.instance.textGO = go;
        //    }

        //    if (CellWarManager.instance.cellPrefab != null && CellWarManager.instance.nodePrefab != null && 
        //        PopUpTextManager.instance.textGO!= null)
        //    {
        //        CellWarView cellWarView = MVCCharge.instance.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
        //        cellWarView.show();
        //    }
        //}

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
