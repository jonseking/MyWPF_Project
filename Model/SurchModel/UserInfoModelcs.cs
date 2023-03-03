using CourseManagement.Common;
using PORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model.SurchModel
{
    public class UserInfoModelcs:NotifyBase
    {
        private string _username;

        [SerachCol(OpSerach = OperaSerach.like, OrdinalIgnoreCase = true)]
        public string Username
        {
            get { return _username; }
            set
            {
                _username = value;
                this.DoNotify();
            }
        }


        private string _fullname;
        [SerachCol(OpSerach = OperaSerach.like, OrdinalIgnoreCase = true)]
        public string Fullname
        {
            get { return _fullname; }
            set
            {
                _fullname = value;
                this.DoNotify();
            }
        }

        private string _IsUsing;
        [SerachCol(OpSerach = OperaSerach.等于, OrdinalIgnoreCase = true)]
        public string IsUsing
        {
            get { return _IsUsing; }
            set
            {
                _IsUsing = value;
                this.DoNotify();
            }
        }

    }
}
