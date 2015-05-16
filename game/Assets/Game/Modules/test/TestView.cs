using Freamwork;
using UnityEngine.UI;
using UnityEngine;

public class TestView : View
{
    private Text txt;
    private Button btn;
    private Button btn2;
    private Text btn2Text;
    private Toggle toggle;

    private TestModel model;

    void Start()
    {
        model = MVCCharge.instance.getModel<TestModel>() as TestModel;

        txt = transform.FindChild("Text").GetComponent<Text>();
        btn = transform.FindChild("Button").GetComponent<Button>();
        btn2 = transform.FindChild("Button2").GetComponent<Button>();
        btn2Text = btn2.transform.FindChild("Text").GetComponent<Text>();
        toggle = transform.FindChild("Toggle").GetComponent<Toggle>();

        btn.onClick.AddListener(btnClicked);
        btn2.onClick.AddListener(btn2Clicked);
        toggle.onValueChanged.AddListener(toggleValueChange);
    }

    protected override void addListeners()
    {
        base.addListeners();

        addListener(TestConstant.COUNT_CHANGE, updateText);
    }

    private void updateText()
    {
        txt.text = "当前总共点击了" + model.clickCount + "次";

        sendCommand<TestCommand>("Command： " + txt.text);
    }

    private void btnClicked()
    {
        model.clickCount++;
    }

    private void btn2Clicked()
    {
        bool listenerDisabled = MVCCharge.instance.listenerDisabled;
        if (listenerDisabled)
        {
            btn2Text.text = "更  新";
        }
        else
        {
            btn2Text.text = "不更新";
        }
        MVCCharge.instance.listenerDisabled = !listenerDisabled;
    }

    private void toggleValueChange(bool boo)
    {
        if (boo)
        {
            MVCCharge.instance.removeDisabledId(TestConstant.COUNT_CHANGE);
        }
        else
        {
            MVCCharge.instance.addDisabledId(TestConstant.COUNT_CHANGE);
        }
    }

}
