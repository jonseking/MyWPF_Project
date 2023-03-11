using CourseManagement.Common;
using CourseManagement.Model;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity;
using static System.Net.Mime.MediaTypeNames;

namespace CourseManagement.Modules.ViewModels.Auxiliary
{
    public class CustomerGptViewModel : TabinfoHelper
    {
        public CommandBase Send { get; set; }  
        
        public CustomerGptInfoModel CustomerGptModel { get; set; }

        public ObservableCollection<MessageInfoModel> MessageInfoList { get; set; }


    public CustomerGptViewModel(IUnityContainer unityContainer, IRegionManager regionManager, IDialogService dialogService)
            : base(unityContainer, regionManager, dialogService)
        {
            //设置Tab显示名称
            string uri = "CustomerGptView";
            var model = GlobalValue.ListMenuInfo.FirstOrDefault(a => a.MENUVIEW == uri);
            PageTitle = model.MENUNAME;
            IsCanClose = true;

            CustomerGptModel=new CustomerGptInfoModel();
            //显示结果集合
            MessageInfoList = new ObservableCollection<MessageInfoModel>();

            //发送事件
            this.Send = new CommandBase();
            //执行通过委托调用方法
            this.Send.DoExecute = new Action<object>(SendAction);
            //判断是否执行逻辑
            this.Send.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="o"></param>
        private void SendAction(object o)
        {
            test(CustomerGptModel.SendText); 
        }

        private void test(string send)
        {
            MessageInfoModel model= new MessageInfoModel();
            model.LeftShow = "Visible";
            model.LeftBrief = "我";
            model.LeftImg = "/Assets/images/people.png";
            model.LeftMessage = send;
            MessageInfoList.Add(model);

            model = new MessageInfoModel();
            model.Alig = "Right";
            model.RightShow = "Visible";
            model.RightBrief = "客服";
            model.RightImg = "/Assets/images/robot.png";
            model.RightMessage = send;
            MessageInfoList.Add(model);

            //OpenAIService service = new OpenAIService(new OpenAiOptions
            //{
            //    ApiKey = "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"
            //});
        }
    }
}
