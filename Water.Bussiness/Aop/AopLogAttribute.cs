using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Water.Business.Aop
{
    /// <summary>
    /// AOP日志记录特性
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class AopLogAttribute : Attribute
    {
        /// <summary>
        /// 日志描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 记录参数名称
        /// </summary>
        public string[] RecordParamNames { get; set; }
    }
}
