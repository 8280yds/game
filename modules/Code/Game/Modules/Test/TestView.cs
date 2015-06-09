using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.UI;

public class TestView : View
{
    private Button btn;
    private Button btn2;
    private Text txt;
    private TestModel model;
    private AaaSssDdDBModel dbModel;

    public override void init(GameObject gameObject, string[] funNames = null)
    {
        base.init(gameObject, new string[] { "Awake", "Update" });
    }

    protected override void Awake()
    {
        base.Awake();
        btn = GameObject.Find("Button").GetComponentInChildren<Button>();
        btn2 = GameObject.Find("Button2").GetComponentInChildren<Button>();
        txt = GameObject.Find("TestText").GetComponent<Text>();
        model = mvcCharge.getInstance(typeof(TestModel) as ICLRType) as TestModel;

        dbModel = mvcCharge.getInstance(typeof(AaaSssDdDBModel) as ICLRType) as AaaSssDdDBModel;

        btn.onClick.AddListener(onClick);
        btn2.onClick.AddListener(onClick2);
    }

    protected override void Update()
    {
        base.Update();
    }

    private void onClick()
    {
        model.count++;
    }

    private void onClick2()
    {
        sendCommand(typeof(TestCommand) as ICLRType, 0);
    }

    protected override void addListeners()
    {
        base.addListeners();
        addListener(TestCostant.COUNT_CHANGE, countChange);
    }

    private void countChange()
    {
        txt.text = "当前鼠标点击次数为：" + model.count;

        AaaSssDdDBVO vo = dbModel.getvoByAge(model.count);
        if (vo != null)
        {
            Debug.Log(vo.name);
        }
    }
    
}
