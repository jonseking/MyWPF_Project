using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.DataAccess
{
    /// <summary>
    /// 表别名属性
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
    public sealed class TableAttribute : System.Attribute
    {
        /// <summary>
        /// 表别名
        /// </summary>
        public string TableName { get; set; }
    }

    /// <summary>
    /// 列属性
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class ColumnAttribute : System.Attribute
    {
        /// <summary>
        /// 是否标示
        /// </summary>
        public bool Identity { get; set; }
        /// <summary>
        /// 是否主键
        /// </summary>
        public bool PrimaryKey { get; set; }
        /// <summary>
        /// 列别名
        /// </summary>
        public string ColumnName { get; set; }
        /// <summary>
        /// Oracle序列名称
        /// </summary>
        public string SeqenceName { get; set; }
    }

    /************查询特性****************/
    /// <summary>
    /// 查询列属性
    /// </summary>
    [AttributeUsage(System.AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public class SerachColAttribute : System.Attribute
    {
        public SerachColAttribute()
        {
            OrdinalIgnoreCase = false;
        }

        /// <summary>
        /// 查询条件 = like < ....等
        /// </summary>
        public OperaSerach OpSerach { get; set; }
        /// <summary>
        /// 查询条件真实列名
        /// </summary>
        public string RealColumnName { get; set; }
        /// <summary>
        /// 不进行查询的值
        /// </summary>
        public string DoesValues { get; set; }
        /// <summary>
        /// 查询忽略大小写
        /// </summary>
        public bool OrdinalIgnoreCase { get; set; }
    }

    /// <summary>
    /// 查询条件枚举
    /// </summary>
    public enum OperaSerach
    {
        等于,
        不等于,
        like,
        lefLike,
        rightLike,
        小于,
        小于等于,
        大于,
        大于等于,
        包含,
        不包含,
        产品名组合包含,
        企业组合包含
    }
}
