using CourseManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.ViewModel
{
    public class MainViewModel
    {
        public string CurrentUserName { get; set; }
        public string CurrentUserImg { get; set; }

        public MainViewModel()
        {
            if (GlobalValue.UserInfo != null)
            {
                CurrentUserName = GlobalValue.UserInfo.USERNAME;
                CurrentUserImg = GlobalValue.UserInfo.IMGURL==null? "../Assets/images/defauluserimg.png" : CurrentUserImg = GlobalValue.UserInfo.IMGURL;
            }
        }
    }
}
