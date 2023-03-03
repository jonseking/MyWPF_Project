using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

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
    }
}
