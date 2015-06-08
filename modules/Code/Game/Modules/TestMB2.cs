using Freamwork;
using UnityEngine;

public class TestMB2 : View
{
    public TestMB2()
        : base()
    {

    }

    override public void init(GameObject gameObject, string[] funNames = null)
    {
        base.init(gameObject, new string[] { "Awake", "Update" });
    }

    protected override void Update()
    {
        base.Update();
        gmb.transform.position += new Vector3(0, 0, 0.02f);
    }

    protected override void Awake()
    {
        base.Awake();
        gmb.transform.position = new Vector3(3f, 0, 0);
    }
}
