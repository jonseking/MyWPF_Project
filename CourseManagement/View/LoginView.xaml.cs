using CourseManagement.Common;
using CourseManagement.DataAccess.PORM.Data;
using CourseManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CourseManagement.View
{
    /// <summary>
    /// LoginView.xaml 的交互逻辑
    /// </summary>
    public partial class LoginView : Window
    {
        ObjectCache cache = MemoryCache.Default;
        public LoginView()
        {
            InitializeComponent();
            GetVercode();
            this.DataContext = new LoginViewModel();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //系统标题转纵向显示
            string systemName = ""; ;
            foreach (var item in txtsystemname.Text)
            {
                systemName += item + "\n";
            }
            txtsystemname.Text = systemName;
        }

        private void weixin_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("微信登录");
        }

        private void weibo_MouseDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("微博登录");
        }

        private void zhifubao_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //MessageBox.Show("支付宝登录");
            //启示刻度
            double min = -20;
            //终止刻度
            double max = 80;
            //区间值
            double siding1 = (max - min);
            double siding = (max - min) / 10;
        }

        /// <summary>
        /// 页面移动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WinMove_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            //调用数据库
            //DBHelper db=new DBHelper();
            //int count =Convert.ToInt32(db.ExecuteScalar(string.Format(@"SELECT COUNT(1) FROM SYS_USER")));

            //Md5加密
            //string s = BaseFunction.EncryptMd5("111111");

            //string s = cache["filecontents"] as string;
            //MessageBox.Show(s);
        }

        public void GetVercode()
        {
            //验证码
            string code;
            Bitmap bitmap = VerificationCodeHelper.CreateVerifyCode(out code);
            ImageSource imageSource = ImageFormatConvertHelper.ChangeBitmapToImageSource(bitmap);
            this.VercodeImg.Source = imageSource;
            //将验证码值存入缓存，失效时间60秒
            CacheItemPolicy policy = new CacheItemPolicy();
            policy.AbsoluteExpiration =
                DateTimeOffset.Now.AddSeconds(60.0);
            cache.Set("Vercodecache", code, policy);
        }

        private void VercodeImg_MouseDown(object sender, MouseButtonEventArgs e)
        {
            GetVercode();
        }
    }
}
