using CourseManagement.Common;
using CourseManagement.Model.EntityModel;
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

namespace CourseManagement.Modules.Views
{
    /// <summary>
    /// UserControl1.xaml 的交互逻辑
    /// </summary>
    public partial class LeftTreeView : UserControl
    {
        //树状结构的菜单列表
        //public List<MenuItemModel> TreeMenuItems { get; set; } = new List<MenuItemModel>();

        //数据库读取出来的无树状结构的菜单列表
        //private IList<SysMenuModel> Menus = GlobalValue.ListMenuInfo;
        public LeftTreeView()
        {
            InitializeComponent();
            //FillMenus(TreeMenuItems, "0");
        }

        /// <summary>
        /// 递归填充树状菜单列表
        /// </summary>
        /// <param name="TreeMenuItems">树状菜单列表</param>
        /// <param name="FatherId"></param>
        //private void FillMenus(List<MenuItemModel> TreeMenuItems, string FatherId)
        //{
        //    var menus = Menus.Where(m => m.FATHERID == FatherId).OrderBy(o => o.MENUINDEX);

        //    if (menus.Count() > 0)
        //    {
        //        foreach (var item in menus)
        //        {
        //            MenuItemModel mm = new MenuItemModel()
        //            {
        //                MENUNAME = item.MENUNAME,
        //                MENUICON = item.MENUICON,
        //                MENUVIEW = item.MENUVIEW
        //            };
        //            TreeMenuItems.Add(mm);

        //            FillMenus(mm.Childs = new List<MenuItemModel>(), item.MENUID);
        //        }
        //    }

        //}
    }
}
