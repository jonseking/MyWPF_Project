using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Common
{
    public class GlobalValue
    {
        public static SysUserModel UserInfo{get;set;}
        public static IList<SysMenuModel> ListMenuInfo { get; set; }
    }
}
