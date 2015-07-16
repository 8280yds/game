using Freamwork;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 冒泡文字管理类
/// </summary>
sealed public class PopUpTextManager
{
    /// <summary>
    /// 获取实例
    /// </summary>
    static public PopUpTextManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new PopUpTextManager();
            }
            return m_instance;
        }
    }
    static private PopUpTextManager m_instance;

    private PopUpTextManager()
    {
        m_instance = this;
        init();
    }

    private List<string> textList;
    private int count;

    public GameObject textGO;

    private void init()
    {
        textList = new List<string>();
        count = 0;
    }

    /// <summary>
    /// 清理
    /// </summary>
    public void clear()
    {
        textList.Clear();
        EnterFrame.instance.removeEnterFrame(showText);
        count = 0;
    }

    //============================================================
    /// <summary>
    /// 添加一个冒泡文字显示
    /// </summary>
    public void addText(string text)
    {
        textList.Add(text);
        EnterFrame.instance.addEnterFrame(showText);
    }

    /// <summary>
    /// 显示
    /// </summary>
    private void showText()
    {
        if (count == 0)
        {
            if (textList.Count == 0)
            {
                EnterFrame.instance.removeEnterFrame(showText);
            }
            else
            {
                GameObject go = GameObject.Instantiate(textGO);
                RectTransform rectTF = go.transform as RectTransform;
                rectTF.SetParent(UIManager.instance.getUILayer(UILayer.PopUpLayer), false);

                PopUpText popUpText = new PopUpText();
                popUpText.init(go);
                popUpText.txt.text = textList[0];
                textList.RemoveAt(0);
            }
        }
        count = count > 100 ? 0 : count + 1;
    }
}
