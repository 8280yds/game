﻿using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.UI;

public class Star : GMB
{
    private int warId;

    private Text txt
    {
        get
        {
            if (m_txt == null)
            {
                m_txt = transform.FindChild("Text").GetComponent<Text>();
            }
            return m_txt;
        }
    }
    private Text m_txt;

    private Image image
    {
        get
        {
            if (m_image == null)
            {
                m_image = transform.GetComponent<Image>();
            }
            return m_image;
        }
    }
    private Image m_image;

    //===========================================================================
    public void setData(int px, int py, Sprite sp, int warId, string str = "")
    {
        RectTransform rectTF = transform as RectTransform;
        rectTF.anchoredPosition = new Vector2(px, py);
        image.sprite = sp;
        txt.text = str;
        this.warId = warId;
    }

    protected override void OnPointerClick(UnityEngine.EventSystems.PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        CellWarView view = MVCCharge.instance.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
        view.show(warId);
        StarsLayerView starsLayerView = MVCCharge.instance.getInstance(typeof(StarsLayerView) as ICLRType) as StarsLayerView;
        starsLayerView.close();
    }
    
}