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

        LoadInfo info = new LoadInfo();
        info.fileName = "cube.assetbundle";
        info.version = 222;
        info.loadStart = loadStart;
        info.loadEnd = loadEnd;
        info.loadFail = loadFail;
        LoadManager.instance.load(info);

        LoadManager.instance.load("cube6.assetbundle", 222, "/", LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);
        LoadManager.instance.load("cube7.assetbundle", 222, "/", LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);
        LoadManager.instance.load("cube8.assetbundle", 222, "/", LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);
        LoadManager.instance.load("cube9.assetbundle", 222, "/", LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);

        LoadInfo info3 = new LoadInfo();
        info3.fileName = "cube.assetbundle";
        info3.version = 222;
        info3.loadEnd = loadEnd;
        info3.loadFail = loadFail;
        LoadManager.instance.load(info3);

        LoadManager.instance.stopLoad("cube8.assetbundle", 222);
        LoadManager.instance.stopLoad("cube6.assetbundle", 222);
        LoadManager.instance.stopLoad("cube7.assetbundle", 222, "/", true);
    }

    void loadStart(LoadInfo loadData)
    {
        Debug.Log(loadData.fileName + " loadStart");
    }

    void loadProgress(LoadInfo loadData)
    {
        Debug.Log(loadData.fileName + " loadProgress:" + (int)(loadData.progress * 100) + "%");
    }

    void loadEnd(LoadInfo loadData)
    {
        Debug.Log(loadData.fileName + " loadEnd:" + loadData);
    }

    void loadFail(LoadInfo loadData)
    {
        Debug.Log(loadData.fileName + " loadFail:" + loadData);
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
