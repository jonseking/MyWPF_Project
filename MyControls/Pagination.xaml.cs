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

namespace MyControls
{
    /// <summary>
    /// Pagination.xaml 的交互逻辑
    /// </summary>
    public partial class Pagination : UserControl
    {
        //定义一个委托
        public delegate void PageChangedHandle(object sender, EventArgs e);
        //定义一个事件
        public event PageChangedHandle PageChanged;
        public Pagination()
        {
            InitializeComponent();
        }

        ////总页数
        //private int totalPage = 0;
        ///// <summary>
        ///// 当前页
        ///// </summary>
        //private int currentPage = 1;

        #region 每页显示的条数
        /// <summary>
        ///每页显示的条数
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty = 
            DependencyProperty.Register("PageSize", typeof(int),typeof(Pagination), 
                new PropertyMetadata(default(int)));

        /// <summary>
        /// 验证当前页
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static bool PageSizeValidation(object value)
        //{
        //    return true;
        //}
        /// <summary>
        /// 每页显示的条数
        /// </summary>
        public int PageSize
        {
            get { return (int)GetValue(PageSizeProperty); }
            set
            {
                SetValue(PageSizeProperty, value);
                //lblPageSize.Content = value;
            }
        }
        #endregion

        #region 当前页
        /// <summary>
        /// 注册当前页
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty = 
            DependencyProperty.Register("CurrentPage", typeof(int),typeof(Pagination), 
                new PropertyMetadata(default(int), new PropertyChangedCallback(OnCurrentPageChanged)));

        /// <summary>
        /// 验证当前页
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static bool CurrentPageValidation(object value)
        //{
        //    return true;
        //}
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get { return (int)GetValue(CurrentPageProperty); }
            set
            {
                SetValue(CurrentPageProperty, value);
                //lblCurrentPage.Content = value;
            }
        }
        #endregion

        #region 总页数
        /// <summary>
        /// 总页数
        /// </summary>
        public static readonly DependencyProperty TotalPageProperty = 
            DependencyProperty.Register("TotalPage", typeof(int), typeof(Pagination), 
                new PropertyMetadata(default(int), new PropertyChangedCallback(OnTotalPageChanged)));

        /// <summary>
        /// 总页数进行验证
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        //public static bool TotalPageValidation(object value)
        //{
        //    return true;
        //}
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get { return (int)GetValue(TotalPageProperty); }
            set
            {
                SetValue(TotalPageProperty, value);
                //lblTotalPage.Content = value;
            }
        }

        #endregion

        /// <summary>
        /// 值改变方法将由此方法来引发事件
        /// </summary>
        private void PageChangedFunc()
        {
            if (PageChanged != null)
            {
                ///引发事件
                PageChanged(this, new EventArgs());
            }
        }

        /// <summary>
        /// 首页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btHomePage_Click(object sender, RoutedEventArgs e)
        {
            CurrentPage = 1;
            PageChangedFunc();
        }
        /// <summary>
        /// 前一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btPreviousPage_Click(object sender, RoutedEventArgs e)
        {
            //currentPage = CurrentPage;
            if (CurrentPage > 1)
            {
                CurrentPage = CurrentPage - 1;
                //CurrentPage = currentPage;
            }
            PageChangedFunc();
        }
        /// <summary>
        /// 后一页
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btNextPage_Click(object sender, RoutedEventArgs e)
        {
            //currentPage = CurrentPage;
            //totalPage = TotalPage;
            if (CurrentPage < TotalPage)
            {
                CurrentPage = CurrentPage + 1;
                //CurrentPage = currentPage;
            }
            PageChangedFunc();
        }
        //尾页
        private void btLastPage_Click(object sender, RoutedEventArgs e)
        {
            //currentPage = TotalPage;
            CurrentPage = TotalPage;
            PageChangedFunc();
        }

        //private int GetIntVal(int val)
        //{
        //    int temp = 0;
        //    if (!int.TryParse(val, out temp))
        //    {
        //        temp = 1;
        //    }
        //    return temp;
        //}
        /// <summary>
        /// 当当前页值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void OnTotalPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Pagination).Refrech();
        }
        /// <summary>
        /// 当当前页值改变
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void OnCurrentPageChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            (sender as Pagination).Refrech();
        }

        private void Refrech() {
            lblTotalPage.Text = this.TotalPage.ToString();
            lblCurrentPage.Text = this.CurrentPage.ToString();
            lblPageSize.Text = this.PageSize.ToString();
        }
    }
}
