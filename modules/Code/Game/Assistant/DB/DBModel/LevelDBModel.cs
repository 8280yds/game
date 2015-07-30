using CLRSharp;
using Freamwork;
using System.Collections.Generic;

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

    public List<LevelDBVO> getAllData()
    {
        analysis();

        List<LevelDBVO> list = new List<LevelDBVO>();
        for (int i = 0; i < count; i++)
        {
            list.Add(getVOById(ids[i]));
        }
        return list;
    }
}
