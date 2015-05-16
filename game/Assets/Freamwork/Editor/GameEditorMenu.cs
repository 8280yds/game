using System.Data;
using System.Data.OleDb;
using System.IO;
using UnityEditor;
using UnityEngine;
using System.Xml;
using System.Data.Odbc;

public class GameEditorMenu : ScriptableObject
{
    /// <summary>
    /// 源Excel文件存放地址
    /// </summary>
    private const string EXCEL_PATH = @"C:\Users\Administrator\Desktop\新建文件夹";

    /// <summary>
    /// 生成的VO存放地址
    /// </summary>
    private const string VO_PATH = "";

    /// <summary>
    /// 目标文件存放地址
    /// </summary>
    private const string TARGET_PATH = "";


    [MenuItem("Game/生成配置文件", false, 1)]
    static void DoPackageConfig()
    {
        packageConfig();
    }

    private static void packageConfig()
    {
        DirectoryInfo dir = new DirectoryInfo(EXCEL_PATH);
        FileInfo[] fileInfos = dir.GetFiles("*.xlsx");

        for (int i = 0, len = fileInfos.Length; i < len; i++)
        {
            //getExcelDataToTable(fileInfos[i]);

        }
    }








}