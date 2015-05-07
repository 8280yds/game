using UnityEngine;
using System.Collections;
using Freamwork.MVC;

public class MVCTest : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {
        Debug.Log("hasModel " + MVCCharge.instance.hasModel<TestModel>());
        Debug.Log("hasModel " + MVCCharge.instance.hasModel<TestModel>());

        TestModel model = MVCCharge.instance.getModel<TestModel>();
        string fullName = model.GetType().FullName;
        Debug.Log(fullName);

        Debug.Log("delModel " + MVCCharge.instance.delModel<TestModel>());
        Debug.Log("hasModel " + MVCCharge.instance.hasModel<TestModel>());

        TestModel2 model2 = MVCCharge.instance.getModel<TestModel2>();
        string fullName2 = model2.GetType().FullName;
        Debug.Log(fullName2);

    }

    // Update is called once per frame
    void Update()
    {

    }
}
