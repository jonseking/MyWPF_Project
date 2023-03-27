using CourseManagement.Common;
using Form.DataAccess.PORM.Data;
using CourseManagement.Model.AdditionalModel;
using CourseManagement.Model.EntityModel;
using Form.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseManagement.DataAccess.AccessOperation
{
    public class UserManagerAction
    {
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="WhereStr"></param>
        /// <returns></returns>
        public List<ISYS_USER> GetUserInfoListAction(string WhereStr, PaginationModel Pagemodel)
        {
            List<ISYS_USER> list = new List<ISYS_USER>();
            using (DBHelper db = new DBHelper())
            {
                string sql = string.Format(@"SELECT * FROM (SELECT A.*,B.ROLENAME FROM SYS_USER A INNER JOIN SYS_ROLE B ON A.ROLEID=B.ROLEID)TB WHERE 1=1 {0} ", WhereStr);
                try
                {
                    list = db.QueryList<ISYS_USER>(sql, Pagemodel).ToList();
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
            string sql = string.Format(@"UPDATE SYS_USER SET USERPWD='{1}' WHERE USERID='{0}'", userid, newpassword);
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
        /// 用户信息删除
        /// </summary>
        /// <param name="userids"></param>
        /// <returns></returns>
        public bool DelUserInfo(string userids)
        {
            bool result=false;
            using (DBHelper db = new DBHelper())
            {
                try
                {
                    string sql = string.Format(@"DELETE FROM SYS_USERAUTH WHERE USERID IN({0})", userids);
                    db.BeginTransaction();
                    db.ExecuteNonQuery(sql);
                    sql = string.Format(@"DELETE FROM SYS_USER WHERE USERID IN({0})", userids);
                    db.ExecuteNonQuery(sql);
                    db.CommitTransaction();
                    result = true;  
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
                return result;
            }
        }
        /// <summary>
        /// 编辑用户信息
        /// </summary>
        /// <param name="model"></param>
        /// <param name="changerole"></param>
        /// <returns></returns>
        public int EditUserInfo(SYS_USER model,bool changerole) {
            using (DBHelper db = new DBHelper()) {
                int result = 0;
                try
                {
                    db.BeginTransaction();
                    if (changerole)
                    {
                        string delsql = string.Format(@"DELETE FROM SYS_USERAUTH WHERE USERID={0}",model.USERID);
                        db.ExecuteNonQuery(delsql);
                    }
                    result=db.Update<SYS_USER>(model);
                    db.CommitTransaction();
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
                return result;
            }
        }
        /// <summary>
        /// 添加用户信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int AddUserInfo(SYS_USER model)
        {
            using (DBHelper db = new DBHelper())
            {
                return db.Insert<SYS_USER>(model);
            }
        }
        /// <summary>
        /// 获取角色信息
        /// </summary>
        /// <returns></returns>
        public List<SYS_ROLE> QueryRoleList()
        { 
            using(DBHelper db = new DBHelper()) {
                string sql = string.Format(@"SELECT * FROM SYS_ROLE WHERE ISUSING=1");
                return db.QueryList<SYS_ROLE>(sql).ToList(); 
            }
        }
    }
}
