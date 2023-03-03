using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseManagement.DataAccess.PORM.Data;

namespace CourseManagement.DataAccess
{
    public class DBHelperFactory
    {
        /// <summary>
        /// 根据连接字符串配置名称初始化一个DBHelpr类
        /// </summary>
        /// <param name="connectionStringName">连接字符串的名称，为null时为配置文件的第一个连接字符串</param>
        /// <returns></returns>
        public static DBHelper Create(string connectionStringName = null)
        {
            return new DBHelper(connectionStringName);
        }
        /// <summary>
        /// 根据连接IDbConnection初始化一个DBHelpr类
        /// </summary>
        /// <param name="connection">IDbConnection实例</param>
        /// <returns></returns>
        public static DBHelper Create(IDbConnection connection)
        {
            return new DBHelper(connection);
        }
        /// <summary>
        /// 根据连接字符串和providerName初始化一个DBHelpr类
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="providerName"></param>
        /// <returns></returns>
        public static DBHelper Create(string connectionString, string providerName)
        {
            return new DBHelper(connectionString, providerName);
        }
        /// <summary>
        /// 根据连接字符串和DbProviderFactory初始化一个DBHelpr类
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="provider"></param>
        /// <returns></returns>
        public static DBHelper Create(string connectionString, DbProviderFactory provider)
        {
            return new DBHelper(connectionString, provider);
        }
    }
}
