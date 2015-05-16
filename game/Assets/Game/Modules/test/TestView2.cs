using Freamwork;
using UnityEngine;
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

        //LoadInfo info = new LoadInfo();
        //info.path = "";
        //info.fileName = "cube.assetbundle";
        //info.version = 222;
        //info.loadStart = loadStart;
        //info.loadProgress = loadProgress;
        //info.loadEnd = loadEnd;
        //info.loadFail = loadFail;
        //LoadManager.instance.load(info);

        //LoadInfo info3 = new LoadInfo();
        //info3.path = "";
        //info3.fileName = "cube.assetbundle";
        //info3.version = 222;
        //info3.loadStart = loadStart;
        //info3.loadProgress = loadProgress;
        //info3.loadEnd = loadEnd;
        //info3.loadFail = loadFail;
        //LoadManager.instance.load(info3);

        //LoadInfo info2 = new LoadInfo();
        //info2.path = "";
        //info2.fileName = "scene1.assetbundle";
        //info2.version = 222;
        //info2.loadStart = loadStart;
        //info2.loadProgress = loadProgress;
        //info2.loadEnd = loadEnd;
        //info2.loadFail = loadFail;
        //LoadManager.instance.load(info2);
    }

    void loadStart(LoadInfo loadData)
    {
        Debug.Log("loadStart:" + loadData);
    }

    void loadProgress(LoadInfo loadData)
    {
        Debug.Log("loadProgress:" + (int)(loadData.progress*100) + "%");
    }

    void loadEnd(LoadInfo loadData)
    {
        Debug.Log("loadEnd:" + loadData);
    }

    void loadFail(LoadInfo loadData)
    {
        Debug.Log("loadFail:" + loadData);
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
