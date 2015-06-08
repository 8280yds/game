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
            if (m_instance != null)
            {
                throw new Exception("GameManager是单例，请使用GameManager.instance来获取其实例！");
            }
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

            EnterFrame.instance.clear();
            ManifestManager.instance.clear();
            LoadManager.instance.clear();
            BundleLoadManager.instance.clear();

            //调用MVCCharge.instance.clear();
            CLRSharpManager clrmana = CLRSharpManager.instance;
            ICLRType clrType = clrmana.getCLRType("Freamwork.MVCCharge");
            object inst = clrmana.Invoke(clrType, "instance");
            clrmana.Invoke(clrType, "clear", inst);
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

            ManifestManager.instance.init(loadStart, loadProgress, loadEnd, loadFail, unZipStart, unZipProgress, unZipEnd);
        }

        private void loadStart(LoadData data)
        {
            
        }

        private void loadProgress(LoadData data)
        {

        }

        private void loadEnd(LoadData data)
        {
            if (BundleLoadManager.instance.getLoadingFullNames().Count == 0 && 
                data.fullName != LoadConstant.MANIFEST_FILE)
            {
                LoadManager.instance.addLoad(GameConstant.MODULES, LoadPriority.zero, LoadType.local,
                    null, null, null, null, unZipStart, unZipProgress, unZipEnd);
            }
        }

        private void loadFail(LoadData data)
        {
            
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
            else if (data.fullName == GameConstant.MODULES)
            {
                //加载并解压dll文件包结束
                CLRSharpManager.instance.init(data.assets[0] as TextAsset);
                LoadManager.instance.addLoad(LoadConstant.DB_FILE, LoadPriority.zero, LoadType.local,
                    null, null, null, null, unZipStart, unZipProgress, unZipEnd);
            }
            else if (data.fullName == LoadConstant.DB_FILE)
            {
                //db加载并解压结束,将字符串交给DBXMLManager管理
                //调用DBXMLManager.instance.init(data.assets[0] as TextAsset).text);
                CLRSharpManager clrmana = CLRSharpManager.instance;
                ICLRType clrType = clrmana.getCLRType("Freamwork.DBXMLManager");
                object inst = clrmana.Invoke(clrType, "instance");
                MethodParamList list = clrmana.getParamTypeList(typeof(string));
                object[] param = new object[] { (data.assets[0] as TextAsset).text };
                clrmana.Invoke(clrType, "init", inst, list, param);

                //开启模块
                startModules();
            }
        }

        /// <summary>
        /// 启动模块
        /// </summary>
        private void startModules()
        {
            CLRSharpManager clrmana = CLRSharpManager.instance;
            ICLRType clrType = clrmana.getCLRType("Freamwork.ModulesStart");
            clrmana.Invoke(clrType, "start");
        }

    }
}
