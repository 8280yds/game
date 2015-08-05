using Freamwork;
using UnityEngine;

public class AI
{
    private ViewStatus viewStatus;
    private int index = 0;

    //等级期望列表
    private int[] levelNums = new int[] { -1000, 0, 50, 100 };

    public void start(ViewStatus viewStatus)
    {
        this.viewStatus = viewStatus;
        EnterFrame.instance.addEnterFrame(onEnterFrame);
    }

    private void onEnterFrame()
    {
        //细胞个数
        int cellCount = viewStatus.cellDataList.Count;
        //间隔时间
        int dele = viewStatus.vo.deleTime / cellCount;
        //轮回次数
        int count;
        //当前真实的索引
        int currentIndex;

        while (true)
        {
            count = index / cellCount;
            currentIndex = index % cellCount;

            if (dele * currentIndex > viewStatus.time - count * viewStatus.vo.deleTime)
            {
                break;
            }
            index++;

            CellData cellData = viewStatus.cellDataList[currentIndex];
            if (cellData.camp == Camp.GREEN)
            {
                break;
            }

            //援攻统计
            int[] helpAndAttackCounts = new int[cellCount];
            CellData cellData2;
            for (int i = 0; i < cellCount; i++)
            {
                helpAndAttackCounts[i] = 0;
                for (int j = 0; j < cellCount; j++)
                {
                    if(i==j)
                    {
                        continue;
                    }

                    cellData2 = viewStatus.cellDataList[j];
                    if(cellData2.tentacleList.Contains(i))
                    {
                        if (cellData2.camp == viewStatus.cellDataList[i].camp)
                        {
                            helpAndAttackCounts[i] = helpAndAttackCounts[i] + 1;
                        }else{
                            helpAndAttackCounts[i] = helpAndAttackCounts[i] - 1;
                        }
                    }
                }
            }

            //总期望值统计
            int[] sums = new int[cellCount];
            int maxIndex = 0;
            int minIndex = 0;

            for (int i = 0; i < cellCount; i++)
            {
                sums[i] = 0;
                if (i == currentIndex)
                {
                    continue;
                }
                cellData2 = viewStatus.cellDataList[i];
                
                //我方等级期望
                sums[i] = sums[i] + levelNums[cellData.vo.level - 1];
                //我方已伸触手期望
                sums[i] = sums[i] + 10 - 10 * cellData.tentacleList.Count;
                //对方攻我期望
                bool attacked = cellData2.tentacleList.Contains(currentIndex);
                if (cellData2.camp == cellData.camp)
                {
                    sums[i] = sums[i] + (attacked ? -30 : 0);
                }
                else
                {
                    sums[i] = sums[i] + (attacked ? 20 : 0);
                }
                //核差期望
                int d = (int)Vector2.Distance(cellData.position, cellData2.position);
                int t = (d - 2 * CellConstant.CELL_R) / CellConstant.NODE_D;
                sums[i] = sums[i] + cellData.hp - cellData2.hp - t;
                //距离期望
                sums[i] = sums[i] + t - 20;
                //我方被援攻期望
                sums[i] = sums[i] + 15 * helpAndAttackCounts[currentIndex];
                //敌方被援攻期望
                sums[i] = sums[i] - 10 * helpAndAttackCounts[i];

                bool contains = cellData.tentacleList.Contains(i);
                if(sums[i] > sums[maxIndex] && !contains)
                {
                    maxIndex = i;
                }
                else if (sums[i] < sums[minIndex] && contains)
                {
                    minIndex = i;
                }
            }

            //切断撤退
            if (sums[minIndex] < -15)
            {
                TentacleData tentacleData = viewStatus.getTentacleData(currentIndex, minIndex);
                int cutIndex;
                if (currentIndex < minIndex)
                {
                    cutIndex = tentacleData.nodeListA.Count;
                }
                else
                {
                    cutIndex = tentacleData.nodeListB.Count;
                }

                ActionData actionData = new ActionData();
                actionData.time = viewStatus.time;
                actionData.cellAIndex = (byte)currentIndex;
                actionData.cellBIndex = (byte)minIndex;
                actionData.type = 1;
                actionData.index = (byte)cutIndex;
                viewStatus.doAction(actionData);
            }

            //主动进攻
            if (sums[maxIndex] > 15)
            {
                ActionData actionData = new ActionData();
                actionData.time = viewStatus.time;
                actionData.cellAIndex = (byte)currentIndex;
                actionData.cellBIndex = (byte)maxIndex;
                actionData.type = 0;
                viewStatus.doAction(actionData);
            }
        }
    }

    public void stop()
    {
        EnterFrame.instance.removeEnterFrame(onEnterFrame);
    }

}