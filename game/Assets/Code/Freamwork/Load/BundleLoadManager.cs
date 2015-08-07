using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// assetBundle加载管理类，具有排队，优先级，版本比对等功能
    /// <para>可以本地加载或网络加载</para>
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

            for (int i = 0, len = waitLists.Length; i < len; i++)
            {
                waitLists[i].clear();
            }

            clearAllCache();

            //强制清除并垃圾回收
            //Resources.UnloadUnusedAssets();
            //GC.Collect();
        }

        //=====================================================================
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
        /// <param name="fullName">fullName</param>
        /// <returns>LoadStatus</returns>
        public LoadStatus getLoadStatus(string fullName)
        {
            if (cacheDic.ContainsKey(fullName))
            {
                return LoadStatus.loaded;
            }
            if (getInLoadingList(fullName) != null)
            {
                return LoadStatus.loading;
            }
            if (getInWaitList(fullName) != null)
            {
                return LoadStatus.wait;
            }
            return LoadStatus.none;
        }

        /// <summary>
        /// 从等待队列中获取
        /// </summary>
        /// <param name="fullName">fullName</param>
        /// <returns></returns>
        private BundleLoadInfo getInWaitList(string fullName)
        {
            for (int i = 0, len = waitLists.Length; i < len; i++ )
            {
                if (waitLists[i].containsKey(fullName))
                {
                    return waitLists[i].getValue(fullName);
                }
            }
            return null;
        }

        /// <summary>
        /// 从当前加载队列中获取
        /// </summary>
        /// <param name="fullName">fullName</param>
        /// <returns></returns>
        private BundleLoadInfo getInLoadingList(string fullName)
        {
            if (loadingList.containsKey(fullName))
            {
                return loadingList.getValue(fullName);
            }
            return null;
        }

        /// <summary>
        /// 获取当前正在加载中的资源的名称列表
        /// </summary>
        /// <returns></returns>
        public List<string> getLoadingFullNames()
        {
            return loadingList.keys;
        }

        /// <summary>
        /// 正在加载中和等待中的数量总和
        /// </summary>
        /// <returns></returns>
        public int loadingAndWaitNum
        {
            get
            {
                int count = loadingList.count;
                for (int i = 0; i < waitLists.Length; i++)
                {
                    count += waitLists[i].count;
                }
                return count;
            }
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
        /// 资源及其依赖的所有资源是否加载完毕
        /// </summary>
        /// <param name="fullName">资源全名</param>
        /// <returns></returns>
        public bool isLoadFinish(string fullName)
        {
            List<string> dependencies = ManifestManager.instance.getAllDependencies(fullName);
            dependencies.Add(fullName);
            foreach (string name in dependencies)
            {
                if (!cacheDic.ContainsKey(name))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 加载资源，如果资源已经加载过，则直接从缓存中获取
        /// </summary>
        /// <param name="fullName">全名</param>
        /// <param name="priority">优先级</param>
        /// <param name="loadType">加载类型</param>
        /// <param name="loadStart">加载开始前执行的方法</param>
        /// <param name="loadProgress">加载开始后且在结束前每帧执行的方法</param>
        /// <param name="loadEnd">加载结束后执行的方法</param>
        /// <param name="loadFail">加载失败后执行的方法</param>
        public void addLoad(string fullName, LoadPriority priority = LoadPriority.two, LoadType loadType = LoadType.local,
            LoadFunctionDele loadStart = null, LoadFunctionDele loadProgress = null, 
            LoadFunctionDele loadEnd = null, LoadFunctionDele loadFail = null)
        {
            BundleLoadInfo loadInfo = new BundleLoadInfo();
            loadInfo.fullName = fullName;
            loadInfo.priority = priority;
            loadInfo.loadType = loadType;
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
                throw new Exception("BundleLoadManager.instance.load()的fullName不能为空");
            }

            List<string> dependencies = ManifestManager.instance.getAllDependencies(loadInfo.fullName);
            if (dependencies == null)
            {
                throw new Exception("未找到" + loadInfo.fullName + "的Manifest信息");
            }
            dependencies.Add(loadInfo.fullName);
            BundleLoadInfo newInfo;
            for (int i = 0, len = dependencies.Count; i < len; i++ )
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
            ManifestVO vo = ManifestManager.instance.getManifestVO(loadInfo.fullName);
            if (File.Exists(LoadConstant.localFilesPath + "/" + loadInfo.fullName + "@" + vo.crc))
            {
                if (loadInfo.loadType == LoadType.web)
                {
                    if (loadInfo.loadStart != null)
                    {
                        loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName));
                    }
                    if (loadInfo.loadEnd != null)
                    {
                        loadInfo.loadEnd(LoadData.getLoadData(loadInfo.fullName, 1));
                    }
                    return;
                }
                else
                {
                    loadInfo.loadType = LoadType.local;
                }
            }
            else
            {
                if (loadInfo.loadType == LoadType.local)
                {
                    Debug.LogWarning(loadInfo.fullName + "加载失败：" + LoadConstant.LOCAL_LOAD_ERROR);
                    if (loadInfo.loadStart != null)
                    {
                        loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName));
                    }
                    if (loadInfo.loadFail != null)
                    {
                        loadInfo.loadFail(LoadData.getLoadData(loadInfo.fullName, 0, LoadConstant.LOCAL_LOAD_ERROR));
                    }
                    return;
                }
                else
                {
                    loadInfo.loadType = LoadType.web;
                }
            }

            //已经在缓存中
            if (cacheDic.ContainsKey(loadInfo.fullName))
            {
                if (loadInfo.loadStart != null)
                {
                    loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName));
                }
                if (loadInfo.loadEnd != null)
                {
                    loadInfo.loadEnd(LoadData.getLoadData(loadInfo.fullName, 1, null, cacheDic[loadInfo.fullName].assetBundle));
                }
                return;
            }

            //已经在加载过程中
            BundleLoadInfo info = getInLoadingList(loadInfo.fullName);
            if (info != null)
            {
                if (loadInfo.loadStart != null)
                {
                    loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName));
                }
                delegateAddition(info.loadProgress, loadInfo.loadProgress);
                delegateAddition(info.loadEnd, loadInfo.loadEnd);
                delegateAddition(info.loadFail, loadInfo.loadFail);
                return;
            }

            //已经在等待列表中
            info = getInWaitList(loadInfo.fullName);
            if (info != null)
            {
                //如果优先级不一致，则转为优先级较高的（即数字小的）
                if ((int)info.priority > (int)loadInfo.priority)
                {
                    waitLists[(int)loadInfo.priority].add(loadInfo.fullName, info);
                    waitLists[(int)info.priority].remove(loadInfo.fullName);
                    info.priority = loadInfo.priority;
                }
                delegateAddition(info.loadStart, loadInfo.loadStart);
                delegateAddition(info.loadProgress, loadInfo.loadProgress);
                delegateAddition(info.loadEnd, loadInfo.loadEnd);
                delegateAddition(info.loadFail, loadInfo.loadFail);
                return;
            }

            //添加到等待列表中
            waitLists[(int)loadInfo.priority].add(loadInfo.fullName, loadInfo);
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
            loadingList.add(loadInfo.fullName, loadInfo);
            if (loadInfo.loadStart != null)
            {
                loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName));
            }

            string url;
            if (loadInfo.loadType == LoadType.local)
            {
                ManifestVO vo = ManifestManager.instance.getManifestVO(loadInfo.fullName);
                url = LoadConstant.LOCAL_TITLE + LoadConstant.localFilesPath + "/" + loadInfo.fullName + "@" + vo.crc;
            }
            else
            {
                url = LoadConstant.CDN + loadInfo.fullName;

                //删除本地的过期版本的资源
                if (LoadConstant.DELETE_OLD_VERSION)
                {
                    string[] files = Directory.GetFiles(LoadConstant.localFilesPath, 
                        loadInfo.fullName + "@*", SearchOption.AllDirectories);
                    for (int i = 0, len = files.Length; i < len; i++ )
                    {
                        File.Delete(files[i]);
                        Debug.Log(("删除了本地过期文件：" + files[i]).Replace("\\", "/"));
                    }
                }
            }
            loadInfo.www = new WWW(url);
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
                    Debug.LogWarning(loadInfo.fullName + "加载失败：" + www.error);
                    loadingList.removeAt(i);
                    i--;

                    if (loadInfo.loadFail != null)
                    {
                        loadInfo.loadFail(LoadData.getLoadData(loadInfo.fullName, www.progress, www.error));
                    }
                    www.Dispose();
                    www = null;
                    loadNext();
                    continue;
                }

                //加载完成
                if (www.isDone)
                {
                    Debug.Log(loadInfo.fullName + "加载完成");
                    loadingList.removeAt(i);
                    i--;

                    store(loadInfo);
                    if (loadInfo.loadEnd != null)
                    {
                        loadInfo.loadEnd(LoadData.getLoadData(loadInfo.fullName, 1, null, www.assetBundle));
                    }
                    www.Dispose();
                    www = null;
                    loadNext();
                    continue;
                }

                //加载进度
                if (loadInfo.loadProgress != null)
                {
                    loadInfo.loadProgress(LoadData.getLoadData(loadInfo.fullName, www.progress));
                }
            }
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        private void store(BundleLoadInfo loadInfo)
        {
            WWW www = loadInfo.www;
            if (www.url.Contains(LoadConstant.CDN))
            {
                ManifestVO vo = ManifestManager.instance.getManifestVO(loadInfo.fullName);
                File.WriteAllBytes(LoadConstant.localFilesPath + "/" + loadInfo.fullName + "@" + vo.crc, www.bytes);
            }
            BundleLoadInfo newInfo = new BundleLoadInfo();
            newInfo.fullName = loadInfo.fullName;
            newInfo.loadProgressNum = 1;
            newInfo.assetBundle = www.assetBundle;
            cacheDic.Add(newInfo.fullName, newInfo);
        }

        /// <summary>
        /// 停止加载或停止等待加载，并移除
        /// <param>【注意】本方法会停止当前所有地方对本资源的加载，慎用！！！</param>
        /// </summary>
        /// <param name="fullName">fullName</param>
        /// <param name="stopIfLoading">如果已经开始了对本资源的加载，是否仍要强制停止加载</param>
        /// <returns>true：已停止加载或停止等待加载； false：当前已经开始加载，且未停止</returns>
        public bool removeLoad(string fullName, bool stopIfLoading = false)
        {
            BundleLoadInfo loadInfo = getInLoadingList(fullName);
            if (loadInfo != null)
            {
                if (!stopIfLoading)
                {
                    return false;
                }

                Debug.Log(loadInfo.fullName + "在加载过程中被终止");
                if (loadInfo.loadFail != null)
                {
                    loadInfo.loadFail(LoadData.getLoadData(loadInfo.fullName, loadInfo.www.progress, 
                        LoadConstant.LOADING_STOP_ERROR));
                }
                loadInfo.www.Dispose();
                loadInfo.www = null;
                loadingList.remove(fullName);
                loadNext();
                LoadManager.instance.removeInLists(fullName);
                return true;
            }

            loadInfo = getInWaitList(fullName);
            if (loadInfo != null)
            {
                Debug.Log(loadInfo.fullName + "在等待加载过程中被终止");
                waitLists[(int)loadInfo.priority].remove(fullName);
            }
            LoadManager.instance.removeInLists(fullName);
            return true;
        }

        /// <summary>
        /// 删除一条缓存资源
        /// </summary>
        /// <param name="fullName">fullName</param>
        /// <returns>true:存在该缓存并且清除 false:不存在该缓存</returns>
        public bool deleteCacheItem(string fullName)
        {
            if (cacheDic.ContainsKey(fullName))
            {
                cacheDic[fullName].assetBundle.Unload(false);
                cacheDic.Remove(fullName);
                Resources.UnloadUnusedAssets();
                return true;
            }
            return false;
        }

        /// <summary>
        /// 清空缓存的资源
        /// </summary>
        public void clearAllCache()
        {
            foreach(BundleLoadInfo info in cacheDic.Values)
            {
                info.assetBundle.Unload(false);
                info.assetBundle = null;
            }
            cacheDic.Clear();
            Resources.UnloadUnusedAssets();
        }

    }
}
