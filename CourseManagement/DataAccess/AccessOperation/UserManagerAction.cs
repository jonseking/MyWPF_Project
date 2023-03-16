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
        public List<SYS_USER> GetUserInfoListAction(string WhereStr, PaginationModel Pagemodel)
        {
            List<SYS_USER> list = new List<SYS_USER>();
            using (DBHelper db = new DBHelper())
            {
                string sql = string.Format(@"SELECT * FROM SYS_USER WHERE 1=1 {0} ", WhereStr);
                try
                {
                    list = db.QueryList<SYS_USER>(sql, Pagemodel).ToList();
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
        public int ChangeUsingStateAction(SYS_USER model) {
            string sql = string.Format(@"UPDATE SYS_USER SET USERSTATE='{1}' WHERE USERID='{0}'",model.USERID,model.USERSTATE=="0"?"1":"0");
            using (DBHelper db = new DBHelper()) {
                return db.ExecuteNonQuery(sql); 
            }
        }

        /// <summary>
        /// 重置密码
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public int ResetPasswordAction(string userid)
        {
            string newpassword = BaseFunction.EncryptMd5("999999");
            string sql = string.Format(@"UPDATE SYS_USER SET PASSWORD='{1}' WHERE USERID='{0}'", userid, newpassword);
            using (DBHelper db = new DBHelper())
            {
                return db.ExecuteNonQuery(sql);
            }
        }

        public SYS_USER GetUserInfoByID(string id)
        {
            using (DBHelper db=new DBHelper())
            {
                string sql = string.Format(@"SELECT * FROM SYS_USER WHERE USERID='{0}'", id);
                return db.QueryModel<SYS_USER>(sql);
            }
        }
        /// <summary>
        /// 用户信息删除（后续增加同时删除权限等信息）
        /// </summary>
        /// <param name="userids"></param>
        /// <returns></returns>
        public int DelUserInfo(string userids)
        {
            string sql = string.Format(@"DELETE FROM SYS_USER WHERE USERID IN({0})", userids);
            using (DBHelper db = new DBHelper())
            {
                return db.ExecuteNonQuery(sql);
            }
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int EditUserInfo(SYS_USER model) {
            using (DBHelper db = new DBHelper()) {
                return db.Update<SYS_USER>(model);
            }
        }
    }
}
