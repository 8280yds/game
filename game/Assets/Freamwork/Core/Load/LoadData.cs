using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// 加载中相关的触发方法
    /// </summary>
    public delegate void LoadFunctionDele(LoadData loadData);

    public class LoadData
    {
        /// <summary>
        /// 加载文件的名称(带路径带后缀)
        /// </summary>
        public string fullName;

        /// <summary>
        /// 加载物件的版本号
        /// </summary>
        public int version = 0;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float loadProgressNum = 0f;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float unZipProgressNum = 0f;

        /// <summary>
        /// 加载出来的assetBundle
        /// </summary>
        public AssetBundle assetBundle = null;

        /// <summary>
        /// 加载出来的Object[]
        /// </summary>
        public Object[] objects = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error = null;

        /// <summary>
        /// 创建一个的LoadData
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="version"></param>
        /// <param name="progress"></param>
        /// <param name="error"></param>
        /// <param name="assetBundle"></param>
        /// <returns></returns>
        public static LoadData getLoadData(string fullName, int version, float loadProgressNum = 0, string error = null,
            AssetBundle assetBundle = null, float unZipProgressNum = 0, Object[] objects = null)
        {
            LoadData data = new LoadData();
            data.fullName = fullName;
            data.version = version;
            data.loadProgressNum = loadProgressNum;
            data.error = error;
            data.assetBundle = assetBundle;
            data.unZipProgressNum = unZipProgressNum;
            data.objects = objects;
            return data;
        }
    }
}
