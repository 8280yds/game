﻿using CLRSharp;
using Freamwork;
using System.Collections.Generic;
using UnityEngine;

public class ViewStatus
{
    /// <summary>
    /// 场景VO
    /// </summary>
    public CellWarSceneDBVO vo;

    /// <summary>
    /// 状态所处的时间点
    /// </summary>
    public int time;

    /// <summary>
    /// 细胞信息列表
    /// </summary>
    public List<CellData> cellDataList = new List<CellData>();

    /// <summary>
    /// 触手信息列表
    /// </summary>
    public Dictionary<string, TentacleData> tentacleDataDic = new Dictionary<string,TentacleData>();

    /// <summary>
    /// 有效操作次数
    /// </summary>
    public int actionCount
    {
        get;
        private set;
    }

    public ViewStatus()
    {

    }

    public ViewStatus(CellWarSceneDBVO vo, int time = 0)
    {
        this.vo = vo;
        this.time = time;

        char[] ch = new char[] { ',' };
        string[] campArr = vo.cellCamp.Split(ch);
        string[] hpArr = vo.cellHP.Split(ch);
        string[] xArr = vo.cellX.Split(ch);
        string[] yArr = vo.cellY.Split(ch);

        CellData cellData;
        for (int i = 0, len = campArr.Length; i < len; i++)
        {
            cellData = new CellData();
            cellData.index = i;
            cellData.addTime = time;
            cellData.outTime = time;
            cellData.hp = int.Parse(hpArr[i]);
            cellData.camp = (Camp)int.Parse(campArr[i]);
            cellData.position = new Vector2(int.Parse(xArr[i]), int.Parse(yArr[i]));
            cellDataList.Add(cellData);
        }
    }

    public ViewStatus clone()
    {
        ViewStatus data = new ViewStatus();
        data.vo = vo;
        data.time = time;
        data.actionCount = actionCount;
        for (int i = 0, len = cellDataList.Count; i < len; i++ )
        {
            data.cellDataList.Add(cellDataList[i].clone());
        }
        foreach (string key in tentacleDataDic.Keys)
        {
            data.tentacleDataDic.Add(key, tentacleDataDic[key].clone());
        }
        return data;
    }

    /// <summary>
    /// 创建TentacleData
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    /// <returns></returns>
    public TentacleData getTentacleData(int indexA, int indexB)
    {
        if (indexA > indexB)
        {
            int t = indexA;
            indexA = indexB;
            indexB = t;
        }
        string key = indexA + ":" + indexB;

        if (tentacleDataDic.ContainsKey(key))
        {
            return tentacleDataDic[key];
        }
        else
        {
            TentacleData tentacleData = new TentacleData(this);
            tentacleData.indexA = indexA;
            tentacleData.indexB = indexB;
            tentacleData.count = getTentacleNodeNum(indexA, indexB);
            tentacleDataDic.Add(key, tentacleData);
            return tentacleData;
        }
    }

    /// <summary>
    /// 是否存在触手信息
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    /// <returns></returns>
    public bool hasTentacleData(int indexA, int indexB)
    {
        string key = indexA < indexB ? indexA + ":" + indexB : indexB + ":" + indexA;
        return tentacleDataDic.ContainsKey(key);
    }

    /// <summary>
    /// 计算细胞间触手单元的数量
    /// </summary>
    /// <param name="indexA"></param>
    /// <param name="indexB"></param>
    /// <returns></returns>
    private int getTentacleNodeNum(int indexA, int indexB)
    {
        Vector2 sour = cellDataList[indexA].position;
        Vector2 dest = cellDataList[indexB].position;

        float d = Vector2.Distance(sour, dest);
        float sourX = sour.x - CellConstant.CELL_R * (sour.x - dest.x) / d;
        float sourY = sour.y - CellConstant.CELL_R * (sour.y - dest.y) / d;
        float destX = dest.x + CellConstant.CELL_R * (sour.x - dest.x) / d;
        float destY = dest.y + CellConstant.CELL_R * (sour.y - dest.y) / d;

        d = Vector2.Distance(new Vector2(sourX, sourY), new Vector2(destX, destY));
        return (int)(d / CellConstant.NODE_D);
    }

