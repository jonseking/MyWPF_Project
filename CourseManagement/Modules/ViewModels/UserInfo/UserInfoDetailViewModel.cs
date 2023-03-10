using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model.EntityModel;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
    public class UserInfoDetailViewModel : NotifyBase, IDialogAware
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
        /// 文本框是否可编辑
        /// </summary>
        private bool _isEnabl = false;
        public bool IsEnabl
        {
            get { return _isEnabl; }
            set { _isEnabl = value; this.DoNotify(); }
        }

        /// <summary>
        /// 按钮是否可用
        /// </summary>
        private string _visible = "Visible";
        public string Visible
        {
            get { return _visible; }
            set { _visible = value; this.DoNotify(); }
        }

        /// <summary>
        /// 接收实体类参数
        /// </summary>
        private SYS_USER _usreModel;

        public SYS_USER UsreModel
        {
            get { return _usreModel; }
            set { _usreModel = value; this.DoNotify(); }
        }

        string operation;
        UserManagerAction action = new UserManagerAction();
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
            //接受窗体数据状态查看/编辑/新增\
            operation = parameters.GetValue<String>("type");
            switch (operation)
            {
                case "show":
                    Title = "用户信息查看";
                    Visible = "Collapsed";
                    //获取要编辑的用户信息
                    UsreModel = action.GetUserInfoByID(parameters.GetValue<string>("id"));
                    break;
                case "edit":
                    Title = "用户信息编辑";
                    IsEnabl = true;
                    //获取要编辑的用户信息
                    UsreModel = action.GetUserInfoByID(parameters.GetValue<string>("id"));
                    break;
                case "add":
                    Title = "添加用户信息";
                    IsEnabl = true;
                    break;
                default:
                    Title = "";
                    Visible = "Collapsed";
                    break;
            }
        }
        //确认
        public ICommand ConfirmCommand
        {
            get => new DelegateCommand(() => 
            {
                if (action.EditUserInfo(UsreModel) > 0)
                {
                    RequestClose?.Invoke(new DialogResult(ButtonResult.OK));
                }
                else 
                {
                    string mess = string.Format(@"{0}用户信息失败", operation == "edit" ? "更新" : "新增");
                    MessageBox.Show(mess, "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            });
        }
        //取消
        public ICommand CancelCommand {
            get => new DelegateCommand(() => {
                RequestClose?.Invoke(new DialogResult(ButtonResult.Cancel));
            });
        }
    }
}
