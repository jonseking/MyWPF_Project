using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model.EntityModel;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace CourseManagement.Modules.ViewModels.UserInfo
{
    /// <summary>
    /// 属性需要通知到页面继承BindableBase
    /// 弹窗页面继承IDialogAware接口
    /// </summary>
    public class MenuInfoDetailViewModel : NotifyBase, IDialogAware
    {
        /// <summary>
        /// 标题
        /// </summary>
        private string _title;
        public string Title
        {
            get { return _title; }
            set { _title = value; this.DoNotify(); }
        }

        /// <summary>
        /// 接收实体类参数
        /// </summary>
        private SYS_MENU _menuModel;

        public SYS_MENU MenuModel
        {
            get { return _menuModel; }
            set { _menuModel = value; this.DoNotify(); }
        }
        MenuManagerAction action = new MenuManagerAction();
        public event Action<IDialogResult> RequestClose;

        /// <summary>
        /// 窗体是否可关闭
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public bool CanCloseDialog() => true;

        public void OnDialogClosed()
        {
           
        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            Title = "菜单信息编辑";
            //获取要编辑的用户信息
            MenuModel = action.GeteMenuInfoByID(parameters.GetValue<string>("id"));
        }

        //确认
        public ICommand ConfirmCommand
        {
            get => new DelegateCommand(() =>
            {
                if (action.EditMenuInfo(MenuModel) > 0)
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
                else
                {
                    MessageBox.Show("更新菜单信息失败", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