    /// <summary>
    /// 在当前状态进行操作
    /// </summary>
    /// <param name="actionData">操作数据</param>
    public void doAction(ActionData actionData)
    {
        TentacleData tentacleData = getTentacleData(actionData.cellAIndex, actionData.cellBIndex);
        if (actionData.type == 0)   //连接
        {
            if (!tentacleData.isAttackA && actionData.cellAIndex < actionData.cellBIndex && 
                tentacleData.cutStatus != CutStatus.CUT_B)
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                tentacleData.timeA = actionData.time;  //设定时间
                tentacleData.isAttackA = true;

                if (cellDataList[tentacleData.indexA].camp == cellDataList[tentacleData.indexB].camp)
                {
                    tentacleData.isAttackB = false;
                }
            }
            else if (!tentacleData.isAttackB && actionData.cellAIndex > actionData.cellBIndex && 
                tentacleData.cutStatus != CutStatus.CUT_A)
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                tentacleData.timeB = actionData.time;  //设定时间
                tentacleData.isAttackB = true;

                if (cellDataList[tentacleData.indexA].camp == cellDataList[tentacleData.indexB].camp)
                {
                    tentacleData.isAttackA = false;
                }
            }
            return;
        }

        if (actionData.type == 1) //斩断
        {
            if (tentacleData.cutStatus != CutStatus.NONE ||
                (!tentacleData.isAttackA && !tentacleData.isAttackB))   //已在斩断状态或者双方都未进攻
            {
                return;
            }

            if(tentacleData.isAttackA && tentacleData.isAttackB)        //双方都在进攻
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                if(actionData.cellAIndex < actionData.cellBIndex)
                {
                    tentacleData.isAttackA = false;
                }
                else
                {
                    tentacleData.isAttackB = false;
                }
                return;
            }

            if(actionData.cellAIndex < actionData.cellBIndex && tentacleData.isAttackA &&
                tentacleData.nodeListA.Count < tentacleData.count)    //A单方进攻，触手尚未触及
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                tentacleData.isAttackA = false;
                return;
            }

            if(actionData.cellAIndex > actionData.cellBIndex && tentacleData.isAttackB &&
                tentacleData.nodeListB.Count < tentacleData.count)    //B单方进攻，触手尚未触及
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                tentacleData.isAttackB = false;
                return;
            }

            if (actionData.cellAIndex < actionData.cellBIndex && tentacleData.isAttackA &&
                tentacleData.nodeListA.Count == tentacleData.count &&
                tentacleData.count > actionData.index)              //A单方输出时切断
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                int len = tentacleData.count - actionData.index;
                if (len <= 0)
                {
                    tentacleData.isAttackA = false;
                    return;
                }
                tentacleData.nodeListB = tentacleData.nodeListA.GetRange(actionData.index, len);
                tentacleData.nodeListB.Reverse();
                tentacleData.nodeListA.RemoveRange(actionData.index, len);
                tentacleData.cutStatus = CutStatus.CUT_A;
                tentacleData.isAttackA = false;
                tentacleData.timeB = actionData.time;
                return;
            }

            if (actionData.cellAIndex > actionData.cellBIndex && tentacleData.isAttackB &&
                tentacleData.nodeListB.Count == tentacleData.count &&
                tentacleData.count > actionData.index)              //B单方输出时切断
            {
                if (cellDataList[actionData.cellAIndex].camp == Camp.GREEN)
                {
                    actionCount++;
                }
                int len = tentacleData.count - actionData.index;
                if (len <= 0)
                {
                    tentacleData.isAttackB = false;
                    return;
                }
                tentacleData.nodeListA = tentacleData.nodeListB.GetRange(actionData.index, len);
                tentacleData.nodeListA.Reverse();
                tentacleData.nodeListB.RemoveRange(actionData.index, len);
                tentacleData.cutStatus = CutStatus.CUT_B;
                tentacleData.isAttackB = false;
                tentacleData.timeA = actionData.time;
                return;
            }
        }
    }

    /// <summary>
    /// 执行动作直至当前时间
    /// </summary>
    /// <param name="currentTime"></param>
    public void doNext(int currentTime)
    {
        int cellTime;
        int tenTime;
        int minTime;

        while (true)
        {
            cellTime = getCellNextTime();
            tenTime = getTentacleNextTime();
            minTime = Mathf.Min(cellTime, tenTime);

            if (minTime > currentTime)
            {
                break;
            }

            if (cellTime == minTime)
            {
                switch (outCellStatus)
                {
                    case CellStatus.ADD_HP:  //细胞DNA增加
                        outCellData.addTime = minTime;
                        outCellData.hp++;
                        break;
                    case CellStatus.OUT_HP:  //细胞DNA输出
                        outCellData.outTime = minTime;
                        outHp(outCellData);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                switch (outTenStatus)
                {
                    case TenStatus.MOVE_RETREAT_A: //A撤退
                        retreatA(outTentacleData);
                        outTentacleData.timeA = minTime;
                        break;
                    case TenStatus.MOVE_RETREAT_B: //B撤退
                        retreatB(outTentacleData);
                        outTentacleData.timeB = minTime;
                        break;
                    case TenStatus.MOVE_ATTACK_A:  //A行进
                        if (!attackA(outTentacleData))
                        {
                            continue;
                        }
                        outTentacleData.timeA = minTime;
                        break;
                    case TenStatus.MOVE_ATTACK_B:  //B行进
                        if (!attackB(outTentacleData))
                        {
                            continue;
                        }
                        outTentacleData.timeB = minTime;
                        break;
                    case TenStatus.MOVE_PK_A:      //A传输
                        pkA(outTentacleData);
                        outTentacleData.timeA = minTime;
                        break;
                    case TenStatus.MOVE_PK_B:      //B传输
                        pkB(outTentacleData);
                        outTentacleData.timeB = minTime;
                        break;
                    case TenStatus.MOVE_AB_A:      //整体右移
                        if (!attackA(outTentacleData))
                        {
                            continue;
                        }
                        retreatB(outTentacleData);
                        outTentacleData.timeA = minTime;
                        break;
                    case TenStatus.MOVE_AB_B:      //整体左移
                        if (!attackB(outTentacleData))
                        {
                            continue;
                        }
                        retreatA(outTentacleData);
                        outTentacleData.timeB = minTime;
                        break;
                    default:
                        break;
                }
            }
            time = minTime;
        }
    }

    /// <summary>
    /// 开始输出
    /// </summary>
    /// <param name="data"></param>
    private void outHp(CellData data)
    {
        TentacleData tentacleData;
        string key;
        foreach(int index in data.tentacleList)
        {
            if (data.index < index)
            {
                key = data.index + ":" + index;
                tentacleData = tentacleDataDic[key];
                if (tentacleData.nodeListA.Count > 0 && !tentacleData.nodeListA[0])
                {
                    tentacleData.nodeListA[0] = true;
                    data.hp--;
                }
            }
            else
            {
                key = index + ":" + data.index;
                tentacleData = tentacleDataDic[key];
                if (tentacleData.nodeListB.Count > 0 && !tentacleData.nodeListB[0])
                {
                    tentacleData.nodeListB[0] = true;
                    data.hp--;
                }
            }
        }
    }

    /// <summary>
    /// A后撤一步
    /// </summary>
    /// <param name="data"></param>
    private void retreatA(TentacleData data)
    {
        CellData cellA = cellDataList[data.indexA];
        CellData cellB = cellDataList[data.indexB];

        int n = 1;
        if (cellA.camp != cellB.camp && data.cutStatus == CutStatus.CUT_B)
        {
            n = -1;
        }
        if (data.nodeListA[0])
        {
            n *= 2;
        }
        data.nodeListA.RemoveAt(0);
        hpChange(cellA, n, cellB.camp);

        if (data.cutStatus == CutStatus.CUT_B && data.nodeListA.Count == 0)
        {
            data.cutStatus = CutStatus.NONE;
        }
    }

    /// <summary>
    /// B后撤一步
    /// </summary>
    /// <param name="data"></param>
    private void retreatB(TentacleData data)
    {
        CellData cellA = cellDataList[data.indexA];
        CellData cellB = cellDataList[data.indexB];

        int n = 1;
        if (cellA.camp != cellB.camp && data.cutStatus == CutStatus.CUT_A)
        {
            n = -1;
        }
        if (data.nodeListB[0])
        {
            n *= 2;
        }
        data.nodeListB.RemoveAt(0);
        hpChange(cellB, n, cellA.camp);

        if (data.cutStatus == CutStatus.CUT_A && data.nodeListB.Count == 0)
        {
            data.cutStatus = CutStatus.NONE;
        }
    }

    /// <summary>
    /// A行进一步
    /// </summary>
    /// <param name="data"></param>
    private bool attackA(TentacleData data)
    {
        if (cellDataList[data.indexA].hp > 0)
        {
            data.nodeListA.Insert(0, false);
            cellDataList[data.indexA].hp--;
            return true;
        }
        else
        {
            data.isAttackA = false;
            return false;
        }
    }

    /// <summary>
    /// B行进一步
    /// </summary>
    /// <param name="data"></param>
    private bool attackB(TentacleData data)
    {
        if (cellDataList[data.indexB].hp > 0)
        {
            data.nodeListB.Insert(0, false);
            cellDataList[data.indexB].hp--;
            return true;
        }
        else
        {
            data.isAttackB = false;
            return false;
        }
    }

    /// <summary>
    /// A传输一步
    /// </summary>
    private void pkA(TentacleData data)
    {
        int len = data.nodeListA.Count;
        if (data.nodeListA[len - 1])
        {
            CellData cellA = cellDataList[data.indexA];
            CellData cellB = cellDataList[data.indexB];
            int n = 1;
            if (cellA.camp != cellB.camp)
            {
                n = -1;
            }
            hpChange(cellB, n, cellA.camp);
        }
        data.nodeListA.RemoveAt(len - 1);
        data.nodeListA.Insert(0, false);
    }

    /// <summary>
    /// B传输一步
    /// </summary>
    private void pkB(TentacleData data)
    {
        int len = data.nodeListB.Count;
        if (data.nodeListB[len - 1])
        {
            CellData cellA = cellDataList[data.indexA];
            CellData cellB = cellDataList[data.indexB];
            int n = 1;
            if (cellA.camp != cellB.camp)
            {
                n = -1;
            }
            hpChange(cellA, n, cellB.camp);
        }
        data.nodeListB.RemoveAt(len - 1);
        data.nodeListB.Insert(0, false);
    }

    /// <summary>
    /// 细胞hp变化
    /// </summary>
    /// <param name="data">细胞</param>
    /// <param name="num">hp的变化量</param>
    /// <param name="camp">如果hp小于零，将转换的阵营</param>
    private void hpChange(CellData data, int num, Camp camp)
    {
        if (data.hp + num > 0)
        {
            data.hp += num;
        }
        else
        {
            data.hp = CellConstant.INIT_HP;
            data.camp = camp;
            retreatAllTentacles(data.index);
        }
    }

    /// <summary>
    /// 收回单个细胞的全部触手
    /// </summary>
    /// <param name="cellIndex">细胞索引</param>
    private void retreatAllTentacles(int cellIndex)
    {
        foreach (TentacleData data in tentacleDataDic.Values)
        {
            if (data.indexA == cellIndex)
            {
                data.isAttackA = false;
            }
            else if (data.indexB == cellIndex)
            {
                data.isAttackB = false;
            }
        }
    }

    /// <summary>
    /// 获取细胞下回动作
    /// </summary>
    /// <returns></returns>
    private int getCellNextTime()
    {
        int nextTime = int.MaxValue;
        outCellData = null;
        outCellStatus = CellStatus.NONE;
        int tentacleCount;
        int _time;

        foreach (CellData data in cellDataList)
        {
            _time = data.addTime + data.vo.addTime;
            if (nextTime > _time)
            {
                outCellStatus = CellStatus.ADD_HP;  //自然增长
                outCellData = data;
                nextTime = _time;
            }

            tentacleCount = data.tentacleList.Count;
            _time = (int)(data.outTime + data.vo.attackTime * tentacleCount * Mathf.Pow(CellConstant.ATTACK_COEFF, tentacleCount - 1));
            if (nextTime > _time && data.hp > tentacleCount && tentacleCount > 0)
            {
                outCellStatus = CellStatus.OUT_HP;  //往外输出
                outCellData = data;
                nextTime = _time;
            }
        }
        return nextTime;
    }
    private CellData outCellData;
    private CellStatus outCellStatus;

    /// <summary>
    /// 获取触手下回动作
    /// </summary>
    /// <returns></returns>
    private int getTentacleNextTime()
    {
        int nextTime = int.MaxValue;
        outTentacleData = null;
        outTenStatus = TenStatus.NONE;
        int countA;
        int countB;

        foreach (TentacleData data in tentacleDataDic.Values)
        {
            countA = data.nodeListA.Count;
            countB = data.nodeListB.Count;

            if (!data.isAttackA)    //A未进攻
            {
                if (countA > 0 && nextTime > data.timeA + CellConstant.MOVE_RETREAT)
                {
                    outTenStatus = TenStatus.MOVE_RETREAT_A;  //在撤退或切断
                    outTentacleData = data;
                    nextTime = data.timeA + CellConstant.MOVE_RETREAT;
                }
            }
            else                   //A在进攻
            {
                if (countA == data.count &&
                    nextTime > data.timeA + CellConstant.MOVE_PK)
                {
                    outTenStatus = TenStatus.MOVE_PK_A;   //单方输出
                    outTentacleData = data;
                    nextTime = data.timeA + CellConstant.MOVE_PK;
                }
                else if (countA == Mathf.Floor(data.count / 2) && data.isAttackB &&
                    countA + countB == data.count &&
                    nextTime > data.timeA + CellConstant.MOVE_PK)
                {
                    outTenStatus = TenStatus.MOVE_PK_A;   //比拼消耗
                    outTentacleData = data;
                    nextTime = data.timeA + CellConstant.MOVE_PK;
                }
                else if (countA + countB != data.count &&
                    nextTime > data.timeA + CellConstant.MOVE_ATTACK)
                {
                    outTenStatus = TenStatus.MOVE_ATTACK_A;   //A右移
                    outTentacleData = data;
                    nextTime = data.timeA + CellConstant.MOVE_ATTACK;
                }
                else if (countA + countB == data.count &&
                    countA < Mathf.Floor(data.count / 2) &&
                    nextTime > data.timeA + CellConstant.MOVE_ATTACK)
                {
                    outTenStatus = TenStatus.MOVE_AB_A;   //整体右移
                    outTentacleData = data;
                    nextTime = data.timeA + CellConstant.MOVE_ATTACK;
                }
            }

            if (!data.isAttackB)    //B未进攻
            {
                if (countB > 0 && nextTime > data.timeB + CellConstant.MOVE_RETREAT)
                {
                    outTenStatus = TenStatus.MOVE_RETREAT_B;  //在撤退或切断
                    outTentacleData = data;
                    nextTime = data.timeB + CellConstant.MOVE_RETREAT;
                }
            }
            else                   //B在进攻
            {
                if (countB == data.count &&
                    nextTime > data.timeB + CellConstant.MOVE_PK)
                {
                    outTenStatus = TenStatus.MOVE_PK_B;   //单方输出
                    outTentacleData = data;
                    nextTime = data.timeB + CellConstant.MOVE_PK;
                }
                else if (countB == Mathf.Ceil(data.count / 2) && data.isAttackA &&
                    countA + countB == data.count &&
                    nextTime > data.timeB + CellConstant.MOVE_PK)
                {
                    outTenStatus = TenStatus.MOVE_PK_B;   //比拼消耗
                    outTentacleData = data;
                    nextTime = data.timeB + CellConstant.MOVE_PK;
                }
                else if (countA + countB != data.count &&
                    nextTime > data.timeB + CellConstant.MOVE_ATTACK)
                {
                    outTenStatus = TenStatus.MOVE_ATTACK_B;   //B左移
                    outTentacleData = data;
                    nextTime = data.timeB + CellConstant.MOVE_ATTACK;
                }
                else if (countA + countB == data.count &&
                    countB < Mathf.Ceil(data.count / 2) &&
                    nextTime > data.timeB + CellConstant.MOVE_ATTACK)
                {
                    outTenStatus = TenStatus.MOVE_AB_B;   //整体左移
                    outTentacleData = data;
                    nextTime = data.timeB + CellConstant.MOVE_ATTACK;
                }
            }
        }
        return nextTime;
    }

    //临时数据，解决out不能用的问题
    private TentacleData outTentacleData;
    private TenStatus outTenStatus;
}

