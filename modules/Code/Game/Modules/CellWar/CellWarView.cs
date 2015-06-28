using Freamwork;
using System.Collections.Generic;
using UnityEngine;

public class CellWarView : Window
{
    private List<Cell> cellList = new List<Cell>();
    private List<Hand> mouseHandList = new List<Hand>();

    /// <summary>
    /// 触手层
    /// </summary>
    public RectTransform handsLayer
    {
        get
        {
            if(m_handsLayer == null)
            {
                m_handsLayer = transform.FindChild("HandsLayer").transform as RectTransform;
            }
            return m_handsLayer;
        }
    }
    private RectTransform m_handsLayer;

    /// <summary>
    /// 细胞层
    /// </summary>
    public RectTransform cellsLayer
    {
        get
        {
            if (m_cellsLayer == null)
            {
                m_cellsLayer = transform.FindChild("CellsLayer").transform as RectTransform;
            }
            return m_cellsLayer;
        }
    }
    private RectTransform m_cellsLayer;

    //============================================================================
    public override void show(string assetName = "cellwarview.assets")
    {
        base.show(assetName);
    }

    protected override void onShow()
    {
        base.onShow();
        CellWarManager mani = CellWarManager.instance;

        cellList.Add(mani.addCell(new Vector3(-227, 75, 0), Camp.GREEN, 100, cellsLayer));
        cellList.Add(mani.addCell(new Vector3(-127, -75, 0), Camp.GREEN, 30, cellsLayer));
        cellList.Add(mani.addCell(new Vector3(127, 75, 0), Camp.BLUE, 50, cellsLayer));
        cellList.Add(mani.addCell(new Vector3(227, -75, 0), Camp.BLUE, 12, cellsLayer));
    }

    public void updateMouseHands(Vector2 sour, Vector2 dest)
    {
        removeMouseHands();
        mouseHandList = CellWarManager.instance.addHandsByTwoPoint(sour, dest, Color.yellow, handsLayer);
    }

    public void removeMouseHands()
    {
        foreach (Hand hand in mouseHandList)
        {
            CellWarManager.instance.removeHand(hand);
        }
        mouseHandList.Clear();
    }

}
