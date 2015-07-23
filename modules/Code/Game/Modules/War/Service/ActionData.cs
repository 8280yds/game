public class ActionData
{
    /// <summary>
    /// 时间
    /// </summary>
    public int time;

    /// <summary>
    /// 细胞索引
    /// </summary>
    public byte cellAIndex;

    /// <summary>
    /// 细胞索引
    /// </summary>
    public byte cellBIndex;

    /// <summary>
    /// 0:连接, 1:切断
    /// </summary>
    public byte type;

    /// <summary>
    /// 切断的位置
    /// </summary>
    public byte index;
}
