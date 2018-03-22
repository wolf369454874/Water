using Water.Business.Aop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Water.Business
{
    /// <summary>
    /// 业务工厂1
    /// </summary>
    public class BusinessFactory : AopProxyBase
    {
        /// <summary>
        /// 创建一个业务工厂实例
        /// </summary>
        /// <param name="obj">业务实体</param>
        public BusinessFactory(MarshalByRefObject obj)
            : base(obj)
        {
        }

        #region AopProxyBase实现
        /// <summary>
        /// 方法执行后处理
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="respondMsg"></param>
        public override void Invoked(System.Runtime.Remoting.Messaging.IMessage requestMsg, System.Runtime.Remoting.Messaging.IMessage respondMsg)
        {

        }

        /// <summary>
        /// 方法执行前处理
        /// </summary>
        /// <param name="requestMsg"></param>
        public override void Invoking(System.Runtime.Remoting.Messaging.IMessage requestMsg)
        {
            var msg = requestMsg as System.Runtime.Remoting.Messaging.IMethodCallMessage;

            #region log
            AopLogAttribute logAttr = null;
            var attrs = msg.MethodBase.GetCustomAttributes(typeof(AopLogAttribute), false);
            if (attrs.Length > 0)
                logAttr = attrs[0] as AopLogAttribute;
            if (logAttr != null)
            {
                using (var business = new BusinessLog())
                {
                    business.LogBusiness(msg, logAttr);
                }
            }
            #endregion
        }

        /// <summary>
        /// 方法异常处理
        /// </summary>
        /// <param name="requestMsg"></param>
        /// <param name="ex"></param>
        public override void OnException(System.Runtime.Remoting.Messaging.IMessage requestMsg, Exception ex)
        {
            var msg = requestMsg as System.Runtime.Remoting.Messaging.IMethodCallMessage;

            #region 异常
            AopLogAttribute logAttr = null;
            var attrs = msg.MethodBase.GetCustomAttributes(typeof(AopLogAttribute), false);
            if (attrs.Length > 0)
                logAttr = attrs[0] as AopLogAttribute;
            using (var business = new BusinessLog())
            {
                business.LogException(msg, logAttr, ex);
            }
            #endregion
        }
        #endregion

        /// <summary>
        /// 获取业务代理
        /// </summary>
        /// <typeparam name="T">被代理业务对象类型</typeparam>
        /// <returns></returns>
        public static T WaterBusiness<T>()
            where T : MarshalByRefObject
        {
            var obj = Activator.CreateInstance<T>();
            BusinessFactory proxy = new BusinessFactory(obj);
            return proxy.GetTransparentProxy() as T;
        }
        /// <summary>
        /// 获取业务代理
        /// </summary>
        /// <param name="businessType">被代理业务对象类型</param>
        /// <returns></returns>
        public static object WaterBusiness(Type businessType)
        {
            var obj = Activator.CreateInstance(businessType) as MarshalByRefObject;
            BusinessFactory proxy = new BusinessFactory(obj);
            return proxy.GetTransparentProxy();
        }
    }
}
