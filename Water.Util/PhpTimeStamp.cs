using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Water.Util
{
    /// <summary>
    /// php时间戳扩展
    /// </summary>
    public static class PhpTimeStamp
    {
        static DateTime timeStamp = new DateTime(1970, 1, 1);

        /// <summary>
        /// 将php时间戳转换为datetime
        /// </summary>
        /// <param name="stamp">php时间戳</param>
        /// <returns></returns>
        public static DateTime PhpTimeStampToDateTime(this long stamp)
        {
            return timeStamp.AddSeconds(stamp).AddHours(8);
        }

        /// <summary>
        /// 获取php时间戳
        /// </summary>
        /// <returns></returns>

        public static long GetPhpTimeStamp(this DateTime now)
        {
            return (now.Ticks - timeStamp.Ticks) / 10000000;
        }
    }
}
