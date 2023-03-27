using CourseManagement.Common;
using Form.Data;

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

        private string _userstate;
        [SerachCol(OpSerach = OperaSerach.等于, OrdinalIgnoreCase = true)]
        public string UserState
        {
            get { return _userstate; }
            set
            {
                _userstate = value;
                this.DoNotify();
            }
        }

        private string _roleid;
        [SerachCol(OpSerach = OperaSerach.等于, OrdinalIgnoreCase = true)]
        public string RoleId
        {
            get { return _roleid; }
            set
            {
                _roleid = value;
                this.DoNotify();
            }
        }

    }
}
