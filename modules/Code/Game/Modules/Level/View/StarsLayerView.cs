using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;

public class StarsLayerView : Window
{
    /// <summary>
    /// 星球列表
    /// </summary>
    private List<Star> starsList;

    /// <summary>
    /// 连线
    /// </summary>
    private VectorPoints vectorPoints;

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

    private Text noteText
    {
        get
        {
            if (m_noteText == null)
            {
                m_noteText = transform.FindChild("NoteText").GetComponent<Text>();
            }
            return m_noteText;
        }
    }
    private Text m_noteText;

    private Text starsNumText
    {
        get
        {
            if (m_starsNumText == null)
            {
                m_starsNumText = transform.FindChild("StarsNumText").GetComponent<Text>();
            }
            return m_starsNumText;
        }
    }
    private Text m_starsNumText;

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
        if (vectorPoints == null)
        {
            vectorPoints = new VectorPoints("VectorPoints", new List<Vector2>(), 5f, transform);
            //vectorPoints.material = getAssetsByName("PointMaterial") as Material;
            vectorPoints.color = Color.green;
        }
        else
        {
            vectorPoints.points2.Clear();
        }

        LevelDBVO dbvo = dbModel.getVOById(model.enemyLevel);
        string[] starX = dbvo.starX.Split(',');
        string[] starY = dbvo.starY.Split(',');
        string[] starImage = dbvo.starImage.Split(',');
        string[] war = dbvo.war.Split(',');
        string[] next = dbvo.next.Split(',');

        titleText.text = dbvo.id + "级敌对文明";
        starsNumText.text = "★ " + model.getLevelStarNum(model.enemyLevel) + "/" + (3 * model.getLevelMaxNum(model.enemyLevel));
        int currentMaxLevel = model.getCurrentMaxLevel();

        if (currentMaxLevel > model.enemyLevel)
        {
            titleText.color = Color.green;
            noteText.color = Color.green;
            noteText.text = "已通关该文明的全部关卡";
        }
        else if (currentMaxLevel == model.enemyLevel)
        {
            titleText.color = Color.green;
            noteText.color = Color.green;
            noteText.text = "尚未通关该文明的全部关卡";
        }
        else
        {
            titleText.color = Color.red;
            noteText.color = Color.red;
            noteText.text = "通关前面文明的全部关卡后方可进攻";
        }

        clearStars();
        int len = starX.Length;
        starsList = new List<Star>();
        GameObject profab = getAssetsByName("star") as GameObject;
        Star star;
        GameObject go;
        Sprite sp;
        int starNum;

        for (int i = 0; i < len; i++)
        {
            go = GameObject.Instantiate(profab);
            go.transform.SetParent(transform, false);

            star = new Star();
            star.init(go);
            sp = getAssetsByName("starImage_" + starImage[i]) as Sprite;
            starNum = model.getLevelStar(dbvo.id, i);
            star.setData(i ,int.Parse(starX[i]), int.Parse(starY[i]), sp, int.Parse(war[i]), starNum);
            starsList.Add(star);
        }

        string[] nextList;
        Vector3 pointA;
        Vector3 pointB;
        for (int i = 0; i < len; i++)
        {
            pointA = starsList[i].rectTransform.anchoredPosition;
            nextList = next[i].Split(':');
            for (int j = 0, len2 = nextList.Length; j < len2; j++)
            {
                int index = int.Parse(nextList[j]);
                if (index >= 0)
                {
                    pointB = starsList[index].rectTransform.anchoredPosition;
                    vectorPoints.points2.AddRange(getPointList(pointA, pointB));
                }
            }
        }
        vectorPoints.Draw();
    }

    private List<Vector2> getPointList(Vector2 pointA, Vector2 PointB)
    {
        List<Vector2> list = new List<Vector2>();
        float d = 10;   //点间距
        float D = Vector2.Distance(pointA, PointB);
        float count = D / d;
        float kx = d * (PointB.x - pointA.x) / D;
        float ky = d * (PointB.y - pointA.y) / D;

        list.Add(pointA);
        for (int i = 1; i <= count; i++)
        {
            list.Add(new Vector2(pointA.x + kx * i, pointA.y + ky * i));
        }
        return list;
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
