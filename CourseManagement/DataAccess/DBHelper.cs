using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.DataAccess
{
    using CourseManagement.Model;
    using global::PORM.Data;

    /**
     * 数据库访问帮助类，部分参照petapoco(http://www.toptensoftware.com/petapoco/)
     * */

    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.Common;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;

    namespace PORM.Data
    {
        /// <summary>
        /// 数据访问帮助类。
        /// </summary>
        /// <remarks>
        /// 注意：连接资源不会主动释放，需要手动释放.可使用using或者Dispose()函数释放；定义DBHelper时不能使用全局的Static变量，不然可能会出现问题
        /// </remarks>
        /// <example>
        /// <code>
        /// using(PORM.Data.DBHelper db=new PORM.Data.DBHelper())
        /// {
        ///     ...
        /// }
        /// 
        /// PORM.Data.DBHelper db=new PORM.Data.DBHelper();
        /// try
        /// {
        ///     ...
        /// }
        /// finally
        /// {
        ///     db.Dispose();
        /// }
        /// </code>
        /// </example>
        public class DBHelper : IDisposable
        {
            private string _connStr = string.Empty;
            private string _providerName = string.Empty;
            private DbProviderFactory _factory;
            private IDbConnection _sharedConn;
            private string _lastSql;
            private object[] _lastSqlArgs;
            private string _paramPrefix = "@";
            private DBType _dbType = DBType.SqlServer;
            private Stopwatch _timeWatch = new Stopwatch();
            private string _sqlPage;
            private string _sqlCount;


            static StringBuilder sbLastSql = new StringBuilder();
            static Regex rxColumns = new Regex(@"\A\s*SELECT\s+((?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|.)*?)(?<!,\s+)\bFROM\b", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
            static Regex rxOrderBy = new Regex(@"\bORDER\s+BY\s+(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?(?:\s*,\s*(?:\((?>\((?<depth>)|\)(?<-depth>)|.?)*(?(depth)(?!))\)|[\w\(\)\.])+(?:\s+(?:ASC|DESC))?)*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
            static Regex rxDistinct = new Regex(@"\ADISTINCT\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);
            static Regex rxGroupBy = new Regex(@"\bGROUP\s+BY\s", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Singleline | RegexOptions.Compiled);


            #region 公共属性
            /// <summary>
            /// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间
            /// </summary>
            public int CommandTimeout { get; set; }

            /// <summary>
            /// 获取或设置在终止执行命令的尝试并生成错误之前的等待时间.
            /// <para>与<see cref="CommandTimeout"/>不同的是该时间设置后只有效一次。</para>
            /// </summary>
            public int OneTimeCommandTimeout { get; set; }

            /// <summary>
            /// 数据库类型
            /// </summary>
            public DBType DbType
            {
                get { return _dbType; }
            }

            /// <summary>
            /// 最后一次执行的SQL语句
            /// </summary>
            public string LastSQL { get { return _lastSql; } }

            /// <summary>
            /// 最后一次执行时的参数
            /// </summary>
            public object[] LastSqlArgs { get { return _lastSqlArgs; } }

            /// <summary>
            /// 最后执行的Command命令
            /// </summary>
            public string LastCommand
            {
                get { return FormatCommand(_lastSql, _lastSqlArgs); }
            }
            #endregion

            #region 构造实体
            /// <summary>
            /// 构造DBHelper实体类
            /// </summary>
            /// <param name="connStrName">连接字符串名称，可选参数。如果为空则默认为配置文件中第一个连接字符串。</param>
            public DBHelper(string connStrName = null)
            {
                var providerName = "System.Data.SqlClient";
                if (string.IsNullOrEmpty(connStrName))
                    connStrName = ConfigurationManager.ConnectionStrings[1].Name;
                if (ConfigurationManager.ConnectionStrings[connStrName] != null)
                {
                    if (!string.IsNullOrEmpty(ConfigurationManager.ConnectionStrings[connStrName].ProviderName))
                        providerName = ConfigurationManager.ConnectionStrings[connStrName].ProviderName;
                }
                else
                {
                    throw new InvalidOperationException("未能找到指定名称的连接字符串！");
                }
                _connStr = ConfigurationManager.ConnectionStrings[connStrName].ConnectionString;
                _providerName = providerName;
                CommonCreate();
            }
            /// <summary>
            /// 构造DBHelper实体类
            /// </summary>
            /// <param name="conn">指定的IDbConnection</param>
            public DBHelper(IDbConnection conn)
            {
                _sharedConn = conn;
                _connStr = conn.ConnectionString;
                CommonCreate();
            }
            /// <summary>
            /// 构造DBHelper实体类
            /// </summary>
            /// <param name="connStr">数据库连接字符串</param>
            /// <param name="providerName">用于访问基础数据存储区的 ADO.NET 提供程序的名称</param>
            public DBHelper(string connStr, string providerName = "System.Data.SqlClient")
            {
                _connStr = connStr;
                _providerName = providerName;
                CommonCreate();
            }
            /// <summary>
            /// 构造DBHelper实体类
            /// </summary>
            /// <param name="connStr">数据库连接字符串</param>
            /// <param name="provider">表示一组方法，这些方法用于创建提供程序对数据源类的实现的实例</param>
            public DBHelper(string connStr, DbProviderFactory provider)
            {
                _connStr = connStr;
                _factory = provider;
                CommonCreate();
            }
            private void CommonCreate()
            {
                if (_providerName != null)
                    _factory = DbProviderFactories.GetFactory(_providerName);
                string dbtype = (_factory == null ? _sharedConn.GetType() : _factory.GetType()).Name;
                if (dbtype.Contains("Oracle")) _dbType = DBType.Oracle;
                else if (dbtype.Contains("MySql")) _dbType = DBType.MySql;
                else if (dbtype.Contains("System.Data.SqlClient")) _dbType = DBType.SqlServer;
                else if (_providerName.IndexOf("Oracle", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.Oracle;
                else if (_providerName.IndexOf("MySql", StringComparison.InvariantCultureIgnoreCase) >= 0) _dbType = DBType.MySql;
                if (_dbType == DBType.MySql && _connStr != null && _connStr.IndexOf("Allow User Variables=true") >= 0)
                    _paramPrefix = "?";
                if (_dbType == DBType.Oracle)
                    _paramPrefix = ":";

                _sharedConn = _factory.CreateConnection();
                _sharedConn.ConnectionString = _connStr;
                _sharedConn.Open();
            }

            /// <summary>
            /// 
            /// </summary>
            public void Dispose()
            {
                if (_tran != null)
                {
                    _tran.Dispose();
                    _tran = null;
                }

                if (_sharedConn != null)
                {
                    _sharedConn.Close();
                    _sharedConn.Dispose();
                    _sharedConn = null;
                }
            }

            #endregion

            #region 事务处理
            IDbTransaction _tran = null;
            /// <summary>
            /// db.BeginTransaction()
            /// try
            /// {
            /// ....
            /// db.CommitTransaction();
            /// }
            /// catch{
            /// db.RollbackTransaction();}
            /// </summary>
            /// <returns></returns>
            public void BeginTransaction()
            {
                _tran = _sharedConn.BeginTransaction();
            }
            /// <summary>
            /// 提交事务
            /// </summary>
            public void CommitTransaction()
            {
                if (_tran != null)
                {
                    _tran.Commit();
                    this.DisposeTransaction();
                }
            }
            /// <summary>
            /// 回滚事务
            /// </summary>
            public void RollbackTransaction()
            {
                if (_tran != null)
                {
                    _tran.Rollback();
                    this.DisposeTransaction();
                }
            }
            /// <summary>
            /// 释放事务资源
            /// </summary>
            private void DisposeTransaction()
            {
                if (_tran != null)
                {
                    _tran.Dispose();
                    _tran = null;
                    _sharedConn.Close();
                }
            }
            #endregion

            /// <summary>
            /// 出现异常时
            /// </summary>
            /// <param name="ex"></param>
            private void OnException(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                System.Diagnostics.Debug.WriteLine(LastCommand);
            }
            /// <summary>
            /// 当执行command命令时
            /// </summary>
            /// <param name="cmd"></param>
            private void OnExecutingCommand(IDbCommand cmd)
            {
                _timeWatch.Reset();
                _timeWatch.Start();
            }

            /// <summary>
            /// 当command命令执行完
            /// </summary>
            /// <param name="cmd"></param>
            private void OnExecutedCommand(IDbCommand cmd)
            {
                _timeWatch.Stop();
                System.Diagnostics.Debug.WriteLine(LastCommand);
            }

            #region 准备Command
            //正则，\w匹配字母或数字或下划线或汉字
            static Regex rxParams = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
            /// <summary>
            /// 处理sql语句中的参数
            /// </summary>
            /// <param name="_sql"></param>
            /// <param name="args_src"></param>
            /// <param name="args_dest"></param>
            /// <returns></returns>
            private string ProcessParams(string _sql, object[] args_src, List<object> args_dest)
            {
                return rxParams.Replace(_sql, m =>
                {
                    string param = m.Value.Substring(1);
                    object arg_val;
                    int paramIndex;
                    if (int.TryParse(param, out paramIndex))
                    {
                        //参数如果是数字
                        if (paramIndex < 0 || paramIndex >= args_src.Length)
                            throw new ArgumentOutOfRangeException(string.Format("Parameter '@{0}' specified but only {1} parameters supplied (in `{2}`)", paramIndex, args_src.Length, _sql));
                        arg_val = args_src[paramIndex];
                    }
                    else
                    {
                        // 参数如果不是数字则在args_src中查找名称相同的项
                        bool found = false;
                        arg_val = null;
                        foreach (var o in args_src)
                        {
                            if (o is IDbDataParameter)
                            {
                                IDbDataParameter idbPara = o as IDbDataParameter;
                                if (idbPara.ParameterName.TrimStart('@') == param)
                                {
                                    arg_val = idbPara.Value;
                                    found = true;
                                    break;
                                }
                            }
                            else
                            {
                                var pi = o.GetType().GetProperty(param);
                                if (pi != null)
                                {
                                    arg_val = pi.GetValue(o, null);
                                    found = true;
                                    break;
                                }
                            }
                        }

                        if (!found)
                            throw new ArgumentException(string.Format("Parameter '@{0}' specified but none of the passed arguments have a property with this name (in '{1}')", param, _sql));
                    }

                    // 将参数全部转换成数字参数
                    if ((arg_val as System.Collections.IEnumerable) != null &&
                        (arg_val as string) == null &&
                        (arg_val as byte[]) == null)
                    {
                        var sb = new StringBuilder();
                        foreach (var i in arg_val as System.Collections.IEnumerable)
                        {
                            sb.Append((sb.Length == 0 ? "@" : ",@") + args_dest.Count.ToString());
                            args_dest.Add(i);
                        }
                        return sb.ToString();
                    }
                    else
                    {
                        args_dest.Add(arg_val);
                        return "@" + (args_dest.Count - 1).ToString();
                    }
                }
                );
            }

            /// <summary>
            /// 给DBCommand加载参数
            /// </summary>
            /// <param name="cmd"></param>
            /// <param name="item"></param>
            /// <param name="ParameterPrefix"></param>
            void AddParam(IDbCommand cmd, object item, string ParameterPrefix)
            {
                bool isStoredProcedure = false;
                if (cmd.CommandType == CommandType.StoredProcedure)
                {
                    isStoredProcedure = true;
                }
                var idbParam = item as IDbDataParameter;
                if (idbParam != null)
                {
                    if (!isStoredProcedure)
                        idbParam.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
                    cmd.Parameters.Add(idbParam);
                    return;
                }

                if (!isStoredProcedure)
                {
                    var p = cmd.CreateParameter();
                    p.ParameterName = string.Format("{0}{1}", ParameterPrefix, cmd.Parameters.Count);
                    if (item == null)
                    {
                        p.Value = DBNull.Value;
                    }
                    else
                    {
                        var t = item.GetType();
                        if (t.IsEnum)       // PostgreSQL .NET driver wont cast enum to int
                        {
                            p.Value = (int)item;
                        }
                        else if (t == typeof(Guid))
                        {
                            p.Value = item.ToString();
                            p.DbType = System.Data.DbType.String;
                            p.Size = 40;
                        }
                        else if (t == typeof(string))
                        {
                            p.Size = Math.Max((item as string).Length + 1, 4000);       // Help query plan caching by using common size
                            p.Value = item;
                        }
                        else if (t == typeof(AnsiString))
                        {
                            // Thanks @DataChomp for pointing out the SQL Server indexing performance hit of using wrong string type on varchar
                            p.Size = Math.Max((item as AnsiString).Value.Length + 1, 4000);
                            p.Value = (item as AnsiString).Value;
                            p.DbType = System.Data.DbType.AnsiString;
                        }
                        else if (item.GetType().Name == "SqlGeography") //SqlGeography is a CLR Type
                        {
                            p.GetType().GetProperty("UdtTypeName").SetValue(p, "geography", null); //geography is the equivalent SQL Server Type
                            p.Value = item;
                        }

                        else if (item.GetType().Name == "SqlGeometry") //SqlGeometry is a CLR Type
                        {
                            p.GetType().GetProperty("UdtTypeName").SetValue(p, "geometry", null); //geography is the equivalent SQL Server Type
                            p.Value = item;
                        }
                        else
                        {
                            p.Value = item;
                        }
                    }
                    cmd.Parameters.Add(p);
                }
                else
                {
                    PropertyInfo[] pis = item.GetType().GetProperties();
                    foreach (PropertyInfo pi in pis)
                    {
                        var p = cmd.CreateParameter();
                        p.ParameterName = string.Format("{0}{1}", ParameterPrefix, pi.Name);
                        var value = pi.GetValue(item, null);
                        if (value == null)
                            p.Value = DBNull.Value;
                        else
                        {
                            if (pi.PropertyType.IsEnum)     // PostgreSQL .NET driver wont cast enum to int
                            {
                                p.Value = (int)value;
                            }
                            else if (pi.PropertyType == typeof(Guid))
                            {
                                p.Value = value.ToString();
                                p.DbType = System.Data.DbType.String;
                                p.Size = 40;
                            }
                            else if (pi.PropertyType == typeof(string))
                            {
                                p.Size = Math.Max((value as string).Length + 1, 4000);
                                p.Value = value;
                            }
                            else if (pi.PropertyType == typeof(AnsiString))
                            {
                                p.Size = Math.Max((value as AnsiString).Value.Length + 1, 4000);
                                p.Value = (value as AnsiString).Value;
                                p.DbType = System.Data.DbType.AnsiString;
                            }
                            else
                            {
                                p.Value = value;
                            }
                        }
                        cmd.Parameters.Add(p);
                    }
                }
            }

            // Create a command
            static Regex rxParamsPrefix = new Regex(@"(?<!@)@\w+", RegexOptions.Compiled);
            /// <summary>
            /// 创建一个Comman命令
            /// </summary>
            /// <param name="connection">数据库连接对象</param>
            /// <param name="commType"><see cref="CommandType"/></param>
            /// <param name="sql">SQL语句</param>
            /// <param name="args">参数</param>
            /// <returns>返回一个<see cref="IDbCommand"/>对象</returns>
            public IDbCommand CreateCommand(CommandType commType, string sql, params object[] args)
            {
                bool isStoredProcedure = false;
                if (commType == CommandType.StoredProcedure)
                    isStoredProcedure = true;
                if (!isStoredProcedure)
                {
                    var new_args = new List<object>();
                    sql = ProcessParams(sql, args, new_args);
                    args = new_args.ToArray();
                }

                // 参数前缀是@还是:
                if (_paramPrefix != "@")
                    sql = rxParamsPrefix.Replace(sql, m => _paramPrefix + m.Value.Substring(1));
                sql = sql.Replace("@@", "@");          // <- double @@ escapes a single @

                // 创建command并且加载参数
                IDbCommand cmd = _sharedConn.CreateCommand();
                cmd.Connection = _sharedConn;
                cmd.CommandText = sql;
                if (_tran != null)
                {
                    cmd.Transaction = _tran;
                }
                cmd.CommandType = commType;
                foreach (var item in args)
                {
                    AddParam(cmd, item, _paramPrefix);
                }

                if (_dbType == DBType.Oracle)
                {
                    if (cmd.GetType().GetProperty("BindByName") != null)
                        cmd.GetType().GetProperty("BindByName").SetValue(cmd, true, null);
                }

                if (!String.IsNullOrEmpty(sql))
                    DoPreExecute(cmd);

                return cmd;
            }

            void DoPreExecute(IDbCommand cmd)
            {
                // Setup command timeout
                if (OneTimeCommandTimeout != 0)
                {
                    cmd.CommandTimeout = OneTimeCommandTimeout;
                    OneTimeCommandTimeout = 0;
                }
                else if (CommandTimeout != 0)
                {
                    cmd.CommandTimeout = CommandTimeout;
                }
                else if (System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"] != null)
                {
                    int commTimeout = 0;
                    if (int.TryParse(System.Configuration.ConfigurationManager.AppSettings["CommandTimeout"], out commTimeout))
                    {
                        cmd.CommandTimeout = commTimeout;
                    }
                }

                // Call hook
                OnExecutingCommand(cmd);

                // Save it
                _lastSql = cmd.CommandText;
                _lastSqlArgs = (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray();
            }

            /// <summary>
            /// 格式化Command命令
            /// </summary>
            /// <param name="cmd"></param>
            /// <returns></returns>
            public string FormatCommand(IDbCommand cmd)
            {
                return FormatCommand(cmd.CommandText, (from IDataParameter parameter in cmd.Parameters select parameter.Value).ToArray());
            }

            /// <summary>
            /// 格式化Command命令
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public string FormatCommand(string sql, object[] args)
            {
                var sb = new StringBuilder();
                if (sql == null)
                    return "";

                sb.AppendFormat("{0}", sql);
                if (args != null && args.Length > 0)
                {
                    sb.Append("\n");
                    for (int i = 0; i < args.Length; i++)
                    {
                        if (args[i] != null)
                            sb.AppendFormat("\t -> {0}{1} [{2}] = \"{3}\"\n", _paramPrefix, i, args[i].GetType().Name, args[i]);
                    }
                    sb.Remove(sb.Length - 1, 1);
                }
                sb.AppendFormat("\n总耗时：{0} ms", _timeWatch.ElapsedMilliseconds.ToString("N0"));
                return sb.ToString();
            }
            #endregion

            #region 执行Command命令
            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public int ExecuteNonQuery(string sql, params object[] args)
            {
                CommandType commType = new CommandType();
                return ExecuteNonQuery(commType, sql, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="commType"></param>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public int ExecuteNonQuery(CommandType commType, string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                try
                {
                    if (_sharedConn.State == ConnectionState.Closed)
                        _sharedConn.Open();
                    using (var cmd = CreateCommand(commType, sql, args))
                    {
                        var retv = cmd.ExecuteNonQuery();
                        OnExecutedCommand(cmd);
                        return retv;
                    }

                }
                catch (Exception x)
                {
                    OnException(x);
                    throw;
                }
                finally
                {
                    if (_tran == null)
                        _sharedConn.Close();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object ExecuteScalar(string sql, params object[] args)
            {
                CommandType commType = new CommandType();
                return ExecuteScalar(commType, sql, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="commType"></param>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public object ExecuteScalar(CommandType commType, string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                try
                {
                    if (_sharedConn.State == ConnectionState.Closed)
                        _sharedConn.Open();
                    using (var cmd = CreateCommand(commType, sql, args))
                    {
                        object val = cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);
                        return val;
                    }
                }
                catch (Exception x)
                {
                    OnException(x);
                    throw;
                }
                finally
                {
                    if (_tran == null)
                        _sharedConn.Close();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public T ExecuteScalar<T>(string sql, params object[] args)
            {
                CommandType commType = new CommandType();
                return ExecuteScalar<T>(commType, sql, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="commType"></param>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public T ExecuteScalar<T>(CommandType commType, string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                try
                {
                    if (_sharedConn.State == ConnectionState.Closed)
                        _sharedConn.Open();
                    using (var cmd = CreateCommand(commType, sql, args))
                    {
                        object val = cmd.ExecuteScalar();
                        OnExecutedCommand(cmd);
                        return CommonMap.MapType<T>(val);
                    }
                }
                catch (Exception x)
                {
                    OnException(x);
                    throw;
                }
                finally
                {
                    if (_tran == null)
                        _sharedConn.Close();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public IDataReader ExecuteDataReader(string sql, params object[] args)
            {
                CommandType commType = new CommandType();
                return ExecuteDataReader(commType, sql, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="commType"></param>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public IDataReader ExecuteDataReader(CommandType commType, string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                try
                {
                    if (_sharedConn.State == ConnectionState.Closed)
                        _sharedConn.Open();
                    var cmd = CreateCommand(commType, sql, args);
                    IDataReader idr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    OnExecutedCommand(cmd);
                    return idr;
                }
                catch (Exception x)
                {
                    OnException(x);
                    throw;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public DataSet ExecuteDataSet(string sql, params object[] args)
            {
                CommandType commType = new CommandType();
                return ExecuteDataSet(commType, sql, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="commType"></param>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public DataSet ExecuteDataSet(CommandType commType, string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                try
                {
                    if (_sharedConn.State == ConnectionState.Closed)
                        _sharedConn.Open();
                    using (var cmd = CreateCommand(commType, sql, args))
                    {
                        DbDataAdapter da = _factory.CreateDataAdapter();
                        da.SelectCommand = (DbCommand)cmd;
                        DataSet ds = new DataSet();
                        da.Fill(ds);
                        OnExecutedCommand(cmd);
                        return ds;
                    }

                }
                catch (Exception x)
                {
                    OnException(x);
                    throw;
                }
                finally
                {
                    if (_tran == null)
                        _sharedConn.Close();
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public DataTable ExecuteDataTable(string sql, params object[] args)
            {
                CommandType commType = new CommandType();
                return ExecuteDataTable(commType, sql, args);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="commType"></param>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public DataTable ExecuteDataTable(CommandType commType, string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                try
                {
                    if (_sharedConn.State == ConnectionState.Closed)
                        _sharedConn.Open();
                    using (var cmd = CreateCommand(commType, sql, args))
                    {
                        DbDataAdapter da = _factory.CreateDataAdapter();
                        da.SelectCommand = (DbCommand)cmd;
                        DataTable dt = new DataTable();
                        da.Fill(dt);
                        OnExecutedCommand(cmd);
                        return dt;
                    }

                }
                catch (Exception x)
                {
                    OnException(x);
                    throw;
                }
                finally
                {
                    if (_tran == null)
                        _sharedConn.Close();
                }
            }
            #endregion

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public IEnumerable<T> Query<T>(string sql, params object[] args) where T : class
            {
                return CommonMap.MapToIEnumerable<T>(ExecuteDataTable(sql, args));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public T QueryModel<T>(string sql, params object[] args) where T : class
            {
                DataTable dt = ExecuteDataTable(sql, args);
                if (dt.Rows.Count > 0)
                    return CommonMap.MapToModel<T>(dt.Rows[0]);
                else
                    return default(T);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sql"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public IEnumerable<T> QueryOneObject<T>(string sql, params object[] args)
            {
                //tannanwritetxt(sql);
                if (_sharedConn.State == ConnectionState.Closed)
                    _sharedConn.Open();
                CommandType commType = new CommandType();
                using (var cmd = CreateCommand(commType, sql, args))
                {
                    IDataReader dr = cmd.ExecuteReader(CommandBehavior.CloseConnection);
                    OnExecutedCommand(cmd);
                    List<string> field = new List<string>(dr.FieldCount);
                    for (int i = 0; i < dr.FieldCount; i++)
                    {
                        field.Add(dr.GetName(i).ToLower());
                    }
                    while (dr.Read())
                    {
                        yield return CommonMap.MapType<T>(dr[0]);
                    }
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sql"></param>
            /// <param name="itemsPerPage"></param>
            /// <param name="currentPage"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public ListPage<T> QueryListPage<T>(string sql, int itemsPerPage = 20, int currentPage = 1, params object[] args) where T : class
            {
                ListPage<T> page = new ListPage<T>() { CurrentPage = currentPage, PerPageItems = itemsPerPage };
                BuilderPageSql(sql, itemsPerPage, currentPage);
                if (itemsPerPage == 0)
                {
                    page.Items = Query<T>(_sqlPage, args).ToList<T>();
                    page.TotalItems = page.Items.Count;
                }
                else
                {
                    int itemcount = ExecuteScalar<int>(_sqlCount, args);
                    page.TotalItems = itemcount;
                    if (itemcount == 0)
                        page.Items = new List<T>();
                    else
                        page.Items = Query<T>(_sqlPage, args).ToList<T>();
                }

                return page;
            }

            public IList<T> QueryList<T>(string sql, int itemsPerPage, int currentPage, out int totalrecord) where T : class
            {
                List<T> page = new List<T>();
                BuilderPageSql(sql, itemsPerPage, currentPage);
                if (itemsPerPage == 0)
                {
                    page = Query<T>(_sqlPage, null).ToList<T>();
                    totalrecord = page.Count;
                }
                else
                {
                    int itemcount = ExecuteScalar<int>(_sqlCount, null);
                    totalrecord = itemcount;
                    if (itemcount == 0)
                        page = new List<T>();
                    else
                        page = Query<T>(_sqlPage, null).ToList<T>();
                }

                return page;
            }

            public IList<T> QueryList<T>(string sql, PaginationModel Pagemodel) where T : class
            {
                List<T> page = new List<T>();
                BuilderPageSql(sql, Pagemodel.PageSize, Pagemodel.CurrentPage);
                if (Pagemodel.PageSize == 0)
                {
                    page = Query<T>(_sqlPage, null).ToList<T>();
                    Pagemodel.TotalPage = page.Count;
                }
                else
                {
                    int itemcount = ExecuteScalar<int>(_sqlCount, null);
                    Pagemodel.TotalPage = itemcount% Pagemodel.PageSize==0? 
                        itemcount/ Pagemodel.PageSize: itemcount / Pagemodel.PageSize+1;
                    if (itemcount == 0)
                        page = new List<T>();
                        page = Query<T>(_sqlPage, null).ToList<T>();
                }
                return page;
            }

            public IList<T> QueryList<T>(string sql) where T : class
            {
                List<T> page = new List<T>();
                BuilderPageSql(sql, 0, 0);
                page = Query<T>(_sqlPage, null).ToList<T>();
                return page;
            }


            public DataTable QueryTable(string sql, int itemsPerPage, int currentPage, out int totalrecord)
            {
                DataTable page = new DataTable();
                BuilderPageSql(sql, itemsPerPage, currentPage);
                if (itemsPerPage == 0)
                {
                    page = ExecuteDataTable(_sqlPage, null);
                    totalrecord = page.Rows.Count;
                }
                else
                {
                    int itemcount = ExecuteScalar<int>(_sqlCount, null);
                    totalrecord = itemcount;
                    if (itemcount == 0)
                        page = new DataTable();
                    else
                        page = ExecuteDataTable(_sqlPage, null);
                }

                return page;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="itemsPerPage"></param>
            /// <param name="currentPage"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public TablePage QueryTablePage(string sql, int itemsPerPage = 20, int currentPage = 1, params object[] args)
            {
                TablePage page = new TablePage() { CurrentPage = currentPage, PerPageItems = itemsPerPage };
                BuilderPageSql(sql, itemsPerPage, currentPage);
                if (itemsPerPage == 0)
                {
                    page.Items = ExecuteDataTable(_sqlPage, args);
                    page.TotalItems = page.Items.Rows.Count;
                }
                else
                {
                    int itemcount = ExecuteScalar<int>(_sqlCount, args);
                    page.TotalItems = itemcount;

                    page.TotalPages = (itemcount % itemsPerPage == 0 ? itemcount / itemsPerPage : itemcount / itemsPerPage + 1);

                    if (itemcount == 0)
                        page.Items = new DataTable();
                    else
                        page.Items = ExecuteDataTable(_sqlPage, args);
                }
                return page;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="sql"></param>
            /// <param name="itemsPerPage"></param>
            /// <param name="currentPage"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public ListPage<T> QueryListPageNoCount<T>(string sql, int itemsPerPage = 20, int currentPage = 1, params object[] args) where T : class
            {
                ListPage<T> page = new ListPage<T>() { CurrentPage = currentPage, PerPageItems = itemsPerPage };
                BuilderPageSql(sql, itemsPerPage, currentPage, true);
                if (itemsPerPage == 0)
                {
                    page.Items = Query<T>(_sqlPage, args).ToList<T>();
                    page.TotalItems = page.Items.Count;
                }
                else
                {
                    DataTable dt = ExecuteDataTable(_sqlPage, args);
                    if (dt.Rows.Count > itemsPerPage)
                    {
                        page.NextPage = currentPage + 1;
                        dt.Rows.RemoveAt(itemsPerPage);
                    }
                    page.Items = CommonMap.MapToIEnumerable<T>(dt).ToList();

                }
                return page;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="sql"></param>
            /// <param name="itemsPerPage"></param>
            /// <param name="currentPage"></param>
            /// <param name="args"></param>
            /// <returns></returns>
            public TablePage QueryTablePageNoCount(string sql, int itemsPerPage = 20, int currentPage = 1, params object[] args)
            {
                TablePage page = new TablePage() { CurrentPage = currentPage, PerPageItems = itemsPerPage };
                BuilderPageSql(sql, itemsPerPage, currentPage, true);
                if (itemsPerPage == 0)
                {
                    page.Items = ExecuteDataTable(_sqlPage, args);
                    page.TotalItems = page.Items.Rows.Count;
                }
                else
                {
                    //page.Items = ExecuteDataTable(_sqlPage, args);
                    DataTable dt = ExecuteDataTable(_sqlPage, args);
                    if (dt.Rows.Count > itemsPerPage)
                    {
                        page.NextPage = currentPage + 1;
                        dt.Rows.RemoveAt(itemsPerPage);
                    }
                    page.Items = dt;
                }
                return page;
            }


            /// <summary>
            /// 单表查询
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="model">参数</param>
            /// <returns>返回查询结果实体T</returns>
            public T QueryModel<T>(T model) where T : class
            {
                string sql = string.Empty;
                string tableName = model.GetType().Name;
                Type t = model.GetType();
                object[] tableAttribute = null;

                tableAttribute = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0)
                    tableName = (tableAttribute[0] as TableAttribute).TableName;

                StringBuilder condition = new StringBuilder();
                PropertyInfo[] pis = t.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(model, null) == null)//如果属性值为null 不更新
                    {
                        continue;
                    }
                    condition.AppendFormat(" and {0}=@{0}", pi.Name);
                }
                if (condition.Length == 0)
                    throw new ArgumentException();

                sql = string.Format("select * from {0} where 1=1 {1}", tableName, condition);

                return QueryModel<T>(sql, model);
            }

            /// <summary>
            /// 对单表进行Insert操作
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="model">实体参数</param>
            /// <param name="tran">事务</param>
            /// <returns>返回影响的行数</returns>
            public int Insert<T>(T model) where T : class
            {
                string sql = string.Empty;
                string tableName = model.GetType().Name;

                object[] customAttribute = null;
                object[] tableAttribute = null;

                Type t = model.GetType();
                tableAttribute = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0)
                    tableName = (tableAttribute[0] as TableAttribute).TableName;

                sql = "insert into " + tableName;
                StringBuilder fieldList = new StringBuilder();
                StringBuilder valueList = new StringBuilder();
                PropertyInfo[] pis = t.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(model, null) == null)//如果属性值为null 不插入
                    {
                        continue;
                    }
                    customAttribute = pi.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (customAttribute.Count() == 0 || (customAttribute.Count() == 1 && !(customAttribute[0] as ColumnAttribute).Identity))
                    {
                        fieldList.AppendFormat(",{0}", pi.Name);
                        valueList.AppendFormat(",@{0}", pi.Name);
                    }
                }
                if (fieldList.Length == 0 || valueList.Length == 0)
                    throw new ArgumentException();

                sql = string.Format("insert into {0}({1}) values({2})", tableName, fieldList.Remove(0, 1), valueList.Remove(0, 1));

                return ExecuteNonQuery(sql, model);
            }

            /// <summary>
            /// 根据实体获取insert语句
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="model">实体参数</param>
            /// <param name="tran">事务</param>
            /// <returns>返回影响的行数</returns>
            public string InsertFormat<T>(T model) where T : class
            {
                string sql = string.Empty;
                string tableName = model.GetType().Name;

                object[] customAttribute = null;
                object[] tableAttribute = null;

                Type t = model.GetType();
                tableAttribute = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0)
                    tableName = (tableAttribute[0] as TableAttribute).TableName;

                sql = "insert into " + tableName;
                StringBuilder fieldList = new StringBuilder();
                StringBuilder valueList = new StringBuilder();
                PropertyInfo[] pis = t.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(model, null) == null)//如果属性值为null 不插入
                    {
                        continue;
                    }
                    customAttribute = pi.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (customAttribute.Count() == 0 || (customAttribute.Count() == 1 && !(customAttribute[0] as ColumnAttribute).Identity))
                    {
                        fieldList.AppendFormat(",{0}", pi.Name);
                        valueList.AppendFormat(",@{0}", pi.Name);
                    }
                }
                if (fieldList.Length == 0 || valueList.Length == 0)
                    throw new ArgumentException();

                sql = string.Format("insert into {0}({1}) values({2})", tableName, fieldList.Remove(0, 1), valueList.Remove(0, 1));

                return sql;
            }
            /// <summary>
            /// 对单表进行Update操作
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="model">实体参数</param>
            /// <param name="tran">事务</param>
            /// <returns>返回影响的行数</returns>
            public int Update<T>(T model) where T : class
            {
                string sql = string.Empty;
                string tableName = model.GetType().Name;
                Type t = model.GetType();
                object[] customAttribute = null;
                object[] tableAttribute = null;

                tableAttribute = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0)
                    tableName = (tableAttribute[0] as TableAttribute).TableName;

                StringBuilder keyValuePaire = new StringBuilder();
                StringBuilder condition = new StringBuilder();
                PropertyInfo[] pis = t.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(model, null) == null)//如果属性值为null 不更新
                    {
                        continue;
                    }
                    var s = pi.GetCustomAttributes();
                    customAttribute = pi.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (customAttribute.Count() == 1 && (customAttribute[0] as ColumnAttribute).PrimaryKey)
                        condition.AppendFormat(" and {0}=@{0}", pi.Name);
                    else
                        keyValuePaire.AppendFormat(",{0}=@{0}", pi.Name);
                }
                if (condition.Length == 0 || keyValuePaire.Length == 0)
                    throw new ArgumentException();

                sql = string.Format("update {0} set {1} where 1=1 {2}", tableName, keyValuePaire.Remove(0, 1), condition);
                return ExecuteNonQuery(sql, model);
            }


            /// <summary>
            /// 根据实体获取update语句
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="model">实体参数</param>
            /// <param name="tran">事务</param>
            /// <returns>返回影响的行数</returns>
            public string UpdateFormat<T>(T model) where T : class
            {
                string sql = string.Empty;
                string tableName = model.GetType().Name;
                Type t = model.GetType();
                object[] customAttribute = null;
                object[] tableAttribute = null;

                tableAttribute = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0)
                    tableName = (tableAttribute[0] as TableAttribute).TableName;

                StringBuilder keyValuePaire = new StringBuilder();
                StringBuilder condition = new StringBuilder();
                PropertyInfo[] pis = t.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(model, null) == null)//如果属性值为null 不更新
                    {
                        continue;
                    }
                    customAttribute = pi.GetCustomAttributes(typeof(ColumnAttribute), false);
                    if (customAttribute.Count() == 1 && (customAttribute[0] as ColumnAttribute).PrimaryKey)
                        condition.AppendFormat(" and {0}=@{0}", pi.Name);
                    else
                        keyValuePaire.AppendFormat(",{0}=@{0}", pi.Name);
                }
                if (condition.Length == 0 || keyValuePaire.Length == 0)
                    throw new ArgumentException();

                sql = string.Format("update {0} set {1} where 1=1 {2}", tableName, keyValuePaire.Remove(0, 1), condition);
                return sql;
            }

            /// <summary>
            /// 对单表进行Delete操作
            /// </summary>
            /// <typeparam name="T">泛型T</typeparam>
            /// <param name="model">实体参数</param>
            /// <param name="tran">事务</param>
            /// <returns>返回影响的行数</returns>
            public int Delete<T>(T model) where T : class
            {
                string sql = string.Empty;
                string tableName = model.GetType().Name;
                Type t = model.GetType();
                object[] tableAttribute = null;

                tableAttribute = t.GetCustomAttributes(typeof(TableAttribute), false);
                if (tableAttribute != null && tableAttribute.Length > 0)
                    tableName = (tableAttribute[0] as TableAttribute).TableName;

                StringBuilder condition = new StringBuilder();
                PropertyInfo[] pis = t.GetProperties();
                foreach (PropertyInfo pi in pis)
                {
                    if (pi.GetValue(model, null) == null)//如果属性值为null 不更新
                    {
                        continue;
                    }
                    condition.AppendFormat(" and {0}=@{0}", pi.Name);
                }
                if (condition.Length == 0)
                    throw new ArgumentException();

                sql = string.Format("delete from {0} where 1=1 {1}", tableName, condition);

                return ExecuteNonQuery(sql, model);
            }

            /// <summary>
            /// 根据对象获取查询条件，对象可以为实体或匿名对象  cyding隐藏
            /// </summary>
            /// <param name="db"></param>
            /// <param name="query"></param>
            /// <param name="createorder"></param>
            /// <returns></returns>
            //public string GetWhereString(object query, bool createorder = true)
            //{
            //    if (query == null)
            //        return string.Empty;
            //    StringBuilder where = new StringBuilder();
            //    StringBuilder keyWhere = new StringBuilder();
            //    string sortfields = string.Empty;
            //    string append = string.Empty;
            //    foreach (PropertyInfo pi in query.GetType().GetProperties())
            //    {
            //        if (pi.GetValue(query, null) == null || string.IsNullOrEmpty(pi.GetValue(query, null).ToString().Trim()))
            //            continue;
            //        //如果是追加的查询条件
            //        if (pi.Name.ToLower() == "append")
            //        {
            //            append = pi.GetValue(query, null).ToString();
            //        }
            //        else if (pi.Name.ToLower() == "sortfields")
            //        {
            //            sortfields = pi.GetValue(query, null).ToString();
            //        }
            //        else
            //        {
            //            if (pi.Name.ToLower().EndsWith("_key"))
            //                keyWhere.AppendFormat(" and {0} =@{1}", pi.Name.Substring(0, pi.Name.Length - 4), pi.Name);
            //            else if (pi.Name.ToLower().Contains("areaid"))
            //            {
            //                if (pi.GetValue(query, null).ToString().EndsWith("0000"))
            //                    keyWhere.AppendFormat(" and {0} like '{1}%'", pi.Name, pi.GetValue(query, null).ToString().Substring(0, 2));
            //                else if (pi.GetValue(query, null).ToString().EndsWith("00"))
            //                    keyWhere.AppendFormat(" and {0} like '{1}%'", pi.Name, pi.GetValue(query, null).ToString().Substring(0, 4));
            //                else
            //                    keyWhere.AppendFormat(" and {0} =@{1}", pi.Name, pi.Name);
            //            }
            //            //else if (pi.Name.ToLower().Contains("id"))
            //            //    keyWhere.AppendFormat(" and {0} =@{1}", pi.Name, pi.Name);
            //            else if (pi.Name.ToLower().EndsWith("_to"))
            //                where.AppendFormat(" and {0} <=@{1}", pi.Name.Substring(0, pi.Name.Length - 3), pi.Name);
            //            else if (pi.Name.ToLower().EndsWith("_from"))
            //                where.AppendFormat(" and {0} >=@{1}", pi.Name.Substring(0, pi.Name.Length - 5), pi.Name);
            //            else
            //            {
            //                if (pi.PropertyType.IsValueType || ((pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))))
            //                    where.AppendFormat(" and {0} =@{0}", pi.Name);
            //                else
            //                {
            //                    if (pi.Name.ToLower().EndsWith("_lower"))
            //                    {
            //                        if (_dbType == DBType.Oracle)
            //                            where.AppendFormat(" and lower({0}) like '%'||@{1}||'%'", pi.Name.Substring(0, pi.Name.Length - 6), pi.Name);
            //                        else if (_dbType == DBType.MySql)
            //                            where.AppendFormat(" and lower({0}) like concat('%',@{1},'%')", pi.Name.Substring(0, pi.Name.Length - 6), pi.Name);
            //                        else
            //                            where.AppendFormat(" and lower({0}) like '%'+@{1}+'%'", pi.Name.Substring(0, pi.Name.Length - 6), pi.Name);
            //                    }
            //                    else if (pi.Name.ToLower().EndsWith("_upper"))
            //                    {
            //                        if (_dbType == DBType.Oracle)
            //                            where.AppendFormat(" and upper({0}) like '%'||@{1}||'%'", pi.Name.Substring(0, pi.Name.Length - 6), pi.Name);
            //                        else if (_dbType == DBType.MySql)
            //                            where.AppendFormat(" and upper({0}) like concat('%',@{1},'%')", pi.Name.Substring(0, pi.Name.Length - 6), pi.Name);
            //                        else
            //                            where.AppendFormat(" and upper({0}) like '%'+@{1}+'%'", pi.Name.Substring(0, pi.Name.Length - 6), pi.Name);
            //                    }
            //                    else
            //                    {
            //                        if (_dbType == DBType.Oracle)
            //                            where.AppendFormat(" and {0} like '%'||@{0}||'%'", pi.Name);
            //                        else if (_dbType == DBType.MySql)
            //                            where.AppendFormat(" and {0} like concat('%',@{0},'%')", pi.Name);
            //                        else
            //                            where.AppendFormat(" and {0} like '%'+@{0}+'%'", pi.Name);
            //                    }
            //                }
            //            }
            //        }
            //    }
            //    if (!string.IsNullOrEmpty(append.Trim()))
            //        where.AppendFormat(" and {0}", append);
            //    if (!string.IsNullOrEmpty(sortfields) && createorder)
            //        where.AppendFormat(" order by {0}", sortfields);
            //    if (keyWhere.Length > 0)
            //        where.Insert(0, keyWhere.ToString());
            //    return where.ToString();
            //}

            /// <summary>
            /// 创建用于分页查询的sql语句
            /// </summary>
            private void BuilderPageSql(string _sql, int _pageSize, int _currentPage, bool noCount = false)
            {
                string sqlSelectRemoved, sqlOrderBy;
                if (_sql.ToUpper().Contains("PARTITION BY"))
                {
                    string sql = _sql.ToUpper();
                    int substar = sql.LastIndexOf("ORDER");
                    sqlOrderBy = sql.Substring(substar, sql.Length - substar);
                    sqlSelectRemoved = sql.Substring(6, sql.Length - 6).Replace(sqlOrderBy, "");
                    _sqlCount = string.Format(@"SELECT COUNT(1) FROM ({0})", _sql);
                }
                else
                {
                    if (!SplitSqlForPaging(_sql, out sqlSelectRemoved, out sqlOrderBy))
                        throw new Exception("分页查询时不能解析用于查询的SQL语句");
                    sqlSelectRemoved = rxOrderBy.Replace(sqlSelectRemoved, "");
                }
                if (_pageSize != 0)
                {
                    if (rxDistinct.IsMatch(sqlSelectRemoved) && _dbType != DBType.MySql)
                    {
                        sqlSelectRemoved = "zj_inner.* FROM (SELECT " + sqlSelectRemoved + ") zj_inner";
                    }
                    if (_dbType == DBType.Oracle)
                    {
                        _sqlPage = string.Format("select * from (select rownum rn,pages.* from (select {1} {0})pages ) zj_paged where rn > {2} and rn <={3}", sqlOrderBy == null ? "ORDER BY  NULL" : sqlOrderBy, sqlSelectRemoved, (_currentPage - 1) * _pageSize, noCount ? (_currentPage * _pageSize) + 1 : _currentPage * _pageSize);
                    }
                    else if (_dbType == DBType.MySql)
                    {
                        _sqlPage = string.Format("SELECT {0} {1} LIMIT {2},{3}", sqlSelectRemoved, sqlOrderBy, (_currentPage - 1) * _pageSize, _pageSize + 1);
                    }
                    else
                    {
                        _sqlPage = string.Format("SELECT * FROM (SELECT ROW_NUMBER() OVER ({0}) rn, {1}) zj_paged WHERE rn> {2} AND rn<= {3}", sqlOrderBy == null ? "ORDER BY (SELECT NULL)" : sqlOrderBy, sqlSelectRemoved, (_currentPage - 1) * _pageSize, noCount ? (_currentPage * _pageSize) + 1 : _currentPage * _pageSize);
                    }
                }
                else
                {
                    _sqlPage = string.Format("SELECT {0} {1}", sqlSelectRemoved, sqlOrderBy);
                }

            }

            /// <summary>
            /// 为分页查询对sql语句进行解析分割
            /// </summary>
            /// <param name="sqlSelectRemoved"></param>
            /// <param name="sqlOrderBy"></param>
            /// <returns></returns>
            private bool SplitSqlForPaging(string _sql, out string sqlSelectRemoved, out string sqlOrderBy)
            {

                sqlSelectRemoved = null;
                sqlOrderBy = null;

                var m = rxOrderBy.Match(_sql);
                Group g;
                if (!m.Success)
                {
                    sqlOrderBy = null;
                }
                else
                {
                    g = m.Groups[0];
                    sqlOrderBy = g.ToString();
                    _sql = _sql.Substring(0, g.Index) + _sql.Substring(g.Index + g.Length);
                    if (_sql.Contains("nulls first"))
                    {
                        _sql = _sql.Replace("nulls first", "");
                        sqlOrderBy = sqlOrderBy + " nulls first";
                    }
                    if (_sql.Contains("nulls last"))
                    {
                        _sql = _sql.Replace("nulls last", "");
                        sqlOrderBy = sqlOrderBy + " nulls last";
                    }

                }

                m = rxColumns.Match(_sql);
                if (!m.Success)
                    return false;

                g = m.Groups[1];
                sqlSelectRemoved = _sql.Substring(g.Index);

                if (rxDistinct.IsMatch(sqlSelectRemoved))
                {
                    _sqlCount = string.Format("select count(*) from ({0} {1} {2})A", _sql.Substring(0, g.Index), m.Groups[1].ToString().Trim(), _sql.Substring(g.Index + g.Length));
                }
                else if (rxGroupBy.IsMatch(sqlSelectRemoved))
                {
                    _sqlCount = string.Format("select MAX(ROWNUM) from ({0} {1} {2})A", _sql.Substring(0, g.Index), m.Groups[1].ToString().Trim(), _sql.Substring(g.Index + g.Length));

                }
                else
                {
                    _sqlCount = _sql.Substring(0, g.Index) + "COUNT(*) " + _sql.Substring(g.Index + g.Length);
                }
                return true;
            }
            //private void tannanwritetxt(string sql)
            //{
            //    string path = @"D:\work\sql.txt";
            //    FileInfo srcFile = new FileInfo(path);
            //    // 创建一个 StreamWriter 对象 writer，它向 FileInfo 的实例 srcFile 所表示的文件追加文本。
            //    StreamWriter writer = srcFile.AppendText();
            //    writer.WriteLine(sql);
            //    // 清理当前编写器的所有缓冲区，并使所有缓冲数据写入基础流
            //    writer.Flush();
            //    writer.Close();
            //}
        }

        /// <summary>
        /// 
        /// </summary>
        public class AnsiString
        {
            /// <summary>
            /// 
            /// </summary>
            /// <param name="str"></param>
            public AnsiString(string str)
            {
                Value = str;
            }
            /// <summary>
            /// 
            /// </summary>
            public string Value { get; private set; }
        }
    }
}
