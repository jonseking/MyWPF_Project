using CourseManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class LoginModelcs:NotifyBase
    {
		private string  _username;
		public string  Username
		{
			get { return _username; }
			set 
            { 
                _username = value;
                this.DoNotify();
            }
		}

        private string _password;
        public string Password
        {
            get { return _password; }
            set 
            { 
                _password = value;
                this.DoNotify();
            }
        }

        private string _vercode;
        public string Vercode
        {
            get { return _vercode; }
            set 
            {
                _vercode = value;
                this.DoNotify();
            }
        }

    }
}
