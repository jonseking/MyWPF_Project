using CourseManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model.EntityModel
{
    public class SysAuthListModel:NotifyBase
    {

        //能否展开
        private String _canexpanded;

        public String CanExpanded
        {
            get { return _canexpanded; }
            set { _canexpanded = value; this.DoNotify(); }
        }

        //显示控制
        private bool _isopen = false;

        public bool IsOpen
        {
            get { return _isopen; }
            set { _isopen = value; this.DoNotify(); }
        }

        //显示控制
        private String _isshow;

        public String IsShow
        {
            get { return _isshow; }
            set { _isshow = value; this.DoNotify(); }
        }

        //是否选中
        private bool _isChecked = false;

        public bool IsChecked
        {
            get { return _isChecked; }
            set { _isChecked = value; this.DoNotify(); }
        }

        //级别
        private int _level;

        public int Level
        {
            get { return _level; }
            set { _level = value; this.DoNotify(); }
        }

        private ISYS_AUTH _sysAuth;

		public ISYS_AUTH SysAuth
        {
			get { return _sysAuth; }
			set { _sysAuth = value; }
		}

	}
}
