using Form.DataAccess;
using Form.Data;

namespace CourseManagement.Model.EntityModel
{
    [Table (TableName= "SYS_MENU")]
    public class SYS_MENU
    {
        [ColumnAttribute(PrimaryKey = true)]
        [ViewAttribute(ViewName = "主键ID")]
        public int MID { get; set; }

        [ViewAttribute(ViewName = "菜单名称")]
        public string MENUNAME { get; set; }

        [ViewAttribute(ViewName = "菜单路径")]
        public string MENUVIEW { get; set; }

        [ViewAttribute(ViewName = "菜单图标")]
        public string MENUICON { get; set; }
        [ViewAttribute(ViewName = "菜单顺序")]
        public int MENUINDEX { get; set; }

        [ViewAttribute(ViewName = "父级菜单")]
        public int PARENTID { get; set; }

        [ViewAttribute(ViewName = "权限ID")]
        public string AUTHID { get; set; }
        [ViewAttribute(ViewName = "启用状态")]
        public int ISUSING { get; set; }
    }
}
