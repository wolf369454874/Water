using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Proxies;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Remoting.Services;
using System.Runtime.Remoting.Activation;

namespace Water.Bussiness.Aop
{
    /// <summary>
    /// AopProxyBase 
    /// 所有自定义AOP代理类都从此类派生，覆写IAopOperator接口，实现具体的前/后处理 。
    /// </summary>
    public abstract class AopProxyBase : RealProxy, IAopOperator
    {
        /// <summary>
        /// 默认透明代理
        /// </summary>
        private readonly MarshalByRefObject target;
        /// <summary>
        /// 方法单独aop实现
        /// </summary>
        IAopOperator methodOperatorProvider;
        /// <summary>
        /// 根据类型获取aop实现接口
        /// </summary>
        /// <param name="providerType">实现接口类型</param>
        /// <returns></returns>
        IAopOperator GetOperatorProvider(Type providerType)
        {
            if (providerType == null || providerType.GetInterface(typeof(IAopOperator).FullName) == null)
                return null;
            if (methodOperatorProvider == null || providerType != methodOperatorProvider.GetType())
            {
                if (providerType == this.target.GetType())
                    methodOperatorProvider = this.target as IAopOperator;
                else methodOperatorProvider = Activator.CreateInstance(providerType) as IAopOperator;
            }
            return methodOperatorProvider;
        }

        /// <summary>
        /// 创建Aop代理基类
        /// </summary>
        /// <param name="obj">被代理对象</param>
        public AopProxyBase(MarshalByRefObject obj)
            : base(obj.GetType())
        {
            this.target = obj;
        }

        /// <summary>
        /// 代理调用方法
        /// </summary>
        /// <param name="msg">方法信息</param>
        /// <returns></returns>
        public override IMessage Invoke(IMessage msg)
        {
            IMethodCallMessage call = (IMethodCallMessage)msg;

            #region 检索AOP特性标记
            AopMethodConfigAttribute methodAopAttr = null;
            var attrs = call.MethodBase.GetCustomAttributes(typeof(AopMethodConfigAttribute), false);
            if (attrs.Length > 0)
                methodAopAttr = attrs[0] as AopMethodConfigAttribute;
            #endregion

            #region 方法执行前拦截
            if (methodAopAttr != null)
            {
                var provider = GetOperatorProvider(methodAopAttr.InvokingProvider);
                if (provider != null)
                    provider.Invoking(msg);
                if (methodAopAttr.UseDefaultBeginAop)
                    this.Invoking(msg);
            }
            #endregion

            #region 执行方法
            IMethodReturnMessage returnMsg = null;
            //如果触发的是构造函数，此时target的构建还未开始
            IConstructionCallMessage ctor = call as IConstructionCallMessage;
            if (ctor != null)
            {
                //获取最底层的默认真实代理
                RealProxy default_proxy = RemotingServices.GetRealProxy(this.target);
                default_proxy.InitializeServerObject(ctor);
                MarshalByRefObject tp = (MarshalByRefObject)this.GetTransparentProxy(); //自定义的透明代理 this
                return EnterpriseServicesHelper.CreateConstructionReturnMessage(ctor, tp);
            }
            else returnMsg = RemotingServices.ExecuteMessage(this.target, call); //将消息转化为堆栈，并执行目标方法，方法完成后，再将堆栈转化为消息
            #endregion

            #region 异常处理
            if (returnMsg.Exception != null)
            {
                IAopOperator provider = null;
                if (methodAopAttr != null)
                    provider = GetOperatorProvider(methodAopAttr.OnExceptionProvider);
                if (provider != null)
                    provider.OnException(msg, returnMsg.Exception);
                else if ((target as IException) != null)
                    ((IException)target).OnException(msg, returnMsg.Exception);
                else this.OnException(msg, returnMsg.Exception);
                object value = null;
                var methodInfo = (System.Reflection.MethodInfo)returnMsg.MethodBase;
                if (methodInfo.ReturnType != typeof(void) && methodAopAttr != null && methodAopAttr.ReturnExceptionDefaultValue)
                    value = Activator.CreateInstance(methodInfo.ReturnType, methodAopAttr.ExceptionDefaultValue);
                returnMsg = new ReturnMessage(value, returnMsg.OutArgs, returnMsg.OutArgCount, returnMsg.LogicalCallContext, call);
            }
            #endregion

            #region 方法执行后拦截
            else if (methodAopAttr != null)
            {
                var provider = GetOperatorProvider(methodAopAttr.InvokedProvider);
                if (provider != null)
                    provider.Invoked(msg, returnMsg);
                if (methodAopAttr.UseDefaultEndAop)
                    this.Invoked(msg, returnMsg);
            }
            #endregion

            return returnMsg;
        }

        /// <summary>
        /// 公共方法执行前拦截
        /// </summary>
        /// <param name="requestMsg">方法信息</param>
        public abstract void Invoking(IMessage requestMsg);

        /// <summary>
        /// 公共方法执行后拦截
        /// </summary>
        /// <param name="requestMsg">请求方法信息</param>
        /// <param name="respondMsg">执行结果信息</param>
        public abstract void Invoked(IMessage requestMsg, IMessage respondMsg);

        /// <summary>
        /// 公共异常处理
        /// </summary>
        /// <param name="requestMsg">方法请求信息</param>
        /// <param name="ex">异常信息</param>
        public abstract void OnException(IMessage requestMsg, Exception ex);
    }
}
