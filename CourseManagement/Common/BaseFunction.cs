using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Security.RightsManagement;

namespace CourseManagement.Common
{
    public class BaseFunction
    {
        /// <summary>
        /// 计算给定字符串的哈希值，双加密先SHA1后MD5
        /// </summary>
        /// <param name="srcStr">待计算的字符串</param>
        /// <returns>计算后的哈希值</returns>
        /// <remarks>
        /// 当字符串传入null或者string.Empty时，返回string.Empty
        /// </remarks>
        /// <example>
        /// 举例说明
        /// <code>
        /// string r11 = BaseFunction.EncryptMd5("123");
        /// </code>
        /// </example>
        public static string EncryptMd5(string srcStr)
        {
            if (string.IsNullOrEmpty(srcStr))
            {
                return string.Empty;
            }
            else
            {
                MD5 md5 = MD5.Create();
                Byte[] pws = md5.ComputeHash(Encoding.UTF8.GetBytes(srcStr));
                string Md5str = "";
                foreach (var item in pws)
                {
                    Md5str += item.ToString("X2");
                }
                return Md5str;
            }
        }

        /// <summary>
        /// 获取Ip
        /// </summary>
        /// <returns></returns>
        public static string GetLocalIp()
        {
            string ip = "";
            string HostName = Dns.GetHostName(); //得到主机名
            IPHostEntry myEntry = Dns.GetHostEntry(HostName);
            for (int i = 0; i < myEntry.AddressList.Length; i++)
            {
                //从IP地址列表中筛选出IPv4类型的IP地址
                //AddressFamily.InterNetwork表示此IP为IPv4,
                //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                if (myEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                {
                    ip = myEntry.AddressList[i].ToString();
                    break;
                }
            }
            return ip;
        }

        //public static T ConversionModel<T>(T model,string data) where T : class
        //{

        //    return model=data.
        //}
    }
}
