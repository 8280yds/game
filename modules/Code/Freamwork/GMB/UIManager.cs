using CLRSharp;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Freamwork
{
    /// <summary>
    /// UI层级，
    /// 在此处添加或者删除层，将会在代码中自动增加或删除，
    /// 排在前面的将会先渲染，先渲染的将会被后渲染的覆盖
    /// </summary>
    public enum UILayer
    {
        WindowLayer,     //窗口层
        TipsLayer,       //Tip层
        PopUpLayer,      //冒泡文字层
        ModelLayer,      //模态层
    }

    /// <summary>
    /// 面板管理类
    /// </summary>
    sealed public class UIManager
    {
        /// <summary>
        /// 获取实例
        /// </summary>
        static public UIManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new UIManager();
                }
                return m_instance;
            }
        }
        static private UIManager m_instance;

        private UIManager()
        {
            m_instance = this;
            init();
        }

        private string[] layerNameList;
        private Transform[] layerList;

        private void init()
        {
            //添加canvas
            GameObject go = new GameObject("UICanvas");
            GameObject.DontDestroyOnLoad(go);
            go.layer = LayerMask.NameToLayer("UI");
            canvas = go.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            CanvasScaler canvasScaler = go.AddComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;

            go.AddComponent<GraphicRaycaster>();

            //添加UI摄像机
            //go = new GameObject("_UICamera");
            //go.transform.SetParent(canvas.transform, false);
            //uiCamera = go.AddComponent<Camera>();
            //uiCamera.clearFlags = CameraClearFlags.Depth;
            //uiCamera.cullingMask = 1 << canvas.gameObject.layer;
            //uiCamera.depth = 10000;
            //uiCamera.renderingPath = RenderingPath.VertexLit;
            
            //添加EventSystem
            go = new GameObject("EventSystem");
            GameObject.DontDestroyOnLoad(go);
            eventSystem = go.AddComponent<EventSystem>();
            go.AddComponent<TouchInputModule>();
            if (Application.isEditor)
            {
                go.AddComponent<StandaloneInputModule>();
            }

            //添加UI层次
            layerNameList = CLRSharpUtil.getEnumItemNames(typeof(UILayer) as ICLRType);
            int len = layerNameList.Length;
            layerList = new Transform[len];
            for (int i = 0; i < len; i++)
            {
                layerList[i] = creatPanel(layerNameList[i]);
            }

            go = getUILayer(UILayer.ModelLayer).gameObject;
            Image image = go.AddComponent<Image>();
            image.color = new Color(0f, 0f, 0f, 0.5f);
            go.SetActive(false);
        }

        /// <summary>
        /// 清理
        /// </summary>
        public void clear()
        {
            if (canvas != null)
            {
                GameObject.Destroy(canvas.gameObject);
            }
            canvas = null;
            //uiCamera = null;
            if (eventSystem != null)
            {
                GameObject.Destroy(eventSystem.gameObject);
            }
            eventSystem = null;
            layerNameList = null;
            layerList = null;
            modelList.Clear();
        }

        //============================================================
        ///// <summary>
        ///// UI摄像机
        ///// </summary>
        //public Camera uiCamera
        //{
        //    get;
        //    private set;
        //}

        /// <summary>
        /// UI画布
        /// </summary>
        public Canvas canvas
        {
            get;
            private set;
        }

        /// <summary>
        /// EventSystem
        /// </summary>
        public EventSystem eventSystem
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建层级容器
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private RectTransform creatPanel(string name)
        {
            GameObject panelGameObject = new GameObject(name);
            RectTransform rectTransform = panelGameObject.AddComponent<RectTransform>();
            rectTransform.SetParent(canvas.transform, false);
            rectTransform.sizeDelta = new Vector2(160f, 30f);
            rectTransform = panelGameObject.GetComponent<RectTransform>();
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            return rectTransform;
        }

        /// <summary>
        /// 获取层容器
        /// </summary>
        /// <param name="layer"></param>
        /// <returns></returns>
        public Transform getUILayer(UILayer layer)
        {
            return layerList[(int)layer];
        }

        /// <summary>
        /// 是否全局模态
        /// </summary>
        public bool modal
        {
            get
            {
                return getUILayer(UILayer.ModelLayer).gameObject.activeSelf;
            }
        }

        /// <summary>
        /// 设置全局模态的状态
        /// </summary>
        /// <param name="name"></param>
        /// <param name="model"></param>
        public void setModel(string id, bool model)
        {
            if (!model)
            {
                modelList.Remove(id);
            }
            else if (!modelList.Contains(id))
            {
                modelList.Add(id);
            }
            else
            {
                return;
            }
            getUILayer(UILayer.ModelLayer).gameObject.SetActive(modelList.Count > 0);
        }
        private List<string> modelList = new List<string>();

    }
}
