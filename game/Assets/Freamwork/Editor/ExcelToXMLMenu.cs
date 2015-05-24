using System.Diagnostics;
using UnityEditor;
using UnityEngine;

public class ExcelToXMLMenu : ScriptableObject
{
    private const string path = "./Resources/DB/ExcelToXML.py";

    [MenuItem("Game/Excel To XML", false, 1)]
    static void ExcelToXML()
    {
        Process proc = new Process();
        proc.StartInfo.FileName = relaToAbs(path);
        proc.Start();
    }

    /// <summary>
    /// 将相对路径转化成绝对路径
    /// </summary>
    /// <param name="relaPath">相对路径</param>
    /// <returns>绝对路径</returns>
    public static string relaToAbs(string relaPath)
    {
        relaPath = relaPath.Trim();
        string absPath = Application.dataPath;
        while (relaPath[0].ToString() == ".")
        {
            relaPath = relaPath.Substring(1);
            absPath = absPath.Substring(0, absPath.LastIndexOf("/"));
        }
        absPath += relaPath;
        return absPath;
    }
}