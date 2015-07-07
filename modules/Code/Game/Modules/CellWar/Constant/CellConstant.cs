using UnityEngine;

public enum Camp
{
    NONE,
    GREEN,
    RED,
    BLUE,
    MAGENTA,
    CYAN,
}

public enum CutStatus
{
    NONE,           //无动作
    CUT_A,          //A
    CUT_B,          //B
}

public enum TenStatus
{
    NONE,           //无动作
    MOVE_RETREAT_A, //A撤退
    MOVE_RETREAT_B, //B撤退
    MOVE_ATTACK_A,  //A行进
    MOVE_ATTACK_B,  //B行进
    MOVE_PK_A,      //A传输
    MOVE_PK_B,      //B传输
    MOVE_AB_A,      //整体右移
    MOVE_AB_B,      //整体左移
}

public class CellConstant
{
    /// <summary>
    /// 阵营颜色数组
    /// </summary>
    public static readonly Color[] CAMP_COLOR_ARR = new Color[] { 
        Color.white, Color.green, Color.red, Color.blue, Color.magenta, Color.cyan };

    /// <summary>
    /// 细胞半径
    /// </summary>
    public const int CELL_R = 20;

    /// <summary>
    /// 触手单元间距
    /// </summary>
    public const int NODE_D = 8;

    /// <summary>
    /// 进攻时移动一格需要的时间ms
    /// </summary>
    public const int MOVE_ATTACK = 60;

    /// <summary>
    /// 撤退时移动一格需要的时间ms
    /// </summary>
    public const int MOVE_RETREAT = 20;

    /// <summary>
    /// 比拼时移动一格需要的时间ms
    /// </summary>
    public const int MOVE_PK = 20;

    /// <summary>
    /// 占领细胞后核内DNA数量
    /// </summary>
    public const int INIT_HP = 10;
}
