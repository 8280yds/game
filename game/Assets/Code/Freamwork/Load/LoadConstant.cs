using UnityEngine;

namespace Freamwork
{
    /// <summary>
    /// <para>加载优先级，可以根据需要添加或删减，数字越小越优先加载</para>
    /// <para>【注意】请按照默认数值不要手动赋值，否则可能导致出错</para>
    /// </summary>
    public enum LoadPriority
    {
        zero,
        one,
        two,
        three,
        four,
    }

    /// <summary>
    /// 资源的加载状态，
    /// <para>none：缓存中没有其资源包，且未准备加载</para>
    /// <para>loaded：已经加载过，且在缓存有其资源包</para>
    /// <para>loading：正在加载过程中</para>
    /// <para>wait：正在排队等待加载</para>
    /// </summary>
    public enum LoadStatus
    {
        none,
        loaded,
        loading,
        wait,
    }

    /// <summary>
    /// 加载的类型
    /// <para>web：从网络加载</para>
    /// <para>local：从本地加载，本地未找到将报错</para>
    /// <para>localOrWeb：从本地加载,本地未找到则从网络加载</para>
    /// </summary>
    public enum LoadType
    {
        web,
        local,
        localOrWeb,
    }

    /// <summary>
    /// 加载相关的常数类
    /// </summary>
    public static class LoadConstant
    {
        /// <summary>
        /// 本地地址开头
        /// </summary>
        public const string LOCAL_TITLE = "file:///";

        /// <summary>
        /// cdn地址
        /// </summary>
        public const string CDN = "http://192.168.1.105/Bundles/";

        /// <summary>
        /// 本地加载文件存储地址
        /// </summary>
        public static string localFilesPath
        {
            get
            {
                return Application.persistentDataPath;
            }
        }

        /// <summary>
        /// 最大同时加载的数量
        /// </summary>
        public const int MAX_LOADERS = 3;

        /// <summary>
        /// 是否在加载的时候删除旧的文件
        /// </summary>
        public const bool DELETE_OLD_VERSION = true;

        /// <summary>
        /// 本地加载文件不存在时的报错信息
        /// </summary>
        public const string LOCAL_LOAD_ERROR = "本地不存在该文件，无法加载";

        /// <summary>
        /// 在加载过程中被终止的报错信息
        /// </summary>
        public const string LOADING_STOP_ERROR = "在加载过程中被终止";

        /// <summary>
        /// 加载信息文件的名称
        /// </summary>
        public const string MANIFEST_FILE = "manifest.assets";

        /// <summary>
        /// db数据文件的名称
        /// </summary>
        public const string DB_FILE = "db.assets";
    }
}