public class CellData
{
    private CellLevelDBModel model
    {
        get
        {
            if (m_model == null)
            {
                m_model = MVCCharge.instance.getInstance(typeof(CellLevelDBModel) as ICLRType) as CellLevelDBModel;
            }
            return m_model;
        }
    }
    private CellLevelDBModel m_model;

    /// <summary>
    /// 拥有的触手key
    /// </summary>
    public List<int> tentacleList = new List<int>();

    /// <summary>
    /// 细胞的索引
    /// </summary>
    public int index;

    /// <summary>
    /// 上次DNA自然增加的时间点
    /// </summary>
    public int addTime;

    /// <summary>
    /// 上次DNA输出的时间点
    /// </summary>
    public int outTime;

    /// <summary>
    /// 细胞DNA的数量
    /// </summary>
    public int hp
    {
        get
        {
            return m_hp;
        }
        set
        {
            m_hp = Mathf.Min(Mathf.Max(0, value), model.maxHp);
            
            if (vo == null || m_hp < vo.minHp || m_hp >= vo.maxHp)
            {
                vo = model.getvoByHp(m_hp);
            }
        }
    }
    private int m_hp;

    /// <summary>
    /// 等级数据vo
    /// </summary>
    public CelllevelDBVO vo
    {
        get;
        private set;
    }

