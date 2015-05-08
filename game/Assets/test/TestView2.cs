using Freamwork.MVC;
using UnityEngine.UI;

public class TestView2 : View
{
    private Text txt;
    private Text txt2;

    private TestModel model;

    void Start()
    {
        model = MVCCharge.instance.getModel<TestModel>() as TestModel;

        txt = transform.FindChild("Text").GetComponent<Text>();
        txt2 = transform.FindChild("Text2").GetComponent<Text>();
    }
    
    public void showText(string str)
    {
        txt.text = str;
    }

    protected override void addListeners()
    {
        base.addListeners();

        addListener(TestConstant.COUNT_CHANGE, updateText);
    }

    private void updateText()
    {
        txt2.text = "当前总共点击了" + model.clickCount + "次";
    }



}
