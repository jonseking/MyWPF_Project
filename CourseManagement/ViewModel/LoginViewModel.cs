using CourseManagement.Common;
using CourseManagement.DataAccess.AccessOperation;
using CourseManagement.DataAccess.PORM.Data;
using CourseManagement.Model;
using CourseManagement.Model.EntityModel;
using CourseManagement.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace CourseManagement.ViewModel
{
    public class LoginViewModel : NotifyBase
    {
        //缓存
        ObjectCache cache = MemoryCache.Default;
        public LoginModelcs Loginmodel { get; set; }
        /// <summary>
        /// 页面关闭
        /// </summary>
        public CommandBase closeapp { get; set; }
        /// <summary>
        /// 登录
        /// </summary>
        public CommandBase login { get; set; }

        private string _errmessage;

        public string ErrMessage
        {
            get { return _errmessage; }
            set
            {
                _errmessage = value;
                this.DoNotify();
            }
        }

        public LoginViewModel()
        {
            this.Loginmodel = new LoginModelcs();
            this.Loginmodel.Username = "Admin";
            this.Loginmodel.Password = "111111";
            this.Loginmodel.Vercode = cache["Vercodecache"] as string;

            //页面关闭时间处理逻辑
            this.closeapp = new CommandBase();
            //执行
            this.closeapp.DoExecute = new Action<object>((o) =>
            {
                (o as Window).Close();
            });
            //是否执行逻辑
            this.closeapp.IsCanExecute = new Func<object, bool>((o) =>
            {
                if ((o as Window).IsLoaded == false)
                {
                    return true;
                }
                else
                {
                    MessageBoxResult result = MessageBox.Show("确定要退出系统？", "系统退出", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                    if (result == MessageBoxResult.OK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            });

            ////登录逻辑处理逻辑
            this.login = new CommandBase();
            ////执行
            this.login.DoExecute = new Action<object>(Login);
            ////判断是否执行逻辑
            this.login.IsCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });
        }

        /// <summary>
        /// 登录事件
        /// </summary>
        /// <param name="o"></param>
        public void Login(object o)
        {
            Button bt=((o as Window).FindName("BtLogin") as Button);
            bt.IsEnabled = false;
            bt.Content="登录中...";
            if (CheckLoginInfo() == "")
            {
                LoginAction loginaction = new LoginAction();
                SYS_USER user;

                //启用线程任务
                Task.Run(new Action( () =>
                {
                    //线程等待
                    //await Task.Delay(2000);
                    int result = loginaction.Login(Loginmodel.Username, Loginmodel.Password, out user);
                    if (result <= 0)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            bt.IsEnabled = true;
                            bt.Content = "登  录";
                        }));
                        switch (result)
                        {
                            case -1:
                                this.ErrMessage = "用户名或密码有误";
                                break;
                            case -2:
                                this.ErrMessage = "当前用户已被禁用";
                                break;
                            case -3:
                                this.ErrMessage = "当前用户已在其它设备登录";
                                break;
                            case -9:
                                this.ErrMessage = "当前系统异常";
                                break;
                            default:
                                this.ErrMessage = "当前系统异常";
                                break;
                        }
                    }
                    else
                    {
                        //给全局变量赋值（用户信息）
                        GlobalValue.UserInfo = user;
                        //给全局变量赋值（菜单列表）
                        GlobalValue.ListMenuInfo = GetMenusByUserID(user.USERID);

                        //
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            (o as Window).DialogResult = true;
                        }));
                    }
                }));
            }
            else
            {
                Application.Current.Dispatcher.Invoke(new Action(() =>
                {
                    bt.IsEnabled = true;
                    bt.Content = "登  录";
                }));
            }
        }

        /// <summary>
        /// 检查登录页面输入
        /// </summary>
        /// <returns></returns>
        public string CheckLoginInfo()
        {
            this.ErrMessage = string.Empty;
            if (string.IsNullOrWhiteSpace(Loginmodel.Username))
            {
                this.ErrMessage = "请输入用户名";
                return ErrMessage;
            }

            if (string.IsNullOrWhiteSpace(Loginmodel.Password))
            {
                this.ErrMessage = "请输入密码";
                return ErrMessage;
            }

            if (string.IsNullOrWhiteSpace(Loginmodel.Vercode))
            {
                this.ErrMessage = "请输入验证码";
                return ErrMessage;
            }

            if (string.IsNullOrEmpty(cache["Vercodecache"] as string))
            {
                this.ErrMessage = "验证码已过期请点击更新";
                return ErrMessage;
            }

            if (!string.IsNullOrWhiteSpace(Loginmodel.Vercode) &&
                Loginmodel.Vercode.ToLower() != cache["Vercodecache"].ToString().ToLower())
            {
                this.ErrMessage = "验证码输入错误";
                return ErrMessage;
            }

            return this.ErrMessage;
        }

        public IList<SysMenuModel> GetMenusByUserID(string UsetID)
        {
            LoginAction loginaction = new LoginAction();
            return loginaction.GetMenusByUserID(UsetID);
        }
    }
}
