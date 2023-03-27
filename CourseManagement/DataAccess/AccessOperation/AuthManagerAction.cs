using CourseManagement.Model;
using CourseManagement.Model.EntityModel;
using Form.DataAccess.PORM.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CourseManagement.DataAccess.AccessOperation
{
    public class AuthManagerAction
    {
        /// <summary>
        /// 获取权限列表
        /// </summary>
        /// <param name="WhereStr"></param>
        /// <returns></returns>
        public List<ISYS_AUTH> GetAuthListAction(string RoleID)
        {
            List<ISYS_AUTH> list = new List<ISYS_AUTH>();
            using (DBHelper db = new DBHelper())
            {
                //读取全部菜单
                string sql = string.Format(@"SELECT A.*,B.ROLEID AS ISHAVE FROM SYS_AUTH A LEFT JOIN 
(SELECT AUTHID,ROLEID FROM SYS_ROLEAUTH WHERE ROLEID={0}) B
ON A.AUTHID=B.AUTHID WHERE A.ISUSING=1", RoleID);
                try
                {
                    list = db.QueryList<ISYS_AUTH>(sql).ToList();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return list;
        }

        /// <summary>
        /// 保存角色权限
        /// </summary>
        /// <param name="list"></param>
        /// <param name="RoleID"></param>
        /// <returns></returns>
        public int SaveRoleAuth(List<SysAuthListModel> list, string RoleID)
        {
            int result =0;
            string delrolesql = string.Format(@"DELETE FROM SYS_ROLEAUTH WHERE ROLEID={0}", RoleID);
            string inssql=string.Empty;
            string delusersql=string.Empty;
            String authids=string.Empty;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    authids += item.SysAuth.AUTHID + ",";
                }
                authids = authids.Remove(authids.LastIndexOf(","));
                inssql = string.Format(@"INSERT INTO SYS_ROLEAUTH(ROLEID,AUTHID) 
                                       SELECT {0},AUTHID FROM SYS_AUTH 
                                       WHERE AUTHID IN({1})", RoleID, authids);
                delusersql = string.Format(@"DELETE FROM SYS_USERAUTH WHERE USERID IN(
SELECT USERID FROM SYS_USER WHERE RoleID={0}
) AND AUTHID NOT IN({1})", RoleID, authids);
            }
            using (DBHelper db = new DBHelper()) {
                try
                {
                    db.BeginTransaction();
                    db.ExecuteNonQuery(delrolesql);
                    result=db.ExecuteNonQuery(inssql);
                    if (result != list.Count)
                    {
                        result = -1;
                        db.RollbackTransaction();
                    }
                    db.ExecuteNonQuery(delusersql);
                    db.CommitTransaction();
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
            }
            return result;
        }

        /// <summary>
        /// 获取用户权限列表
        /// </summary>
        /// <param name="WhereStr"></param>
        /// <returns></returns>
        public List<ISYS_AUTH> GetUserAuthListAction(string UserID)
        {
            List<ISYS_AUTH> list = new List<ISYS_AUTH>();
            using (DBHelper db = new DBHelper())
            {
                //读取全部菜单
                string sql = string.Format(@"SELECT A.*,B.USERID AS ISHAVE FROM SYS_AUTH A LEFT JOIN 
(SELECT USERID,AUTHID FROM SYS_USERAUTH WHERE USERID={0}) B
ON A.AUTHID=B.AUTHID
WHERE A.AUTHID IN(SELECT AUTHID FROM SYS_ROLEAUTH 
WHERE ROLEID=(SELECT ROLEID FROM SYS_USER WHERE USERID={0})) ", UserID);
                try
                {
                    list = db.QueryList<ISYS_AUTH>(sql).ToList();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return list;
        }

        /// <summary>
        /// 保存用户权限
        /// </summary>
        /// <param name="list"></param>
        /// <param name="UserID"></param>
        /// <returns></returns>
        public int SaveUserAuth(List<SysAuthListModel> list, string UserID)
        {
            int result = 0;
            string delrolesql = string.Format(@"DELETE FROM SYS_USERAUTH WHERE USERID={0}", UserID);
            string inssql = string.Empty;
            String authids = string.Empty;
            if (list.Count > 0)
            {
                foreach (var item in list)
                {
                    authids += item.SysAuth.AUTHID + ",";
                }
                authids = authids.Remove(authids.LastIndexOf(","));
                inssql = string.Format(@"INSERT INTO SYS_USERAUTH(USERID,AUTHID) 
                                       SELECT {0},AUTHID FROM SYS_AUTH 
                                       WHERE AUTHID IN({1})", UserID, authids);
            }
            using (DBHelper db = new DBHelper())
            {
                try
                {
                    db.BeginTransaction();
                    db.ExecuteNonQuery(delrolesql);
                    result = db.ExecuteNonQuery(inssql);
                    if (result != list.Count)
                    {
                        result = -1;
                        db.RollbackTransaction();
                    }
                    db.CommitTransaction();
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
            }
            return result;
        }
    }
}
