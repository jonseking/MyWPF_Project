using Prism.Commands;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CourseManagement.Model.EntityModel
{
    public class MenuItemModel
    {
        public string MENUNAME { get; set; }
        public string MENUVIEW { get; set; }
        public string MENUICON { get; set; }

        //是否打开
        private bool _isExpanded;

        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value; }
        }

        public List<MenuItemModel> Childs { get; set;}

        public ICommand OpenViewCommand { 
        get=>new DelegateCommand(() => {
            if ((this.Childs == null || this.Childs.Count == 0) && !string.IsNullOrEmpty(this.MENUVIEW))
            {
                //页面跳转
                _regionManager.RequestNavigate("MainRegionManager", this.MENUVIEW);
            }
            else {
                this.IsExpanded = !this.IsExpanded;
            }
        }) ;
        }

        IRegionManager _regionManager = null;

        public MenuItemModel(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }
    }
}
