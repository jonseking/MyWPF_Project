using CourseManagement.Common;
using CourseManagement.Model.EntityModel;
using Microsoft.Win32.SafeHandles;
using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace CourseManagement.Modules.ViewModels.BaseInfo
{
    public class DefaulViewModel:TabinfoHelper,INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void DoNotify([CallerMemberName] string propName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
        private int _DashboardVar;

        public int DashboardVar
        {
            get { return _DashboardVar; }
            set { _DashboardVar = value;this.DoNotify(); }
        }
        Random random= new Random();
        public DefaulViewModel(IUnityContainer unityContainer, IRegionManager regionManager)
            : base(unityContainer, regionManager)
        {
            PageTitle = "首页";
            IsCanClose = false;

            //动态化仪表盘
            Task.Factory.StartNew(new Action(async () => {
                while (true)
                {
                    DashboardVar = random.Next(1, 100);
                    await Task.Delay(1000);
                }
            }));

        }

    }
}
