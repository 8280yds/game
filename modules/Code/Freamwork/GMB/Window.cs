using System;
using UnityEngine;

namespace Freamwork
{
    public abstract class Window : View
    {
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
        /// <param name="assetName">面板对应的资源名称</param>
        virtual public void show(string assetName = "")
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception(getCLRType.FullName + "的资源名称不能为空，请在show方法中为assetName设置默认参数");
            }

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

            LoadManager.instance.addLoad(assetName, LoadPriority.one, LoadType.local, null, null, null,
                null, null, null, unZipEnd);
        }

        private void unZipEnd(LoadData data)
        {
            GameObject go = GameObject.Instantiate(data.assets[0]) as GameObject;
            init(go);
        }

        public override void init(GameObject gameObject)
        {
            if (inited)
            {
                throw new Exception(getCLRType.FullName + "试图重复初始化");
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
            dispose();
        }

    }
}