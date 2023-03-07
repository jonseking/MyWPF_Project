using Prism.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Pagination.ViewModel
{
    //定义一个委托
    public delegate void PageChangedHandle(object sender, EventArgs e);
    public class PaginationViewModel
    {
        //定义一个事件
        public event PageChangedHandle PageChanged;

        //总页数
        private int totalPage = 1;
        /// <summary>
        /// 当前页
        /// </summary>
        private int currentPage = 1;

        #region 每页显示的条数(依赖属性)
        /// <summary>
        /// 注册当前页
        /// </summary>
        public static readonly DependencyProperty PageSizeProperty = DependencyProperty.Register("PageSize", typeof(String),
        typeof(NextPageControl), new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.AffectsMeasure), new ValidateValueCallback(CurrentPageValidation));

        /// <summary>
        /// 验证当前页
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool PageSizeValidation(object value)
        {
            return true;
        }
        /// <summary>
        /// 当前页
        /// </summary>
        public string PageSize
        {
            get { return GetValue(NextPageControl.PageSizeProperty).ToString(); }
            set
            {
                SetValue(NextPageControl.PageSizeProperty, value);
                lblPageSize.Content = value;
            }
        }
        #endregion

        #region 当前页
        /// <summary>
        /// 注册当前页
        /// </summary>
        public static readonly DependencyProperty CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(String),
        typeof(NextPageControl), new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnCurrentPageChanged)), new ValidateValueCallback(CurrentPageValidation));

        /// <summary>
        /// 验证当前页
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool CurrentPageValidation(object value)
        {
            return true;
        }
        /// <summary>
        /// 当前页
        /// </summary>
        public string CurrentPage
        {
            get { return GetValue(NextPageControl.CurrentPageProperty).ToString(); }
            set
            {
                SetValue(NextPageControl.CurrentPageProperty, value);

                lblCurrentPage.Content = value;
            }
        }


        #endregion

        #region 总页数
        /// <summary>
        /// 总页数
        /// </summary>
        public static readonly DependencyProperty TotalPageProperty = DependencyProperty.Register("TotalPage", typeof(String), typeof(NextPageControl), new FrameworkPropertyMetadata("1", FrameworkPropertyMetadataOptions.AffectsMeasure, new PropertyChangedCallback(OnTotalPageChanged)), new ValidateValueCallback(TotalPageValidation));

        /// <summary>
        /// 总页数进行验证
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool TotalPageValidation(object value)
        {
            return true;
        }
        /// <summary>
        /// 总页数
        /// </summary>
        public string TotalPage
        {
            get { return GetValue(NextPageControl.TotalPageProperty).ToString(); }
            set
            {
                SetValue(NextPageControl.TotalPageProperty, value);

            }
        }

        #endregion
    }

}
