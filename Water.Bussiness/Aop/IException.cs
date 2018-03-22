using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Water.Bussiness.Aop
{
    /// <summary>
    /// 处理异常接口
    /// </summary>
    public interface IException
    {
        /// <summary>
        /// 处理异常
        /// </summary>
        /// <param name="ex">异常信息</param>
        /// <param name="requestMsg">方法请求信息</param>
        void OnException(IMessage requestMsg, Exception ex);
    }
}
