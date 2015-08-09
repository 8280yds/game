using System;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载管理类，能够自动加载依赖包，并在加载完成后自动解压获得游戏资源
    /// </summary>
    sealed public class LoadManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private LoadManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public LoadManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new LoadManager();
                }
                return m_instance;
            }
        }

        private LoadManager()
        {
            m_instance = this;
            init();
        }

        /// <summary>
        /// 加载列表
        /// </summary>
        private Dictionary<string, LoadInfo> loadDic;

        /// <summary>
        /// 解压列表
        /// </summary>
        private HashList<string, LoadInfo> unZipList;

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            loadDic = new Dictionary<string, LoadInfo>();
            unZipList = new HashList<string, LoadInfo>();
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            EnterFrame.instance.removeEnterFrame(enterFrame);
            loadDic.Clear();
            unZipList.clear();
        }

        //=====================================================================
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
        /// <param name="unZipStart">解压开始前执行的方法</param>
        /// <param name="unZipProgress">解压开始后且在结束前每帧执行的方法</param>
        /// <param name="unZipEnd">解压完毕后执行的方法</param>
        /// <param name="unZipFail">解压失败后执行的方法</param>
        public void addLoad(string fullName, LoadPriority priority = LoadPriority.two, LoadType loadType = LoadType.local,
            LoadFunctionDele loadStart = null, LoadFunctionDele loadProgress = null, 
            LoadFunctionDele loadEnd = null, LoadFunctionDele loadFail = null,
            LoadFunctionDele unZipStart = null, LoadFunctionDele unZipProgress = null,
            LoadFunctionDele unZipEnd = null, LoadFunctionDele unZipFail = null)
        {
            LoadInfo loadInfo = new LoadInfo();
            loadInfo.fullName = fullName;
            loadInfo.priority = priority;
            loadInfo.loadType = loadType;
            loadInfo.loadStart = loadStart;
            loadInfo.loadProgress = loadProgress;
            loadInfo.loadEnd = loadEnd;
            loadInfo.loadFail = loadFail;
            loadInfo.unZipStart = unZipStart;
            loadInfo.unZipProgress = unZipProgress;
            loadInfo.unZipEnd = unZipEnd;
            loadInfo.unZipFail = unZipFail;
            addLoad(loadInfo);
        }

        /// <summary>
        /// 加载资源，如果资源已经加载过，则直接从缓存中获取
        /// </summary>
        /// <param name="loadInfo">加载资源的相关信息</param>
        public void addLoad(LoadInfo loadInfo)
        {
            if (string.IsNullOrEmpty(loadInfo.fullName))
            {
                throw new Exception("LoadManager.instance.load()的fullName不能为空");
            }
            if (loadDic.ContainsKey(loadInfo.fullName))
            {
                LoadInfo info = loadDic[loadInfo.fullName];
                delegateAddition(info.loadStart, loadInfo.loadStart);
                delegateAddition(info.loadProgress, loadInfo.loadProgress);
                delegateAddition(info.loadEnd, loadInfo.loadEnd);
                delegateAddition(info.loadFail, loadInfo.loadFail);
                delegateAddition(info.unZipStart, loadInfo.unZipStart);
                delegateAddition(info.unZipProgress, loadInfo.unZipProgress);
                delegateAddition(info.unZipEnd, loadInfo.unZipEnd);
                delegateAddition(info.unZipFail, loadInfo.unZipFail);
            }
            else
            {
                if (unZipList.containsKey(loadInfo.fullName))
                {
                    LoadInfo info = unZipList.getValue(loadInfo.fullName);
                    if (loadInfo.loadStart != null)
                    {
                        loadInfo.loadStart(LoadData.getLoadData(loadInfo.fullName));
                    }
                    if (loadInfo.loadEnd != null)
                    {
                        loadInfo.loadEnd(LoadData.getLoadData(loadInfo.fullName, 1, null, info.assetBundle));
                    }
                    if (loadInfo.unZipStart != null)
                    {
                        loadInfo.unZipStart(LoadData.getLoadData(loadInfo.fullName, 1));
                    }
                    delegateAddition(info.unZipProgress, loadInfo.unZipProgress);
                    delegateAddition(info.unZipEnd, loadInfo.unZipEnd);
                    delegateAddition(info.unZipFail, loadInfo.unZipFail);
                    return;
                }
                else
                {
                    loadDic.Add(loadInfo.fullName, loadInfo);
                }
            }
            BundleLoadInfo newInfo = loadInfo.getBundleLoadInfo();
            newInfo.loadEnd = loadInfo.loadEnd + this.loadEnd;
            BundleLoadManager.instance.addLoad(newInfo);
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
        /// 加载结束
        /// </summary>
        /// <param name="loadData"></param>
        private void loadEnd(LoadData loadData)
        {
            if (loadDic.ContainsKey(loadData.fullName))
            {
                LoadInfo info = loadDic[loadData.fullName];
                info.assetBundle = loadData.assetBundle;
            }

            List<LoadInfo> infos = new List<LoadInfo>(loadDic.Values);
            for (int i = 0, len = infos.Count; i < len; i++)
            {
                if (BundleLoadManager.instance.isLoadFinish(infos[i].fullName))
                {
                    unZipStart(infos[i]);
                }
            }
        }

        /// <summary>
        /// 开始解压
        /// </summary>
        /// <param name="loadData"></param>
        private void unZipStart(LoadData loadData)
        {
            LoadInfo loadInfo = loadDic[loadData.fullName];
            loadInfo.request = loadData.assetBundle.LoadAllAssetsAsync();

            unZipList.add(loadData.fullName, loadInfo);
            loadDic.Remove(loadData.fullName);
            EnterFrame.instance.addEnterFrame(enterFrame);

            if (loadInfo.unZipStart != null)
            {
                loadInfo.unZipStart(loadData);
            }
        }

        /// <summary>
        /// 解压完毕
        /// </summary>
        /// <param name="loadData"></param>
        private void unZipEnd(LoadData loadData)
        {
            LoadInfo loadInfo = unZipList.remove(loadData.fullName);
            Debug.Log(loadInfo.fullName + "解压完成");

            if (unZipList.count == 0)
            {
                EnterFrame.instance.removeEnterFrame(enterFrame);
            }

            if (loadInfo.unZipEnd != null)
            {
                loadInfo.unZipEnd(loadData);
            }
        }

        /// <summary>
        /// 帧频事件
        /// </summary>
        private void enterFrame()
        {
            AssetBundleRequest request;
            LoadInfo loadInfo;

            for (int i = 0; i < unZipList.count; i++)
            {
                loadInfo = unZipList.getValueAt(i);
                request = loadInfo.request;

                //解压完成
                if (request.isDone)
                {
                    unZipEnd(LoadData.getLoadData(loadInfo.fullName, 1, null, null, 1, request.allAssets));
                    i--;
                    continue;
                }

                //解压进度
                if (loadInfo.unZipProgress != null)
                {
                    loadInfo.unZipProgress(LoadData.getLoadData(loadInfo.fullName, 1, null, null, request.progress));
                }
            }
        }

        /// <summary>
        /// 停止加载或停止等待加载，并移除，如果已经在解压中也会终止解压
        /// <param>【注意】本方法会停止当前所有地方对本资源的加载，慎用！！！</param>
        /// </summary>
        /// <param name="fullName">fullName</param>
        /// <param name="stopIfLoading">如果已经开始了对本资源的加载，是否仍要强制停止加载</param>
        /// <returns>true：已停止加载或停止等待加载； false：当前已经开始加载，且未停止</returns>
        public bool removeLoad(string fullName, bool stopIfLoading = false)
        {
            return BundleLoadManager.instance.removeLoad(fullName, stopIfLoading);
        }

        /// <summary>
        /// 从加载和解压列表中移除
        /// <param>【注意】请调用removeLoad方法，因为此方法无法保证真正停止加载，只是从列表中移除</param>
        /// </summary>
        /// <param name="fullName"></param>
        public void removeInLists(string fullName)
        {
            loadDic.Remove(fullName);

            LoadInfo loadInfo = unZipList.remove(fullName);
            if (loadInfo != null)
            {
                Debug.Log(fullName + "在解压过程中被终止");
                if (loadInfo.unZipFail != null)
                {
                    loadInfo.unZipFail(LoadData.getLoadData(fullName, 1));
                }
            }
            if (unZipList.count == 0)
            {
                EnterFrame.instance.removeEnterFrame(enterFrame);
            }
        }

    }
}
