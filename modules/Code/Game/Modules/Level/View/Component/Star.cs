using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Star : GMB
{
    private int warId;
    private int index;

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
    public void setData(int index, int px, int py, Sprite sp, int warId, int starNum)
    {
        string str = "";
        if (starNum > 0)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < starNum)
                {
                    str += "★";
                }
                else
                {
                    str += "☆";
                }
            }
        }

        RectTransform rectTF = transform as RectTransform;
        rectTF.anchoredPosition = new Vector2(px, py);
        image.sprite = sp;
        txt.text = str;
        this.warId = warId;
        this.index = index;
    }

    protected override void OnPointerClick(PointerEventData eventData)
    {
        base.OnPointerClick(eventData);

        MVCCharge mvcCharge = MVCCharge.instance;
        LevelModel model = mvcCharge.getInstance(typeof(LevelModel) as ICLRType) as LevelModel;

        if (model.getLevelStar(model.enemyLevel, index) <= 0)
        {
            LevelDBModel dbModel = mvcCharge.getInstance(typeof(LevelDBModel) as ICLRType) as LevelDBModel;
            LevelDBVO dbvo = dbModel.getVOById(model.enemyLevel);
            string[] nexts = dbvo.next.Split(',');
            string[] list = nexts[index].Split(':');
            bool canAttack = false;
            int maxLevel = model.getCurrentMaxLevel();

            for (int i = 0, len = list.Length; i < len; i++)
            {
                int t = int.Parse(list[i]);
                if ((t >= 0 && model.getLevelStar(dbvo.id, t) > 0) || (t < 0 && maxLevel >= model.enemyLevel))
                {
                    canAttack = true;
                    break;
                }
            }

            if (!canAttack)
            {
                PopUpTextManager.instance.addText("<color=red>请先通关前面的关卡</color>");
                return;
            }
        }
        model.starIndex = index;
        CellWarView view = mvcCharge.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
        view.show(warId);
        StarsLayerView starsLayerView = mvcCharge.getInstance(typeof(StarsLayerView) as ICLRType) as StarsLayerView;
        starsLayerView.close();
    }

}