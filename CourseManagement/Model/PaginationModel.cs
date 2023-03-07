using CourseManagement.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class PaginationModel:NotifyBase
    {
        private int _totalPage;
        /// <summary>
        /// 总页数
        /// </summary>
        public int TotalPage
        {
            get { return _totalPage; }
            set
            {
                _totalPage = value;
                this.DoNotify();
            }
        }

        private int _currentPage=1;
        /// <summary>
        /// 当前页
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                this.DoNotify();
            }
        }

        private int _pageSize = 10;
        /// <summary>
        /// 每页显示的记录数
        /// </summary>
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                this.DoNotify();
            }
        }

        private int _totalCount;
        /// <summary>
        /// 数据总条数
        /// </summary>
        public int TotalCount
        {
            get { return _totalCount; }
            set
            {
                _totalCount = value;
                this.DoNotify() ;
            }
        }
    }
}
