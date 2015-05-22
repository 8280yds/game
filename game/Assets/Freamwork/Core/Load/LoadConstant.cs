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
    /// 资源是否在加载中，
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
    /// 加载相关的常数类
    /// </summary>
    public static class LoadConstant
    {   
        ///// <summary>
        ///// 资源包后缀
        ///// </summary>
        //public const string BUNDLE_SUFFIX = ".assetbundle";

        /// <summary>
        /// 本地地址开头
        /// </summary>
        public const string LOCAL_TITLE = "file:///";

        /// <summary>
        /// cdn地址
        /// </summary>
        public const string CDN = "";

        /// <summary>
        /// 最大同时加载的数量
        /// </summary>
        public const int MAX_LOADERS = 3;

        /// <summary>
        /// 是否在加载的时候删除旧的文件
        /// </summary>
        public const bool DELETE_OLD_VERSION = true;
    }
}