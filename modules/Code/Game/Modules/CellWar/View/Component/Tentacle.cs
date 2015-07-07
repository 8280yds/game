using CLRSharp;
using Freamwork;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : GMB
{
    private List<TentacleNode> m_nodes = new List<TentacleNode>();

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
    public void setNodes(Cell sourCell, Cell destCell)
    {
        Vector3 p = sourCell.transform.position;
        Vector2 sour = new Vector2(p.x, p.y);
        p = destCell.transform.position;
        Vector2 dest = new Vector2(p.x, p.y);

        float d = Vector2.Distance(sour, dest);
        float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
        float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
        float destX = dest.x + CellConstant.CELL_R * (sour.x - dest.x) / d;
        float destY = dest.y + CellConstant.CELL_R * (sour.y - dest.y) / d;

        setNodes(new Vector2(sourX, sourY), new Vector2(destX, destY)); 
    }

    public void setNodes(Cell sourCell, Vector2 dest)
    {
        Vector3 p = sourCell.transform.position;
        Vector2 sour = new Vector2(p.x, p.y);
        float d = Vector2.Distance(sour, dest);
        if (d <= CellConstant.CELL_R + CellConstant.NODE_D)
        {
            clearNodes();
            return;
        }
        float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
        float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
        setNodes(new Vector2(sourX, sourY), dest);
    }

    public void setNodes(Vector2 sour, Vector2 dest)
    {
        clearNodes();

        //计算触手旋转角度
        float angle = VectorUtil.Vector2Angle(dest - sour, Vector2.right);
        Vector3 rotation = new Vector3(0, 0, angle);

        //计算触手单元数量
        float d = Vector2.Distance(sour, dest);
        int len = (int)(d / CellConstant.NODE_D);

        //步长
        float dx2 = CellConstant.NODE_D * (sour.x - dest.x) / d;
        float dy2 = CellConstant.NODE_D * (sour.y - dest.y) / d;
        Vector3 dv = new Vector3(-dx2, -dy2, 0);

        //多余的距离分摊到触手两端，并且移动半个步长(因为原点在中心)
        float f = 0.5f + (d - len * CellConstant.NODE_D) / CellConstant.NODE_D / 2;
        Vector3 position = new Vector3(sour.x + f * dv.x, sour.y + f * dv.y, 0);

        CellWarManager mana = CellWarManager.instance;
        for (int i = 0; i < len; i++)
        {
            m_nodes.Add(mana.addNode(position + dv * i, rotation, Color.yellow, transform));
        }
    }

    /// <summary>
    /// 清除全部触手单元
    /// </summary>
    public void clearNodes()
    {
        CellWarManager mana = CellWarManager.instance;
        foreach (TentacleNode node in m_nodes)
        {
            mana.removeNode(node);
        }
        m_nodes.Clear();
    }
    
    public void clear()
    {
        CellWarManager mana = CellWarManager.instance;
        foreach (TentacleNode node in m_nodes)
        {
            node.transform.SetParent(null);
            mana.removeNode(node);
        }
        m_nodes.Clear();
    }

    /// <summary>
    /// 根据TentacleData更新
    /// </summary>
    public void updateByData(TentacleData data)
    {
        if (data.count != m_nodes.Count)
        {
            throw new Exception("TentacleData.count与m_nodes.Count不一致");
        }

        TentacleNode node;
        int lenA = data.nodeListA.Count;
        int lenB = data.nodeListB.Count;
        Color colorA = view.getColorByCellIndex(data.indexA);
        Color colorB = view.getColorByCellIndex(data.indexB);

        for (int i = 0, len = data.count; i < len; i++)
        {
            node = m_nodes[i];
            if (i < lenA)
            {
                node.color = data.nodeListA[i] ? Color.yellow : colorA;
                node.gameObject.SetActive(true);
            }
            else if(i >= len - lenB)
            {
                node.color = data.nodeListB[len-i-1] ? Color.yellow : colorB;
                node.gameObject.SetActive(true);
            }
            else
            {
                node.gameObject.SetActive(false);
            }
        }
    }
}
