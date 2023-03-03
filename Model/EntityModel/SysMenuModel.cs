using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model.EntityModel
{
    public class SysMenuModel
    {
        public string MENUID { get; set; }
        public string MENUNAME { get; set; }
        public string MENUVIEW { get; set; }
        public string MENUICON { get; set; }
        public int MENUINDEX { get; set; }
        public string FATHERID { get; set; }
        public string AUTHID { get; set; }
        public int ISUSING { get; set; }
    }
}
