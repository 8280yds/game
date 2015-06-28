using Freamwork;
using System;
using System.Collections.Generic;
using UnityEngine;

sealed public class CellWarManager
{
    static public CellWarManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new CellWarManager();
            }
            return m_instance;
        }
    }
    static private CellWarManager m_instance;

    private CellWarManager()
    {
        if (m_instance != null)
        {
            throw new Exception("CellWarManager是单例，请使用CellWarManager.instance来获取其实例！");
        }

        m_instance = this;
        init();
    }

    private List<Hand> handPool;

    public GameObject cellPrefab;
    public GameObject handPrefab;

    private void init()
    {
        handPool = new List<Hand>();
    }

    /// <summary>
    /// 清除所有帧频事件
    /// </summary>
    public void clear()
    {
        foreach (Hand hand in handPool)
        {
            hand.dispose();
        }
        handPool.Clear();
    }

    //======================================================
    /// <summary>
    /// 添加触手单元
    /// </summary>
    public Hand addHand(Vector3 position, Vector3 roration, Color color, Transform parent)
    {
        Hand hand;
        if (handPool.Count > 0)
        {
            hand = handPool[0];
            hand.color = color;
            handPool.RemoveAt(0);
        }
        else
        {
            GameObject handGO = GameObject.Instantiate<GameObject>(handPrefab);
            hand = new Hand(color);
            hand.init(handGO);
        }
        Transform tf = hand.transform;

        if (tf.parent != parent)
        {
            tf.SetParent(parent, false);
        }
        else
        {
            tf.SetAsLastSibling();
        }
        tf.position = position;
        tf.eulerAngles = roration;
        tf.gameObject.SetActive(true);
        return hand;
    }

    /// <summary>
    /// 移除触手单元
    /// </summary>
    /// <param name="hand"></param>
    public void removeHand(Hand hand)
    {
        hand.gameObject.SetActive(false);
        handPool.Add(hand);
    }

    /// <summary>
    /// 添加细胞
    /// </summary>
    public Cell addCell(Vector3 position, Camp camp, int hp, Transform parent)
    {
        GameObject cellGO = GameObject.Instantiate<GameObject>(cellPrefab);
        Cell cell = new Cell(camp, hp);
        cell.init(cellGO);
        cell.transform.position = position;
        cell.transform.SetParent(parent, false);
        cellGO.SetActive(true);
        return cell;
    }

    /// <summary>
    /// 移除细胞
    /// </summary>
    public void removeCell(Cell cell)
    {
        cell.dispose();
    }

    /// <summary>
    /// 在两点间创建触手
    /// </summary>
    public List<Hand> addHandsByTwoPoint(Vector2 sour, Vector2 dest, Color color, Transform parent)
    {
        //计算触手旋转角度
        float angle = VectorUtil.Vector2Angle(dest - sour, Vector2.right);
        Vector3 rotation = new Vector3(0, 0, angle);

        //计算触手单元数量
        float d = Vector2.Distance(sour, dest);
        int len = (int)(d / CellConstant.HAND_D);

        //步长
        float dx2 = CellConstant.HAND_D * (sour.x - dest.x) / d;
        float dy2 = CellConstant.HAND_D * (sour.y - dest.y) / d;
        Vector3 dv = new Vector3(-dx2, -dy2, 0);

        //多余的距离分摊到触手两端，并且移动半个步长(因为原点在中心)
        float f = 0.5f + (d - len * CellConstant.HAND_D) / CellConstant.HAND_D / 2;
        Vector3 position = new Vector3(sour.x + f * dv.x, sour.y + f * dv.y, 0);

        List<Hand> handList = new List<Hand>();
        for (int i = 0; i < len; i++)
        {
            handList.Add(addHand(position + dv * i, rotation, color, parent));
        }
        return handList;
    }





}