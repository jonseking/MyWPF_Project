using Form.DataAccess;
using Form.Data;

namespace CourseManagement.Model.EntityModel
{
    [Table(TableName = "SYS_AUTH")]
    public class SYS_AUTH
    {
        [ColumnAttribute(PrimaryKey = true)]
        [ViewAttribute(ViewName = "主键ID")]
        public int AUTHID { get; set; }

        [ViewAttribute(ViewName = "权限名称")]
        public string AUTHNAME { get; set; }

        [ViewAttribute(ViewName = "权限排序")]
        public int AUTHINDEX { get; set; }

        [ViewAttribute(ViewName = "父级权限")]
        public int PARENTID { get; set; }

        [ViewAttribute(ViewName = "启用状态")]
        public int ISUSING { get; set; }
    }
}
