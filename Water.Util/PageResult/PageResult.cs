using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Water.Util
{
    /// <summary>
    /// 分页结果集
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageResult<T>
    {
        /// <summary>
        /// 分页信息
        /// </summary>
        public PageInfo PageInfo { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public IList<T> Data { get; set; }
    }

    /// <summary>
    /// 分页信息
    /// </summary>
    public class PageInfo
    {
        private int pageIndex = 1;
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex
        {
            get { return pageIndex; }
            set { pageIndex = value; }
        }

        private int pageSize = 10;
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get { return pageSize; } set { pageSize = value; } }

        /// <summary>
        /// 总页数
        /// </summary>
        public int PagesCount
        {
            get
            {
                return TotalCount / PageSize + (TotalCount % PageSize == 0 ? 0 : 1);
            }
        }

        /// <summary>
        /// 总数
        /// </summary>
        public int TotalCount { get; set; }

        private List<SortInfo> sortInfos = new List<SortInfo>() { new SortInfo { SortFild = "Id", SortType = ResultSortType.ASC } };
        /// <summary>
        /// 排序信息
        /// </summary>
        public List<SortInfo> SortInfos { get { return sortInfos; } set { sortInfos = value; } }
    }

    /// <summary>
    /// 排序信息
    /// </summary>
    public class SortInfo
    {
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortFild { get; set; }

        /// <summary>
        /// 排序方式
        /// </summary>
        public ResultSortType SortType { get; set; }
    }

    /// <summary>
    /// 排序方式
    /// </summary>
    public enum ResultSortType
    {
        /// <summary>
        /// 升序
        /// </summary>
        ASC = 0,

        /// <summary>
        /// 降序
        /// </summary>
        DESC = 1
    }
}
