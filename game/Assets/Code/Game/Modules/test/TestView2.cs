﻿using Freamwork;
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

        Debug.Log(Application.persistentDataPath);
        ManifestManager.instance.init(loadStart, null, loadEnd, loadFail, null, null, unZipEnd);
    }

    void loadStart(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 开始加载");
    }

    void loadProgress(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 加载进度:" + (int)(loadData.loadProgressNum * 100) + "%");
    }

    void loadEnd(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 加载完毕");
    }

    void loadFail(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 加载失败");
    }

    void unZipStart(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 开始解压");
    }

    void unZipProgress(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 解压进度:" + (int)(loadData.loadProgressNum * 100) + "%");
    }

    void unZipEnd(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 解压完毕");
        LoadManager.instance.addLoad("db",LoadPriority.two, LoadType.localOrWeb, 
            loadStart, null,loadEnd2,loadFail, null, null, unZipEnd2, unZipFail);
    }

    void unZipEnd2(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 解压完毕");
        Debug.Log((loadData.assets[0] as TextAsset).text);
    }

    void unZipFail(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 解压失败");
    }


    void loadEnd2(LoadData loadData)
    {
        Debug.Log(loadData.fullName + " 加载完毕");
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