using CLRSharp;
using System.Collections.Generic;
using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 更新侦听的Delegate
    /// </summary>
    public delegate void ListenerDelegate();

    public class GMonoBehaviour : MonoBehaviour
    {
        /// <summary>
        /// 初始化时设置临时变量，勿要调用！！！
        /// </summary>
        /// <param name="clrInst"></param>
        public static void setProvisionalData(object clrInst)
        {
            s_clrInst = clrInst as CLRSharp_Instance;
        }
        private static CLRSharp_Instance s_clrInst;

        //================================================================
        /// <summary>
        /// 方法列表
        /// </summary>
        private Dictionary<GMBEventMethod, IMethod> funDic;

        /// <summary>
        /// 对应的L#类的实例
        /// </summary>
        public CLRSharp_Instance clrInst
        {
            get;
            private set;
        }

        /// <summary>
        /// 是否已经调用了销毁方法
        /// </summary>
        public bool isDestorying
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        public void init()
        {
            isDestorying = false;
            clrInst = s_clrInst;
            funDic = new Dictionary<GMBEventMethod, IMethod>();

            //传送实例
            MethodParamList TypeList = CLRSharpManager.instance.getParamTypeList(typeof(object));
            object[] paramList = new object[] { this };
            CLRSharpManager.instance.Invoke(clrInst.type, "setGMB", clrInst, TypeList, paramList);
        }

        /// <summary>
        /// 执行L#对应的方法
        /// </summary>
        /// <param name="funName">方法</param>
        /// <param name="paramTypes">参数类型列表</param>
        /// <param name="param">参数列表</param>
        protected void doFun(GMBEventMethod method, MethodParamList paramTypes = null, object[] param = null)
        {
            if (funDic == null)
            {
                init();
            }
            if (!funDic.ContainsKey(method))
            {
                funDic.Add(method, GMBManager.instance.getGMBEventMethod(clrInst.type, method, paramTypes));
            }
            if (funDic[method] != null)
            {
                funDic[method].Invoke(CLRSharpManager.instance.context, clrInst, param);
            }
        }

        //===============================事件=============================
        virtual protected void Awake()
        {
            doFun(GMBEventMethod.Awake);
        }

        virtual protected void FixedUpdate()
        {
            doFun(GMBEventMethod.FixedUpdate);
        }

        virtual protected void LateUpdate()
        {
            doFun(GMBEventMethod.LateUpdate);
        }

        virtual protected void OnAnimatorIK(int layerIndex)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(int));
            object[] paramList = new object[]{layerIndex};
            doFun(GMBEventMethod.OnAnimatorIK, paramTypeList, paramList);
        }

        virtual protected void OnAnimatorMove()
        {
            doFun(GMBEventMethod.OnAnimatorMove);
        }

        virtual protected void OnApplicationFocus(bool focusStatus)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(bool));
            object[] paramList = new object[] { focusStatus };
            doFun(GMBEventMethod.OnApplicationFocus, paramTypeList, paramList);
        }

        virtual protected void OnApplicationPause(bool pauseStatus)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(bool));
            object[] paramList = new object[] { pauseStatus };
            doFun(GMBEventMethod.OnApplicationPause, paramTypeList, paramList);
        }

        virtual protected void OnApplicationQuit()
        {
            doFun(GMBEventMethod.OnApplicationQuit);
        }

        virtual protected void OnAudioFilterRead(float[] data, int channels)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(float[]), typeof(int));
            object[] paramList = new object[] { data, channels };
            doFun(GMBEventMethod.OnAudioFilterRead, paramTypeList, paramList);
        }

        virtual protected void OnBecameInvisible()
        {
            doFun(GMBEventMethod.OnBecameInvisible);
        }

        virtual protected void OnBecameVisible()
        {
            doFun(GMBEventMethod.OnBecameVisible);
        }

        virtual protected void OnCollisionEnter(Collision collision)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision));
            object[] paramList = new object[] { collision };
            doFun(GMBEventMethod.OnCollisionEnter, paramTypeList, paramList);
        }

        virtual protected void OnCollisionEnter2D(Collision2D coll)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision2D));
            object[] paramList = new object[] { coll };
            doFun(GMBEventMethod.OnCollisionEnter2D, paramTypeList, paramList);
        }

        virtual protected void OnCollisionExit(Collision collisionInfo)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision));
            object[] paramList = new object[] { collisionInfo };
            doFun(GMBEventMethod.OnCollisionExit, paramTypeList, paramList);
        }

        virtual protected void OnCollisionExit2D(Collision2D coll)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision2D));
            object[] paramList = new object[] { coll };
            doFun(GMBEventMethod.OnCollisionExit2D, paramTypeList, paramList);
        }

        virtual protected void OnCollisionStay(Collision collisionInfo)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision));
            object[] paramList = new object[] { collisionInfo };
            doFun(GMBEventMethod.OnCollisionStay, paramTypeList, paramList);
        }

        virtual protected void OnCollisionStay2D(Collision2D coll)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision2D));
            object[] paramList = new object[] { coll };
            doFun(GMBEventMethod.OnCollisionStay2D, paramTypeList, paramList);
        }

        virtual protected void OnConnectedToServer()
        {
            doFun(GMBEventMethod.OnConnectedToServer);
        }

        virtual protected void OnControllerColliderHit(ControllerColliderHit hit)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(ControllerColliderHit));
            object[] paramList = new object[] { hit };
            doFun(GMBEventMethod.OnControllerColliderHit, paramTypeList, paramList);
        }

        virtual protected void OnDestroy()
        {
            isDestorying = true;
            doFun(GMBEventMethod.OnDestroy);
            
            //销毁
            CLRSharpManager.instance.Invoke(clrInst.type, "dispose", clrInst);
        }

        virtual protected void OnDisable()
        {
            doFun(GMBEventMethod.OnDisable);
        }

        virtual protected void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkDisconnection));
            object[] paramList = new object[] { info };
            doFun(GMBEventMethod.OnDisconnectedFromServer, paramTypeList, paramList);
        }

        virtual protected void OnEnable()
        {
            doFun(GMBEventMethod.OnEnable);
        }

        virtual protected void OnFailedToConnect(NetworkConnectionError error)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkConnectionError));
            object[] paramList = new object[] { error };
            doFun(GMBEventMethod.OnFailedToConnect, paramTypeList, paramList);
        }

        virtual protected void OnFailedToConnectToMasterServer(NetworkConnectionError info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkConnectionError));
            object[] paramList = new object[] { info };
            doFun(GMBEventMethod.OnFailedToConnectToMasterServer, paramTypeList, paramList);
        }

        virtual protected void OnJointBreak(float breakForce)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(float));
            object[] paramList = new object[] { breakForce };
            doFun(GMBEventMethod.OnJointBreak, paramTypeList, paramList);
        }

        virtual protected void OnLevelWasLoaded(int level)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(int));
            object[] paramList = new object[] { level };
            doFun(GMBEventMethod.OnLevelWasLoaded, paramTypeList, paramList);
        }

        virtual protected void OnMasterServerEvent(MasterServerEvent msEvent)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(MasterServerEvent));
            object[] paramList = new object[] { msEvent };
            doFun(GMBEventMethod.OnMasterServerEvent, paramTypeList, paramList);
        }

        virtual protected void OnNetworkInstantiate(NetworkMessageInfo info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkMessageInfo));
            object[] paramList = new object[] { info };
            doFun(GMBEventMethod.OnNetworkInstantiate, paramTypeList, paramList);
        }

        virtual protected void OnParticleCollision(GameObject other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(GameObject));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnParticleCollision, paramTypeList, paramList);
        }

        virtual protected void OnPlayerConnected(NetworkPlayer player)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkPlayer));
            object[] paramList = new object[] { player };
            doFun(GMBEventMethod.OnPlayerConnected, paramTypeList, paramList);
        }

        virtual protected void OnPlayerDisconnected(NetworkPlayer player)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkPlayer));
            object[] paramList = new object[] { player };
            doFun(GMBEventMethod.OnPlayerDisconnected, paramTypeList, paramList);
        }

        virtual protected void OnPostRender()
        {
            doFun(GMBEventMethod.OnPostRender);
        }

        virtual protected void OnPreCull()
        {
            doFun(GMBEventMethod.OnPreCull);
        }

        virtual protected void OnPreRender()
        {
            doFun(GMBEventMethod.OnPreRender);
        }

        virtual protected void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(RenderTexture), typeof(RenderTexture));
            object[] paramList = new object[] { src, dest };
            doFun(GMBEventMethod.OnRenderImage, paramTypeList, paramList);
        }

        virtual protected void OnRenderObject()
        {
            doFun(GMBEventMethod.OnRenderObject);
        }

        virtual protected void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(BitStream), typeof(NetworkMessageInfo));
            object[] paramList = new object[] { stream, info };
            doFun(GMBEventMethod.OnSerializeNetworkView, paramTypeList, paramList);
        }

        virtual protected void OnServerInitialized()
        {
            doFun(GMBEventMethod.OnServerInitialized);
        }

        virtual protected void OnTransformChildrenChanged()
        {
            doFun(GMBEventMethod.OnTransformChildrenChanged);
        }

        virtual protected void OnTransformParentChanged()
        {
            doFun(GMBEventMethod.OnTransformParentChanged);
        }

        virtual protected void OnTriggerEnter(Collider other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnTriggerEnter, paramTypeList, paramList);
        }

        virtual protected void OnTriggerEnter2D(Collider2D other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider2D));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnTriggerEnter2D, paramTypeList, paramList);
        }

        virtual protected void OnTriggerExit(Collider other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnTriggerExit, paramTypeList, paramList);
        }

        virtual protected void OnTriggerExit2D(Collider2D other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider2D));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnTriggerExit2D, paramTypeList, paramList);
        }

        virtual protected void OnTriggerStay(Collider other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnTriggerStay, paramTypeList, paramList);
        }

        virtual protected void OnTriggerStay2D(Collider2D other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider2D));
            object[] paramList = new object[] { other };
            doFun(GMBEventMethod.OnTriggerStay2D, paramTypeList, paramList);
        }

        virtual protected void OnValidate()
        {
            doFun(GMBEventMethod.OnValidate);
        }

        virtual protected void OnWillRenderObject()
        {
            doFun(GMBEventMethod.OnWillRenderObject);
        }

        virtual protected void Reset()
        {
            doFun(GMBEventMethod.Reset);
        }

        virtual protected void Start()
        {
            doFun(GMBEventMethod.Start);
        }

        virtual protected void Update()
        {
            doFun(GMBEventMethod.Update);
        }
    }
}
