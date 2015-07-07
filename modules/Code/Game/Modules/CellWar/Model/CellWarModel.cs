using Freamwork;

public class CellWarModel : Model
{
    /// <summary>
    /// 最后一次操作后的状态
    /// </summary>
    private ViewStatus lastActionViewStatus;

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

        if (actionData != null && actionData.time >= lastActionViewStatus.time)
        {
            lastActionViewStatus.doNext(actionData.time);
            lastActionViewStatus.doAction(actionData);
            lastUpdateViewStatus = lastActionViewStatus.clone();
        }

        lastUpdateViewStatus.doNext(getCurrentTime());
        actionData = null;
        dispatch(CellWarUpdate.UPDATE_VIEW_STATUS);

        //??在此处可以贩毒案胜利
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
        lastActionViewStatus = lastUpdateViewStatus.clone();
    }

    /// <summary>
    /// 游戏结束
    /// </summary>
    public void gameEnd()
    {
        gameBegin = false;
        beginTime = 0;
        lastUpdateViewStatus = null;
        lastActionViewStatus = null;
        actionData = null;
    }

    public override void clearAll()
    {
        base.clearAll();
        gameEnd();
    }
}
