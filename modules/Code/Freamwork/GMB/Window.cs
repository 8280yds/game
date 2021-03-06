﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 窗口面板基类
    /// </summary>
    public abstract class Window : View
    {
        /// <summary>
        /// 资源列表
        /// </summary>
        private Dictionary<string, UnityEngine.Object> assetsDic;

        /// <summary>
        /// 面板参数
        /// </summary>
        protected object windowParam;

        /// <summary>
        /// 面板是否在准备显示的过程中
        /// </summary>
        public bool readyToShow
        {
            get;
            private set;
        }

        /// <summary>
        /// 面板是否在显示中
        /// </summary>
        public bool isShow
        {
            get;
            private set;
        }

        /// <summary>
        /// 显示面板
        /// </summary>
        virtual public void show(object windowParam = null)
        {
            this.windowParam = windowParam;

            if (inited)
            {
                onShow();
                return;
            }

            if (readyToShow)
            {
                return;
            }
            readyToShow = true;

            string viewName = clrType.Name;
            UIManager.instance.setModel(viewName, true);
            LoadManager.instance.addLoad(viewName.ToLower() + ".assets", LoadPriority.one, LoadType.local,
                null, null, null, null, null, null, unZipEnd);
        }

        private void unZipEnd(LoadData data)
        {
            string viewName = clrType.Name;
            if (data.fullName == viewName.ToLower() + ".assets")
            {
                UIManager.instance.setModel(viewName, false);

                GameObject assetsGO = data.assets[0] as GameObject;
                Assets assets = assetsGO.GetComponent<Assets>();
                if (assets == null)
                {
                    throw new Exception(clrType.FullName + "面板资源包中不存在Assets组件");
                }

                assetsDic = new Dictionary<string, UnityEngine.Object>();
                foreach (UnityEngine.Object obj in assets.assetsList)
                {
                    assetsDic.Add(obj.name, obj);
                }

                GameObject prefab = getAssetsByName(viewName) as GameObject;
                GameObject go = GameObject.Instantiate<GameObject>(prefab);
                go.name = viewName;
                init(go);
            }
        }

        public override void init(GameObject gameObject)
        {
            if (inited)
            {
                throw new Exception(clrType.FullName + "试图重复初始化");
            }
            inited = true;
            
            Transform uiLayer = UIManager.instance.getUILayer(UILayer.WindowLayer);
            gameObject.transform.SetParent(uiLayer, false);
            gameObject.transform.SetAsLastSibling();

            this.gameObject = gameObject;
            GMonoBehaviour.setProvisionalData(this);
            gameObject.AddComponent<GMonoBehaviour>();
        }

        protected override void Awake()
        {
            base.Awake();
            onShow();
        }

        virtual protected void onShow()
        {
            isShow = true;
            readyToShow = false;
        }

        /// <summary>
        /// 关闭面板
        /// </summary>
        virtual public void close()
        {
            GameObject.Destroy(gameObject);
        }

        /// <summary>
        /// 根据名称获取资源
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected UnityEngine.Object getAssetsByName(string name)
        {
            if (assetsDic.ContainsKey(name))
            {
                return assetsDic[name];
            }
            return null;
        }

        public override void dispose()
        {
            assetsDic.Clear();
            assetsDic = null;
            windowParam = null;

            base.dispose();
        }

    }
}