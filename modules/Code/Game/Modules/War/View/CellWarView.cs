using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Vectrosity;

public class CellWarView : Window
{
    private List<Cell> cellList;

    private VectorLine mouseLine;
    private List<long> mouseLinePointTime;

    public bool isMouseDown
    {
        get;
        private set;
    }

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
    protected override void onShow()
    {
        base.onShow();

        CellWarManager.instance.setPrefab(getAssetsByName("cell") as GameObject, getAssetsByName("node") as GameObject);
        cellList = new List<Cell>();
        tentacleDic = new Dictionary<string, Tentacle>();
        initScene((int)windowParam);
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

        //数据初始化
        CellWarSceneDBModel sceneDBModel = mvcCharge.getInstance(typeof(CellWarSceneDBModel) as ICLRType) as CellWarSceneDBModel;
        CellWarSceneDBVO sceneDBVO = sceneDBModel.getVOById(id);
        model.initScene(sceneDBVO);

        //布置细胞
        ViewStatus viewStatus = model.lastUpdateViewStatus;
        Cell cell;
        CellData cellData;
        for (int i = 0, len = viewStatus.cellDataList.Count; i < len; i++)
        {
            cellData = viewStatus.cellDataList[i];
            cell = CellWarManager.instance.addCell(cellData.position, cellsLayer);
            cell.index = cellData.index;
            cellList.Add(cell);
        }

        //初始化鼠标指挥线
        mouseLine = new VectorLine("MouseLine", new List<Vector2>(), 5f, LineType.Continuous, transform);
        mouseLine.color = Color.red;
        mouseLinePointTime = new List<long>();
    }

    protected override void addListeners()
    {
        base.addListeners();
        addListener(CellWarUpdate.UPDATE_VIEW_STATUS, updateViewStatus);
        addListener(CellWarUpdate.GAME_OVER, gameOver);
    }

