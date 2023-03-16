using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.Model.EntityModel;
using CourseManagement.Modules;
using CourseManagement.View;
using CourseManagement.ViewModel;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Unity;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace CourseManagement
{
    /// <summary>
    /// App.xaml 的交互逻辑
    /// </summary>
    public partial class App : PrismApplication
    {
        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    base.OnStartup(e);

        //    if (new LoginView().ShowDialog() == true)
        //    {
        //        new MainView().ShowDialog();
        //    }
        //    Application.Current.Shutdown();
        //}

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }

        protected override Window CreateShell()
        {
            return Container.Resolve<MainView>();
        }
        //程序退出
        protected override void OnExit(ExitEventArgs e)
        {
            if (GlobalValue.UserInfo != null)
            {
                //修改登录信息
                SYS_USER info = new SYS_USER();
                info.USERID = GlobalValue.UserInfo.USERID;
                info.ISONLINE = 0;
                LoginAction action = new LoginAction();
                action.LoginInfoChange(info);
            }
            base.OnExit(e);
        }

        protected override void InitializeShell(Window shell)
        {
            if (Container.Resolve<LoginView>().ShowDialog()==false)
            {
                Application.Current.Shutdown();
            }
            shell.DataContext = new MainViewModel();
            base.InitializeShell(shell);
        }
        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            //可自动扫描
            moduleCatalog.AddModule<MainModule>();
        }
    }
}
