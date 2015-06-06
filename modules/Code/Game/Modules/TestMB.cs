using UnityEngine;

public class TestMB : GMB
{
    public static void start()
    {
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        TestMB testMB = new TestMB(cube);
    }

    public TestMB(GameObject gameObject)
        : base(gameObject, new string[] { "Awake", "Update" })
    {

    }

    protected override void Update()
    {
        base.Update();
        gmb.transform.position -= new Vector3(0, 0, 0.01f);
    }

    protected override void Awake()
    {
        base.Awake();
        gmb.transform.position = new Vector3(0, 0, -5f);
    }
}
