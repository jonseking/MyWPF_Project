using CourseManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class DataGridCheckModel:NotifyBase
    {
        private string _isCheckAll = "False";

        public string IsCheckAll
        {
            get { return _isCheckAll; }
            set { _isCheckAll = value;this.DoNotify(); }
        }

        private string _checkTitle = "全选";

        public string CheckTitle
        {
            get { return _checkTitle; }
            set { _checkTitle = value; this.DoNotify(); }
        }
    }
}
