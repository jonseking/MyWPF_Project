using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model;
using CourseManagement.Model.EntityModel;
using CourseManagement.Model.SurchModel;
using CourseManagement.Modules.Views.UserInfo;
using PORM.Data;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Unity;

namespace CourseManagement.Modules.ViewModels.UserInfo
{
    public class UserManageViewModel : TabinfoHelper
    {
        /// <summary>
        /// 定义查询条件
        /// </summary>
        public UserInfoModelcs Surchmodel { get; set; }
        /// <summary>
        /// 定义查询方法
        /// </summary>
        public CommandBase surch { get; set; }
        /// <summary>
        /// 定义启用禁用方法
        /// </summary>
        public CommandBase ChangeUsing { get; set; }
        /// <summary>
        /// 重置密码
        /// </summary>
        public CommandBase ResetPassword { get; set; }
        
        /// <summary>
        /// 定义字典值
        /// </summary>
        public List<DictionaryInfo> diclist { get; set; }
        public DictionaryInfo Currentdic { get; set; }
        //查询结果集合
        public ObservableCollection<SysUserModel> UserList { get; set; }

        UserManagerAction action = new UserManagerAction();
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityContainer"></param>
        /// <param name="regionManager"></param>
        public UserManageViewModel(IUnityContainer unityContainer, IRegionManager regionManager)
            : base(unityContainer, regionManager)   
        {
            //设置Tab显示名称
            string uri = "UserManageView";
            var model = GlobalValue.ListMenuInfo.FirstOrDefault(a => a.MENUVIEW == uri);
            PageTitle = model.MENUNAME;
            IsCanClose = true;
            //初始化查询条件
            Surchmodel = new UserInfoModelcs();
            UserList = new ObservableCollection<SysUserModel>();
            //页面绑定事件
            Bindinfo();



            //启用/禁用事件
            this.ChangeUsing = new CommandBase();
            //执行通过委托调用方法
            this.ChangeUsing.DoExecute = new Action<object>(ChangeUsingStateAction);
            //判断是否执行逻辑
            this.ChangeUsing.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            //查询按钮事件
            this.surch = new CommandBase();
            //执行通过委托调用方法
            this.surch.DoExecute = new Action<object>(SurchUserListAction);
            //判断是否执行逻辑
            this.surch.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            //重置密码事件
            this.ResetPassword = new CommandBase();
            //执行通过委托调用方法
            this.ResetPassword.DoExecute = new Action<object>(ResetPasswordAction);
            //判断是否执行逻辑
            this.ResetPassword.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        public void SurchUserListAction(object o)
        {
            UserList.Clear();
            Surchmodel.IsUsing = Currentdic.DicId;
            string WhereStr = QueryParam.GetWhereString<UserInfoModelcs>(Surchmodel, true, true);
            List<SysUserModel> list = action.GetUserInfoListAction(WhereStr);
            list.ForEach(p => UserList.Add(p));
        }

        public void Bindinfo()
        {
            diclist = new List<DictionaryInfo>();
            diclist.Add(new DictionaryInfo() { DicId = "", DicName = "全部" });
            diclist.Add(new DictionaryInfo() { DicId = "1", DicName = "启用" });
            diclist.Add(new DictionaryInfo() { DicId = "0", DicName = "禁用" });
            Currentdic = diclist[1];
        }

        public void ChangeUsingStateAction(object o)
        {
            SysUserModel model = ((o as Button).Tag as SysUserModel);
            if (action.ChangeUsingStateAction(model) >= 0)
            {
                string mess = string.Format(@"{0}用户成功！", model.ISUSING == "1" ? "禁用" : "启用");
                MessageBox.Show(mess, "系统消息", MessageBoxButton.OK, MessageBoxImage.Information);
                SurchUserListAction(null);
            }
            else {
                MessageBox.Show("操作失败！", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void ResetPasswordAction(object o)
        {
            string username = o.ToString();
            if (action.ResetPasswordAction(username) >= 0)
            {
                string mess = string.Format(@"重置用户密码成功！");
                MessageBox.Show(mess, "系统消息", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("操作失败！", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
