using CourseManagement.Common;
using CourseManagement.Model.SurchModel;
using CourseManagement.Model;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using System.Windows.Input;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model.EntityModel;
using System.Collections.ObjectModel;
using PORM.Data;
using Prism.Services.Dialogs;
using Prism.Commands;
using System.Windows.Controls;
using System.Windows;

namespace CourseManagement.Modules.ViewModels.UserInfo
{
    public class RoleManagerViewModel : TabinfoHelper
    {
        /// <summary>
        /// 定义查询条件
        /// </summary>
        public RoleIfoModelcs Surchmodel { get; set; }
        /// <summary>
        /// 定义分页条件
        /// </summary>
        public PaginationModel Pagemodel { get; set; }
        /// <summary>
        /// 定义查询方法
        /// </summary>
        public ICommand Surch { get; set; }
        /// <summary>
        /// 定义新增方法
        /// </summary>
        public ICommand Add { get; set; }
        /// <summary>
        /// 定义启用禁用方法
        /// </summary>
        public ICommand ChangeUsing { get; set; }
        /// <summary>
        /// 查看详情
        /// </summary>
        public ICommand ShowDetail { get; set; }
        /// <summary>
        /// 编辑详情
        /// </summary>
        public ICommand EditDetail { get; set; }
        /// <summary>
        /// 分页管理
        /// </summary>
        public ICommand PageSearchCommand { get; set; }
        /// <summary>
        /// 定义字典值
        /// </summary>
        public List<DictionaryInfo> diclist { get; set; }
        public DictionaryInfo Currentdic { get; set; }
        //查询结果集合
        public ObservableCollection<SysRoleListModel> RoleList { get; set; }

        RoleManagerAction action = new RoleManagerAction();

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="unityContainer"></param>
        /// <param name="regionManager"></param>
        public RoleManagerViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IDialogService dialogService)
            : base(unityContainer, regionManager, dialogService)
        {
            //设置Tab显示名称
            string uri = "RoleManagerView";
            var model = GlobalValue.ListMenuInfo.FirstOrDefault(a => a.MENUVIEW == uri);
            PageTitle = model.MENUNAME;
            IsCanClose = true;

            //初始化查询条件
            Surchmodel = new RoleIfoModelcs();
            Pagemodel = new PaginationModel();

            RoleList = new ObservableCollection<SysRoleListModel>();
            //页面绑定事件
            Bindinfo();
            //页面默认加载时查询
            SurchRoleListAction(null);

            //查询方法
            this.Surch = new DelegateCommand<object>(SurchRoleListAction);
            //分页事件
            this.PageSearchCommand = new DelegateCommand<object>(PageSearchAction);
            //新增方法
            this.Add = new DelegateCommand<object>(AddRoleAction);
            //启用禁用
            this.ChangeUsing= new DelegateCommand<object>(ChangeUsingStateAction);
            //查看详情
            this.ShowDetail = new DelegateCommand<object>(ShowDetailAction);
            //编辑详情
            this.EditDetail = new DelegateCommand<object>(EditDetailAction);
        }

        public void Bindinfo()
        {
            diclist = new List<DictionaryInfo>();
            diclist.Add(new DictionaryInfo() { DicId = "", DicName = "全部" });
            diclist.Add(new DictionaryInfo() { DicId = "1", DicName = "启用" });
            diclist.Add(new DictionaryInfo() { DicId = "0", DicName = "禁用" });
            //设置默认选择项
            Currentdic = diclist[0];
        }

        public void SurchRoleListAction(object o)
        {
            RoleList.Clear();
            Surchmodel.IsUsing = Currentdic.DicId;
            string WhereStr = QueryParam.GetWhereString<RoleIfoModelcs>(Surchmodel, true, true);
            List<SYS_ROLE> list = action.GetRoleInfoListAction(WhereStr, Pagemodel);
            list.ForEach(p => RoleList.Add(new SysRoleListModel() { SysRole = p }));
        }

        /// <summary>
        /// 新增方法
        /// </summary>
        /// <param name="o"></param>
        public void AddRoleAction(object o)
        {
            DialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("type", "add");
            DetailAction(dialogParameters);
        }

        /// <summary>
        /// 查看详情
        /// </summary>
        /// <param name="o"></param>
        public void ShowDetailAction(object o) 
        {
            DialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("id", o.ToString());
            dialogParameters.Add("type", "show");
            DetailAction(dialogParameters);
        }

        /// <summary>
        /// 编辑详情
        /// </summary>
        /// <param name="o"></param>
        public void EditDetailAction(object o)
        {
            DialogParameters dialogParameters = new DialogParameters();
            dialogParameters.Add("id", o.ToString());
            dialogParameters.Add("type", "edit");
            DetailAction(dialogParameters);
        }

        /// <summary>
        /// 弹出明细窗口
        /// </summary>
        private void DetailAction(DialogParameters dialogParameters)
        {
            _dialogService.ShowDialog("RoleInfoDetailView", dialogParameters, result =>
            {
                if (result.Result == ButtonResult.OK)
                {
                    string mess = string.Format(@"{0}角色信息成功！", dialogParameters.GetValue<String>("type") == "edit" ? "更新" : "新增");
                    MessageBox.Show(mess, "系统消息", MessageBoxButton.OK, MessageBoxImage.Information);
                    SurchRoleListAction(null);
                }
            });
        }
        /// <summary>
        /// 翻页
        /// </summary>
        /// <param name="o"></param>
        public void PageSearchAction(object o) 
        {
            SurchRoleListAction(null);
        }

        /// <summary>
        /// 启用禁用
        /// </summary>
        /// <param name="o"></param>
        public void ChangeUsingStateAction(object o) 
        {
            SYS_ROLE model = ((o as Button).Tag as SysRoleListModel).SysRole;
            if (action.ChangeUsingStateAction(model))
            {
                string mess = string.Format(@"{0}角色成功！", model.ISUSING == 1 ? "禁用" : "启用");
                MessageBox.Show(mess, "系统消息", MessageBoxButton.OK, MessageBoxImage.Information);
                SurchRoleListAction(null);
            }
            else
            {
                MessageBox.Show("操作失败！", "系统消息", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
