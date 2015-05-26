using UnityEngine;
using System.Collections;
using Freamwork;

public class StartModuleData
{

}

public class StartModuleCommand<T> : Command where T : PanelView
{

    public override void execute<TParam>(TParam param)
    {
//        StartModuleData data = param as StartModuleData;




    }


    
}
