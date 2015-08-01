using Freamwork;
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

    /// <summary>
    /// moduless.dll的相对位置
    /// </summary>
    private const string dllPath = "../modules/bin/Debug";

    /// <summary>
    /// cdn的绝对位置
    /// </summary>
    private const string cdnPath = "C:/inetpub/wwwroot/Bundles";

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

    //[MenuItem("Game/Pack Bundles/Copy TO CDN", false, 2)]
    static void CopyToCDN()
    {
        if (!Directory.Exists(cdnPath))
        {
            EditorUtility.DisplayDialog("出错啦", "CDN目录未找到！", "确定");
            return;
        }
        DirectoryInfo folder = new DirectoryInfo(bundlesDirePath);
        foreach (FileInfo file in folder.GetFiles("*.assets"))
        {
            File.Copy(file.FullName, cdnPath + "/" + file.Name, true);
        }
    }

    /// <summary>
    /// 打包资源
    /// </summary>
    static void PackBundles()
    {
        //确保目标文件夹存在
        AssetDatabase.Refresh();
        if (!Directory.Exists(bundlesDirePath + "/.Temporary"))
        {
            Directory.CreateDirectory(bundlesDirePath + "/.Temporary");
        }

        //确保db.xml存在
        if (!File.Exists(dbPath + "/db.xml"))
        {
            Directory.Delete(bundlesDirePath + "/.Temporary", true);
            EditorUtility.DisplayDialog("出错啦", "db.xml未找到！", "确定");
            return;
        }
        File.Copy(dbPath + "/db.xml", "Assets/db.xml", true);

        //确保modules.dll存在
        if (!File.Exists(dllPath + "/modules.dll"))
        {
            Directory.Delete(bundlesDirePath + "/.Temporary", true);
            EditorUtility.DisplayDialog("出错啦", "modules.dll未找到！", "确定");
            return;
        }
        File.Copy(dllPath + "/modules.dll", "Assets/modules.bytes", true);

        //确保modules.pdb存在
        if (!File.Exists(dllPath + "/modules.pdb"))
        {
            Directory.Delete(bundlesDirePath + "/.Temporary", true);
            EditorUtility.DisplayDialog("出错啦", "modules.pdb未找到！", "确定");
            return;
        }
        File.Copy(dllPath + "/modules.pdb", "Assets/Code/Freamwork/Editor/Resources/modules.pdb.bytes", true);

        //单独打包db.xml和modules.dll
        AssetDatabase.Refresh();
        AssetBundleBuild[] buildMap = new AssetBundleBuild[2];
        buildMap[0].assetBundleName = "db.assets";
        buildMap[0].assetNames = new string[] { "Assets/db.xml" };
        buildMap[1].assetBundleName = "modules.assets";
        buildMap[1].assetNames = new string[] { "Assets/modules.bytes" };
        BuildPipeline.BuildAssetBundles(bundlesDirePath + "/.Temporary", buildMap, 
            BuildAssetBundleOptions.None, BuildTarget.Android);
        File.Copy(bundlesDirePath + "/.Temporary/db.assets", bundlesDirePath + "/db.assets", true);
        File.Copy(bundlesDirePath + "/.Temporary/db.assets.manifest", bundlesDirePath + "/db.assets.manifest", true);
        File.Copy(bundlesDirePath + "/.Temporary/modules.assets", bundlesDirePath + "/modules.assets", true);
        File.Copy(bundlesDirePath + "/.Temporary/modules.assets.manifest", bundlesDirePath + "/modules.assets.manifest", true);
        File.Delete("Assets/db.xml");
        File.Delete("Assets/modules.bytes");

        //打包资源
        AssetDatabase.Refresh();
        BuildPipeline.BuildAssetBundles(bundlesDirePath, BuildAssetBundleOptions.None, BuildTarget.Android);

        //写入manifest.xml
        string xmlStr = "<manifest>" + getXMLStr() + "</manifest>";
        File.WriteAllText(bundlesDirePath + "/manifest.xml", xmlStr);

        //单独打包manifest.xml
        File.Copy(bundlesDirePath + "/manifest.xml", "Assets/manifest.xml", true);
        AssetDatabase.Refresh();
        buildMap = new AssetBundleBuild[1];
        buildMap[0].assetBundleName = "manifest.assets";
        buildMap[0].assetNames = new string[] { "Assets/manifest.xml" };
        BuildPipeline.BuildAssetBundles(bundlesDirePath + "/.Temporary", buildMap, 
            BuildAssetBundleOptions.None, BuildTarget.Android);
        File.Copy(bundlesDirePath + "/.Temporary/manifest.assets", bundlesDirePath + "/manifest.assets", true);
        File.Copy(bundlesDirePath + "/.Temporary/manifest.assets.manifest", bundlesDirePath + "/manifest.assets.manifest", true);
        File.Delete("Assets/manifest.xml");

        //打包完成
        Directory.Delete(bundlesDirePath + "/.Temporary", true);
        AssetDatabase.Refresh();
        CopyToCDN();    //拷贝到CDN目录
        EditorUtility.DisplayDialog("提示", "资源打包完毕", "确定");
    }

    /// <summary>
    /// 获取XML字符串
    /// </summary>
    /// <returns></returns>
    private static string getXMLStr()
    {
        string direName = StringUtil.getLastStr(bundlesDirePath);
        string manifestPath = bundlesDirePath + "/" + direName + ".manifest";

        Dictionary<string, ManifestVO> voDic = new Dictionary<string, ManifestVO>();
        //手动添加db
        ManifestVO manifestVO = new ManifestVO();
        manifestVO.name = "db.assets";
        voDic.Add(manifestVO.name, manifestVO);
        //手动添加modules
        manifestVO = new ManifestVO();
        manifestVO.name = "modules.assets";
        voDic.Add(manifestVO.name, manifestVO);

        string line;
        StreamReader fs;
        if (File.Exists(manifestPath))
        {
            fs = new StreamReader(manifestPath);
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
        }

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
}