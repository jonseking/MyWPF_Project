using CourseManagement.Common;
using Form.Data;

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
