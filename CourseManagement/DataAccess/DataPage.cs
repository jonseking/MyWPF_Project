using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.DataAccess
{
    /// <summary>
    /// 分页查询结果实体
    /// </summary>
    public abstract class DataPage
    {
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage { get; set; }
        /// <summary>
        /// 下一页页码
        /// </summary>
        public int? NextPage { get; set; }
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPages { get; set; }
        /// <summary>
        /// 总条目数
        /// </summary>
        public int TotalItems { get; set; }
        /// <summary>
        /// 每页条目数
        /// </summary>
        public int PerPageItems { get; set; }
    }

    /// <summary>
    /// 查询结果为<c>List</c>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ListPage<T> : DataPage
    {
        /// <summary>
        /// 查询结果
        /// </summary>
        public List<T> Items { get; set; }
    }

    /// <summary>
    /// 查询结果为<c>DataTable</c>
    /// </summary>
    public class TablePage : DataPage
    {
        /// <summary>
        /// 查询结果
        /// </summary>
        public DataTable Items { get; set; }
    }
}
