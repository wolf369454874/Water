using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Water.Util
{
    /// <summary>
    /// 操作消息
    /// </summary>
    [Serializable]
    [System.Runtime.Serialization.DataContract]
    public class StatusResult
    {
        /// <summary>
        /// 操作执行状态
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public Status Status { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public string Message { get; set; }

        /// <summary>
        /// 自定义数据
        /// </summary>
        [System.Runtime.Serialization.DataMember]
        public object UserData { get; set; }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StatusResult()
        {
            Status = Status.Success;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        public StatusResult(Status status, string message)
        {
            Status = status;
            Message = message;
        }
    }

    /// <summary>
    /// 执行状态枚举
    /// </summary>
    [System.Runtime.Serialization.DataContract]
    public enum Status
    {
        /// <summary>
        /// 成功
        /// </summary>
        [System.Runtime.Serialization.EnumMember]
        Success = 0,

        /// <summary>
        /// 错误
        /// </summary>
        [System.Runtime.Serialization.EnumMember]
        Error = 1,

        /// <summary>
        /// 警告
        /// </summary>
        [System.Runtime.Serialization.EnumMember]
        Wrong = 2
    }
}
