using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.UI;

public class TestView : Window
{
    private Button btn;
    private Button btn2;
    private Button btn3;
    private Text txt;
    private Text txt2;
    private TestModel model;
    private AaaSssDdDBModel dbModel;

    public override void show(string assetName = "test.assets")
    {
        base.show(assetName);
    }

    protected override void onShow()
    {
        base.onShow();

        btn = gmb.transform.FindChild("Button").GetComponentInChildren<Button>();
        btn2 = gmb.transform.FindChild("Button2").GetComponentInChildren<Button>();
        btn3 = gmb.transform.FindChild("Button3").GetComponentInChildren<Button>();
        txt = gmb.transform.FindChild("TestText").GetComponent<Text>();
        txt2 = gmb.transform.FindChild("Text2").GetComponent<Text>();

        model = mvcCharge.getInstance(typeof(TestModel) as ICLRType) as TestModel;
        dbModel = mvcCharge.getInstance(typeof(AaaSssDdDBModel) as ICLRType) as AaaSssDdDBModel;

        btn.onClick.AddListener(onClick);
        btn2.onClick.AddListener(onClick2);
        btn3.onClick.AddListener(onClick3);
    }

    private void onClick()
    {
        model.count++;

        //TestService service = MVCCharge.instance.getInstance(typeof(TestService) as ICLRType) as TestService;
        //service.sendMessage();
    }

    private void onClick2()
    {
        sendCommand(typeof(TestCommand) as ICLRType, 0);
    }

    private void onClick3()
    {
        close();
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

    protected override void Update()
    {
        base.Update();
        int t = TimeUtil.getTimeStamp();
        txt2.text = TimeUtil.StampToDateTime(t).ToString();
    }
    
}
