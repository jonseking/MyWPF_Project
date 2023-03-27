using Form.DataAccess;
using Form.Data;
using System;

namespace CourseManagement.Model.EntityModel
{
    [Table(TableName = "SYS_ROLE")]
    public class SYS_ROLE
    {
        [ColumnAttribute(PrimaryKey = true)]
        [ViewAttribute(ViewName = "主键ID")]
        public int? ROLEID { get; set; }

        [ViewAttribute(ViewName = "角色名称")]
        public String ROLENAME { get; set; }

        [ViewAttribute(ViewName = "职能描述")]
        public String REMARK { get; set; }

        [ViewAttribute(ViewName = "启用状态")]
        public int ISUSING { get; set; }

        [ViewAttribute(ViewName = "添加人")]
        public String CREATOR { get; set; }

        [ViewAttribute(ViewName = "添加时间")]
        public DateTime CREATETIME { get; set; }
    }
}