    protected override void Update()
    {
        base.Update();
        model.getCurrentViewStatus();

        if (isMouseDown && Cell.SelectedCell == null)
        {
            Vector2 pos;
            Canvas canvas = UIManager.instance.canvas;
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                canvas.transform as RectTransform, Input.mousePosition, Camera.main, out pos))
            {
                int len = mouseLine.points2.Count;
                if (len > 0 && Vector2.Distance(pos, mouseLine.points2[len - 1]) > 2)
                {
                    //判断切割
                    cut(mouseLine.points2[len - 1], pos);
                    //绘制轨迹
                    mouseLine.points2.Add(pos);
                    mouseLinePointTime.Add(TimeUtil.getTimeStamp(false));
                }
                else if (len == 0)
                {
                    //绘制轨迹
                    mouseLine.points2.Add(pos);
                    mouseLinePointTime.Add(TimeUtil.getTimeStamp(false));
                }
            }
        }

        if (mouseLinePointTime != null && mouseLinePointTime.Count > 0)
        {
            long time = TimeUtil.getTimeStamp(false);
            while (time - mouseLinePointTime[0] > 100)
            {
                mouseLinePointTime.RemoveAt(0);
                mouseLine.points2.RemoveAt(0);

                if (mouseLinePointTime.Count == 0)
                {
                    return;
                }
            }
            mouseLine.Draw();
        }
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
            cell.UpdateByData(cellData);
        }

        TentacleData data;
        Tentacle ten;
        foreach (string key in viewStatus.tentacleDataDic.Keys)
        {
            data = viewStatus.tentacleDataDic[key];

            if (data.nodeListA.Count > 0 || data.nodeListB.Count > 0)
            {
                ten = getTentacle(cellList[data.indexA], cellList[data.indexB]);
                ten.updateByData(data);
            }
            else if (tentacleDic.ContainsKey(key))
            {
                tentacleDic[key].clear();
                tentacleDic.Remove(key);
            }
        }
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    private void gameOver()
    {
        close();
        WarResultView view = mvcCharge.getInstance(typeof(WarResultView) as ICLRType) as WarResultView;
        view.show();
    }

    //==============================================================================
    /// <summary>
    /// 更新鼠标指引触手
    /// </summary>
    /// <param name="sourCell"></param>
    /// <param name="destCell"></param>
    public void updateMouseTentacle(Cell sourCell, Cell destCell)
    {
        RectTransform rectTF = sourCell.transform as RectTransform;
        Vector2 sour = rectTF.anchoredPosition;
        rectTF = destCell.transform as RectTransform;
        Vector2 dest = rectTF.anchoredPosition;

        mouseLine.points2.Clear();
        mouseLinePointTime.Clear();
        mouseLine.points2.Add(sour);
        mouseLine.points2.Add(dest);
        mouseLine.Draw();
    }

    /// <summary>
    /// 更新鼠标指引触手
    /// </summary>
    /// <param name="sourCell"></param>
    /// <param name="dest"></param>
    public void updateMouseTentacle(Cell sourCell, Vector2 dest)
    {
        RectTransform rectTF = sourCell.transform as RectTransform;
        Vector2 sour = rectTF.anchoredPosition;

        mouseLine.points2.Clear();
        mouseLinePointTime.Clear();
        mouseLine.points2.Add(sour);
        mouseLine.points2.Add(dest);
        mouseLine.Draw();
    }

    /// <summary>
    /// 隐藏鼠标指示触手
    /// </summary>
    /// <param name="sourCell"></param>
    /// <param name="dest"></param>
    public void hideMouseTentacle()
    {
        mouseLine.points2.Clear();
        mouseLine.Draw();
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

        Tentacle tentacle = new Tentacle();
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
            tentacle.clear();
        }
        tentacleDic.Clear();
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

    /// <summary>
    /// 攻击
    /// </summary>
    public void attack(Cell cellA, Cell cellB)
    {
        ViewStatus viewStatus = model.lastUpdateViewStatus;
        if (viewStatus.hasTentacleData(cellA.index, cellB.index))
        {
            TentacleData tentacleData = viewStatus.getTentacleData(cellA.index, cellB.index);
            if ((cellA.index < cellB.index && tentacleData.isAttackA) ||
                (cellA.index > cellB.index && tentacleData.isAttackB) ||
                tentacleData.cutStatus != CutStatus.NONE)
            {
                return;
            }
        }
        ActionData actionData = new ActionData();
        actionData.time = model.getCurrentTime();
        actionData.cellAIndex = (byte)cellA.index;
        actionData.cellBIndex = (byte)cellB.index;
        actionData.type = 0;    //0:连接 1:切断
        model.actionData = actionData;
    }

    /// <summary>
    /// 切断
    /// </summary>
    private void retreat(int indexA, int indexB, int index)
    {
        ActionData actionData = new ActionData();
        actionData.time = model.getCurrentTime();
        actionData.cellAIndex = (byte)indexA;
        actionData.cellBIndex = (byte)indexB;
        actionData.type = 1;    //0:连接 1:切断
        actionData.index = (byte)index;
        model.actionData = actionData;
    }

    protected override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

        isMouseDown = true;
        if (mouseLine != null)
        {
            mouseLine.points2.Clear();
            mouseLinePointTime.Clear();
        }
    }

    protected override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        isMouseDown = false;
    }

    /// <summary>
    /// 通过计算交点，计算出切断位置
    /// </summary>
    /// <param name="pos1"></param>
    /// <param name="pos2"></param>
    private void cut(Vector2 pos1, Vector2 pos2)
    {
        ViewStatus viewStatus = model.lastUpdateViewStatus;
        Vector3 pos;
        Vector2 pos3;
        Vector2 pos4;
        int cutIndex;
        CellData cellDataA;
        CellData cellDataB;

        foreach (TentacleData data in viewStatus.tentacleDataDic.Values)
        {
            if (data.cutStatus == CutStatus.NONE)
            {
                cellDataA = viewStatus.cellDataList[data.indexA];
                cellDataB = viewStatus.cellDataList[data.indexB];
                pos3 = cellDataA.position;
                pos4 = cellDataB.position;

                if (cellDataA.camp == Camp.GREEN && data.isAttackA)
                {
                    pos = VectorUtil.GetIntersection(pos1, pos2, pos3, pos4);
                    if (pos.z == 1)
                    {
                        cutIndex = (int)(data.count * Vector2.Distance(pos3, pos) / Vector2.Distance(pos3, pos4));
                        if (cutIndex <= data.nodeListA.Count)
                        {
                            retreat(data.indexA, data.indexB, cutIndex);
                        }
                    }
                    continue;
                }

                if (cellDataB.camp == Camp.GREEN && data.isAttackB)
                {
                    pos = VectorUtil.GetIntersection(pos1, pos2, pos3, pos4);
                    if (pos.z == 1)
                    {
                        cutIndex = (int)(data.count * Vector2.Distance(pos3, pos) / Vector2.Distance(pos3, pos4));
                        cutIndex = data.count - cutIndex - 1;
                        if (cutIndex <= data.nodeListB.Count)
                        {
                            retreat(data.indexB, data.indexA, cutIndex);
                        }
                    }
                    continue;
                }
            }
        }
    }

    public override void dispose()
    {
        CellWarManager.instance.clear();
        base.dispose();
    }

}
