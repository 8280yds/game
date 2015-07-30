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
            //加载DB数据
            LoadManager.instance.addLoad(LoadConstant.DB_FILE, LoadPriority.zero, LoadType.local,
                null, null, null, null, null, null, unZipEnd);
        }

        private static void unZipEnd(LoadData data)
        {
            if(data.fullName == LoadConstant.DB_FILE)
            {
                GameStart.setProgressData(97, "加载进程：");
                DBXMLManager.instance.init((data.assets[0] as TextAsset).text);

                //加载MainUI
                LoadManager.instance.addLoad("mainui.assets", LoadPriority.zero, LoadType.local,
                    null, null, null, null, null, null, unZipEnd);
            }
            else if (data.fullName == "mainui.assets")
            {
                GameStart.setProgressData(99, "加载进程：");
                AssetsManager.instance.setMainUIAssets(data.assets[0] as GameObject);

                //加载模块第一个场景
                LoadManager.instance.addLoad("cell_war_scene.assets", LoadPriority.two, LoadType.local,
                    null, null, null, null, null, null, unZipEnd);
            }
            else if (data.fullName == "cell_war_scene.assets")
            {
                GameStart.setProgressData(100, "加载进程：");

                LevelModel levelModel = MVCCharge.instance.getInstance(typeof(LevelModel) as ICLRType) as LevelModel;
                levelModel.initLevelData();

                Application.LoadLevel("cell_war_scene");
                OpenGateView view = MVCCharge.instance.getInstance(typeof(OpenGateView) as ICLRType) as OpenGateView;
                view.show();
            }
        }

        /// <summary>
        /// 清除单例等，主要是用于项目重启时的清理
        /// </summary>
        public static void clear()
        {
            MVCCharge.instance.clear();
        }

    }
}
