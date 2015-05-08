using UnityEngine;
using System.Collections;
using Freamwork.MVC;

public class StartModuleData
{

}

public class StartModuleCommand : Command
{

    public override void execute(object param)
    {
        StartModuleData data = param as StartModuleData;




    }


    
}
