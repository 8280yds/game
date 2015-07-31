using CLRSharp;
using Freamwork;
using UnityEngine;
using UnityEngine.UI;

public class Title : GMB
{
    private Text levelText
    {
        get
        {
            if (m_levelText == null)
            {
                m_levelText = transform.FindChild("LevelText").GetComponent<Text>();
            }
            return m_levelText;
        }
    }
    private Text m_levelText;

    private Text actionText
    {
        get
        {
            if (m_actionText == null)
            {
                m_actionText = transform.FindChild("ActionText").GetComponent<Text>();
            }
            return m_actionText;
        }
    }
    private Text m_actionText;

    private Text timeText
    {
        get
        {
            if (m_timeText == null)
            {
                m_timeText = transform.FindChild("TimeText").GetComponent<Text>();
            }
            return m_timeText;
        }
    }
    private Text m_timeText;

    private Text starText2
    {
        get
        {
            if (m_starText2 == null)
            {
                m_starText2 = transform.FindChild("StarText_2").GetComponent<Text>();
            }
            return m_starText2;
        }
    }
    private Text m_starText2;

    private Text starText3
    {
        get
        {
            if (m_starText3 == null)
            {
                m_starText3 = transform.FindChild("StarText_3").GetComponent<Text>();
            }
            return m_starText3;
        }
    }
    private Text m_starText3;

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

    private LevelModel levelModel
    {
        get
        {
            if (m_levelModel == null)
            {
                m_levelModel = MVCCharge.instance.getInstance(typeof(LevelModel) as ICLRType) as LevelModel;
            }
            return m_levelModel;
        }
    }
    private LevelModel m_levelModel;

    //==========================================================================
    public override void init(GameObject gameObject)
    {
        base.init(gameObject);
        backBtn.onClick.AddListener(onBackBtnClick);
    }

    private void onBackBtnClick()
    {
        CellWarView cellWarView = MVCCharge.instance.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
        cellWarView.close();
        StarsLayerView view = MVCCharge.instance.getInstance(typeof(StarsLayerView) as ICLRType) as StarsLayerView;
        view.show();
    }

    public void updateView(ViewStatus viewStatus)
    {
        levelText.text = levelModel.enemyLevel + "级文明";
        actionText.text = "操作：" + viewStatus.actionCount + "次";

        int time = (int)(viewStatus.time / 1000);
        timeText.text = "时间：" + time + "秒";

        string[] starTimes = viewStatus.vo.star.Split(',');
        
        int starTime = int.Parse(starTimes[0]);
        float alpha = Mathf.Max(1 - (float)time / starTime, 0);
        Color color = starText3.color;
        starText3.color = new Color(color.r, color.g, color.b, alpha);

        time -= starTime;
        starTime = int.Parse(starTimes[1]) - starTime;
        alpha = Mathf.Max(1 - (float)time / starTime, 0);
        color = starText2.color;
        starText2.color = new Color(color.r, color.g, color.b, alpha);
    }

    public override void dispose()
    {
        backBtn.onClick.RemoveListener(onBackBtnClick);
        base.dispose();
    }

}
