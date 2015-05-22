using UnityEditor;
using UnityEngine;
using System.Diagnostics;

public class GameEditorMenu : ScriptableObject
{
    [MenuItem("Game/ExcelToXML", false, 1)]
    static void DoPackageConfig()
    {
        Process proc = new Process();
        proc.StartInfo.FileName = Application.dataPath + "/Game/DB/Editor/ExcelToXML.py";
        proc.Start();
    }


}