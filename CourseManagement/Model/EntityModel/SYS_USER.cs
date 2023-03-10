

using PORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CourseManagement.Model.EntityModel
{
    public class SYS_USER
    {
        [ColumnAttribute(PrimaryKey = true)]
        [ViewAttribute(ViewName = "主键GUID")]
        public string USERID { get; set; }

        [ViewAttribute(ViewName = "账号")]
        public string USERNAME { get; set; }
        [ViewAttribute(ViewName = "用户姓名")]
        public string FULLNAME { get; set; }
        [ViewAttribute(ViewName = "密码")]
        public string PASSWORD { get; set; }
        [ViewAttribute(ViewName = "是否在线")]
        public int ISONLIN { get; set; }
        [ViewAttribute(ViewName = "是否启用")]
        public string  ISUSING { get; set; }
        [ViewAttribute(ViewName = "头像地址")]
        public string IMGURL { get; set; }
        [ViewAttribute(ViewName = "是否管理账号")]
        public string ISADMIN { get; set; }
        [ViewAttribute(ViewName = "所属机构编号")]
        public string ORGID { get; set; }
    }
}
