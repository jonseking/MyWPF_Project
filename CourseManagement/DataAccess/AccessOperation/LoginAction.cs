using Form.DataAccess.PORM.Data;
using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using Form.Data;
using Newtonsoft.Json;
using System.Configuration;
using CourseManagement.Model;
using Newtonsoft.Json.Linq;
using CourseManagement.Common;

namespace CourseManagement.DataAccess.AccessOperation
{
    public class LoginAction
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="UserIfo"></param>
        /// <returns></returns>
        public string Login(string Username, string Password,out string message)
        {
            SYS_USER UserIfo = new SYS_USER() { USERNAME=Username,USERPWD=Password };
            #region 调用Api方式
            string parameters = JsonConvert.SerializeObject(UserIfo);
            string Url = ConfigurationManager.AppSettings["AuthorizationWebApi"].ToString().Trim();
            AuthorizationApiResult Apiresult;
            try
            {
                 Apiresult =JsonConvert.DeserializeObject<AuthorizationApiResult>
                    (WebRequestApiHelper.PostJsonAPI(Url, parameters, RequestTypes.JSON));
            }
            catch (Exception ex)
            {
                throw;
            }
            if (Apiresult.Result == "false")
            {
                message = Apiresult.ErrMessage;
                return Apiresult.Result;
            }
            else
            {
                message = Apiresult.Token;
                return Apiresult.Result;
            }
            #endregion
        }

        /// <summary>
        /// 根据ID获取用户信息
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public SYS_USER QueryUserInfoByID(string Userid) 
        {
            string parameters = string.Format("Userid={0}", Userid);
            string Url = ConfigurationManager.AppSettings["VocationalWorkWebApi"].ToString().Trim()+ "/QueryUserInfoByID";
            SYS_USER user;
            try
            {
                VocationalWorkApiResult Apiresult =
    JsonConvert.DeserializeObject<VocationalWorkApiResult>
    (WebRequestApiHelper.PostJsonAPIWithToken(Url, parameters, RequestTypes.X_WWW_FORM_URLENCODED, GlobalValue.Token));
                user=JsonConvert.DeserializeObject<SYS_USER>(Apiresult.Data.ToString());
            }
            catch (Exception ex)
            {
                throw;
            }
            return user;
        }

        /// <summary>
        /// 获取用户菜单
        /// </summary>
        /// <param name="Userid"></param>
        /// <returns></returns>
        public IList<SYS_MENU> GetMenusByUserID(string Userid)
        {
            string parameters = string.Format("Userid={0}", Userid);
            IList<SYS_MENU> listmenus= new List<SYS_MENU>();
            string Url = ConfigurationManager.AppSettings["VocationalWorkWebApi"].ToString().Trim() + "/QueryUserMenuList";
            try
            {
                VocationalWorkApiResult Apiresult =
    JsonConvert.DeserializeObject<VocationalWorkApiResult>
    (WebRequestApiHelper.PostJsonAPIWithToken(Url, parameters, RequestTypes.X_WWW_FORM_URLENCODED, GlobalValue.Token));
                Apiresult.DataList.ForEach(m => listmenus.Add(JsonConvert.DeserializeObject<SYS_MENU>(m.ToString())));
            }
            catch (Exception ex)
            {
                throw;
            }
            return listmenus;
        }

        //修改用户登录信息
        public int LoginInfoChange(SYS_USER user)
        {
            int res = 0;
            string parameters = JsonConvert.SerializeObject(user);
            string Url = ConfigurationManager.AppSettings["VocationalWorkWebApi"].ToString().Trim()+ "/UpdateUserInfo";
            try
            {
                VocationalWorkApiResult Apiresult =
    JsonConvert.DeserializeObject<VocationalWorkApiResult>
    (WebRequestApiHelper.PostJsonAPIWithToken(Url, parameters, RequestTypes.JSON, GlobalValue.Token));
                if (Apiresult.ReturnCode == "true")
                {
                    res = 1;
                }
            }
            catch (Exception)
            {
                throw;
            }
            return res;
        }
    }
}
