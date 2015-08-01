using CLRSharp;
using Freamwork;

public class CellWarModel : Model
{
    /// <summary>
    /// 最后一次操作后的状态
    /// </summary>
    //private ViewStatus lastActionViewStatus;

    /// <summary>
    /// 最后一次更新后的状态
    /// </summary>
    public ViewStatus lastUpdateViewStatus
    {
        get;
        private set;
    }

    /// <summary>
    /// 游戏开始时间戳
    /// </summary>
    private long beginTime;

    /// <summary>
    /// 游戏是否已开始
    /// </summary>
    public bool gameBegin
    {
        get;
        private set;
    }

    /// <summary>
    /// 最近一次操作
    /// </summary>
    public ActionData actionData;

    /// <summary>
    /// 获取当前状态
    /// </summary>
    public void getCurrentViewStatus()
    {
        if(!gameBegin)
        {
            return;
        }

        //if (actionData != null && actionData.time >= lastActionViewStatus.time)
        //{
        //    lastActionViewStatus.doNext(actionData.time);
        //    lastActionViewStatus.doAction(actionData);
        //    lastUpdateViewStatus = lastActionViewStatus.clone();
        //}

        if (actionData != null)
        {
            lastUpdateViewStatus.doNext(actionData.time);
            lastUpdateViewStatus.doAction(actionData);
            actionData = null;
        }
        lastUpdateViewStatus.doNext(getCurrentTime());
        dispatch(CellWarUpdate.UPDATE_VIEW_STATUS);

        //在此处判断胜利========================
        int count = 0;
        foreach (CellData data in lastUpdateViewStatus.cellDataList)
        {
            if (data.camp == Camp.GREEN)
            {
                count++;
            }
        }

        int starNum = 0;
        int time = lastUpdateViewStatus.time / 1000;

        if(count == 0)  //失败
        {
            starNum = 0;
        }
        else if (count == lastUpdateViewStatus.cellDataList.Count)  //胜利
        {
            string[] starTimes = lastUpdateViewStatus.vo.star.Split(new char[] { ',' });
            if (time < int.Parse(starTimes[0]))
            {
                starNum = 3;
            }
            else if (time < int.Parse(starTimes[1]))
            {
                starNum = 2;
            }
            else
            {
                starNum = 1;
            }
        }
        else
        {
            return;
        }

        LevelModel levelModel = mvcCharge.getInstance(typeof(LevelModel) as ICLRType) as LevelModel;
        levelModel.setLevelStar(levelModel.enemyLevel, levelModel.starIndex, starNum);

        WarResultModel model = mvcCharge.getInstance(typeof(WarResultModel) as ICLRType) as WarResultModel;
        model.starNum = starNum;
        model.time = time;
        model.actionCount = lastUpdateViewStatus.actionCount;
        dispatch(CellWarUpdate.GAME_OVER);
    }

    /// <summary>
    /// 获取相对于游戏开始时间的时间戳
    /// </summary>
    /// <returns></returns>
    public int getCurrentTime()
    {
        return (int)(TimeUtil.getTimeStamp(false) - beginTime);
    }

    /// <summary>
    /// 初始化场景(游戏开始)
    /// </summary>
    /// <param name="sceneDBVO">场景VO</param>
    public void initScene(CellWarSceneDBVO sceneDBVO)
    {
        actionData = null;
        gameBegin = true;

        beginTime = TimeUtil.getTimeStamp(false);
        lastUpdateViewStatus = new ViewStatus(sceneDBVO);
        //lastActionViewStatus = new ViewStatus(sceneDBVO);
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void gameEnd()
    {
        gameBegin = false;
        beginTime = 0;
        lastUpdateViewStatus = null;
        //lastActionViewStatus = null;
        actionData = null;
    }

    public override void clearAll()
    {
        base.clearAll();
        gameEnd();
    }
}
