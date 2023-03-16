using CourseManagement.Common;
using PORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model.SurchModel
{
    public class MenuIfoModelcs : NotifyBase
    {
        private string _menuname;

        [SerachCol(OpSerach = OperaSerach.like, OrdinalIgnoreCase = true)]
        public string Menuname
        {
            get { return _menuname; }
            set
            {
                _menuname = value;
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
