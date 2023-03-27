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
        public static string Token { get; set; }
        public static SYS_USER UserInfo {get;set;}
        public static IList<SYS_MENU> ListMenuInfo { get; set; }
    }
}
