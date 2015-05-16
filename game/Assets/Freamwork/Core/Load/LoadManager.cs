using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载管理类，具有排队，优先级，超时检测等功能，
    /// 并能根绝情况确定是否需要到网络加载
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
            if (m_instance != null)
            {
                throw new Exception("LoadManager是单例，请使用LoadManager.instance来获取其实例！");
            }
            m_instance = this;
            init();
        }

        /// <summary>
        /// 已加载资源的缓存
        /// </summary>
        private Dictionary<string, LoadInfo> cacheDic;

        /// <summary>
        /// 加载队列，数组下标就是优先级
        /// </summary>
        private HashList<string, LoadInfo>[] waitLists;

        /// <summary>
        /// 加载中的队列
        /// </summary>
        private HashList<string, LoadInfo> loadingList;

        /// <summary>
        /// 初始化
        /// </summary>
        private void init()
        {
            m_isIdle = true;
            cacheDic = new Dictionary<string, LoadInfo>();
            loadingList = new HashList<string, LoadInfo>();

            int len = Enum.GetValues(typeof(LoadPriority)).Length;
            waitLists = new HashList<string, LoadInfo>[len];
            for (int i = 0; i < len; i++)
            {
                waitLists[i] = new HashList<string, LoadInfo>();
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
                        www.Dispose();
                        www = null;
                    }
                }
            }

            cacheDic.Clear();
            loadingList.clear();

            for (int i = 0, len = waitLists.Length; i < len; i++)
            {
                waitLists[i].clear();
            }
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
        /// 从等待队列中获取
        /// </summary>
        /// <param name="str">fullName+Version</param>
        /// <returns></returns>
        private LoadInfo getInWaitList(string str)
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
        private LoadInfo getInLoadingList(string str)
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
        private LoadInfo deleteFirstInInWaitList()
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
        /// <param name="loadInfo">加载资源的相关信息</param>
        public void load(LoadInfo loadInfo)
        {
            //if (string.IsNullOrEmpty(loadInfo.path))
            //{
            //    throw new Exception("LoadManager.instance.load()的loadInfo参数的path不能为空");
            //}
            if (string.IsNullOrEmpty(loadInfo.fileName))
            {
                throw new Exception("LoadManager.instance.load()的loadInfo参数的fileName不能为空");
            }
            if (loadInfo.version == 0)
            {
                throw new Exception("LoadManager.instance.load()的loadInfo参数的version未设置");
            }

            string fullName_version = loadInfo.fullName + loadInfo.version;
            //已经在缓存中
            if (cacheDic.ContainsKey(fullName_version))
            {
                if (loadInfo.loadStart != null)
                {
                    loadInfo.loadStart(getStartLoadInfo(loadInfo.path, loadInfo.fileName, loadInfo.version));
                }
                if (loadInfo.loadEnd != null)
                {
                    loadInfo.loadEnd(getEndLoadInfo(fullName_version));
                }
                return;
            }

            //已经在加载过程中
            LoadInfo info = getInLoadingList(fullName_version);
            if (info != null)
            {
                if (loadInfo.loadStart != null)
                {
                    loadInfo.loadStart(getStartLoadInfo(loadInfo.path, loadInfo.fileName, loadInfo.version));
                }
                if (loadInfo.loadProgress != null)
                {
                    info.loadProgress = loadInfo.loadProgress + info.loadProgress;
                }
                if (loadInfo.loadEnd != null)
                {
                    info.loadEnd = loadInfo.loadEnd + info.loadEnd;
                }
                if (loadInfo.loadFail != null)
                {
                    info.loadFail = loadInfo.loadFail + info.loadFail;
                }
                return;
            }

            //已经在等待列表中
            info = getInWaitList(fullName_version);
            if (info != null)
            {
                if (loadInfo.loadStart != null)
                {
                    info.loadStart = loadInfo.loadStart + info.loadStart;
                }
                if (loadInfo.loadProgress != null)
                {
                    info.loadProgress = loadInfo.loadProgress + info.loadProgress;
                }
                if (loadInfo.loadEnd != null)
                {
                    info.loadEnd = loadInfo.loadEnd + info.loadEnd;
                }
                if (loadInfo.loadFail != null)
                {
                    info.loadFail = loadInfo.loadFail + info.loadFail;
                }
                return;
            }

            //添加到等待列表中
            waitLists[(int)loadInfo.priority].add(fullName_version, loadInfo);
            loadNext();
        }

        private LoadInfo getStartLoadInfo(string path, string fileName, int version)
        {
            LoadInfo info = new LoadInfo();
            info.path = path;
            info.fileName = fileName;
            info.version = version;
            return info;
        }

        private LoadInfo getProgressInfo(string path, string fileName, int version, float progress)
        {
            LoadInfo info = new LoadInfo();
            info.path = path;
            info.fileName = fileName;
            info.version = version;
            info.progress = progress;
            return info;
        }

        private LoadInfo getEndLoadInfo(string fullName_version)
        {
            return cacheDic[fullName_version].clone();
        }

        private LoadInfo getFailLoadInfo(string path, string fileName, int version, string error)
        {
            LoadInfo info = new LoadInfo();
            info.path = path;
            info.fileName = fileName;
            info.version = version;
            info.error = error;
            return info;
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

            LoadInfo loadInfo = deleteFirstInInWaitList();
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
                loadInfo.loadStart(getStartLoadInfo(loadInfo.path, loadInfo.fileName, loadInfo.version));
            }

            string url = Application.persistentDataPath + "/" + loadInfo.fileName + loadInfo.version;
            if (File.Exists(url))
            {
                url = LoadConstant.LOCAL_TITLE + Application.persistentDataPath + loadInfo.fullName + loadInfo.version;
            }
            else
            {
                url = loadInfo.fullName;

                //删除本地的过期版本
                if (LoadConstant.DELETE_OLD_VERSION)
                {
                    string[] files = Directory.GetFiles(Application.persistentDataPath + loadInfo.path,
                        loadInfo.fileName + "*", SearchOption.TopDirectoryOnly);
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
        /// 帧频事件
        /// </summary>
        private void enterFrame()
        {
            WWW www;
            LoadInfo loadInfo;

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
                        loadInfo.loadFail(getFailLoadInfo(loadInfo.path, loadInfo.fileName, loadInfo.version, www.error));
                    }
                    www.Dispose();
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
                        loadInfo.loadEnd(getEndLoadInfo(loadInfo.fullName + loadInfo.version));
                    }
                    www.Dispose();
                    loadingList.removeAt(i);
                    i--;
                    loadNext();
                    continue;
                }

                //加载进度
                if (loadInfo.loadProgress != null)
                {
                    loadInfo.loadProgress(getProgressInfo(loadInfo.path, loadInfo.fileName, loadInfo.version, www.progress));
                }
            }
        }

        /// <summary>
        /// 存储数据
        /// </summary>
        private void store(LoadInfo loadInfo)
        {
            WWW www = loadInfo.www;
            if(!www.url.Contains(LoadConstant.LOCAL_TITLE))
            {
                File.WriteAllBytes(loadInfo.fullName + loadInfo.version, www.bytes);
            }
            LoadInfo newInfo = new LoadInfo();
            newInfo.path = loadInfo.path;
            newInfo.fileName = loadInfo.fileName;
            newInfo.version = loadInfo.version;
            newInfo.progress = 1;
            newInfo.assetBundle = www.assetBundle;
            cacheDic.Add(newInfo.fullName + newInfo.version, newInfo);
        }

    }
}
