using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Water.Util
{
    /// <summary>
    /// 提示文本访问
    /// </summary>
    public class NoticeText
    {
        /// <summary>
        /// 提示文本缓存集合
        /// </summary>
        Dictionary<string, string> texts = new Dictionary<string, string>();
        static NoticeText _instance = null;
        /// <summary>
        /// 提示文本访问实例
        /// </summary>
        public static NoticeText Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new NoticeText();
                return _instance;
            }
        }

        /// <summary>
        /// 提示文本访问实例构造函数
        /// </summary>
        public NoticeText()
        {
            RefreshText();
        }

        /// <summary>
        /// 刷新文本
        /// </summary>
        public void RefreshText()
        {
            texts.Clear();
            try
            {
                XDocument doc = XDocument.Load(AppDomain.CurrentDomain.BaseDirectory + "Resource\\" + System.Configuration.ConfigurationManager.AppSettings["NoticeTextFileName"]);
                doc.Descendants("Text").ToList().ForEach(e => texts.Add(e.Attribute("code").Value, e.Value));
            }
            catch { }
        }

        /// <summary>
        /// 根据提示文本编码获取文本
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns></returns>
        public string this[string code]
        {
            get
            {
                if (texts.ContainsKey(code))
                    return texts[code];
                return string.Empty;
            }
        }
    }
}
