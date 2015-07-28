using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class WarResultView : Window
{
    private Text titleText
    {
        get
        {
            if (m_titleText == null)
            {
                m_titleText = transform.FindChild("TitleText").GetComponent<Text>();
            }
            return m_titleText;
        }
    }
    private Text m_titleText;

    private Text starText
    {
        get
        {
            if (m_starText == null)
            {
                m_starText = transform.FindChild("StarText").GetComponent<Text>();
            }
            return m_starText;
        }
    }
    private Text m_starText;

    private Text destText
    {
        get
        {
            if (m_destText == null)
            {
                m_destText = transform.FindChild("DestText").GetComponent<Text>();
            }
            return m_destText;
        }
    }
    private Text m_destText;

    private Image bgImage
    {
        get
        {
            if (m_bgImage == null)
            {
                m_bgImage = transform.GetComponent<Image>();
            }
            return m_bgImage;
        }
    }
    private Image m_bgImage;

    private WarResultModel model
    {
        get
        {
            if (m_model == null)
            {
                m_model = mvcCharge.getInstance(typeof(WarResultModel) as ICLRType) as WarResultModel;
            }
            return m_model;
        }
    }
    private WarResultModel m_model;

    //======================================================================
    protected override void onShow()
    {
        base.onShow();
        updateView();
    }

    private void updateView()
    {
        switch(model.starNum)
        {
            case 0:
                titleText.text = "失  败";
                titleText.color = Color.red;
                starText.text = "☆ ☆ ☆";
                break;
            case 1:
                titleText.text = "小  胜";
                titleText.color = Color.green;
                starText.text = "★ ☆ ☆";
                break;
            case 2:
                titleText.text = "大  胜";
                titleText.color = Color.green;
                starText.text = "★ ★ ☆";
                break;
            case 3:
                titleText.text = "完  胜";
                titleText.color = Color.green;
                starText.text = "★ ★ ★";
                break;
        }
        destText.text = "用时：" + model.time + "秒\n操作：" + model.optionNum + "次";

        LoadManager.instance.addLoad("result_bg_" + model.starNum + ".assets", LoadPriority.two, LoadType.local,
            null, null, null, null, null, null, unZipEnd);
    }

    private void unZipEnd(LoadData data)
    {
        if(disposed)
        {
            return;
        }

        foreach(Object obj in data.assets)
        {
            Sprite sp = obj as Sprite;
            if(sp != null)
            {
                bgImage.sprite = sp;
                break;
            }
        }
    }

    protected override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);
        close();

        StarsLayerView view = mvcCharge.getInstance(typeof(StarsLayerView) as ICLRType) as StarsLayerView;
        view.show();
    }
}
