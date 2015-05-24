using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class PackBundlesMenu : ScriptableObject
{
    /// <summary>
    /// 打包的地址,必须是相对路径
    /// </summary>
    private const string bundlesDirePath = "./Resources/Bundles";

    /// <summary>
    /// DB(db.xml)的相对位置
    /// </summary>
    private const string dbPath = "./Resources/DB";

    [MenuItem("Game/Pack Bundles/Pack(Not Clear)", false, 2)]
    static void Pack()
    {
        PackBundles();
    }

    [MenuItem("Game/Pack Bundles/Pack After Clear", false, 2)]
    static void PackAfterClear()
    {
        if (Directory.Exists(bundlesDirePath))
        {
            Directory.Delete(bundlesDirePath, true);
        }
        PackBundles();
    }

    /// <summary>
    /// 打包资源
    /// </summary>
    static void PackBundles()
    {
        //确保目标文件夹存在
        AssetDatabase.Refresh();
        if (!Directory.Exists(bundlesDirePath))
        {
            Directory.CreateDirectory(bundlesDirePath);
        }

        //删除manifest
        string direName = getLastStr(bundlesDirePath);
        string manifestPath = bundlesDirePath + "/" + direName + ".manifest";
        if (File.Exists(manifestPath))
        {
            File.Delete(manifestPath);
        }

        //打包资源
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(bundlesDirePath, BuildAssetBundleOptions.None, BuildTarget.Android);
        string xmlStr = "<manifest>" + getXMLStr() + "</manifest>";
        File.WriteAllText(bundlesDirePath + "/manifest.xml", xmlStr);

        //单独打包manifest.xml和db.xml
        bool packDB = File.Exists(dbPath + "/db.xml");
        if (packDB)
        {
            File.Copy(dbPath + "/db.xml", "Assets/db.xml");
        }
        else
        {
            Debug.LogWarning ("未找到db.xml");
        }
        File.Copy(bundlesDirePath + "/manifest.xml", "Assets/manifest.xml");

        AssetDatabase.Refresh();
        List<AssetBundleBuild> buildMapList = new List<AssetBundleBuild>();
        AssetBundleBuild build = new AssetBundleBuild();
        build.assetBundleName = "manifest";
        build.assetNames = new string[] { "Assets/manifest.xml" };
        buildMapList.Add(build);
        if (packDB)
        {
            build = new AssetBundleBuild();
            build.assetBundleName = "db";
            build.assetNames = new string[] { "Assets/db.xml" };
            buildMapList.Add(build);
        }
        BuildPipeline.BuildAssetBundles(bundlesDirePath, buildMapList.ToArray(), BuildAssetBundleOptions.None, BuildTarget.Android);

        if (packDB)
        {
            File.Delete("Assets/db.xml");
        }
        File.Delete("Assets/manifest.xml");

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog("提示", "资源打包完毕", "确定");
    }

    /// <summary>
    /// 获取XML字符串
    /// </summary>
    /// <returns></returns>
    private static string getXMLStr()
    {
        string direName = getLastStr(bundlesDirePath);
        string manifestPath = bundlesDirePath + "/" + direName + ".manifest";
        if (!File.Exists(manifestPath))
        {
            return "";
        }

        Dictionary<string, ManifestVO> voDic = new Dictionary<string, ManifestVO>();
        StreamReader fs = new StreamReader(manifestPath);
        string line;

        //获取总信息
        while ((line = fs.ReadLine()) != null)
        {
            line = line.Replace("\n", "");
            int index = line.IndexOf("Name:");
            if (index >= 0)
            {
                ManifestVO vo = new ManifestVO();
                vo.name = line.Substring(index + 6);

                fs.ReadLine();

                while ((line = fs.ReadLine()) != null)
                {
                    index = line.IndexOf("Dependency_");
                    if (index < 0)
                    {
                        break;
                    }

                    line = line.Replace("\n", "");
                    index = line.IndexOf(":");
                    if (string.IsNullOrEmpty(vo.deps))
                    {
                        vo.deps = line.Substring(index + 2);
                    }
                    else
                    {
                        vo.deps = "," + line.Substring(index + 2);
                    }
                }
                voDic.Add(vo.name, vo);
            }
        }
        fs.Close();

        //读取CRC和assets信息
        foreach (string fileName in voDic.Keys)
        {
            ManifestVO vo = voDic[fileName];
            int status = 0;
            fs = new StreamReader(bundlesDirePath + "/" + fileName + ".manifest");

            while ((line = fs.ReadLine()) != null)
            {
                line = line.Replace("\n", "");
                if (status == 0)
                {
                    int index = line.IndexOf("CRC:");
                    if (index >= 0)
                    {
                        vo.crc = line.Substring(index + 5);
                        status++;
                    }
                }
                else if (status == 1)
                {
                    int index = line.IndexOf("Assets:");
                    if (index >= 0)
                    {
                        vo.assets = "";
                        while ((line = fs.ReadLine()) != null && line.IndexOf("- ") == 0)
                        {
                            if (!string.IsNullOrEmpty(vo.assets))
                            {
                                vo.assets += ",";
                            }
                            vo.assets += line.Substring(2);
                        }
                    }
                }
            }
            fs.Close();
        }

        string allStr = "";
        foreach (ManifestVO vo in voDic.Values)
        {
            allStr += "<" + vo.name
                + " crc=\"" + vo.crc + "\""
                + " deps=\"" + vo.deps + "\""
                + " assets=\"" + vo.assets + "\""
                + "/>";
        }
        return allStr;
    }

    /// <summary>
    /// 获取最后一个“/”后面的字符
    /// </summary>
    /// <param name="targetStr"></param>
    /// <returns></returns>
    private static string getLastStr(string targetStr)
    {
        string str = targetStr;
        int index = targetStr.LastIndexOf("/");
        if (index >= 0)
        {
            str = targetStr.Substring(index + 1);
        }
        return str;
    }
}

/// <summary>
/// 数据VO
/// </summary>
public class ManifestVO
{
    public string name;
    public string crc;
    public string deps;
    public string assets;
}