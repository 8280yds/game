using Freamwork;
using System;
using System.Runtime.InteropServices;
using UnityEngine;

public class TestMB2 : View
{
    protected override void Update()
    {
        base.Update();
        gmb.transform.position += new Vector3(0, 0, 0.02f);
    }

    protected override void Awake()
    {
        base.Awake();
        gmb.transform.position = new Vector3(3f, 0, 0);

        //StructObj2 structObj = new StructObj2();
        //structObj.id2 = 2222;
        //structObj.name2 = "jack2";
        //structObj.num2 = 222.222f;

        //Package package = new Package();
        //package.timeStamp = TimeUtil.getTimeStamp();
        //package.protocol = 22223333;
        //package.len = 100;
        //package.structObj = structObj;
        //Debug.Log("[返回] " + package.toString());
    }
}

//[Serializable] // 指示可序列化
//[StructLayout(LayoutKind.Sequential, Pack = 1)] // 按1字节对齐
//public struct StructObj2
//{
//    public ushort id2;
//    public string name2;
//    public float num2;
//}
