using CourseManagement.Common;
using PORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model.SurchModel
{
    public class RoleIfoModelcs : NotifyBase
    {
        private string _rolename;

        [SerachCol(OpSerach = OperaSerach.like, OrdinalIgnoreCase = true)]
        public string RoleName
        {
            get { return _rolename; }
            set
            {
                _rolename = value;
                this.DoNotify();
            }
        }

        private string _isusing;
        [SerachCol(OpSerach = OperaSerach.等于, OrdinalIgnoreCase = true)]
        public string IsUsing
        {
            get { return _isusing; }
            set
            {
                _isusing = value;
                this.DoNotify();
            }
        }
    }
}
