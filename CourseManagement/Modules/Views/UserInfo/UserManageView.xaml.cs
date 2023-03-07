using CourseManagement.Modules.ViewModels   ;
using CourseManagement.Modules.ViewModels.UserInfo;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unity;

namespace CourseManagement.Modules.Views.UserInfo
{
    /// <summary>
    /// UserManageView.xaml 的交互逻辑
    /// </summary>
    public partial class UserManageView : UserControl
    {
        public UserManageView()
        {
            InitializeComponent();
        }

        private void griduserinfo_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            int currpage = PageControl.CurrentPage;
            int pagesize = PageControl.PageSize;
            e.Row.Header = (currpage-1)*pagesize+e.Row.GetIndex() + 1;
        }
    }
}
