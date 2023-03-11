using CourseManagement.Modules.Views;
using CourseManagement.Modules.Views.Auxiliary;
using CourseManagement.Modules.Views.BaseInfo;
using CourseManagement.Modules.Views.UserInfo;
using CourseManagement.View;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Modules
{
    public class MainModule : IModule
    {
        //初始化时添加一个组件到指定区域
        public void OnInitialized(IContainerProvider containerProvider)
        {
            //需要一个RegionManager
            //左侧菜单
            var regionManager = containerProvider.Resolve<IRegionManager>();
            regionManager.RegisterViewWithRegion("LeftMenuTreeRegion", typeof(LeftTreeView));

        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.Register<LeftTreeView>();

            containerRegistry.RegisterForNavigation<UserManageView>();
            containerRegistry.RegisterForNavigation<CustomerGptView>();
            containerRegistry.RegisterForNavigation<DefaulView>();

            containerRegistry.RegisterDialog<UserInfoDetailView>(); 
        }
    }
}
