using Water.Bussiness.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Water.Bussiness
{
    /// <summary>
    /// 日志记录
    /// </summary>
    public class BusinessLog : IDisposable
    {
        /// <summary>
        /// 异常记录
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logAttr"></param>
        /// <param name="ex"></param>
        public void LogException(System.Runtime.Remoting.Messaging.IMessage msg, AopLogAttribute logAttr, Exception ex)
        {
            //TODO 异常记录
        }

        /// <summary>
        /// 业务日志记录
        /// </summary>
        /// <param name="msg"></param>
        /// <param name="logAttr"></param>
        public void LogBusiness(System.Runtime.Remoting.Messaging.IMessage msg, AopLogAttribute logAttr)
        {
            //TODO 业务日志记录
        }

        public void Dispose()
        {
            
        }
    }
}
