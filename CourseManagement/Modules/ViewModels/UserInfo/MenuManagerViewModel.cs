using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model;
using CourseManagement.Model.EntityModel;
using CourseManagement.Model.SurchModel;
using PORM.Data;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Input;
using Unity;
using IDialogService = Prism.Services.Dialogs.IDialogService;
using Prism.Services.Dialogs;

namespace CourseManagement.Modules.ViewModels.UserInfo
{
    public class MenuManagerViewModel: TabinfoHelper
    {
        /// <summary>
        /// 定义启用禁用方法
        /// </summary>
        public CommandBase ChangeUsing { get; set; }

        /// <summary>
        /// 显示子菜单
        /// </summary>
        public ICommand ShowChild { get; set; }

        /// <summary>
        /// 编辑详情
        /// </summary>
        public ICommand EditDetail { get; set; }

        //查询结果集合
        public ObservableCollection<SysMenuListModel> MenuList { get; set; }
        public List<SysMenuListModel> TreeMenuItems=new List<SysMenuListModel>();
        MenuManagerAction action =new MenuManagerAction();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityContainer"></param>
        /// <param name="regionManager"></param>
        public MenuManagerViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IDialogService dialogService)
            : base(unityContainer, regionManager, dialogService)
        {
            //设置Tab显示名称
            string uri = "MenuManagerView";
            var model = GlobalValue.ListMenuInfo.FirstOrDefault(a => a.MENUVIEW == uri);
            PageTitle = model.MENUNAME;
            IsCanClose = true;

            MenuList = new ObservableCollection<SysMenuListModel>();
            //页面默认加载时查询
            SurchMenuListAction(null);

            //启用/禁用事件
            this.ChangeUsing = new CommandBase();
            //执行通过委托调用方法
            this.ChangeUsing.DoExecute = new Action<object>(ChangeUsingStateAction);
            //判断是否执行逻辑
            this.ChangeUsing.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            //编辑详情
            this.EditDetail = new DelegateCommand<object>(EditDetailAction);
            //显示子项事件
            this.ShowChild = new DelegateCommand<object>(ShowChildAction);
        }

        public void SurchMenuListAction(object o)
        {
            MenuList.Clear();
            TreeMenuItems.Clear();
            FillMenus(TreeMenuItems,0,0);
            TreeMenuItems.ForEach(p => MenuList.Add(new SysMenuListModel
            {
                SysMenu = p.SysMenu,
                CanExpanded = p.CanExpanded,
                IsShow = p.IsShow,
                Level= p.Level
            }));
        }

        ///// <summary>
        ///// 递归填充树状菜单列表
        ///// </summary>
        ///// <param name="TreeMenuItems">树状菜单列表</param>
        ///// <param name="FatherId"></param>
        private void FillMenus(List<SysMenuListModel> TreeMenuItems, int ParentId,int Num)
        {
            Num = Num + 1;
            List<SYS_MENU> list = action.GetMenuListAction();
            var menus = list.Where(m => m.PARENTID == ParentId).OrderBy(o => o.MENUINDEX);

            if (menus.Count() > 0)
            {
                foreach (var item in menus)
                {
                    SysMenuListModel mm = new SysMenuListModel()
                    {
                        SysMenu = item,
                        CanExpanded = list.Where(m => m.PARENTID == item.MID).Count() > 0 ? "Visible" : "Collapsed",
                        IsShow= item.PARENTID==0 ? "Visible" : "Collapsed",
                        Level=Num
                    };

                    TreeMenuItems.Add(mm);

                    FillMenus(TreeMenuItems, item.MID, Num);
                }
            }

        }

        public void ShowChildAction(object o) 
        { 
            int mid=Convert.ToInt32(o);
            string state = MenuList.First(m => m.SysMenu.MID == mid).IsOpen == true ? "close" : "open";
            if (state == "open")
            {
                OpenChild(mid);
            }
            else 
            {
                CloseChild(mid);
            }
        }

        public void OpenChild(int mid)
        {
            foreach (var item in MenuList)
            {
                if (item.SysMenu.MID == mid)
                {
                    item.IsOpen = true;
                }
                if (item.SysMenu.PARENTID == mid)
                {
                    item.IsShow = "Visible";
                }
            }
        }

        public void CloseChild(int mid)
        {
            foreach (var item in MenuList)
            {
                if (item.SysMenu.MID == mid)
                {
                    item.IsOpen = false;
                }
                if (item.SysMenu.PARENTID == mid)
                {
                    item.IsShow = "Collapsed";
                    if (MenuList.Where(a => a.SysMenu.PARENTID == item.SysMenu.MID).Count() > 0)
                    {
                        CloseChild(item.SysMenu.MID);
                    }
                }
            }
        }

        /// <summary>
        /// 用户启/禁用
        /// </summary>
        /// <param name="o"></param>
        public void ChangeUsingStateAction(object o)
        {
            SYS_MENU model=((o as Button).Tag as SysMenuListModel).SysMenu;
            
            if (action.ChangeIsUsingAction(model) >= 0)
            {
                string mess = string.Format(@"{0}菜单成功！", model.ISUSING == 1 ? "禁用" : "启用");
                MessageBox.Show(mess, "系统消息", MessageBoxButton.OK, MessageBoxImage.Information);
                SurchMenuListAction(null);
            }
            else
            {
                MessageBox.Show("操作失败！", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        /// <summary>
        /// 编辑详情页面
        /// </summary>
        /// <param name="o"></param>
        private void EditDetailAction(object o)
        {

            DialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("id", o.ToString());
            DetailAction(dialogParameters);
        }

        /// <summary>
        /// 弹出明细窗口
        /// </summary>
        private void DetailAction(DialogParameters dialogParameters)
        {
            _dialogService.ShowDialog("MenuInfoDetailView", dialogParameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    MessageBox.Show("更新菜单信息成功！", "系统消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    SurchMenuListAction(null);
                }
            });
        }
    }
}
