using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model.EntityModel
{
    public class SysUserModel
    {
        public string USERID { get; set; }
        public string USERNAME { get; set; }
        public string FULLNAME { get; set; }
        public string PASSWORD { get; set; }
        public int ISONLIN { get; set; }
        public string  ISUSING { get; set; }
        public string IMGURL { get; set; }
        public string ISADMIN { get; set; }
        public string ORGID { get; set; }
    }
}
