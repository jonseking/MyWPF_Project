using CourseManagement.Common;
using CourseManagement.DataAccess.PORM.Data;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CourseManagement.DataAccess.AccessOperation
{
    public class LoginAction
    {
        public int Login(string Username, string Password,out SYS_USER UserIfo)
        {
            int result = 0;
            UserIfo=new SYS_USER();    
            using (DBHelper db=new DBHelper())
            {
                String Sql = string.Format(@"SELECT * FROM SYS_USER WHERE USERNAME='{0}' AND USERPWD='{1}'", Username, BaseFunction.EncryptMd5(Password));
                try
                {
                    UserIfo = db.QueryModel<SYS_USER>(Sql);
                }
                catch (Exception e)
                {
                    //当前系统异常
                    result = -9;
                    throw;
                }

                if (UserIfo == null)
                {
                    //用户名或密码有误
                    result = -1;
                }
                else 
                {
                    if (UserIfo.USERSTATE == "0")
                    {
                        //当前用户已被禁用
                        result = -2;
                    }
                    //else if (UserIfo.ISONLIN == 1)
                    //{
                    //    //当前用户已在其它设备登录
                    //    result = -3;
                    //}
                    else
                    {
                        result = 1;
                    }
                }
            }
            return result;
        }

        public IList<SYS_MENU> GetMenusByUserID(int UsetID)
        {
            IList<SYS_MENU> listmenus= new List<SYS_MENU>();
            using (DBHelper db = new DBHelper())
            {
                //暂时先读取全部菜单
                string sql = string.Format(@"SELECT * FROM SYS_MENU ");
                try
                {
                    listmenus = db.QueryList<SYS_MENU>(sql);
                }
                catch (Exception e)
                {
                    throw;
                }
            }
            return listmenus;
        }
    }
}
