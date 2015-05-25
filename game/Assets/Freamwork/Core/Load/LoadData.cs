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
        /// 加载文件的名称
        /// </summary>
        public string fullName;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float loadProgressNum = 0f;

        /// <summary>
        /// 解压进度
        /// </summary>
        public float unZipProgressNum = 0f;

        /// <summary>
        /// 加载出来的assetBundle
        /// </summary>
        public AssetBundle assetBundle = null;

        /// <summary>
        /// 解压出来的Object[]
        /// </summary>
        public Object[] assets = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error = null;

        /// <summary>
        /// 创建一个的LoadData
        /// </summary>
        /// <param name="fullName"></param>
        /// <param name="loadProgressNum"></param>
        /// <param name="error"></param>
        /// <param name="assetBundle"></param>
        /// <param name="unZipProgressNum"></param>
        /// <param name="objects"></param>
        /// <returns></returns>
        public static LoadData getLoadData(string fullName, float loadProgressNum = 0, string error = null,
            AssetBundle assetBundle = null, float unZipProgressNum = 0, Object[] assets = null)
        {
            LoadData data = new LoadData();
            data.fullName = fullName;
            data.loadProgressNum = loadProgressNum;
            data.error = error;
            data.assetBundle = assetBundle;
            data.unZipProgressNum = unZipProgressNum;
            data.assets = assets;
            return data;
        }
    }
}
