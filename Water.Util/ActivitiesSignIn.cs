using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Water.Util
{
    /// <summary>
    /// 活动签到
    /// </summary>
    public class ActivitiesSignIn
    {
        /// <summary>
        /// 签到说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime BeginDate { get; set; }

        /// <summary>
        /// 结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 赠送觅宝
        /// </summary>
        public int MeBao { get; set; }
    }


    public struct SignTag
    {
        public int TagVal;
        public List<string> SignNote;
    }
}
