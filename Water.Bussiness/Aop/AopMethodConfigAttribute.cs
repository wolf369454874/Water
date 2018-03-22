using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Water.Bussiness.Aop
{
    /// <summary>
    /// 方法aop特性标签
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
    public class AopMethodConfigAttribute : Attribute
    {
        /// <summary>
        /// 创建一个方法aop特性
        /// </summary>
        public AopMethodConfigAttribute()
        {
            UseDefaultBeginAop = true;
            UseDefaultEndAop = true;
        }

        object[] exceptionDefaultValue = null;
        /// <summary>
        /// 发生异常后构造方法默认返回值的元数据（构造函数参数）
        /// </summary>
        public object[] ExceptionDefaultValue
        {
            get { return exceptionDefaultValue; }
            set
            {
                if (value != null)
                {
                    List<object> os = new List<object>();
                    foreach (object o in value)
                    {
                        if (o is string)
                        {
                            string msg = Water.Util.NoticeText.Instance[o.ToString()];
                            if (string.IsNullOrEmpty(msg))
                                os.Add(o);
                            else os.Add(msg);
                        }
                        else os.Add(o);
                    }
                    exceptionDefaultValue = os.ToArray();
                }
                exceptionDefaultValue = null;
            }
        }

        /// <summary>
        /// 是否在发生异常时返回默认值
        /// </summary>
        public bool ReturnExceptionDefaultValue { get; set; }

        /// <summary>
        /// 是否使用默认起始拦截
        /// </summary>
        public bool UseDefaultBeginAop { get; set; }

        /// <summary>
        /// 是否使用默认结束拦截
        /// </summary>
        public bool UseDefaultEndAop { get; set; }

        /// <summary>
        /// 方法执行前拦截提供程序
        /// </summary>
        public Type InvokingProvider { get; set; }

        /// <summary>
        /// 方法执行后拦截提供程序
        /// </summary>
        public Type InvokedProvider { get; set; }

        /// <summary>
        /// 方法执行异常处理提供程序
        /// </summary>
        public Type OnExceptionProvider { get; set; }
    }
}
