using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Water.Bussiness;
using Water.Util;

namespace TestForm
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = InvokeNomalMethod("UserBussiness", "GetUserList", null);
        }

        public string InvokeNomalMethod(string typeName, string fucName, string jsonParas)
        {
            try
            {
                    var jObject = string.IsNullOrEmpty(jsonParas) ? null : Newtonsoft.Json.Linq.JObject.Parse(jsonParas);
                    return Invoke(typeName, fucName, jObject);
            }
            catch (Exception ex)
            {
                string msg = "服务器异常";
                if (ex is Newtonsoft.Json.JsonReaderException)
                    msg = "输入信息错误";
                return Newtonsoft.Json.JsonConvert.SerializeObject(new Water.Util.StatusResult() { Status = Status.Error, Message = msg });
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
    }
}
