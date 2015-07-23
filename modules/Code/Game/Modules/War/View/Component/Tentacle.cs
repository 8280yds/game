using CLRSharp;
using Freamwork;
using UnityEngine;

public class Tentacle
{
    private static int getSiblingIndex()
    {
        return m_SiblingIndex++;
    }
    private static int m_SiblingIndex = 0;

    //===============================================
    private TentacleNode[] nodeArr;
    private Vector2[] nodePositionArr;
    private Vector3 nodeRetation;
    private int siblingIndex;

    private TentacleData data;

    public CellWarView view
    {
        get
        {
            if (m_view == null)
            {
                m_view = MVCCharge.instance.getInstance(typeof(CellWarView) as ICLRType) as CellWarView;
            }
            return m_view;
        }
    }
    private CellWarView m_view;

    //==================================================================
    public Tentacle()
    {
        siblingIndex = getSiblingIndex();
    }

    public void setNodes(Cell sourCell, Cell destCell)
    {
        RectTransform rectTF = sourCell.transform as RectTransform;
        Vector2 sour = rectTF.anchoredPosition;
        rectTF = destCell.transform as RectTransform;
        Vector2 dest = rectTF.anchoredPosition;

        float d = Vector2.Distance(sour, dest);
        float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
        float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
        float destX = dest.x + CellConstant.CELL_R * (sour.x - dest.x) / d;
        float destY = dest.y + CellConstant.CELL_R * (sour.y - dest.y) / d;

        setNodes(new Vector2(sourX, sourY), new Vector2(destX, destY)); 
    }

    public void setNodes(Cell sourCell, Vector2 dest)
    {
        RectTransform rectTF = sourCell.transform as RectTransform;
        Vector2 sour = rectTF.anchoredPosition;
        float d = Vector2.Distance(sour, dest);
        if (d <= CellConstant.CELL_R + CellConstant.NODE_D)
        {
            clear();
            return;
        }
        float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
        float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
        setNodes(new Vector2(sourX, sourY), dest);
    }

    public void setNodes(Vector2 sour, Vector2 dest)
    {
        clear();

        //计算触手旋转角度
        float angle = VectorUtil.Vector2Angle(dest - sour, Vector2.right);
        nodeRetation = new Vector3(0, 0, angle);

        //计算触手单元数量
        float d = Vector2.Distance(sour, dest);
        int len = (int)(d / CellConstant.NODE_D);

        //步长
        float dx2 = CellConstant.NODE_D * (sour.x - dest.x) / d;
        float dy2 = CellConstant.NODE_D * (sour.y - dest.y) / d;
        Vector2 dv = new Vector2(-dx2, -dy2);

        //多余的距离分摊到触手两端，并且移动半个步长(因为原点在中心)
        float f = 0.5f + (d - len * CellConstant.NODE_D) / CellConstant.NODE_D / 2;
        Vector2 position = new Vector2(sour.x + f * dv.x, sour.y + f * dv.y);

        nodePositionArr = new Vector2[len];
        nodeArr = new TentacleNode[len];
        for (int i = 0; i < len; i++)
        {
            nodePositionArr[i] = position + dv * i;
        }
    }

    /// <summary>
    /// 清除全部触手单元
    /// </summary>
    public void clear()
    {
        if(nodeArr != null)
        {
            CellWarManager mana = CellWarManager.instance;
            for (int i = 0, len = nodePositionArr.Length; i < len; i++)
            {
                if (nodeArr[i] != null)
                {
                    mana.removeNode(nodeArr[i]);
                }
            }
            nodeArr = null;
        }
        data = null;
    }

    /// <summary>
    /// 根据TentacleData更新
    /// </summary>
    public void updateByData(TentacleData data)
    {
        this.data = data;

        TentacleNode node;
        int lenA = data.nodeListA.Count;
        int lenB = data.nodeListB.Count;
        Color colorA = view.getColorByCellIndex(data.indexA);
        Color colorB = view.getColorByCellIndex(data.indexB);
        CellWarManager mana = CellWarManager.instance;

        Color color;
        Vector3 retation;

        for (int i = 0, len = data.count; i < len; i++)
        {
            node = nodeArr[i];
            
            if (i < lenA)
            {
                if (data.cutStatus == CutStatus.CUT_B)
                {
                    color = data.nodeListA[i] ? Color.yellow : colorB;
                    retation = nodeRetation + new Vector3(0, 0, 180);
                }
                else
                {
                    color = data.nodeListA[i] ? Color.yellow : colorA;
                    retation = nodeRetation;
                }

                if (node == null)
                {
                    node = mana.addNode(nodePositionArr[i], retation, view.tentacleLayer);
                    node.transform.SetSiblingIndex(siblingIndex);
                    node.color = color;
                    nodeArr[i] = node;
                }
                else
                {
                    node.color = color;
                    node.transform.eulerAngles = retation;
                    node.gameObject.SetActive(true);
                }
            }
            else if(i >= len - lenB)
            {
                if (data.cutStatus == CutStatus.CUT_A)
                {
                    color = data.nodeListB[len - i - 1] ? Color.yellow : colorA;
                    retation = nodeRetation;
                }
                else
                {
                    color = data.nodeListB[len - i - 1] ? Color.yellow : colorB;
                    retation = nodeRetation + new Vector3(0, 0, 180);
                }

                if (node == null)
                {
                    node = mana.addNode(nodePositionArr[i], retation, view.tentacleLayer);
                    node.transform.SetSiblingIndex(siblingIndex);
                    node.color = color;
                    nodeArr[i] = node;
                }
                else
                {
                    node.color = color;
                    node.transform.eulerAngles = retation;
                    node.gameObject.SetActive(true);
                }
            }
            else if (node != null)
            {
                node.gameObject.SetActive(false);
            }
        }
    }
}
