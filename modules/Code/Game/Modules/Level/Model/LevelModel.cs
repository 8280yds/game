using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using System.IO;
using System.Xml;

public class LevelModel : Model
{
    public int enemyLevel
    {
        get
        {
            return m_enemyLevel;
        }
        set
        {
            m_enemyLevel = value;
            dispatch(LevelUpdateConstant.UPDATE_LEVEL_VIEW);
        }
    }
    private int m_enemyLevel = 1;

    public int starIndex
    {
        get
        {
            return m_starIndex;
        }
        set
        {
            m_starIndex = value;
        }
    }
    private int m_starIndex;

    private List<List<int>> levelList;

    /// <summary>
    /// 初始化等级数据
    /// </summary>
    public void initLevelData()
    {
        string xmlStr = "<Data><Level></Level></Data>";
        if (File.Exists(LoadConstant.localFilesPath + "/data.xml"))
        {
            xmlStr = File.ReadAllText(LoadConstant.localFilesPath + "/data.xml");
        }

        XmlDocument xmlDocument = new XmlDocument();
        xmlDocument.LoadXml(xmlStr);

        LevelDBModel dbModel = mvcCharge.getInstance(typeof(LevelDBModel) as ICLRType) as LevelDBModel;
        List<LevelDBVO> voList = dbModel.getAllData();

        levelList = new List<List<int>>();
        XmlNode node;

        for (int i = 0, len = voList.Count; i < len; i++)
        {
            List<int> list = new List<int>();
            levelList.Add(list);

            int len2 = voList[i].war.Split(',').Length;
            node = xmlDocument.SelectSingleNode("Data/Level/Item[@level='" + voList[i].id + "']");
            string nodeString = node != null ? ((XmlElement)node).GetAttribute("stars") : "";
            string[] strs = nodeString.Split(',');
            int strsLen = strs.Length;

            for (int j = 0; j < len2; j++)
            {
                list.Add(j < strsLen && !string.IsNullOrEmpty(strs[j]) ? int.Parse(strs[j]) : 0);
            }
        }
    }

    /// <summary>
    /// 设置星星数量
    /// </summary>
    public void setLevelStar(int level, int index, int starNum)
    {
        if (levelList[level - 1][index] >= starNum)
        {
            return;
        }
        levelList[level - 1][index] = starNum;

        string str = "<Data><Level>";
        for (int i = 0, len = levelList.Count; i < len; i++)
        {
            str += "<Item level = \"" + (i + 1) + "\" stars = \"";
            for (int j = 0, len2 = levelList[i].Count; j < len2; j++)
            {
                str += j == 0 ? "" + levelList[i][j] : "," + levelList[i][j];
            }
            str += "\" />";
        }
        str += "</Level></Data>";
        File.WriteAllText(LoadConstant.localFilesPath + "/data.xml", str);
    }

    /// <summary>
    /// 获取星星数量
    /// </summary>
    /// <param name="level"></param>
    /// <param name="index"></param>
    public int getLevelStar(int level, int index)
    {
        return levelList[level - 1][index];
    }

    /// <summary>
    /// 当前可以进攻的最高文明等级
    /// </summary>
    /// <returns></returns>
    public int getCurrentMaxLevel()
    {
        int maxLevel = 1;
        for (int i = 0, len = levelList.Count; i < len; i++)
        {
            for (int j = 0, len2 = levelList[i].Count; j < len2; j++)
            {
                if (levelList[i][j] <= 0)
                {
                    return maxLevel;
                }
            }
            maxLevel++;
        }
        return levelList.Count;
    }

    /// <summary>
    /// 当前关卡的关数
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int getLevelMaxNum(int level)
    {
        return levelList[level - 1].Count;
    }

    /// <summary>
    /// 当前关卡的总星数
    /// </summary>
    /// <param name="level"></param>
    /// <returns></returns>
    public int getLevelStarNum(int level)
    {
        int starNum = 0;
        for (int i = 0, len = levelList[level - 1].Count; i < len; i++)
        {
            starNum += levelList[level - 1][i];
        }
        return starNum;
    }

}
