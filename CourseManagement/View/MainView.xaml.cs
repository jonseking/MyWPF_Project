using CourseManagement.Common;
using CourseManagement.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseManagement.View
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.PrimaryScreenHeight;
        }

        private void WinMove_LeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            { 
                this.DragMove();    
            }
        }

        private void btnMin_Clik(object sender, RoutedEventArgs e)
        {
            this.WindowState= WindowState.Minimized;
        }

        private void btnMax_Clik(object sender, RoutedEventArgs e)
        {
            this.WindowState= this.WindowState==WindowState.Maximized? WindowState.Normal:WindowState.Maximized;    
        }

        private void btClose_Clik(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("确定要退出系统码？","退出系统",MessageBoxButton.YesNo,MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                Application.Current.Shutdown();
            }
            else
            {
                return;
            }

        }
    }
}
