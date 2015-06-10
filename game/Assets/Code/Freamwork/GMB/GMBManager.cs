using CLRSharp;
using System;
using System.Collections.Generic;

namespace Freamwork
{
    public enum GMBEventMethod
    {
        Awake,
        FixedUpdate,
        LateUpdate,
        OnAnimatorIK,
        OnAnimatorMove,
        OnApplicationFocus,
        OnApplicationPause,
        OnApplicationQuit,
        OnAudioFilterRead,
        OnBecameInvisible,
        OnBecameVisible,
        OnCollisionEnter,
        OnCollisionEnter2D,
        OnCollisionExit,
        OnCollisionExit2D,
        OnCollisionStay,
        OnCollisionStay2D,
        OnConnectedToServer,
        OnControllerColliderHit,
        OnDestroy,
        OnDisable,
        OnDisconnectedFromServer,
        OnEnable,
        OnFailedToConnect,
        OnFailedToConnectToMasterServer,
        OnJointBreak,
        OnLevelWasLoaded,
        OnMasterServerEvent,
        OnNetworkInstantiate,
        OnParticleCollision,
        OnPlayerConnected,
        OnPlayerDisconnected,
        OnPostRender,
        OnPreCull,
        OnPreRender,
        OnRenderImage,
        OnRenderObject,
        OnSerializeNetworkView,
        OnServerInitialized,
        OnTransformChildrenChanged,
        OnTransformParentChanged,
        OnTriggerEnter,
        OnTriggerEnter2D,
        OnTriggerExit,
        OnTriggerExit2D,
        OnTriggerStay,
        OnTriggerStay2D,
        OnValidate,
        OnWillRenderObject,
        Reset,
        Start,
        Update,

        OnPointerDown,
        OnPointerUp,
        OnPointerClick,
        OnPointerEnter,
        OnPointerExit,
    }

    sealed public class GMBManager
    {
        /// <summary>
        /// 实例
        /// </summary>
        static private GMBManager m_instance;

        /// <summary>
        /// 获取实例
        /// </summary>
        static public GMBManager instance
        {
            get
            {
                if (m_instance == null)
                {
                    m_instance = new GMBManager();
                }
                return m_instance;
            }
        }

        private GMBManager()
        {
            if (m_instance != null)
            {
                throw new Exception("GMBManager是单例，请使用GMBManager.instance来获取其实例！");
            }

            m_instance = this;
            init();
        }

        private Dictionary<string, Dictionary<string, IMethod>> dic;
        private List<string> totalMethodNameList;

        private void init()
        {
            totalMethodNameList = new List<string>(Enum.GetNames(typeof(GMBEventMethod)));
            dic = new Dictionary<string, Dictionary<string, IMethod>>();
        }

        /// <summary>
        /// 清除
        /// </summary>
        public void clear()
        {
            foreach (Dictionary<string, IMethod> methodDic in dic.Values)
            {
                methodDic.Clear();
            }
            dic.Clear();
        }

        //============================================================
        /// <summary>
        /// 获取GMonoBehaviour事件的方法
        /// </summary>
        /// <param name="clrType"></param>
        /// <param name="method"></param>
        /// <param name="paramTypes"></param>
        /// <returns></returns>
        public IMethod getGMBEventMethod(ICLRType clrType, GMBEventMethod method, MethodParamList paramTypes = null)
        {
            if (paramTypes == null)
            {
                paramTypes = MethodParamList.constEmpty();
            }

            if (clrType == null || clrType.FullName == "Freamwork.GMB")
            {
                return null;
            }

            Dictionary<string, IMethod> methodDic;
            if (!dic.ContainsKey(clrType.FullName))
            {
                methodDic = new Dictionary<string, IMethod>();
                dic.Add(clrType.FullName, methodDic);
            }
            else
            {
                methodDic = dic[clrType.FullName];
            }

            string methodName = totalMethodNameList[(int)method];
            if (methodDic.ContainsKey(methodName))
            {
                return methodDic[methodName];
            }

            IMethod eventMethod = clrType.GetMethod(methodName, paramTypes);
            if (eventMethod == null)
            {
                Type_Common_CLRSharp type = clrType as Type_Common_CLRSharp;
                if (type != null)
                {
                    eventMethod = getGMBEventMethod(type.BaseType, method, paramTypes);
                }
            }
            methodDic[methodName] = eventMethod;
            return eventMethod;
        }

    }
}