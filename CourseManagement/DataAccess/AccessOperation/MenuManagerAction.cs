using CourseManagement.DataAccess.PORM.Data;
using CourseManagement.Model.EntityModel;
using CourseManagement.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace CourseManagement.DataAccess.AccessOperation
{
    public class MenuManagerAction
    {
        /// <summary>
        /// 获取菜单列表
        /// </summary>
        /// <param name="WhereStr"></param>
        /// <returns></returns>
        public List<SYS_MENU> GetMenuListAction()
        {
            List<SYS_MENU> list = new List<SYS_MENU>();
            using (DBHelper db = new DBHelper())
            {
                //读取全部菜单
                string sql = string.Format(@"SELECT * FROM SYS_MENU");
                try
                {
                    list = db.QueryList<SYS_MENU>(sql).ToList();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return list;
        }
        /// <summary>
        /// 更新启用状态
        /// </summary>
        /// <param name="menu"></param>
        /// <returns></returns>
        public int ChangeIsUsingAction(SYS_MENU model)
        {
            DataTable dt =QueryChildIDByParentId(model.MID);
            string mids="'"+model.MID.ToString()+"',";
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    mids += "'" + dt.Rows[i][0].ToString() + "',";
                }
            }
            mids=mids.Remove(mids.LastIndexOf(","));
            string sql = string.Format(@"UPDATE SYS_MENU SET ISUSING='{1}' WHERE MID IN({0})", mids, model.ISUSING == 0 ? 1 : 0);
            using (DBHelper db = new DBHelper())
            {
                return db.ExecuteNonQuery(sql);
            }
        }
        /// <summary>
        /// 递归查询子菜单
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public DataTable QueryChildIDByParentId(int parentid)
        {
            using (DBHelper db = new DBHelper())
            {
                string sql = string.Format(@"WITH CTE AS(
 SELECT MID 
 FROM SYS_MENU
 WHERE ParentId={0}
 UNION ALL
 SELECT c.MID
 FROM CTE P
 INNER JOIN SYS_MENU c ON p.MID=c.ParentId
)
SELECT MID from CTE", parentid);
                return db.ExecuteDataTable(sql);
            }
        }
        /// <summary>
        /// 根据ID查询单个菜单
        /// </summary>
        /// <param name="mid"></param>
        /// <returns></returns>
        public SYS_MENU GeteMenuInfoByID(String mid)
        {
            using (DBHelper db=new DBHelper()) 
            {
                string sql = string.Format(@"SELECT * FROM SYS_MENU WHERE MID={0}", mid);
                return db.QueryModel<SYS_MENU>(sql);
            }
        }
        /// <summary>
        /// 编辑菜单信息
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int EditMenuInfo(SYS_MENU model)
        {
            using (DBHelper db = new DBHelper())
            {
                return db.Update<SYS_MENU>(model);
            }
        }
    }
}
