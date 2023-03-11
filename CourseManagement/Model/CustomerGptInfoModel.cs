using CourseManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace CourseManagement.Model
{
    public class CustomerGptInfoModel : NotifyBase
    {
		private string _richText;

		public string RichText
        {
			get { return _richText; }
			set { _richText = value; this.DoNotify(); }
		}

        private string _sendText;

        public string SendText
        {
            get { return _sendText; }
            set { _sendText = value; this.DoNotify(); }
        }
    }
}