    /// <summary>
    /// 细胞的阵营
    /// </summary>
    public Camp camp;

    /// <summary>
    /// 细胞的坐标
    /// </summary>
    public Vector2 position;

    public CellData clone()
    {
        CellData data = new CellData();
        data.tentacleList = new List<int>(tentacleList);
        data.index = index;
        data.addTime = addTime;
        data.outTime = outTime;
        data.hp = hp;
        data.camp = camp;
        data.position = position;
        return data;
    }
}

public class TentacleData
{
    /// <summary>
    /// 触手单元总数
    /// </summary>
    public int count;

    /// <summary>
    /// 细胞A索引
    /// </summary>
    public int indexA;

    /// <summary>
    /// 细胞B索引
    /// </summary>
    public int indexB;

    /// <summary>
    /// 切断模式
    /// </summary>
    public CutStatus cutStatus = CutStatus.NONE;

    /// <summary>
    /// 细胞A是否处于进攻状态
    /// </summary>
    public bool isAttackA
    {
        get
        {
            return m_isAttackA;
        }
        set
        {
            if (m_isAttackA != value)
            {
                m_isAttackA = value;
                List<int> tentacleList = viewStatus.cellDataList[indexA].tentacleList;
                if (m_isAttackA)
                {
                    if (!tentacleList.Contains(indexB))
                    {
                        tentacleList.Add(indexB);
                    }
                }
                else
                {
                    tentacleList.Remove(indexB);
                }
            }
        }
    }
    private bool m_isAttackA;

