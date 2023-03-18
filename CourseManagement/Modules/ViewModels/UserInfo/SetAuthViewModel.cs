using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model;
using CourseManagement.Model.EntityModel;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Unity;

namespace CourseManagement.Modules.ViewModels.UserInfo
{
    public class SetAuthViewModel : NotifyBase, IDialogAware
    {
        /// <summary>
        /// 显示子菜单
        /// </summary>
        public ICommand ShowChild { get; set; }
        /// <summary>
        /// 选择框事件
        /// </summary>
        public ICommand CheckCommand { get; set; }
        //查询结果集合
        public ObservableCollection<SysAuthListModel> AuthList { get; set; }
        public List<SysAuthListModel> TreeAuthItems = new List<SysAuthListModel>();
        AuthManagerAction action = new AuthManagerAction();
        /// <summary>
        /// 标题
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; this.DoNotify(); }
        }
        public string id { get; set; }
        public string type { get; set; }

        List<ISYS_AUTH> list;
        public SetAuthViewModel()
        {
            AuthList = new ObservableCollection<SysAuthListModel>();
            this.ShowChild = new DelegateCommand<object>(ShowChildAction);
            this.CheckCommand = new DelegateCommand<object>(CheckAction);
        }

        public event Action<IDialogResult> RequestClose;

        public bool CanCloseDialog() => true;
        public void OnDialogClosed()
        {
          
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            id = parameters.GetValue<string>("id");
            type= parameters.GetValue<string>("type");
            if (type == "roleauth")
            {
                Title = "角色权限分配";
                list = action.GetAuthListAction(id);
            }
            else
            {
                Title = "用户权限分配";
                list = action.GetUserAuthListAction(id);
            }
            SurchMenuListAction(null);
        }

        //确认
        public ICommand ConfirmCommand
        {
            get => new DelegateCommand(() =>
            {
                List<SysAuthListModel> list=AuthList.Where(a=>a.IsChecked==true).ToList();
                if (type == "roleauth")
                {
                    if (action.SaveRoleAuth(list, id) > 0)
                    {
                        RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                    }
                    else
                    {
                        MessageBox.Show("保存权限信息失败", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    if (action.SaveUserAuth(list, id) > 0)
                    {
                        RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                    }
                    else
                    {
                        MessageBox.Show("保存权限信息失败", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            });
        }
        //取消
        public ICommand CancelCommand
        {
            get => new DelegateCommand(() => {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }

        public void SurchMenuListAction(object o)
        {
            AuthList.Clear();
            TreeAuthItems.Clear();
            FillAuths(TreeAuthItems,0,0);
            TreeAuthItems.ForEach(a => AuthList.Add(new SysAuthListModel
            {
                SysAuth = a.SysAuth,
                CanExpanded = a.CanExpanded,
                IsShow =a.SysAuth.PARENTID==0? a.IsShow: TreeAuthItems.Where(t=>t.SysAuth.AUTHID==a.SysAuth.PARENTID).First().IsChecked==true? "Visible" : a.IsShow,
                Level= a.Level,
                IsOpen= a.IsOpen,
                IsChecked= a.IsChecked
            }));
        }

        ///// <summary>
        ///// 递归填充树状菜单列表
        ///// </summary>
        ///// <param name="TreeMenuItems">树状菜单列表</param>
        ///// <param name="FatherId"></param>
        private void FillAuths(List<SysAuthListModel> TreeMenuItems, int ParentId,int Num )
        {
            Num=Num+1;
            
            var ahths = list.Where(m => m.PARENTID == ParentId).OrderBy(o => o.AUTHINDEX);

            if (ahths.Count() > 0)
            {
                foreach (var item in ahths)
                {
                    SysAuthListModel mo = new SysAuthListModel()
                    {
                        SysAuth = item,
                        CanExpanded = list.Where(m => m.PARENTID == item.AUTHID).Count() > 0 ? "Visible" : "Collapsed",
                        IsShow = item.PARENTID == 0 ? "Visible" :item.IsHave!=null? "Visible" : "Collapsed",
                        Level = Num,
                        IsOpen= item.IsHave != null ? true : false,
                        IsChecked = item.IsHave != null ? true: false
                    };
                    TreeMenuItems.Add(mo);
                    FillAuths(TreeMenuItems, item.AUTHID, Num);
                }
            }
        }
        /// <summary>
        /// 显示子项
        /// </summary>
        /// <param name="o"></param>
        public void ShowChildAction(object o)
        {
            int authid = Convert.ToInt32(o);
            string state = AuthList.First(m => m.SysAuth.AUTHID == authid).IsOpen == true ? "close" : "open";
            if (state == "open")
            {
                OpenChild(authid);
            }
            else
            {
                CloseChild(authid);
            }
        }

        public void OpenChild(int authid)
        {
            foreach (var item in AuthList)
            {
                if (item.SysAuth.AUTHID == authid)
                {
                    item.IsOpen = true;
                }
                if (item.SysAuth.PARENTID == authid)
                {
                    item.IsShow = "Visible";
                }
            }
        }

        public void CloseChild(int authid)
        {
            foreach (var item in AuthList)
            {
                if (item.SysAuth.AUTHID == authid)
                {
                    item.IsOpen = false;
                }
                if (item.SysAuth.PARENTID == authid)
                {
                    item.IsShow = "Collapsed";
                    if (AuthList.Where(a => a.SysAuth.PARENTID == item.SysAuth.AUTHID).Count() > 0)
                    {
                        CloseChild(item.SysAuth.AUTHID);
                    }
                }
            }
        }
        /// <summary>
        /// 复选框选择事件
        /// </summary>
        /// <param name="o"></param>
        public void CheckAction(object o)
        {
            SysAuthListModel model = AuthList.FirstOrDefault(a => a.SysAuth.AUTHID == Convert.ToInt32(o));
            AuthList.FirstOrDefault(a => a.SysAuth.AUTHID == Convert.ToInt32(o)).IsOpen=model.IsOpen==true? model.IsOpen:!model.IsOpen;
            //先找上层
            if (AuthList.Where(a => a.SysAuth.AUTHID == model.SysAuth.PARENTID).Count() > 0)
            {
                CheckUp(model);
            }
            //再找下层
            if (AuthList.Where(a => a.SysAuth.PARENTID == model.SysAuth.AUTHID).Count() > 0)
            {
                CheckDown(model);
            }
        }
        /// <summary>
        /// 向上处理权限
        /// </summary>
        /// <param name="model"></param>
        public void CheckUp(SysAuthListModel model)
        {
            foreach (var item in AuthList)
            {
                if (item.SysAuth.AUTHID == model.SysAuth.PARENTID)
                {
                    if (AuthList.Where(a =>
                    a.SysAuth.PARENTID == item.SysAuth.AUTHID && a.IsChecked == true
                    ).Count() > 0)
                    {
                        item.IsChecked = true;
                    }
                    else {
                        item.IsChecked = false;
                    }
                    item.IsShow = "Visible";
                    item.IsOpen = true;
                    if (AuthList.Where(a => a.SysAuth.AUTHID == item.SysAuth.PARENTID).Count() > 0)
                    {
                        CheckUp(item);
                    }
                }
            }
        }

        /// <summary>
        /// 向下处理权限
        /// </summary>
        /// <param name="model"></param>
        public void CheckDown(SysAuthListModel model)
        {
            foreach (var item in AuthList)
            {
                if (item.SysAuth.PARENTID == model.SysAuth.AUTHID)
                {
                    item.IsChecked = model.IsChecked;
                    item.IsOpen = true;
                    item.IsShow = "Visible";
                    if (AuthList.Where(a => a.SysAuth.PARENTID == model.SysAuth.AUTHID).Count() > 0)
                    {
                        CheckDown(item);
                    }
                }
            }
        }
    }
}
