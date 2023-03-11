using CourseManagement.Common;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class MessageInfoModel:NotifyBase
    {
        //停靠位置
        private string _alig = "Left";

        public string Alig
        {
            get { return _alig; }
            set { _alig = value; this.DoNotify(); }
        }

        //左侧显示状态
        private string _leftShow= "Hidden";

		public string LeftShow
        {
			get { return _leftShow; }
			set { _leftShow = value; this.DoNotify(); }
		}

        //右侧显示状态
        private string _rightShow= "Hidden";

        public string RightShow
        {
            get { return _rightShow; }
            set { _rightShow = value; this.DoNotify(); }
        }

        //左侧简讯
        private string _leftBrief=string.Empty;

        public string LeftBrief
        {
            get { return _leftBrief; }
            set { _leftBrief = value; this.DoNotify(); }
        }

        //右侧简讯
        private string _rightBrief=string.Empty;

        public string RightBrief
        {
            get { return _rightBrief; }
            set { _rightBrief = value; this.DoNotify(); }
        }

        //左侧图片
        private string _leftImg = string.Empty;

        public string LeftImg
        {
            get { return _leftImg; }
            set { _leftImg = value; this.DoNotify(); }
        }

        //右侧图片
        private string _rightImg = string.Empty;

        public string RightImg
        {
            get { return _rightImg; }
            set { _rightImg = value; this.DoNotify(); }
        }

        //左侧消息
        private string _leftMessage = string.Empty;

        public string LeftMessage
        {
            get { return _leftMessage; }
            set { _leftMessage = value; this.DoNotify(); }
        }

        //右侧消息
        private string _rightMessage = string.Empty;

        public string RightMessage
        {
            get { return _rightMessage; }
            set { _rightMessage = value; this.DoNotify(); }
        }
    }
}
