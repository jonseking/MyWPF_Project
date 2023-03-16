using CourseManagement.Common;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class SysMenuListModel:NotifyBase
    {
        private SYS_MENU _sysmenu;

        public SYS_MENU SysMenu
        {
            get { return _sysmenu; }
            set { _sysmenu = value; this.DoNotify(); }
        }

        //能否展开
        private String  _canexpanded;

        public String CanExpanded
        {
            get { return _canexpanded; }
            set { _canexpanded = value; this.DoNotify(); }
        }

        //显示控制
        private bool _isopen=false;

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

        //private List<SysMenuListModel> _childs;
        //public List<SysMenuListModel> Childs 
        //{
        //    get { return _childs; }
        //    set { _childs = value; this.DoNotify(); }
        //}

    }
}