    /// <summary>
    /// 细胞B是否处于进攻状态
    /// </summary>
    public bool isAttackB
    {
        get
        {
            return m_isAttackB;
        }
        set
        {
            if (m_isAttackB != value)
            {
                m_isAttackB = value;
                List<int> tentacleList = viewStatus.cellDataList[indexB].tentacleList;
                if (m_isAttackB)
                {
                    if (!tentacleList.Contains(indexA))
                    {
                        tentacleList.Add(indexA);
                    }
                }
                else
                {
                    tentacleList.Remove(indexA);
                }
            }
        }
    }
    private bool m_isAttackB;

    /// <summary>
    /// 细胞A节点列表，true代表DNA传输点
    /// </summary>
    public List<bool> nodeListA = new List<bool>();

    /// <summary>
    /// 细胞B节点列表，true代表DNA传输点
    /// </summary>
    public List<bool> nodeListB = new List<bool>();

    /// <summary>
    /// 上次触手A发生变化的时间
    /// </summary>
    public int timeA;

    /// <summary>
    /// 上次触手B发生变化的时间
    /// </summary>
    public int timeB;

    private ViewStatus viewStatus;

    public TentacleData(ViewStatus viewStatus)
    {
        this.viewStatus = viewStatus;
    }

    public TentacleData clone()
    {
        TentacleData data = new TentacleData(viewStatus);
        data.count = count;
        data.indexA = indexA;
        data.indexB = indexB;
        data.cutStatus = cutStatus;
        data.isAttackA = isAttackA;
        data.isAttackB = isAttackB;
        data.nodeListA = new List<bool>(nodeListA);
        data.nodeListB = new List<bool>(nodeListB);
        data.timeA = timeA;
        data.timeB = timeB;
        return data;
    }
}
