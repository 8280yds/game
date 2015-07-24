using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelView : Window
{
    private Button leftBtn
    {
        get
        {
            if (m_leftBtn == null)
            {
                m_leftBtn = transform.FindChild("LeftBtn").GetComponent<Button>();
            }
            return m_leftBtn;
        }
    }
    private Button m_leftBtn;

    private Button rightBtn
    {
        get
        {
            if (m_rightBtn == null)
            {
                m_rightBtn = transform.FindChild("RightBtn").GetComponent<Button>();
            }
            return m_rightBtn;
        }
    }
    private Button m_rightBtn;

    private Text descText
    {
        get
        {
            if (m_descText == null)
            {
                m_descText = transform.FindChild("DescText").GetComponent<Text>();
            }
            return m_descText;
        }
    }
    private Text m_descText;

    private Image ufoImage
    {
        get
        {
            if (m_ufoImage == null)
            {
                m_ufoImage = transform.FindChild("UFOImage").GetComponent<Image>();
            }
            return m_ufoImage;
        }
    }
    private Image m_ufoImage;

    private Button ufoBtn
    {
        get
        {
            if (m_ufoBtn == null)
            {
                m_ufoBtn = transform.FindChild("UFOImage").GetComponent<Button>();
            }
            return m_ufoBtn;
        }
    }
    private Button m_ufoBtn;

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

        leftBtn.onClick.AddListener(onLeftBtnClick);
        rightBtn.onClick.AddListener(onRightBtnClick);
        ufoBtn.onClick.AddListener(onUFOImageClick);

        updateView();
    }

    protected override void addListeners()
    {
        base.addListeners();

        addListener(LevelUpdateConstant.UPDATE_LEVEL_VIEW, updateView);
    }

    private void updateView()
    {
        LevelDBVO vo = dbModel.getVOById(model.enemyLevel);
        if (vo != null)
        {
            descText.text = "文明等级：" + vo.id;
            ufoImage.sprite = getAssetsByName(vo.ufoImage) as Sprite;
            ufoImage.SetNativeSize();

            LevelDBVO vo2 = dbModel.getVOById(model.enemyLevel - 1);
            leftBtn.gameObject.SetActive(vo2 != null);

            vo2 = dbModel.getVOById(model.enemyLevel + 1);
            rightBtn.gameObject.SetActive(vo2 != null);
        }
    }

    private void onLeftBtnClick()
    {
        int enemyLevel = model.enemyLevel - 1;
        LevelDBVO vo = dbModel.getVOById(enemyLevel);
        if(vo != null)
        {
            model.enemyLevel = enemyLevel;
        }
    }

    private void onRightBtnClick()
    {
        int enemyLevel = model.enemyLevel + 1;
        LevelDBVO vo = dbModel.getVOById(enemyLevel);
        if (vo != null)
        {
            model.enemyLevel = enemyLevel;
        }
    }

    private void onUFOImageClick()
    {
        StarsLayerView starsLayerView = mvcCharge.getInstance(typeof(StarsLayerView) as ICLRType) as StarsLayerView;
        starsLayerView.show();

        close();
    }

    public override void dispose()
    {
        leftBtn.onClick.RemoveListener(onLeftBtnClick);
        rightBtn.onClick.RemoveListener(onRightBtnClick);
        ufoBtn.onClick.RemoveListener(onUFOImageClick);

        base.dispose();
    }
}
