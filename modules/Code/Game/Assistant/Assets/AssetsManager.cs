using Freamwork;
using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class AssetsManager
{
    static public AssetsManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new AssetsManager();
            }
            return m_instance;
        }
    }
    static private AssetsManager m_instance;

    private AssetsManager()
    {
        m_instance = this;
    }

    /// <summary>
    /// 清除所有帧频事件
    /// </summary>
    public void clear()
    {
        if (mainUIAssetsDic != null)
        {
            mainUIAssetsDic.Clear();
            mainUIAssetsDic = null;
        }
    }

    //======================================================
    /// <summary>
    /// 设置MainUI资源包
    /// </summary>
    public void setMainUIAssets(GameObject assetsGO)
    {
        if (mainUIAssetsDic != null)
        {
            throw new Exception("试图重复设置mainUI资源");
        }
        mainUIAssetsDic = new Dictionary<string, UnityEngine.Object>();

        Assets assets = assetsGO.GetComponent<Assets>();
        foreach (UnityEngine.Object obj in assets.assetsList)
        {
            mainUIAssetsDic.Add(obj.name, obj);
        }
    }
    private Dictionary<string, UnityEngine.Object> mainUIAssetsDic;

    /// <summary>
    /// 获取MainUI中的资源prefab
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UnityEngine.Object getMainUIPrefab(string name)
    {
        if (mainUIAssetsDic.ContainsKey(name))
        {
            return mainUIAssetsDic[name];
        }
        return null;
    }

}
