using Freamwork;
using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class ExcelToXMLMenu : ScriptableObject
{
    private const string path = "/Code/Freamwork/Editor/ExcelToXML.py";

    [MenuItem("Game/Excel To XML", false, 1)]
    static void ExcelToXML()
    {
        Process proc = new Process();
        proc.StartInfo.FileName = StringUtil.relaToAbs(path);
        proc.Start();
    }
}