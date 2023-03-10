using CourseManagement.Model.EntityModel;
using Prism.Commands;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Unity;

namespace CourseManagement.Common
{
    public abstract class TabinfoHelper : INavigationAware
    {
        public string PageTitle { get; set; }

        public bool IsCanClose { get; set; }

        public static string NavUri { get; set; }
        //关闭操作
        public DelegateCommand CloseCommand
        {
            get => new DelegateCommand(() => {

                //更具NavUri获取已注册对象名称
                var obj = _unityContaine.Registrations.FirstOrDefault(v => v.Name == NavUri);
                string name = obj.MappedToType.Name;
                //根据对应名称从Region的Views中获取对象
                if (!string.IsNullOrEmpty(name))
                {
                    var region = _regionManager.Regions["MainRegionManager"];
                    var view = region.Views.FirstOrDefault(v => v.GetType().Name == name);
                    if (view != null)
                    {
                        region.Remove(view);
                    }
                }
                //把对象从Region的Views中删除
            });
        }

        IUnityContainer _unityContaine;
        IRegionManager _regionManager;
         public IDialogService _dialogService;

        public TabinfoHelper(IUnityContainer unityContainer, IRegionManager regionManager,IDialogService dialogService)
        {
            _unityContaine = unityContainer;
            _regionManager = regionManager;
            _dialogService = dialogService; 
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public void OnNavigatedFrom(NavigationContext navigationContext)
        {
            
        }

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            NavUri = navigationContext.Uri.ToString();
        }

        //public event PropertyChangedEventHandler PropertyChanged;
        //public void DoNotify([CallerMemberName] string propName = "")
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        //}
    }
}
