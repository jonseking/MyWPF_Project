using CourseManagement.Common;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class SysRoleListModel:NotifyBase
    {
		private SYS_ROLE _sysrole;

		public SYS_ROLE SysRole
		{
			get { return _sysrole; }
			set { _sysrole = value; this.DoNotify(); }
		}

	}
}
