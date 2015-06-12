using CLRSharp;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Freamwork
{
    /// <summary>
    /// L#中的动作类，与C#中GMonoBehaviour搭档共同实现各种功能
    /// </summary>
    public abstract class GMB
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="gameObject">要绑定的物体</param>
        virtual public void init(GameObject gameObject)
        {
            if (inited)
            {
                throw new Exception(getCLRType.FullName + "试图重复初始化");
            }
            inited = true;
            this.gameObject = gameObject;
            GMonoBehaviour.setProvisionalData(this);
            gameObject.AddComponent<GMonoBehaviour>();
        }

        public ICLRType getCLRType
        {
            get
            {
                return CLRSharpManager.instance.getCLRTypeByInst(this);
            }
        }

        /// <summary>
        /// 是否已经初始化过了
        /// </summary>
        public bool inited
        {
            get
            {
                return m_inited;
            }
            protected set
            {
                m_inited = value;
            }
        }
        private bool m_inited = false;

        /// <summary>
        /// 物体实例
        /// </summary>
        public GameObject gameObject
        {
            get;
            protected set;
        }

        /// <summary>
        /// C#的GMonoBehaviour脚本实例
        /// </summary>
        public GMonoBehaviour gmb
        {
            get;
            private set;
        }

        /// <summary>
        /// 设置实例，仅在GMonoBehaviour中调用
        /// </summary>
        /// <param name="gmb"></param>
        private void setGMB(object gmb)
        {
            this.gmb = gmb as GMonoBehaviour;
        }

        /// <summary>
        /// 释放
        /// </summary>
        virtual public void dispose()
        {
            if (gmb != null && !gmb.isDestorying)
            {
                GameObject.Destroy(gmb);
                return;
            }
            if (disposed)
            {
                Debug.Log(getCLRType.FullName + "对象重复释放！");
                return;
            }
            disposed = true;
            inited = false;
            gameObject = null;
            gmb = null;
        }
        protected bool disposed = false;

        //========================MonoBehaviour事件=======================
        virtual protected void Awake()
        {

        }

        virtual protected void FixedUpdate()
        {

        }

        virtual protected void LateUpdate()
        {

        }

        virtual protected void OnAnimatorIK(int layerIndex)
        {

        }

        virtual protected void OnAnimatorMove()
        {

        }

        virtual protected void OnApplicationFocus(bool focusStatus)
        {

        }

        virtual protected void OnApplicationPause(bool pauseStatus)
        {

        }

        virtual protected void OnApplicationQuit()
        {

        }

        virtual protected void OnAudioFilterRead(float[] data, int channels)
        {

        }

        virtual protected void OnBecameInvisible()
        {

        }

        virtual protected void OnBecameVisible()
        {

        }

        virtual protected void OnCollisionEnter(Collision collision)
        {

        }

        virtual protected void OnCollisionEnter2D(Collision2D coll)
        {

        }

        virtual protected void OnCollisionExit(Collision collisionInfo)
        {

        }

        virtual protected void OnCollisionExit2D(Collision2D coll)
        {

        }

        virtual protected void OnCollisionStay(Collision collisionInfo)
        {

        }

        virtual protected void OnCollisionStay2D(Collision2D coll)
        {

        }

        virtual protected void OnConnectedToServer()
        {

        }

        virtual protected void OnControllerColliderHit(ControllerColliderHit hit)
        {

        }

        virtual protected void OnDestroy()
        {
            
        }

        virtual protected void OnDisable()
        {

        }

        virtual protected void OnDisconnectedFromServer(NetworkDisconnection info)
        {

        }

        virtual protected void OnEnable()
        {

        }

        virtual protected void OnFailedToConnect(NetworkConnectionError error)
        {

        }

        virtual protected void OnFailedToConnectToMasterServer(NetworkConnectionError info)
        {

        }

        virtual protected void OnJointBreak(float breakForce)
        {

        }

        virtual protected void OnLevelWasLoaded(int level)
        {

        }

        virtual protected void OnMasterServerEvent(MasterServerEvent msEvent)
        {

        }

        virtual protected void OnNetworkInstantiate(NetworkMessageInfo info)
        {

        }

        virtual protected void OnParticleCollision(GameObject other)
        {

        }

        virtual protected void OnPlayerConnected(NetworkPlayer player)
        {

        }

        virtual protected void OnPlayerDisconnected(NetworkPlayer player)
        {

        }

        virtual protected void OnPostRender()
        {

        }

        virtual protected void OnPreCull()
        {

        }

        virtual protected void OnPreRender()
        {

        }

        virtual protected void OnRenderImage(RenderTexture src, RenderTexture dest)
        {

        }

        virtual protected void OnRenderObject()
        {

        }

        virtual protected void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {

        }

        virtual protected void OnServerInitialized()
        {

        }

        virtual protected void OnTransformChildrenChanged()
        {

        }

        virtual protected void OnTransformParentChanged()
        {

        }

        virtual protected void OnTriggerEnter(Collider other)
        {

        }

        virtual protected void OnTriggerEnter2D(Collider2D other)
        {

        }

        virtual protected void OnTriggerExit(Collider other)
        {

        }

        virtual protected void OnTriggerExit2D(Collider2D other)
        {

        }

        virtual protected void OnTriggerStay(Collider other)
        {

        }

        virtual protected void OnTriggerStay2D(Collider2D other)
        {

        }

        virtual protected void OnValidate()
        {

        }

        virtual protected void OnWillRenderObject()
        {

        }

        virtual protected void Reset()
        {

        }

        virtual protected void Start()
        {

        }

        virtual protected void Update()
        {

        }

        //========================UGUI事件=======================
        virtual protected void OnPointerDown(PointerEventData eventData)
        {

        }

        virtual protected void OnPointerUp(PointerEventData eventData)
        {

        }

        virtual protected void OnPointerClick(PointerEventData eventData)
        {

        }

        virtual protected void OnPointerEnter(PointerEventData eventData)
        {

        }

        virtual protected void OnPointerExit(PointerEventData eventData)
        {

        }

        virtual protected void OnBeginDrag(PointerEventData eventData)
        {

        }

        virtual protected void OnInitializePotentialDrag(PointerEventData eventData)
        {

        }

        virtual protected void OnDrag(PointerEventData eventData)
        {

        }

        virtual protected void OnEndDrag(PointerEventData eventData)
        {

        }

        virtual protected void OnDrop(PointerEventData eventData)
        {

        }

        virtual protected void OnScroll(PointerEventData eventData)
        {

        }

        virtual protected void OnUpdateSelected(BaseEventData eventData)
        {

        }

        virtual protected void OnSelect(BaseEventData eventData)
        {

        }

        virtual protected void OnDeselect(BaseEventData eventData)
        {
            
        }

        virtual protected void OnMove(AxisEventData eventData)
        {
            
        }

        virtual protected void OnSubmit(BaseEventData eventData)
        {
            
        }

        virtual protected void OnCancel(BaseEventData eventData)
        {
            
        }

    }
}
