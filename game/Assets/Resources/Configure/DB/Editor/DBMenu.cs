using UnityEngine;
using UnityEditor;
using System.IO;

public class DBMenu : ScriptableObject
{
    /// <summary>
    /// 源Excel文件存放地址
    /// </summary>
    private const string EXCEL_PATH = "";

    /// <summary>
    /// 生成的VO存放地址
    /// </summary>
    private const string VO_PATH = "";

    /// <summary>
    /// 目标文件存放地址
    /// </summary>
    private const string TARGET_PATH = "";


    [MenuItem("Game/生成配置文件")]
    static void PackagData()
    {
        packagData();
    }


    static void packagData()
    {
        //string[] dirs = Directory.GetFiles(@"d:\\MyDocuments\\MyPictures", "*.xlsx");
        //for (int i = 0, len = dirs.Length; i < len; i++)
        //{
            

        //}


    }



}