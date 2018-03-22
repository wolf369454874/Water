using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Water.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(Name = "MobileService", Namespace = "www.xdlgzs.cn")]
    public interface IService1
    {
        /// <summary>
        /// 访问普通方法
        /// </summary>
        /// <param name="typeName"></param>
        /// <param name="fucName"></param>
        /// <param name="jsonParas"></param>
        /// <returns></returns>
        [OperationContract]
        string InvokeNomalMethod(string typeName, string fucName, string jsonParas);

        // TODO: 在此添加您的服务操作
    }
}
