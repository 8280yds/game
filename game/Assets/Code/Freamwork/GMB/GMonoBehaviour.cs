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
        /// <param name="funNames"></param>
        public static void setProvisionalData(object clrInst, string[] funNames)
        {
            s_clrInst = clrInst as CLRSharp_Instance;
            s_funNames = funNames;
        }
        private static CLRSharp_Instance s_clrInst;
        private static string[] s_funNames;

        //================================================================
        /// <summary>
        /// 方法列表
        /// </summary>
        private Dictionary<string, IMethod> funDic;

        /// <summary>
        /// 对应的L#类
        /// </summary>
        public ICLRType clrType
        {
            get;
            private set;
        }

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
            this.clrInst = s_clrInst;
            this.clrType = CLRSharpManager.instance.getCLRType(clrInst.type.FullName);

            funDic = new Dictionary<string, IMethod>();
            for (int i = 0, len = s_funNames.Length; i < len; i++)
            {
                //funDic.Add(s_funNames[i], null);
                funDic[s_funNames[i]] = null;
            }

            //传送实例
            MethodParamList TypeList = CLRSharpManager.instance.getParamTypeList(typeof(object));
            object[] paramList = new object[] { this };
            CLRSharpManager.instance.Invoke(clrType, "setGMB", clrInst, TypeList, paramList);
        }

        /// <summary>
        /// 执行L#对应的方法
        /// </summary>
        /// <param name="funName">方法名称</param>
        /// <param name="paramTypes">参数类型列表</param>
        /// <param name="param">参数列表</param>
        private void doFun(string funName, MethodParamList paramTypes = null, object[] param = null)
        {
            if (funDic == null)
            {
                init();
            }
            if (!funDic.ContainsKey(funName))
            {
                return;
            }
            if (funDic[funName] == null)
            {
                if (paramTypes == null)
                {
                    paramTypes = MethodParamList.constEmpty();
                }
                funDic[funName] = CLRSharpManager.instance.GetMethod(clrType, funName, paramTypes);
            }
            funDic[funName].Invoke(CLRSharpManager.instance.context, clrInst, param);
        }

        //===============================事件=============================
        virtual protected void Awake()
        {
            doFun("Awake");
        }

        virtual protected void FixedUpdate()
        {
            doFun("FixedUpdate");
        }

        virtual protected void LateUpdate()
        {
            doFun("LateUpdate");
        }

        virtual protected void OnAnimatorIK(int layerIndex)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(int));
            object[] paramList = new object[]{layerIndex};
            doFun("OnAnimatorIK", paramTypeList, paramList);
        }

        virtual protected void OnAnimatorMove()
        {
            doFun("OnAnimatorMove");
        }

        virtual protected void OnApplicationFocus(bool focusStatus)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(bool));
            object[] paramList = new object[] { focusStatus };
            doFun("OnApplicationFocus", paramTypeList, paramList);
        }

        virtual protected void OnApplicationPause(bool pauseStatus)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(bool));
            object[] paramList = new object[] { pauseStatus };
            doFun("OnApplicationPause", paramTypeList, paramList);
        }

        virtual protected void OnApplicationQuit()
        {
            doFun("OnApplicationQuit");
        }

        virtual protected void OnAudioFilterRead(float[] data, int channels)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(float[]), typeof(int));
            object[] paramList = new object[] { data, channels };
            doFun("OnAudioFilterRead", paramTypeList, paramList);
        }

        virtual protected void OnBecameInvisible()
        {
            doFun("OnBecameInvisible");
        }

        virtual protected void OnBecameVisible()
        {
            doFun("OnBecameVisible");
        }

        virtual protected void OnCollisionEnter(Collision collision)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision));
            object[] paramList = new object[] { collision };
            doFun("OnCollisionEnter", paramTypeList, paramList);
        }

        virtual protected void OnCollisionEnter2D(Collision2D coll)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision2D));
            object[] paramList = new object[] { coll };
            doFun("OnCollisionEnter2D", paramTypeList, paramList);
        }

        virtual protected void OnCollisionExit(Collision collisionInfo)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision));
            object[] paramList = new object[] { collisionInfo };
            doFun("OnCollisionExit", paramTypeList, paramList);
        }

        virtual protected void OnCollisionExit2D(Collision2D coll)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision2D));
            object[] paramList = new object[] { coll };
            doFun("OnCollisionExit2D", paramTypeList, paramList);
        }

        virtual protected void OnCollisionStay(Collision collisionInfo)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision));
            object[] paramList = new object[] { collisionInfo };
            doFun("OnCollisionStay", paramTypeList, paramList);
        }

        virtual protected void OnCollisionStay2D(Collision2D coll)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collision2D));
            object[] paramList = new object[] { coll };
            doFun("OnCollisionStay2D", paramTypeList, paramList);
        }

        virtual protected void OnConnectedToServer()
        {
            doFun("OnConnectedToServer");
        }

        virtual protected void OnControllerColliderHit(ControllerColliderHit hit)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(ControllerColliderHit));
            object[] paramList = new object[] { hit };
            doFun("OnControllerColliderHit", paramTypeList, paramList);
        }

        virtual protected void OnDestroy()
        {
            isDestorying = true;
            doFun("OnDestroy");
            
            //销毁
            CLRSharpManager.instance.Invoke(clrType, "dispose", clrInst);
        }

        virtual protected void OnDisable()
        {
            doFun("OnDisable");
        }

        virtual protected void OnDisconnectedFromServer(NetworkDisconnection info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkDisconnection));
            object[] paramList = new object[] { info };
            doFun("OnDisconnectedFromServer", paramTypeList, paramList);
        }

        virtual protected void OnEnable()
        {
            doFun("OnEnable");
        }

        virtual protected void OnFailedToConnect(NetworkConnectionError error)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkConnectionError));
            object[] paramList = new object[] { error };
            doFun("OnFailedToConnect", paramTypeList, paramList);
        }

        virtual protected void OnFailedToConnectToMasterServer(NetworkConnectionError info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkConnectionError));
            object[] paramList = new object[] { info };
            doFun("OnFailedToConnectToMasterServer", paramTypeList, paramList);
        }

        virtual protected void OnJointBreak(float breakForce)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(float));
            object[] paramList = new object[] { breakForce };
            doFun("OnJointBreak", paramTypeList, paramList);
        }

        virtual protected void OnLevelWasLoaded(int level)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(int));
            object[] paramList = new object[] { level };
            doFun("OnLevelWasLoaded", paramTypeList, paramList);
        }

        virtual protected void OnMasterServerEvent(MasterServerEvent msEvent)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(MasterServerEvent));
            object[] paramList = new object[] { msEvent };
            doFun("OnMasterServerEvent", paramTypeList, paramList);
        }

        virtual protected void OnNetworkInstantiate(NetworkMessageInfo info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkMessageInfo));
            object[] paramList = new object[] { info };
            doFun("OnNetworkInstantiate", paramTypeList, paramList);
        }

        virtual protected void OnParticleCollision(GameObject other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(GameObject));
            object[] paramList = new object[] { other };
            doFun("OnParticleCollision", paramTypeList, paramList);
        }

        virtual protected void OnPlayerConnected(NetworkPlayer player)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkPlayer));
            object[] paramList = new object[] { player };
            doFun("OnPlayerConnected", paramTypeList, paramList);
        }

        virtual protected void OnPlayerDisconnected(NetworkPlayer player)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(NetworkPlayer));
            object[] paramList = new object[] { player };
            doFun("OnPlayerDisconnected", paramTypeList, paramList);
        }

        virtual protected void OnPostRender()
        {
            doFun("OnPostRender");
        }

        virtual protected void OnPreCull()
        {
            doFun("OnPreCull");
        }

        virtual protected void OnPreRender()
        {
            doFun("OnPreRender");
        }

        virtual protected void OnRenderImage(RenderTexture src, RenderTexture dest)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(RenderTexture), typeof(RenderTexture));
            object[] paramList = new object[] { src, dest };
            doFun("OnRenderImage", paramTypeList, paramList);
        }

        virtual protected void OnRenderObject()
        {
            doFun("OnRenderObject");
        }

        virtual protected void OnSerializeNetworkView(BitStream stream, NetworkMessageInfo info)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(BitStream), typeof(NetworkMessageInfo));
            object[] paramList = new object[] { stream, info };
            doFun("OnSerializeNetworkView", paramTypeList, paramList);
        }

        virtual protected void OnServerInitialized()
        {
            doFun("OnServerInitialized");
        }

        virtual protected void OnTransformChildrenChanged()
        {
            doFun("OnTransformChildrenChanged");
        }

        virtual protected void OnTransformParentChanged()
        {
            doFun("OnTransformParentChanged");
        }

        virtual protected void OnTriggerEnter(Collider other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider));
            object[] paramList = new object[] { other };
            doFun("OnTriggerEnter", paramTypeList, paramList);
        }

        virtual protected void OnTriggerEnter2D(Collider2D other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider2D));
            object[] paramList = new object[] { other };
            doFun("OnTriggerEnter2D", paramTypeList, paramList);
        }

        virtual protected void OnTriggerExit(Collider other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider));
            object[] paramList = new object[] { other };
            doFun("OnTriggerExit", paramTypeList, paramList);
        }

        virtual protected void OnTriggerExit2D(Collider2D other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider2D));
            object[] paramList = new object[] { other };
            doFun("OnTriggerExit2D", paramTypeList, paramList);
        }

        virtual protected void OnTriggerStay(Collider other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider));
            object[] paramList = new object[] { other };
            doFun("OnTriggerStay", paramTypeList, paramList);
        }

        virtual protected void OnTriggerStay2D(Collider2D other)
        {
            MethodParamList paramTypeList = CLRSharpManager.instance.getParamTypeList(typeof(Collider2D));
            object[] paramList = new object[] { other };
            doFun("OnTriggerStay2D", paramTypeList, paramList);
        }

        virtual protected void OnValidate()
        {
            doFun("OnValidate");
        }

        virtual protected void OnWillRenderObject()
        {
            doFun("OnWillRenderObject");
        }

        virtual protected void Reset()
        {
            doFun("Reset");
        }

        virtual protected void Start()
        {
            doFun("Start");
        }

        virtual protected void Update()
        {
            doFun("Update");
        }
    }
}
