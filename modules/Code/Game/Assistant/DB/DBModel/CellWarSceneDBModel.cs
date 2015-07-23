using CLRSharp;
using Freamwork;

public class CellWarSceneDBModel : DBModel
{
    protected override void initDBModel(CLRSharp.ICLRType voCLRType = null, string sheet = "", bool order = false)
    {
        base.initDBModel(typeof(CellWarSceneDBVO) as ICLRType, "cell_war_scene");
    }

    public CellWarSceneDBVO getVOById(int id)
    {
        return getVoById(id) as CellWarSceneDBVO;
    }
}
