using Form.DataAccess;
using Form.Data;
using System;

namespace CourseManagement.Model.EntityModel
{
    public class SYS_USER
    {
        [ColumnAttribute(PrimaryKey = true)]
        [ViewAttribute(ViewName = "USERID")]
        public int? USERID { get; set; }

        [ViewAttribute(ViewName = "账号")]
        public string USERNAME { get; set; }

        [ViewAttribute(ViewName = "用户姓名")]
        public string FULLNAME { get; set; }

        [ViewAttribute(ViewName = "密码")]
        public string USERPWD { get; set; }

        [ViewAttribute(ViewName = "添加人")]
        public string CREATOR { get; set; }

        [ViewAttribute(ViewName = "添加时间")]
        public DateTime? CreateTime { get; set; }

        [ViewAttribute(ViewName = "启用状态")]
        public string USERSTATE { get; set; }

        [ViewAttribute(ViewName = "头像地址")]
        public string IMGURL { get; set; }

        [ViewAttribute(ViewName = "是否管理账号")]
        public string ISADMIN { get; set; }

        [ViewAttribute(ViewName = "登录IP")]
        public string LOGINIP { get; set; }

        [ViewAttribute(ViewName = "最后一次登录时间")]
        public DateTime? LASTLOGINTIME { get; set; }

        [ViewAttribute(ViewName = "用户记住密码")]
        public string USERREALPWD { get; set; }
        
        [ViewAttribute(ViewName = "是否在线")]
        public int? ISONLINE { get; set; }

        [ViewAttribute(ViewName = "角色ID")]
        public int? ROLEID { get; set; }
        
    }
}
