using CLRSharp;
using Freamwork;

public class LevelDBModel : DBModel
{
    protected override void initDBModel(CLRSharp.ICLRType voCLRType = null, string sheet = "", bool order = false)
    {
        base.initDBModel(typeof(LevelDBVO) as ICLRType, "level", true);
    }

    public LevelDBVO getVOById(int id)
    {
        return getVoById(id) as LevelDBVO;
    }
}
