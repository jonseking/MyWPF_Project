using CourseManagement.Common;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class SysUserListModel:NotifyBase
    {
		private bool _isChecked=false;

		public bool IsChecked
        {
			get { return _isChecked; }
			set { _isChecked = value; this.DoNotify(); }
		}

		private SysUserModel _sysUser;

		public SysUserModel SysUser
        {
			get { return _sysUser; }
			set { _sysUser = value; this.DoNotify(); }
		}


	}
}
