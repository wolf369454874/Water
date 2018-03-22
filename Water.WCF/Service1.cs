using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Linq;
using Water.Business;
using Water.Util;

namespace Water.WCF
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的类名“Service1”。
    public class Service1 : IService1
    {
        /// <summary>
        /// 异常锁
        /// </summary>
        object exceptionLock = new object();

        public string InvokeNomalMethod(string typeName, string fucName, string jsonParas)
        {
            try
            {
                if (!CheckAuthMethod(typeName, fucName))
                {
                    var jObject = string.IsNullOrEmpty(jsonParas) ? null : Newtonsoft.Json.Linq.JObject.Parse(jsonParas);
                    return Invoke(typeName, fucName, jObject);
                }
                return Newtonsoft.Json.JsonConvert.SerializeObject(new StatusResult() { Status = Status.Wrong, Message = "错误请求授权访问方法" });
            }
            catch (Exception ex)
            {
                string msg = "服务器异常";
                if (ex is Newtonsoft.Json.JsonReaderException)
                    msg = "输入信息错误";
                LogException(ex);
                return Newtonsoft.Json.JsonConvert.SerializeObject(new StatusResult() { Status = Status.Error, Message = msg });
            }
        }

        /// <summary>
        /// 解析加密认证字符串
        /// </summary>
        /// <param name="authCode">加密串</param>
        /// <param name="userID">用户编码</param>
        /// <returns></returns>
        bool DesAuthCode(string authCode, string userID)
        {
            try
            {
                string deStr = Water.Util.Encrypt.DecryptString(authCode, null);
                XDocument doc = XDocument.Parse(deStr);
                var userElement = doc.Descendants("User").First();
                return userElement.Descendants("ID").First().Value.Equals(userID);
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 执行业务方法
        /// </summary>
        /// <param name="typeName">业务类型</param>
        /// <param name="fucName">方法名称</param>
        /// <param name="JObject">参数</param>
        /// <returns></returns>
        string Invoke(string typeName, string fucName, Newtonsoft.Json.Linq.JObject jObject)
        {
            var type = Type.GetType(string.Format("Water.Bussiness.{0},Water.Bussiness", typeName));

            var fuc = type.GetMethod(fucName);
            object[] paras = null;
            var fucParas = fuc.GetParameters();
            if (fucParas != null && fucParas.Length > 0 && jObject != null)
            {
                paras = new object[fucParas.Length];
                for (int i = 0; i < fucParas.Length; i++)
                {
                    Newtonsoft.Json.Linq.JToken paraValue;
                    if (jObject.TryGetValue(fucParas[i].Name, out paraValue))
                        paras[i] = paraValue.ToObject(fucParas[i].ParameterType);
                }
            }
            string result = string.Empty;
            var instance = Activator.CreateInstance(type);
            if (fuc.ReturnType != typeof(void))
                result = Newtonsoft.Json.JsonConvert.SerializeObject(fuc.Invoke(instance, paras));
            else result = Newtonsoft.Json.JsonConvert.SerializeObject(new StatusResult() { Status = Status.Success });
            return result;
        }

        /// <summary>
        /// 授权方法验证
        /// </summary>
        /// <param name="typeName">方法类型</param>
        /// <param name="fucName">方法名</param>
        /// <returns></returns>
        bool CheckAuthMethod(string typeName, string fucName)
        {
            XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "AuthMethod.xml");
            var typeElement = doc.Element("AuthMethods").Elements().FirstOrDefault(e => e.Attribute("name").Value == typeName);
            if (typeElement != null)
            {
                var fucElement = typeElement.Elements().FirstOrDefault(e => e.Attribute("name").Value == fucName);
                return fucElement != null;
            }
            return false;
        }

        /// <summary>
        /// 记录日志
        /// </summary>
        /// <param name="ex"></param>
        void LogException(Exception ex)
        {
            lock (exceptionLock)
            {
                try
                {
                    var message = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ");
                    while (ex != null)
                    {
                        message += string.Format("Source:{0}\r\nMessage:{1}\r\nStackTrace:{2}\r\n", ex.Source, ex.Message, ex.StackTrace);
                        ex = ex.InnerException;
                    }
                    System.IO.File.AppendAllText(AppDomain.CurrentDomain.BaseDirectory + "ServiceException.txt", message);
                }
                catch { }
            }
        }
    }
}
