using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Form.Data
{
    /// <summary>
    /// Api访问帮助类
    /// </summary>
    public class WebRequestApiHelper
    {
        /// <summary>
        /// Get请求
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public static string GetJsonAPI(string uri)
        {
            try
            {
                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(uri);
                webRequest.Method = "GET";
                webRequest.ContentType = "application/json";
                webRequest.Accept = "application/json";
                //webRequest.Headers.Add("Authorization", GlobalVariable.NowLoginUser.JwtKey);

                HttpWebResponse webResponse = (HttpWebResponse)webRequest.GetResponse();
                StreamReader reader = new StreamReader(webResponse.GetResponseStream(), Encoding.UTF8);
                String res = reader.ReadToEnd();
                reader.Close();
                return res.Trim();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        /// <summary>
        /// Post请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="parameters"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string PostJsonAPI(string url, string parameters, string type)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = RequestTypes.X_WWW_FORM_URLENCODED;
            }

            UTF8Encoding encoding = new UTF8Encoding();

            byte[] bytesToPost = encoding.GetBytes(parameters); //转换为bytes数据

            string responseResult = String.Empty;
            HttpWebRequest req = null;
            HttpWebResponse cnblogsRespone = null;
            try
            {
                req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/" + type + ";charset=utf-8";
                req.ContentLength = bytesToPost.Length;

                // 解决通过网关请求微服务接口无法获取返回数据的问题
                req.ServicePoint.Expect100Continue = false;
                req.ProtocolVersion = HttpVersion.Version11;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bytesToPost, 0, bytesToPost.Length);
                }
                cnblogsRespone = (HttpWebResponse)req.GetResponse();
                if (cnblogsRespone != null && cnblogsRespone.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr;
                    using (sr = new StreamReader(cnblogsRespone.GetResponseStream()))
                    {
                        responseResult = sr.ReadToEnd();
                    }
                    sr.Close();
                }
                return responseResult;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (cnblogsRespone != null)
                {
                    cnblogsRespone.Close();
                }
            }
        }
        /// <summary>
        /// Post请求带Token
        /// </summary>
        /// <param name="url"></param>
        /// <param name="data"></param>
        /// <param name="type"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string PostJsonAPIWithToken(string url, string parameters, string type, String token)
        {
            if (string.IsNullOrEmpty(type))
            {
                type = RequestTypes.X_WWW_FORM_URLENCODED;
            }
            UTF8Encoding encoding = new UTF8Encoding();

            byte[] bytesToPost = encoding.GetBytes(parameters); //转换为bytes数据

            string responseResult = String.Empty;
            HttpWebRequest req = null;
            HttpWebResponse cnblogsRespone = null;
            try
            {
                req = (HttpWebRequest)HttpWebRequest.Create(url);
                req.Method = "POST";
                req.ContentType = "application/" + type + ";charset=utf-8";
                req.ContentLength = bytesToPost.Length;
                req.Headers.Add("Authorization", "Bearer " + token);

                // 解决通过网关请求微服务接口无法获取返回数据的问题
                req.ServicePoint.Expect100Continue = false;
                req.ProtocolVersion = HttpVersion.Version11;

                using (Stream reqStream = req.GetRequestStream())
                {
                    reqStream.Write(bytesToPost, 0, bytesToPost.Length);
                }
                cnblogsRespone = (HttpWebResponse)req.GetResponse();
                if (cnblogsRespone != null && cnblogsRespone.StatusCode == HttpStatusCode.OK)
                {
                    StreamReader sr;
                    using (sr = new StreamReader(cnblogsRespone.GetResponseStream()))
                    {
                        responseResult = sr.ReadToEnd();
                    }
                    sr.Close();
                }
                return responseResult;
            }
            catch (Exception ex)
            {
                return null;
            }
            finally
            {
                if (cnblogsRespone != null)
                {
                    cnblogsRespone.Close();
                }
            }
        }
    }
}
