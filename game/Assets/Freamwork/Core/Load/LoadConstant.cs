namespace Freamwork
{
    /// <summary>
    /// 加载优先级，可以根据需要添加或删减，数字越小越优先加载，
    /// 请按照默认数值不要手动赋值，否则可能导致出错
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
    /// 加载相关的常数类
    /// </summary>
    public static class LoadConstant
    {   
        /// <summary>
        /// 资源包后缀
        /// </summary>
        public const string BUNDLE_SUFFIX = ".assetbundle";
        /// <summary>
        /// 本地地址开头
        /// </summary>
        public const string LOCAL_TITLE = "file:///";
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