using CLRSharp;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 框架管理类
    /// </summary>
    sealed public class GameManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private GameManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public GameManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GameManager();
                }
                return m_instance;
            }
        }

        private GameManager()
        {
            m_instance = this;
            init();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            started = false;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            started = false;

            //ModulesStart.clear();
            CLRSharpManager clrmana = CLRSharpManager.instance;
            ICLRType clrType = clrmana.getCLRType("Freamwork.ModulesStart");
            clrmana.Invoke(clrType, "clear");

            GMBManager.instance.clear();
            CLRSharpManager.instance.clear();
            EnterFrame.instance.clear();
            ManifestManager.instance.clear();
            LoadManager.instance.clear();
            BundleLoadManager.instance.clear();
        }

        //=====================================================================
        /// <summary>
        /// 框架是否已经启动
        /// </summary>
        public bool started
        {
            get;
            private set;
        }

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

            if (Application.loadedLevelName != GameConstant.FIRST_SCENE)
            {
                Application.LoadLevel(GameConstant.FIRST_SCENE);
                return;
            }

            //强制清除并垃圾回收
            Resources.UnloadUnusedAssets();
            GC.Collect();

            ManifestManager.instance.init(LoadConstant.CDN + LoadConstant.MANIFEST_FILE, 
                loadStart, loadProgress, loadEnd, loadFail, unZipStart, unZipProgress, unZipEnd);
        }

        private void loadStart(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {
                GameStart.setProgressData(0, "加载进程：");
            }
        }

        private void loadProgress(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {
                GameStart.setProgressData((int)data.loadProgressNum * 10, "加载进程：");
            }
        }

        private void loadEnd(LoadData data)
        {
            if (data.fullName != LoadConstant.MANIFEST_FILE)
            {
                int totleCount = ManifestManager.instance.getAllFullName.Count;
                int currentCount = BundleLoadManager.instance.getLoadingFullNames().Count;
                int progress = 10 + 80 * currentCount / totleCount;
                GameStart.setProgressData(progress, "加载进程：");
            }
            else
            {
                GameStart.setProgressData(10, "加载进程：");
            }

            if (BundleLoadManager.instance.getLoadingFullNames().Count == 0 && 
                data.fullName != LoadConstant.MANIFEST_FILE)
            {
                GameStart.setProgressData(90, "加载进程：");
                LoadManager.instance.addLoad(GameConstant.MODULES + GameConstant.SUFFIX, LoadPriority.zero, LoadType.local,
                    null, null, null, null, unZipStart, unZipProgress, unZipEnd);
            }
        }

        private void loadFail(LoadData data)
        {
            if (data.fullName == LoadConstant.MANIFEST_FILE)
            {
                Debug.Log(LoadConstant.MANIFEST_FILE + "网络加载失败，放弃更新！");

                string url = LoadConstant.LOCAL_TITLE + LoadConstant.localFilesPath + "/" + LoadConstant.MANIFEST_FILE;
                ManifestManager.instance.init(url, loadStart, loadProgress, loadEnd, loadFail, unZipStart, unZipProgress, unZipEnd);
            }
        }

        private void unZipStart(LoadData data)
        {
            
        }

        private void unZipProgress(LoadData data)
        {
            
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
                        loadStart, loadProgress, loadEnd, loadFail);
                }
            }
            else if (data.fullName == GameConstant.MODULES + GameConstant.SUFFIX)
            {
                GameStart.setProgressData(95, "加载进程：");
                //加载并解压dll文件包结束
                CLRSharpManager.instance.init(data.assets[0] as TextAsset);
                //开启模块
                startModules();
            }
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        private void startModules()
        {
            Debug.Log("==================正式启动L#模块===================");
            CLRSharpManager clrmana = CLRSharpManager.instance;
            ICLRType clrType = clrmana.getCLRType("Freamwork.ModulesStart");
            clrmana.Invoke(clrType, "start");
        }

    }
}
