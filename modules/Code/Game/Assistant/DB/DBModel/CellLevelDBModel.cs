using CLRSharp;
using Freamwork;

public class CellLevelDBModel : DBModel
{
    protected override void initDBModel(CLRSharp.ICLRType voCLRType = null, string sheet = "", bool order = false)
    {
        base.initDBModel(typeof(CelllevelDBVO) as ICLRType, "cellLevel", true);
    }

    /// <summary>
    /// 根据hp获取vo
    /// </summary>
    /// <param name="hp"></param>
    /// <returns></returns>
    public CelllevelDBVO getvoByHp(int hp)
    {
        analysis();

        CelllevelDBVO vo;
        foreach (object obj in dataDic.Values)
        {
            vo = obj as CelllevelDBVO;
            if (hp >= vo.minHp && hp < vo.maxHp)
            {
                return vo;
            }
        }
        return null;
    }

    /// <summary>
    /// 细胞最大生命值
    /// </summary>
    public int maxHp
    {
        get
        {
            analysis();

            if (m_maxHp == 0)
            {
                CelllevelDBVO lastVO = dataDic[ids[ids.Length - 1]] as CelllevelDBVO;
                m_maxHp = lastVO.maxHp - 1;
            }
            return m_maxHp;
        }
    }
    private int m_maxHp;
}
