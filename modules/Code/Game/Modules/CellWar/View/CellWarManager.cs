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
        m_instance = this;
        init();
    }

    private List<TentacleNode> nodePool;

    public GameObject cellPrefab;
    public GameObject nodePrefab;

    private void init()
    {
        nodePool = new List<TentacleNode>();
    }

    /// <summary>
    /// 清除所有帧频事件
    /// </summary>
    public void clear()
    {
        foreach (TentacleNode node in nodePool)
        {
            node.dispose();
        }
        nodePool.Clear();
    }

    //======================================================
    /// <summary>
    /// 添加触手单元
    /// </summary>
    public TentacleNode addNode(Vector3 position, Vector3 rotation, Color color, Transform parent)
    {
        TentacleNode node;
        if (nodePool.Count > 0)
        {
            node = nodePool[0];
            node.color = color;
            nodePool.RemoveAt(0);
        }
        else
        {
            GameObject go = GameObject.Instantiate<GameObject>(nodePrefab);
            node = new TentacleNode(color);
            node.init(go);
        }
        Transform tf = node.transform;

        if (tf.parent != parent)
        {
            tf.SetParent(parent, false);
        }
        tf.position = position;
        tf.eulerAngles = rotation;
        tf.gameObject.SetActive(true);
        return node;
    }

    /// <summary>
    /// 移除触手单元
    /// </summary>
    /// <param name="node"></param>
    public void removeNode(TentacleNode node)
    {
        node.gameObject.SetActive(false);
        nodePool.Add(node);
    }

    /// <summary>
    /// 添加细胞
    /// </summary>
    public Cell addCell(Vector3 position, Transform parent)
    {
        GameObject cellGO = GameObject.Instantiate<GameObject>(cellPrefab);
        Cell cell = new Cell();
        cell.init(cellGO);
        cell.transform.SetParent(parent, false);
        cell.transform.localPosition = position;
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
    /// 添加触手
    /// </summary>
    /// <param name="parent"></param>
    /// <returns></returns>
    public Tentacle addTentacle(Transform parent)
    {
        GameObject go = new GameObject("Tentacle");
        RectTransform tf = go.AddComponent<RectTransform>();
        Tentacle tentacle = new Tentacle();
        tentacle.init(go);
        tf.SetParent(parent, false);
        return tentacle;
    }

    /// <summary>
    /// 移除触手
    /// </summary>
    /// <param name="tentacle"></param>
    public void removeTentacle(Tentacle tentacle)
    {
        tentacle.clear();
        tentacle.dispose();
    }
}