using UnityEngine;
using System.Collections;

public class GoodsDBModel : DBModel<GoodsDBVO>
{
    public GoodsDBModel()
    {
        
    }

    protected override void initDBModel(string path = "", bool order = false)
    {
        base.initDBModel(Application.dataPath + "/Configure/DB/Editor/XML/goods.xml", true);
    }




}
