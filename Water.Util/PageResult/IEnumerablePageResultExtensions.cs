using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Linq.Dynamic;

namespace Water.Util
{
    /// <summary>
    /// IEnumerable分页扩展
    /// </summary>
    public static class IEnumerablePageResultExtensions
    {
        /// <summary>
        /// 从IEnumerable&#60;T&#62;创建一个分页数据
        /// </summary>
        /// <typeparam name="T">实体类型</typeparam>
        /// <param name="query">自身</param>
        /// <param name="pageInfo">分页信息</param>
        /// <returns>分页数据</returns>
        public static PageResult<T> ToPage<T>(this IQueryable<T> query, PageInfo pageInfo)
        {
            StringBuilder sortQueryBuilder = new StringBuilder();
            foreach (SortInfo sortInfo in pageInfo.SortInfos)
            {
                if (sortQueryBuilder.Length > 0)
                    sortQueryBuilder.AppendFormat(",");
                sortQueryBuilder.AppendFormat("{0} {1}", sortInfo.SortFild, sortInfo.SortType == ResultSortType.ASC ? "asc" : "desc");
            }
            query = query.OrderBy(sortQueryBuilder.ToString());
            pageInfo.TotalCount = query.Count();
            return new PageResult<T>()
            {
                Data = query.Skip(pageInfo.PageSize * (pageInfo.PageIndex - 1)).Take(pageInfo.PageSize).ToList(),
                PageInfo = new PageInfo()
                {
                    PageSize = pageInfo.PageSize,
                    SortInfos = pageInfo.SortInfos,
                    PageIndex = pageInfo.PageIndex,
                    TotalCount = pageInfo.TotalCount
                }
            };
        }
    }
}
