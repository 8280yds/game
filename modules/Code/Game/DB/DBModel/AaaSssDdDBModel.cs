using CLRSharp;
using Freamwork;
using System.Collections.Generic;

public class AaaSssDdDBModel : DBModel
{
    protected override void initDBModel(CLRSharp.ICLRType voCLRType = null, string sheet = "", bool order = false)
    {
        base.initDBModel(typeof(AaaSssDdDBVO) as ICLRType, "aaa_sss_dd");
    }

    public AaaSssDdDBVO getvoByAge(int age)
    {
        analysis();

        List<object> values = new List<object> (dataDic.Values);
        AaaSssDdDBVO vo;
        for (int i = 0, len = values.Count; i < len; i++ )
        {
            vo = values[i] as AaaSssDdDBVO;
            if (age == vo.age)
            {
                return vo;
            }
        }
        return null;
    }

}
