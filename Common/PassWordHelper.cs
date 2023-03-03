using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace CourseManagement.Common
{
    /// <summary>
    /// 用于PassWord输入框绑定
    /// </summary>
    public class PassWordHelper
    {
        static bool isupdate=false; 
        /// <summary>
        /// 后台绑定前台(依赖属性)
        /// </summary>
        public static readonly DependencyProperty PasswordProperty = 
            DependencyProperty.RegisterAttached("Password", typeof(string), typeof(PassWordHelper),
            new FrameworkPropertyMetadata(string.Empty, new PropertyChangedCallback(OnPropertyChanged)));

        public static string GetPassword(DependencyObject d)
        { 
            return d.GetValue(PasswordProperty).ToString(); 
        }

        public static void SetPassword(DependencyObject d, string value)
        { 
            d.SetValue(PasswordProperty, value);    
        }

        public static void OnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        { 
            PasswordBox password= d as PasswordBox;
            password.PasswordChanged -= Password_PasswordChanged;
            if (!isupdate )
            {
                password.Password = e.NewValue?.ToString();
            }
            password.PasswordChanged += Password_PasswordChanged;
        }

        /// <summary>
        /// 前台传入后台
        /// </summary>
        /// 
        public static readonly DependencyProperty AttachProperty =
    DependencyProperty.RegisterAttached("Attach", typeof(bool), typeof(PassWordHelper),
    new FrameworkPropertyMetadata(default(bool), new PropertyChangedCallback(OnAttached)));

        public static bool GetAttach(DependencyObject d)
        {
            return (bool)d.GetValue(AttachProperty);
        }

        public static void SetAttach(DependencyObject d, bool value)
        {
            d.SetValue(AttachProperty, value);
        }
        public static void OnAttached(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            PasswordBox password = d as PasswordBox;
            password.PasswordChanged += Password_PasswordChanged;
        }

        private static void Password_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordBox passwordBox = sender as PasswordBox;
            SetPassword(passwordBox, passwordBox.Password);
        }
    }
}
