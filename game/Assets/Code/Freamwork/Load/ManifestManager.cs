using System;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;
using System.IO;

namespace Freamwork
{
    sealed public class ManifestManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private ManifestManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public ManifestManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new ManifestManager();
                }
                return m_instance;
            }
        }

        private ManifestManager()
        {
            m_instance = this;
        }

        //=====================================================================
        /// <summary>
        /// ManifestVO列表
        /// </summary>
        private Dictionary<string, ManifestVO> dic;

        /// <summary>
        /// 是否已经初始化完毕
        /// </summary>
        public bool isInit
        {
            get
            {
                return m_isInit;
            }
        }
        private bool m_isInit = false;

        /// <summary>
        /// 获取所有资源的名称
        /// </summary>
        public List<string> getAllFullName
        {
            get
            {
                return new List<string>(dic.Keys);
            }
        }

        private LoadFunctionDele m_loadStart, m_loadProgress, m_loadEnd, m_loadFail, m_unZipStart,
            m_unZipProgress, m_unZipEnd;

        private WWW www;
        private AssetBundleRequest request;

        /// <summary>
        /// 初始化
        /// </summary>
        public void init(string url, LoadFunctionDele loadStart = null, LoadFunctionDele loadProgress = null,
            LoadFunctionDele loadEnd = null, LoadFunctionDele loadFail = null, LoadFunctionDele unZipStart = null,
            LoadFunctionDele unZipProgress = null, LoadFunctionDele unZipEnd = null)
        {
            if (isInit)
            {
                throw new Exception("ManifestManager试图重复初始化，初始化将清空所有Manifest信息" +
                    "重新从网络加载，如果确信要这么做请在clear()后调用此方法");
            }

            Debug.Log("ManifestManager开始初始化……");
            BundleLoadManager.instance.clear();

            m_loadStart = loadStart;
            m_loadProgress = loadProgress;
            m_loadEnd = loadEnd;
            m_loadFail = loadFail;
            m_unZipStart = unZipStart;
            m_unZipProgress = unZipProgress;
            m_unZipEnd = unZipEnd;

            EnterFrame.instance.addEnterFrame(enterframe);
            www = new WWW(url);
            if (m_loadStart != null)
            {
                m_loadStart(LoadData.getLoadData(LoadConstant.MANIFEST_FILE));
            }
        }

        /// <summary>
        /// 帧频事件
        /// </summary>
        private void enterframe()
        {
            if (www != null)
            {
                //加载失败
                if (!string.IsNullOrEmpty(www.error))
                {
                    Debug.LogWarning(LoadConstant.MANIFEST_FILE + "加载失败：" + www.error);

                    LoadFunctionDele _loadFail = m_loadFail;
                    WWW _www = www;
                    clear();
                    www = null;

                    if (_loadFail != null)
                    {
                        _loadFail(LoadData.getLoadData(LoadConstant.MANIFEST_FILE, _www.progress, _www.error));
                    }
                    _www.Dispose();
                    return;
                }

                //加载完成
                if (www.isDone)
                {
                    Debug.Log(LoadConstant.MANIFEST_FILE + "加载完成");

                    //存入本地
                    if (www.url.Contains(LoadConstant.CDN))
                    {
                        File.WriteAllBytes(LoadConstant.localFilesPath + "/" + LoadConstant.MANIFEST_FILE, www.bytes);
                    }

                    if (m_loadEnd != null)
                    {
                        m_loadEnd(LoadData.getLoadData(LoadConstant.MANIFEST_FILE, 1));
                    }

                    request = www.assetBundle.LoadAllAssetsAsync<TextAsset>();
                    www.Dispose();
                    www = null;

                    if (m_unZipStart != null)
                    {
                        m_unZipStart(LoadData.getLoadData(LoadConstant.MANIFEST_FILE, 1));
                    }
                    return;
                }

                //加载进度
                if (m_loadProgress != null)
                {
                    m_loadProgress(LoadData.getLoadData(LoadConstant.MANIFEST_FILE, www.progress));
                }
            }
            else if (request != null)
            {
                //解压完成
                if (request.isDone)
                {
                    Debug.Log(LoadConstant.MANIFEST_FILE + "解压完成");
                    EnterFrame.instance.removeEnterFrame(enterframe);

                    analysis(request.allAssets[0] as TextAsset);
                    m_isInit = true;

                    if(m_unZipEnd != null)
                    {
                        m_unZipEnd(LoadData.getLoadData(LoadConstant.MANIFEST_FILE, 1, null, null, 1, request.allAssets));
                    }
                    return;
                }

                //解压进度
                if (m_unZipProgress != null)
                {
                    m_unZipProgress(LoadData.getLoadData(LoadConstant.MANIFEST_FILE, 1, null, null, request.progress));
                }
            }
        }

        /// <summary>
        /// 解析数据
        /// </summary>
        /// <param name="textAsset"></param>
        private void analysis(TextAsset textAsset)
        {
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(textAsset.text);
            XmlNodeList xmlNodeList = xmlDocument.SelectSingleNode("manifest").ChildNodes;

            dic = new Dictionary<string, ManifestVO>();
            ManifestVO vo;
            XmlElement xmlelement;

            for(int i=0, len = xmlNodeList.Count; i<len; i++)
            {
                xmlelement = (XmlElement)xmlNodeList[i];
                vo = new ManifestVO();
                vo.name = xmlelement.LocalName;
                vo.assets = xmlelement.GetAttribute("assets");
                vo.crc = xmlelement.GetAttribute("crc");
                vo.deps = xmlelement.GetAttribute("deps");
                dic.Add(vo.name, vo);
            }
        }

        /// <summary>
        /// 根据fullName获取ManifestVO
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public ManifestVO getManifestVO(string fullName)
        {
            return dic[fullName];
        }

        /// <summary>
        /// 获取依赖列表(不包含本身)
        /// </summary>
        /// <param name="fullName"></param>
        /// <returns></returns>
        public List<string> getAllDependencies(string fullName)
        {
            ManifestVO vo = dic[fullName];
            if (vo.deps.Length == 0)
            {
                return new List<string>();
            }
            List<string> list = new List<string>(vo.deps.Split(new char[] { ',' }));
            for (int i = 0, len = list.Count; i < len; i++)
            {
                list.AddRange(getAllDependencies(list[i]));
            }
            return list;
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            if (dic != null)
            {
                dic.Clear();
                dic = null;
            }
            m_isInit = false;
            EnterFrame.instance.removeEnterFrame(enterframe);

            m_loadStart = null;
            m_loadProgress = null;
            m_loadEnd = null;
            m_loadFail = null;
            m_unZipStart = null;
            m_unZipProgress = null;
            m_unZipEnd = null;
        }

    }
}