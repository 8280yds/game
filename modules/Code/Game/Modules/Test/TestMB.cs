using Freamwork;
using UnityEngine;

public class TestMB : GMB
{
    protected override void Update()
    {
        base.Update();
        gmb.transform.position += new Vector3(0, 0, 0.01f);
    }

    protected override void Awake()
    {
        base.Awake();
        gmb.transform.position = new Vector3(0, 0, 2f);
    }
}
