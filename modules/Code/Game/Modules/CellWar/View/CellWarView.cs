using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using UnityEngine;

public class CellWarView : Window
{
    private List<Cell> cellList;
    private Tentacle mouseTentacle;

    /// <summary>
    /// 触手列表，key的形式是"3:7"
    /// </summary>
    private Dictionary<string, Tentacle> tentacleDic;

    /// <summary>
    /// 触手层
    /// </summary>
    public RectTransform tentacleLayer
    {
        get
        {
            if (m_tentacleLayer == null)
            {
                m_tentacleLayer = transform.FindChild("TentaclesLayer").transform as RectTransform;
            }
            return m_tentacleLayer;
        }
    }
    private RectTransform m_tentacleLayer;

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

    private CellWarModel model
    {
        get
        {
            if (m_model == null)
            {
                m_model = mvcCharge.getInstance(typeof(CellWarModel) as ICLRType) as CellWarModel;
            }
            return m_model;
        }
    }
    private CellWarModel m_model;

    //============================================================================
    public override void show(string assetName = "")
    {
        base.show("cellwarview.assets");
    }

    protected override void onShow()
    {
        base.onShow();

        cellList = new List<Cell>();
        tentacleDic = new Dictionary<string, Tentacle>();

        initScene(1);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    /// <param name="id"></param>
    private void initScene(int id)
    {
        Cell.SelectedCell = null;
        Cell.DestCell = null;
        clearAllTentacle();
        clearAllCell();
        mouseTentacle = CellWarManager.instance.addTentacle(tentacleLayer);

        //数据初始化
        CellWarSceneDBModel sceneDBModel = mvcCharge.getInstance(typeof(CellWarSceneDBModel) as ICLRType) as CellWarSceneDBModel;
        CellWarSceneDBVO sceneDBVO = sceneDBModel.getVOById(id);
        model.initScene(sceneDBVO);

        //布置细胞
        ViewStatus viewStatus = model.lastUpdateViewStatus;
        Cell cell;
        CellData cellData;
        for (int i = 0, len = viewStatus.cellDataList.Count; i < len; i++ )
        {
            cellData = viewStatus.cellDataList[i];
            cell = CellWarManager.instance.addCell(cellData.position, cellsLayer);
            cell.index = cellData.index;
            cell.camp = cellData.camp;
            cell.hp = cellData.hp;
            cellList.Add(cell);
        }
    }

    protected override void addListeners()
    {
        base.addListeners();
        addListener(CellWarUpdate.UPDATE_VIEW_STATUS, updateViewStatus);
    }

    protected override void Update()
    {
        base.Update();
        model.getCurrentViewStatus();
    }

    /// <summary>
    /// 更新界面
    /// </summary>
    private void updateViewStatus()
    {
        ViewStatus viewStatus = model.lastUpdateViewStatus;
        Cell cell;
        foreach (CellData cellData in viewStatus.cellDataList)
        {
            cell = cellList[cellData.index];
            cell.camp = cellData.camp;
            cell.hp = cellData.hp;
        }

        TentacleData data;
        foreach (string key in viewStatus.tentacleDataDic.Keys)
        {
            data = viewStatus.tentacleDataDic[key];
            if (data.nodeListA.Count == 0 && data.nodeListB.Count == 0)
            {
                if (tentacleDic.ContainsKey(key))
                {
                    CellWarManager.instance.removeTentacle(tentacleDic[key]);
                    tentacleDic.Remove(key);
                }
            }
            else
            {
                if (!tentacleDic.ContainsKey(key))
                {
                    getTentacle(cellList[data.indexA], cellList[data.indexB]);
                }
                tentacleDic[key].updateByData(data);
            }
        }
    }

    //==============================================================================
    /// <summary>
    /// 更新鼠标指引触手
    /// </summary>
    /// <param name="sourCell"></param>
    /// <param name="destCell"></param>
    public void updateMouseTentacle(Cell sourCell, Cell destCell)
    {
        mouseTentacle.setNodes(sourCell, destCell);
    }

    /// <summary>
    /// 更新鼠标指引触手
    /// </summary>
    /// <param name="sourCell"></param>
    /// <param name="dest"></param>
    public void updateMouseTentacle(Cell sourCell, Vector2 dest)
    {
        mouseTentacle.setNodes(sourCell, dest);
    }

    /// <summary>
    /// 隐藏鼠标指示触手
    /// </summary>
    /// <param name="sourCell"></param>
    /// <param name="dest"></param>
    public void hideMouseTentacle()
    {
        mouseTentacle.clearNodes();
    }

    /// <summary>
    /// 获取两个细胞间的触手，如果不存在将创建
    /// </summary>
    /// <param name="cell1"></param>
    /// <param name="cell2"></param>
    /// <returns></returns>
    private Tentacle getTentacle(Cell cellA, Cell cellB)
    {
        if (cellA.index > cellB.index)
        {
            Cell cell = cellA;
            cellA = cellB;
            cellB = cell;
        }

        string key = cellA.index + ":" + cellB.index;
        if (tentacleDic.ContainsKey(key))
        {
            return tentacleDic[key];
        }

        Tentacle tentacle = CellWarManager.instance.addTentacle(tentacleLayer);
        tentacle.setNodes(cellA, cellB);
        tentacleDic.Add(key, tentacle);
        return tentacle;
    }

    /// <summary>
    /// 清除全部触手
    /// </summary>
    private void clearAllTentacle()
    {
        foreach (Tentacle tentacle in tentacleDic.Values)
        {
            CellWarManager.instance.removeTentacle(tentacle);
        }
        if (mouseTentacle != null)
        {
            CellWarManager.instance.removeTentacle(mouseTentacle);
        }
    }

    /// <summary>
    /// 清除所有细胞
    /// </summary>
    private void clearAllCell()
    {
        foreach (Cell cell in cellList)
        {
            CellWarManager.instance.removeCell(cell);
        }
        cellList.Clear();
    }

    /// <summary>
    /// 根据细胞索引获取细胞颜色
    /// </summary>
    /// <returns></returns>
    public Color getColorByCellIndex(int index)
    {
        return CellConstant.CAMP_COLOR_ARR[(int)cellList[index].camp];
    }
}
