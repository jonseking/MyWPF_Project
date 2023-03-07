using CourseManagement.Common;
using CourseManagement.DataAccess.PORM.Data;
using CourseManagement.Model;
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
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="WhereStr"></param>
        /// <returns></returns>
        public List<SysUserModel> GetUserInfoListAction(string WhereStr, PaginationModel Pagemodel)
        {
            List<SysUserModel> list = new List<SysUserModel>();
            using (DBHelper db = new DBHelper())
            {
                //暂时先读取全部菜单
                string sql = string.Format(@"SELECT * FROM SYS_USER WHERE 1=1 {0} ", WhereStr);
                try
                {
                    list = db.QueryList<SysUserModel>(sql, Pagemodel).ToList();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return list;
        }

        /// <summary>
        /// 启用/禁用
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int ChangeUsingStateAction(SysUserModel model) {
            string sql = string.Format(@"UPDATE SYS_USER SET ISUSING='{1}' WHERE USERNAME='{0}'",model.USERNAME,model.ISUSING=="0"?"1":"0");
            using (DBHelper db = new DBHelper()) {
                return db.ExecuteNonQuery(sql); 
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int ResetPasswordAction(string username)
        {
            string newpassword = BaseFunction.EncryptMd5(username);
            string sql = string.Format(@"UPDATE SYS_USER SET PASSWORD='{1}' WHERE USERNAME='{0}'", username, newpassword);
            using (DBHelper db = new DBHelper())
            {
                return db.ExecuteNonQuery(sql);
            }
        }
    }
}
