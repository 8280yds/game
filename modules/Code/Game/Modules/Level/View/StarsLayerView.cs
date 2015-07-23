﻿using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarsLayerView : Window
{
    private List<Star> starsList;

    private Button backBtn
    {
        get
        {
            if (m_backBtn == null)
            {
                m_backBtn = transform.FindChild("BackBtn").GetComponent<Button>();
            }
            return m_backBtn;
        }
    }
    private Button m_backBtn;

    private LevelModel model
    {
        get
        {
            if (m_model == null)
            {
                m_model = mvcCharge.getInstance(typeof(LevelModel) as ICLRType) as LevelModel;
            }
            return m_model;
        }
    }
    private LevelModel m_model;

    private LevelDBModel dbModel
    {
        get
        {
            if (m_dbModel == null)
            {
                m_dbModel = mvcCharge.getInstance(typeof(LevelDBModel) as ICLRType) as LevelDBModel;
            }
            return m_dbModel;
        }
    }
    private LevelDBModel m_dbModel;

    //======================================================================
    protected override void onShow()
    {
        base.onShow();

        updateView();
        backBtn.onClick.AddListener(onBack);
    }

    protected override void addListeners()
    {
        base.addListeners();

        addListener(LevelUpdateConstant.UPDATE_STARTS_LAYER_VIEW, updateView);
    }

    private void updateView()
    {
        LevelDBVO dbvo = dbModel.getVOById(model.enemyLevel);
        char[] ch = new char[] { ',' };
        string[] starX = dbvo.starX.Split(ch);
        string[] starY = dbvo.starY.Split(ch);
        string[] starImage = dbvo.starImage.Split(ch);
        string[] war = dbvo.war.Split(ch);
        string[] next = dbvo.next.Split(ch);

        clearStars();
        int len = starX.Length;
        starsList = new List<Star>();
        GameObject profab = getAssetsByName("star") as GameObject;
        Star star;
        GameObject go;
        Sprite sp;

        for (int i = 0; i < len; i++)
        {
            go = GameObject.Instantiate(profab);
            go.transform.SetParent(transform, false);

            star = new Star();
            star.init(go);
            sp = getAssetsByName("starImage_" + starImage[i]) as Sprite;
            star.setData(int.Parse(starX[i]), int.Parse(starY[i]), sp, int.Parse(war[i]), "★★☆");
            starsList.Add(star);
        }
    }

    private void clearStars()
    {
        if (starsList != null)
        {
            foreach(Star star in starsList)
            {
                star.dispose();
            }
            starsList.Clear();
        }
    }

    private void onBack()
    {
        LevelView levelView = mvcCharge.getInstance(typeof(LevelView) as ICLRType) as LevelView;
        levelView.show();

        close();
    }

    public override void dispose()
    {
        backBtn.onClick.RemoveListener(onBack);
        base.dispose();
    }
}
