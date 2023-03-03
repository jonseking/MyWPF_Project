using CourseManagement.DataAccess.PORM.Data;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.DataAccess.AccessOperation
{
    public class UserManagerAction
    {
        public List<SysUserModel> GetUserInfoList(string WhereStr)
        {
            List<SysUserModel> list = new List<SysUserModel>();
            using (DBHelper db = new DBHelper())
            {
                //暂时先读取全部菜单
                string sql = string.Format(@"SELECT * FROM SYS_USER WHERE 1=1 {0} ", WhereStr);
                try
                {
                    list = db.QueryList<SysUserModel>(sql).ToList();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return list;
        }
    }
}
