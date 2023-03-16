using CourseManagement.Common;
using CourseManagement.DataAccess;
using CourseManagement.Model.EntityModel;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace CourseManagement.Modules.ViewModels
{
    public class LeftTreeViewModel
    {
        //树状结构的菜单列表
        public List<MenuItemModel> TreeMenuItems { get; set; } = new List<MenuItemModel>();

        //数据库读取出来的无树状结构的菜单列表
        private IList<SYS_MENU> Menus = GlobalValue.ListMenuInfo;

        public IRegionManager _regionManager;
        public LeftTreeViewModel(IRegionManager RegionManager)
        {
            //初始加载默认页面
            _regionManager = RegionManager;
            //string username = GlobalValue.UserInfo.USERNAME;
            if (Menus != null)
            {
                ////此处需根据用户角色进入不同的默认页面
                _regionManager.RequestNavigate("MainRegionManager", "DefaulView");
                //填充右侧菜单
                FillMenus(TreeMenuItems,0);
            }
        }

        /// <summary>
        /// 递归填充树状菜单列表
        /// </summary>
        /// <param name="TreeMenuItems">树状菜单列表</param>
        /// <param name="FatherId"></param>
        private void FillMenus(List<MenuItemModel> TreeMenuItems, int ParentId)
        {
            var menus = Menus.Where(m => m.PARENTID == ParentId).OrderBy(o => o.MENUINDEX);

            if (menus.Count() > 0)
            {
                foreach (var item in menus)
                {
                    MenuItemModel mm = new MenuItemModel(_regionManager)
                    {
                        MENUNAME = item.MENUNAME,
                        MENUICON = item.MENUICON==null? ((char)int.Parse("e621", NumberStyles.HexNumber)).ToString() : ((char)int.Parse(item.MENUICON, NumberStyles.HexNumber)).ToString(),
                        MENUVIEW = item.MENUVIEW
                    };

                    TreeMenuItems.Add(mm);

                    FillMenus(mm.Childs = new List<MenuItemModel>(), item.MID);
                }
            }
            
        }



    }
}
