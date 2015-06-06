using Freamwork;
using System;
using UnityEngine;

public class GMB
{
    public GMB(GameObject gameObject, string[] funNames)
    {
        if (gameObject == null)
        {
            throw new Exception("初始化时gameObject参数不能为null");
        }
        this.gameObject = gameObject;
        GMonoBehaviour.setProvisionalData(this, funNames);
        gameObject.AddComponent<GMonoBehaviour>();
    }

    public GameObject gameObject
    {
        get;
        private set;
    }

    public GMonoBehaviour gmb
    {
        get;
        private set;
    }

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
            Debug.Log(GetType().FullName + "对象重复释放！");
            return;
        }
        disposed = true;

        gameObject = null;
        gmb = null;
    }
    protected bool disposed = false;

    //===============================事件=============================
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
}
