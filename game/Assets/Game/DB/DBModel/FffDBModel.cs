using Freamwork;
using System.Collections.Generic;

public class FffDBModel : DBModel<FffDBVO>
{
    protected override void initDBModel(string name = "", bool order = false)
    {
        base.initDBModel("fff", order);
    }
    
    /// <summary>
    /// 根据姓名获取VO列表
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public FffDBVO getVOByName(string name)
    {
        analysis();
        foreach(FffDBVO vo in dataDic.Values)
        {
            if (vo.name == name)
            {
                return vo;
            }
        }
        return null;
    }

    /// <summary>
    /// 根据性别获取VO列表
    /// </summary>
    /// <param name="sex"></param>
    /// <returns></returns>
    public List<FffDBVO> getVOBySex(string sex)
    {
        analysis();

        List<FffDBVO> list = new List<FffDBVO>();
        foreach (FffDBVO vo in dataDic.Values)
        {
            if (vo.sex == sex)
            {
                list.Add(vo);
            }
        }
        return list;
    }

}
