using System;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 框架管理类
    /// </summary>
    sealed public class FreamworkManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private FreamworkManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public FreamworkManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new FreamworkManager();
                }
                return m_instance;
            }
        }

        private FreamworkManager()
        {
            if (m_instance != null)
            {
                throw new Exception("FreamworkManager是单例，请使用FreamworkManager.instance来获取其实例！");
            }
            m_instance = this;
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            m_started = false;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            m_started = false;

            EnterFrame.instance.clear();
            ManifestManager.instance.clear();
            MVCCharge.instance.clear();
            LoadManager.instance.clear();
            BundleLoadManager.instance.clear();
        }

        //=====================================================================
        /// <summary>
        /// 框架是否已经启动
        /// </summary>
        public bool started
        {
            get
            {
                return m_started;
            }
        }
        private bool m_started;

        /// <summary>
        /// 启动框架，重启框架意味着重启游戏
        /// </summary>
        public void start()
        {
            if (started)
            {
                throw new Exception("试图重复启动框架，重启框架将意味着整个游戏重新运行，" +
                    "如果确定需要重启，如果确信要这么做请在clear()后调用此方法");
            }

            if (Application.loadedLevelName != FreamworkConstant.FIRST_SCENE)
            {
                Application.LoadLevel(FreamworkConstant.FIRST_SCENE);
                return;
            }

            //强制清除并垃圾回收
            Resources.UnloadUnusedAssets();
            GC.Collect();

            ManifestManager.instance.init(loadStart, loadProgress, loadEnd, loadFail, unZipStart, unZipProgress, unZipEnd);
        }

        private void loadStart(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {

                return;
            }
        }

        private void loadProgress(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {

                return;
            }
        }

        private void loadEnd(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {

                return;
            }
            if (BundleLoadManager.instance.getLoadingFullNames().Count == 0)
            {
                LoadManager.instance.addLoad(LoadConstant.DB_FILE, LoadPriority.zero, LoadType.local,
                    null, null, null, null, unZipStart, unZipProgress, unZipEnd);
            }
        }

        private void loadFail(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {

                return;
            }
        }

        private void unZipStart(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {

                return;
            }
        }

        private void unZipProgress(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {

                return;
            }
        }

        private void unZipEnd(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {
                //从网络下载更新资源包到本地
                List<string> list = ManifestManager.instance.getAllFullName;
                for (int i = 0, len = list.Count; i < len; i++)
                {
                    BundleLoadManager.instance.addLoad(list[i], LoadPriority.zero, LoadType.web,
                        loadStart,loadProgress,loadEnd,loadFail);
                }
                return;
            }
            if (data.fullName == LoadConstant.DB_FILE)
            {
                //db解压结束
                DBXMLManager.instance.init((data.assets[0] as TextAsset).text);

                FffDBModel dbModel = MVCCharge.instance.getModel<FffDBModel>();
                FffDBVO vo = dbModel.getVOByName("小红");
                Debug.Log(vo);
                List<FffDBVO> list = dbModel.getVOBySex("女");
                Debug.Log(list);
            }
        }


    }
}
