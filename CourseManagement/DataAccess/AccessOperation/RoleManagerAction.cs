using Form.DataAccess.PORM.Data;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using CourseManagement.Common;
using Form.DataAccess;

namespace CourseManagement.DataAccess.AccessOperation
{
    public class RoleManagerAction
    {
        /// <summary>
        /// 获取用户信息列表
        /// </summary>
        /// <param name="WhereStr"></param>
        /// <returns></returns>
        public List<SYS_ROLE> GetRoleInfoListAction(string WhereStr, PaginationModel Pagemodel)
        {
            List<SYS_ROLE> list = new List<SYS_ROLE>();
            using (DBHelper db = new DBHelper())
            {
                string sql = string.Format(@"SELECT * FROM SYS_ROLE WHERE 1=1 {0} ", WhereStr);
                try
                {
                    list = db.QueryList<SYS_ROLE>(sql, Pagemodel).ToList();
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
        public bool ChangeUsingStateAction(SYS_ROLE model)
        {
            bool res=false;
            string sql = string.Format(@"UPDATE SYS_ROLE SET ISUSING='{1}' WHERE ROLEID='{0}'", model.ROLEID, model.ISUSING == 0 ?1 : 0);
            using (DBHelper db = new DBHelper())
            {
                db.BeginTransaction();
                try
                {
                    int count = db.ExecuteNonQuery(sql);
                    //如果是禁用角色则角色下用户一同禁用
                    if (count > 0 && model.ISUSING==1)
                    {
                        sql = string.Format(@"UPDATE SYS_USER SET USERSTATE=0 WHERE ROLEID={0}", model.ROLEID);
                        db.ExecuteNonQuery(sql);
                    }
                    db.CommitTransaction();
                    res = true;
                }
                catch (Exception)
                {
                    db.RollbackTransaction();
                    throw;
                }
                return res;
            }
        }

        public SYS_ROLE GetRoleInfoByID(string id)
        {
            using (DBHelper db = new DBHelper())
            {
                string sql = string.Format(@"SELECT * FROM SYS_ROLE WHERE ROLEID='{0}'", id);
                return db.QueryModel<SYS_ROLE>(sql);
            }
        }

        /// <summary>
        /// 编辑角色信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int EditRoleInfo(SYS_ROLE model)
        {
            using (DBHelper db = new DBHelper())
            {
                return db.Update<SYS_ROLE>(model);
            }
        }

        /// <summary>
        /// 添加角色信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public int AddRoleInfo(SYS_ROLE model)
        {
            using (DBHelper db = new DBHelper())
            {
                model.ISUSING = 1;
                model.CREATETIME = DateTime.Now;
                model.CREATOR = GlobalValue.UserInfo.USERNAME;
                return db.Insert<SYS_ROLE>(model);
            }
        }
    }
}
