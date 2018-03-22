using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting.Messaging;

namespace Water.Business.Aop
{
    /// <summary>
    /// aop操作方法
    /// </summary>
    public interface IAopOperator : IException
    {
        /// <summary>
        /// 方法调用前拦截
        /// </summary>
        /// <param name="requestMsg">请求消息</param>
        void Invoking(IMessage requestMsg);

        /// <summary>
        /// 方法调用后拦截
        /// </summary>
        /// <param name="requestMsg">请求消息</param>
        /// <param name="responceMsg">完成消息</param>
        void Invoked(IMessage requestMsg, IMessage responceMsg);
    }
}
