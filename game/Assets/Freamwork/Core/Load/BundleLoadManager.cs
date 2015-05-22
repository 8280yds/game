﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// assetBundle加载管理类，具有排队，优先级等功能
    /// <para>能根绝情况确定是否需要到网络加载</para>
    /// <para>能够自动加载依赖包</para>
    /// </summary>
    sealed public class BundleLoadManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private BundleLoadManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public BundleLoadManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new BundleLoadManager();
                }
                return m_instance;
            }
        }

        private BundleLoadManager()
        {
            if (m_instance != null)
            {
                throw new Exception("BundleLoadManager是单例，请使用BundleLoadManager.instance来获取其实例！");
            }
            m_instance = this;
            init();
        }

        /// <summary>
        /// 已加载资源的缓存
        /// </summary>
        private Dictionary<string, BundleLoadInfo> cacheDic;

        /// <summary>
        /// 等待队列，数组下标就是优先级
        /// </summary>
        private HashList<string, BundleLoadInfo>[] waitLists;

        /// <summary>
        /// 加载队列
        /// </summary>
        private HashList<string, BundleLoadInfo> loadingList;

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            m_isIdle = true;
            cacheDic = new Dictionary<string, BundleLoadInfo>();
            loadingList = new HashList<string, BundleLoadInfo>();

            int len = Enum.GetValues(typeof(LoadPriority)).Length;
            waitLists = new HashList<string, BundleLoadInfo>[len];
            for (int i = 0; i < len; i++)
            {
                waitLists[i] = new HashList<string, BundleLoadInfo>();
            }
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            if (m_isIdle == false)
            {
                m_isIdle = true;
                EnterFrame.instance.removeEnterFrame(enterFrame);

                for (int i = 0, len = loadingList.count; i < len; i++)
                {
                    WWW www = loadingList.getValueAt(i).www;
                    if (www != null)
                    {
                        if (www.assetBundle != null)
                        {
                            www.assetBundle.Unload(false);
                        }
                        www.Dispose();
                        www = null;
                    }
                }
            }
            loadingList.clear();

            foreach(string str in cacheDic.Keys)
            {
                deleteCacheItem(str);
            }
            cacheDic.Clear();

            for (int i = 0, len = waitLists.Length; i < len; i++)
            {
                waitLists[i].clear();
            }
        }

        //=====================================================================
        /// <summary>
        /// 加载的总依赖信息表
        /// </summary>
        public AssetBundleManifest mainfest
        {
            get
            {
                return m_mainfest;
            }
        }
        private AssetBundleManifest m_mainfest;

        /// <summary>
        /// 获取依赖列表(其中包括自身,排在最后一个)
        /// </summary>
        /// <returns></returns>
        public string[] getAllDependencies(string fullName)
        {
            string[] dependencies = new string[] { fullName };
            if (m_mainfest != null)
            {
                m_mainfest.GetAllDependencies(fullName).CopyTo(dependencies, 0);
            }
            return dependencies;
        }

        /// <summary>
        /// 当前是否闲置
        /// </summary>
        public bool isIdle
        {
            get
            {
                return m_isIdle;
            }
        }
        private bool m_isIdle;

        /// <summary>
        /// 资源的加载状态
        /// <para>【注意】需要便利一些列表，在不必要的时候少用</para>
        /// </summary>
        /// <param name="str">fullName+Version</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus getLoadStatus(string str)
        {
            if (cacheDic.ContainsKey(str))
            {
                return LoadStatus.loaded;
            }
            if(getInLoadingList(str) != null)
            {
                return LoadStatus.loading;
            }
            if (getInWaitList(str) != null)
            {
                return LoadStatus.wait;
            }
            return LoadStatus.none;
        }

        /// <summary>
        /// 从等待队列中获取
        /// </summary>
        /// <param name="str">fullName+Version</param>
        /// <returns></returns>
        private BundleLoadInfo getInWaitList(string str)
        {
            for (int i = 0, len = waitLists.Length; i < len; i++ )
            {
                if (waitLists[i].containsKey(str))
                {
                    return waitLists[i].getValue(str);
                }
            }
            return null;
        }

        /// <summary>
        /// 从当前加载队列中获取
        /// </summary>
        /// <param name="str">fullName+Version</param>
        /// <returns></returns>
        private BundleLoadInfo getInLoadingList(string str)
        {
            if (loadingList.containsKey(str))
            {
                return loadingList.getValue(str);
            }
            return null;
        }

        /// <summary>
        /// 根据优先级在等待队列中删除并返回第一条信息
        /// </summary>
        /// <returns></returns>
        private BundleLoadInfo deleteFirstInInWaitList()
        {
            for (int i = 0, len = waitLists.Length; i < len; i++)
            {
                if (waitLists[i].count != 0)
                {
                    return waitLists[i].removeAt(0);
                }
            }
            return null;
        }

        /// <summary>
        /// 加载资源，如果资源已经加载过，则直接从缓存中获取
        /// </summary>
        /// <param name="fullName">全名</param>
        /// <param name="version">版本号</param>
        /// <param name="priority">优先级</param>
        /// <param name="loadStart">加载开始前执行的方法</param>
        /// <param name="loadProgress">加载开始后且在结束前每帧执行的方法</param>
        /// <param name="loadEnd">加载结束后执行的方法</param>
        /// <param name="loadFail">加载失败后执行的方法</param>
        public void addLoad(string fullName, int version, LoadPriority priority = LoadPriority.two,
            LoadFunctionDele loadStart = null, LoadFunctionDele loadProgress = null, 
            LoadFunctionDele loadEnd = null, LoadFunctionDele loadFail = null)
        {
            BundleLoadInfo loadInfo = new BundleLoadInfo();
            loadInfo.fullName = fullName;
            loadInfo.version = version;
            loadInfo.priority = priority;
            loadInfo.loadStart = loadStart;
            loadInfo.loadProgress = loadProgress;
            loadInfo.loadEnd = loadEnd;
            loadInfo.loadFail = loadFail;
            addLoad(loadInfo);
        }

        /// <summary>
        /// 加载资源，如果资源已经加载过，则直接从缓存中获取
        /// </summary>
        /// <param name="loadInfo">加载资源的相关信息</param>
        public void addLoad(BundleLoadInfo loadInfo)
        {
            if (string.IsNullOrEmpty(loadInfo.fullName))
            {
                throw new Exception("BundleLoadManager.instance.load()的loadInfo的fullName不能为空");
            }
            if (loadInfo.version == 0)
            {
                throw new Exception("BundleLoadManager.instance.load()的loadInfo的version未设置");
            }

            string fullName_version = loadInfo.fullName + loadInfo.version;
            string[] dependencies = getAllDependencies(fullName_version);
            BundleLoadInfo newInfo;
            for (int i = 0, len = dependencies.Length; i < len; i++ )
            {
                newInfo = loadInfo.clone();
                newInfo.fullName = dependencies[i];
                addLoadItem(newInfo);
            }
        }

        /// <summary>
        /// 加载具体的项
        /// </summary>
        /// <param name="loadInfo"></param>
        private void addLoadItem(BundleLoadInfo loadInfo)
        {
            string fullName_version = loadInfo.fullName + loadInfo.version;
            //已经在缓存中
            if (cacheDic.ContainsKey(fullName_version))
            {
                if (loadInfo.loadStart != null)
                {
                    loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName, loadInfo.version));
                }
                if (loadInfo.loadEnd != null)
                {
                    loadInfo.loadEnd(LoadData.getLoadData(loadInfo.fullName, loadInfo.version, 1,
                        null, cacheDic[fullName_version].assetBundle));
                }
                return;
            }

            //已经在加载过程中
            BundleLoadInfo info = getInLoadingList(fullName_version);
            if (info != null)
            {
                if (loadInfo.loadStart != null)
                {
                    loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName, loadInfo.version));
                }
                delegateAddition(info.loadProgress, loadInfo.loadProgress);
                delegateAddition(info.loadEnd, loadInfo.loadEnd);
                delegateAddition(info.loadFail, loadInfo.loadFail);
                return;
            }

            //已经在等待列表中
            info = getInWaitList(fullName_version);
            if (info != null)
            {
                //如果优先级不一致，则转为优先级较高的（即数字小的）
                if ((int)info.priority > (int)loadInfo.priority)
                {
                    waitLists[(int)loadInfo.priority].add(fullName_version, info);
                    waitLists[(int)info.priority].remove(fullName_version);
                    info.priority = loadInfo.priority;
                }
                delegateAddition(info.loadStart, loadInfo.loadStart);
                delegateAddition(info.loadProgress, loadInfo.loadProgress);
                delegateAddition(info.loadEnd, loadInfo.loadEnd);
                delegateAddition(info.loadFail, loadInfo.loadFail);
                return;
            }

            //添加到等待列表中
            waitLists[(int)loadInfo.priority].add(fullName_version, loadInfo);
            loadNext();
        }

        /// <summary>
        /// LoadFunctionDele加法，将dele2加到dele1上
        /// </summary>
        /// <param name="dele1"></param>
        /// <param name="dele2"></param>
        private LoadFunctionDele delegateAddition(LoadFunctionDele dele1, LoadFunctionDele dele2)
        {
            if (dele1 != null)
            {
                dele1 += dele2;
            }
            else
            {
                dele1 = dele2;
            }
            return dele1;
        }

        /// <summary>
        /// 开始下一个加载
        /// </summary>
        private void loadNext()
        {
            //当前正在加载数目已经达到最大量
            if (loadingList.count >= LoadConstant.MAX_LOADERS)
            {
                return;
            }

            BundleLoadInfo loadInfo = deleteFirstInInWaitList();
            //等待列表已空
            if (loadInfo == null)
            {
                //正在加载的列表也已空
                if (loadingList.count == 0)
                {
                    m_isIdle = true;
                    EnterFrame.instance.removeEnterFrame(enterFrame);
                }
                return;
            }

            //开始加载
            if (m_isIdle)
            {
                m_isIdle = false;
                EnterFrame.instance.addEnterFrame(enterFrame);
            }
            loadingList.add(loadInfo.fullName + loadInfo.version, loadInfo);
            if (loadInfo.loadStart != null)
            {
                loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName, loadInfo.version));
            }

            string url = Application.persistentDataPath + "/" + loadInfo.fullName + loadInfo.version;
            if (File.Exists(url))
            {
                url = LoadConstant.LOCAL_TITLE + url;
            }
            else
            {
                url = LoadConstant.CDN + loadInfo.fullName;

                //删除本地的过期版本
                if (LoadConstant.DELETE_OLD_VERSION)
                {
                    string[] files = Directory.GetFiles(Application.persistentDataPath,
                        getFileName(loadInfo.fullName) + "*", SearchOption.AllDirectories);
                    for (int i = 0, len = files.Length; i < len; i++ )
                    {
                        File.Delete(files[i]);
                        Debug.Log("删除了本地过期文件：" + files[i]);
                    }
                }
            }
            loadInfo.www = new WWW(url);
        }

        /// <summary>
        /// 获取文件名
        /// </summary>
        /// <returns></returns>
        private string getFileName(string fullName)
        {
            int lastIndexOf = fullName.LastIndexOf("\\");
            return fullName.Substring(lastIndexOf + 1, fullName.Length - 1 - lastIndexOf);
        }

        /// <summary>
        /// 帧频事件
        /// </summary>
        private void enterFrame()
        {
            WWW www;
            BundleLoadInfo loadInfo;

            for (int i = 0; i < loadingList.count; i++)
            {
                loadInfo = loadingList.getValueAt(i);
                www = loadInfo.www;

                //加载失败
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogWarning(loadInfo.fullName + loadInfo.version + "加载失败：" + www.error);
                    if (loadInfo.loadFail != null)
                    {
                        loadInfo.loadFail(LoadData.getLoadData(loadInfo.fullName, loadInfo.version, www.progress, www.error));
                    }
                    www.Dispose();
                    www = null;
                    loadingList.removeAt(i);
                    i--;
                    loadNext();
                    continue;
                }

                //加载完成
                if (www.isDone)
                {
                    Debug.Log(loadInfo.fullName + loadInfo.version + "加载完成");
                    store(loadInfo);
                    if (loadInfo.loadEnd != null)
                    {
                        loadInfo.loadEnd(LoadData.getLoadData(loadInfo.fullName, loadInfo.version, 1, null, www.assetBundle));
                    }
                    www.Dispose();
                    www = null;
                    loadingList.removeAt(i);
                    i--;
                    loadNext();
                    continue;
                }

                //加载进度
                if (loadInfo.loadProgress != null)
                {
                    loadInfo.loadProgress(LoadData.getLoadData(loadInfo.fullName, loadInfo.version, www.progress));
                }
            }
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        private void store(BundleLoadInfo loadInfo)
        {
            WWW www = loadInfo.www;
            if(!www.url.Contains(LoadConstant.LOCAL_TITLE))
            {
                File.WriteAllBytes(loadInfo.fullName + loadInfo.version, www.bytes);
            }
            BundleLoadInfo newInfo = new BundleLoadInfo();
            newInfo.fullName = loadInfo.fullName;
            newInfo.version = loadInfo.version;
            newInfo.loadProgressNum = 1;
            newInfo.assetBundle = www.assetBundle;
            cacheDic.Add(newInfo.fullName + newInfo.version, newInfo);
        }

        /// <summary>
        /// 停止加载或停止等待加载，并移除
        /// <param>【注意】本方法会停止当前所有地方对本资源的加载，慎用！！！</param>
        /// </summary>
        /// <param name="fullName_version">fullName+Version</param>
        /// <param name="stopIfLoading">如果已经开始了对本资源的加载，是否仍要强制停止加载</param>
        /// <returns>true：已停止加载或停止等待加载； false：当前已经开始加载，且未停止</returns>
        public bool removeLoad(string fullName_version, bool stopIfLoading = false)
        {
            BundleLoadInfo loadInfo = getInLoadingList(fullName_version);
            if (loadInfo != null)
            {
                if (!stopIfLoading)
                {
                    return false;
                }
                Debug.Log(loadInfo.fullName + loadInfo.version + "在加载过程中被终止");
                loadInfo.www.Dispose();
                loadInfo.www = null;
                loadingList.remove(fullName_version);
                loadNext();
                return true;
            }

            loadInfo = getInWaitList(fullName_version);
            if (loadInfo != null)
            {
                Debug.Log(loadInfo.fullName + loadInfo.version + "在等待加载过程中被终止");
                waitLists[(int)loadInfo.priority].remove(fullName_version);
            }
            return true;
        }

        /// <summary>
        /// 清除指定的缓存
        /// </summary>
        /// <param name="fullName_version">fullName+Version</param>
        /// <returns>true:存在该缓存并且清除 false:不存在该缓存</returns>
        public bool deleteCacheItem(string fullName_version)
        {
            if (cacheDic.ContainsKey(fullName_version))
            {
                cacheDic[fullName_version].assetBundle.Unload(false);
                cacheDic.Remove(fullName_version);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 插队加载，每次会将此加载添加到当前优先级队列的最前面
        /// </summary>
        /// <param name="fullName">名称</param>
        /// <param name="version">版本号</param>
        /// <param name="priority">优先级</param>
        /// <param name="loadStart">加载开始前执行的方法</param>
        /// <param name="loadProgress">加载开始后且在结束前每帧执行的方法</param>
        /// <param name="loadEnd">加载结束后执行的方法</param>
        /// <param name="loadFail">加载失败后执行的方法</param>
        public void insertLoad(string fullName, int version, LoadPriority priority = LoadPriority.zero,
            LoadFunctionDele loadStart = null, LoadFunctionDele loadProgress = null, 
            LoadFunctionDele loadEnd = null, LoadFunctionDele loadFail = null)
        {
            BundleLoadInfo loadInfo = new BundleLoadInfo();
            loadInfo.fullName = fullName;
            loadInfo.version = version;
            loadInfo.priority = priority;
            loadInfo.loadStart = loadStart;
            loadInfo.loadProgress = loadProgress;
            loadInfo.loadEnd = loadEnd;
            loadInfo.loadFail = loadFail;
            waitLists[(int)priority].insert(0, loadInfo.fullName + loadInfo.version, loadInfo);
            addLoad(loadInfo);
        }

    }
}
