using Freamwork.MVC;
using UnityEngine.UI;
using UnityEngine;

public class TestView2 : View
{
    private Text txt;
    private Text txt2;

    private TestModel model;

    private GoodsDBModel goodsDBModel;

    void Start()
    {
        model = MVCCharge.instance.getModel<TestModel>() as TestModel;

        txt = transform.FindChild("Text").GetComponent<Text>();
        txt2 = transform.FindChild("Text2").GetComponent<Text>();


        goodsDBModel = MVCCharge.instance.getModel<GoodsDBModel>();
        GoodsDBVO vo =  goodsDBModel.getVoById(1003);
        GoodsDBVO vo2 = goodsDBModel.getVoById(1001);
        GoodsDBVO vo3 = goodsDBModel.getVoById(1002);
        GoodsDBVO vo4 = goodsDBModel.getVoById(1100);
        GoodsDBVO vo5 = goodsDBModel.getVoById(1004);
        Debug.Log(vo);
        Debug.Log(vo2);
        Debug.Log(vo3);
        Debug.Log(vo4);
        Debug.Log(vo5);
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
