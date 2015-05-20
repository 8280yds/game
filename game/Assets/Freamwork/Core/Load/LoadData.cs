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
        /// 加载路径
        /// </summary>
        public string path = "/";

        /// <summary>
        /// 加载文件的名称(带后缀)
        /// </summary>
        public string fileName;

        /// <summary>
        /// 加载物件的版本号
        /// </summary>
        public int version = 0;

        /// <summary>
        /// 加载进度
        /// </summary>
        public float progress = 0f;

        /// <summary>
        /// 加载出来的数据
        /// </summary>
        public AssetBundle assetBundle = null;

        /// <summary>
        /// 错误信息
        /// </summary>
        public string error = null;

        /// <summary>
        /// 全名（路径+文件名）
        /// </summary>
        public string fullName
        {
            get
            {
                return path + fileName;
            }
        }
        
    }
}
