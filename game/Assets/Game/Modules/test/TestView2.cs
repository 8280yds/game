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

        BundleLoadInfo info = new BundleLoadInfo();
        info.fullName = "cube.assetbundle";
        info.version = 222;
        info.loadStart = loadStart;
        info.loadEnd = loadEnd;
        info.loadFail = loadFail;
        BundleLoadManager.instance.addLoad(info);

        BundleLoadManager.instance.addLoad("cube6.assetbundle", 222, LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);
        BundleLoadManager.instance.addLoad("cube7.assetbundle", 222, LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);
        BundleLoadManager.instance.addLoad("cube8.assetbundle", 222, LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);
        BundleLoadManager.instance.addLoad("cube9.assetbundle", 222, LoadPriority.two, loadStart, loadProgress, loadEnd, loadFail);

        BundleLoadInfo info3 = new BundleLoadInfo();
        info3.fullName = "cube.assetbundle";
        info3.version = 222;
        info3.loadEnd = loadEnd;
        info3.loadFail = loadFail;
        BundleLoadManager.instance.addLoad(info3);

        BundleLoadManager.instance.removeLoad("/cube8.assetbundle222");
        BundleLoadManager.instance.removeLoad("/cube6.assetbundle222");
        BundleLoadManager.instance.removeLoad("/cube7.assetbundle222", true);
    }

    void loadStart(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " loadStart");
    }

    void loadProgress(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " loadProgress:" + (int)(loadData.loadProgressNum * 100) + "%");
    }

    void loadEnd(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " loadEnd:" + loadData);
    }

    void loadFail(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " loadFail:" + loadData);
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
