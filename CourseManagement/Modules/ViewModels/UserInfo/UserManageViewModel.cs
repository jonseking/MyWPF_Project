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
        /// 定义字典值
        /// </summary>
        public List<DictionaryInfo> diclist { get; set; }
        public DictionaryInfo Currentdic { get; set; }
        //查询结果集合
        public ObservableCollection<SysUserModel> UserList { get; set; }

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



            //查询按钮事件
            this.surch = new CommandBase();
            //执行通过委托调用方法
            this.surch.DoExecute = new Action<object>(SurchUserList);
            //判断是否执行逻辑
            this.surch.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        public void SurchUserList(object o)
        {
            Surchmodel.IsUsing = Currentdic.DicId;
            string WhereStr = QueryParam.GetWhereString<UserInfoModelcs>(Surchmodel, true, true);
            UserManagerAction action = new UserManagerAction();
            List<SysUserModel> list = action.GetUserInfoList(WhereStr);
            list.ForEach(p => UserList.Add(p));
        }

        public void Bindinfo()
        {
            diclist = new List<DictionaryInfo>();
            diclist.Add(new DictionaryInfo() { DicId = "", DicName = "全部" });
            diclist.Add(new DictionaryInfo() { DicId = "1", DicName = "启用" });
            diclist.Add(new DictionaryInfo() { DicId = "2", DicName = "禁用" });
            Currentdic = diclist[1];
        }
    }
}
