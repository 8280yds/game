using System;

namespace Freamwork
{
    public class TimeUtil
    {
        /// <summary>  
        /// 获取当前时间戳(秒)
        /// </summary>
        /// <returns></returns>  
        public static int getTimeStamp()
        {
            TimeSpan ts = DateTime.Now - new DateTime(1970, 1, 1);
            return Convert.ToInt32(ts.TotalSeconds);
        }

        /// <summary>
        /// 获取当前时间戳
        /// </summary>  
        /// <param name="bflag">为真时获取10位时间戳,为假时获取13位时间戳.</param>  
        /// <returns></returns>  
        public static long getTimeStamp(bool bflag)
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1);
            if (bflag)
                return Convert.ToInt64(ts.TotalSeconds);
            else
                return Convert.ToInt64(ts.TotalMilliseconds);
        }  

        /// <summary>
        /// 时间戳转为DateTime
        /// </summary>
        /// <param name="timeStamp">时间戳（秒）</param>
        /// <returns></returns>
        public static DateTime StampToDateTime(int timeStamp)
        {
            DateTime baseTime = new DateTime(1970, 1, 1);
            TimeSpan toNow = new TimeSpan((long)timeStamp * 10000000);
            return baseTime.Add(toNow);
        }

        /// <summary>
        /// DateTime转为时间戳
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static int DateTimeToStamp(DateTime time)
        {
            DateTime baseTime = new DateTime(1970, 1, 1);
            return (int)(time - baseTime).TotalSeconds;
        }
    }
}