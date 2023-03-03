using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PORM.Data
{
    /// <summary>
    /// 常用映射关系帮助类
    /// </summary>
    public class CommonMap
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dReader"></param>
        /// <returns></returns>
        public static IEnumerable<T> MapToIEnumerable<T>(IDataReader dReader) where T : class
        {
            using (dReader)
            {
                List<string> drFields = new List<string>(dReader.FieldCount);
                for (int i = 0; i < dReader.FieldCount; i++)
                {
                    drFields.Add(dReader.GetName(i).ToLower());
                }
                while (dReader.Read())
                {
                    T model = Activator.CreateInstance<T>();
                    foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (drFields.Contains(pi.Name.ToLower()))
                        {
                            if (pi.PropertyType.IsEnum)
                            {
                                object enumName = Enum.ToObject(pi.PropertyType, pi.GetValue(model, null));
                                pi.SetValue(model, enumName, null);
                            }
                            else
                            {
                                if (!IsNullOrEmptyOrDBNull(dReader[pi.Name]))
                                {
                                    pi.SetValue(model, MapNullableType(dReader[pi.Name], pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    yield return model;
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="table"></param>
        /// <returns></returns>
        public static IEnumerable<T> MapToIEnumerable<T>(DataTable table) where T : class
        {
            foreach (DataRow row in table.Rows)
            {
                yield return MapToModel<T>(row);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dReader"></param>
        /// <returns></returns>
        public static T MapToModel<T>(IDataReader dReader) where T : class
        {
            using (dReader)
            {
                if (dReader.Read())
                {
                    List<string> drFields = new List<string>(dReader.FieldCount);
                    for (int i = 0; i < dReader.FieldCount; i++)
                    {
                        drFields.Add(dReader.GetName(i).ToLower());
                    }
                    T model = Activator.CreateInstance<T>();
                    foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                    {
                        if (drFields.Contains(pi.Name.ToLower()))
                        {
                            if (pi.PropertyType.IsEnum)
                            {
                                object enumName = Enum.ToObject(pi.PropertyType, pi.GetValue(model, null));
                                pi.SetValue(model, enumName, null);
                            }
                            else
                            {
                                if (!IsNullOrEmptyOrDBNull(dReader[pi.Name]))
                                {
                                    pi.SetValue(model, MapNullableType(dReader[pi.Name], pi.PropertyType), null);
                                }
                            }
                        }
                    }
                    return model;
                }
            }
            return default(T);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dRow"></param>
        /// <returns></returns>
        public static T MapToModel<T>(DataRow dRow) where T : class
        {
            try
            {
                List<string> drItems = new List<string>(dRow.ItemArray.Length);
                for (int i = 0; i < dRow.ItemArray.Length; i++)
                {
                    drItems.Add(dRow.Table.Columns[i].ColumnName.ToLower());
                }
                T model = Activator.CreateInstance<T>();
                foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (drItems.Contains(pi.Name.ToLower()))
                    {
                        if (pi.PropertyType.IsEnum) //属性类型是否表示枚举
                        {
                            object enumName = Enum.ToObject(pi.PropertyType, pi.GetValue(model, null));
                            pi.SetValue(model, enumName, null); //获取枚举值，设置属性值
                        }
                        else
                        {
                            if (!IsNullOrEmptyOrDBNull(dRow[pi.Name]))
                            {
                                pi.SetValue(model, MapNullableType(dRow[pi.Name], pi.PropertyType), null);
                            }
                        }
                    }
                }
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="mType"></param>
        /// <returns></returns>
        public static object MapNullableType(object value, Type mType)
        {
            if (mType.IsGenericType && mType.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                if (IsNullOrEmptyOrDBNull(value))
                    return null;
                System.ComponentModel.NullableConverter nullableConverter = new System.ComponentModel.NullableConverter(mType);
                mType = nullableConverter.UnderlyingType;
            }
            if (mType == typeof(bool) || mType == typeof(Boolean))
            {
                if (value is string)
                {
                    if (value.ToString() == "1")
                        return true;
                    else
                        return false;
                }
            }
            if (mType.IsEnum) //属性类型是否表示枚举
            {
                int intvalue;
                if (int.TryParse(value.ToString(), out intvalue))
                    return Enum.ToObject(mType, Convert.ToInt32(value));
                else
                    return System.Enum.Parse(mType, value.ToString(), false);
            }
            //如果这里传入的是字符串类型，然后接收类型是数值类型需要转换提示
            try
            {
                return Convert.ChangeType(value, mType);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T MapType<T>(object value)
        {
            Type type = typeof(T);
            if (CommonMap.IsNullOrEmptyOrDBNull(value))
                value = type.IsValueType ? Activator.CreateInstance(type) : null;
            if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
            {
                try
                {
                    return (T)Convert.ChangeType(value, type.GetGenericArguments()[0]);
                }
                catch
                {
                    value = null;
                    return (T)value;
                }
            }
            if (type.IsEnum)
                return (T)Enum.ToObject(type, value);
            return (T)Convert.ChangeType(value, typeof(T));
        }

        /// <summary>
        /// 判断null或DBNull或空字符串
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsNullOrEmptyOrDBNull(object obj)
        {
            return ((obj is DBNull) || obj == null || string.IsNullOrEmpty(obj.ToString())) ? true : false;
        }

        /// <summary>
        /// 将string转换成datetime
        /// </summary>
        /// <param name="value">yyyyMMdd或yyyyMMddHHmmss的字符串</param>
        /// <returns></returns>
        public static DateTime ConvertToDateTime(string value)
        {
            if (value.Length == 8)
                return DateTime.ParseExact(value, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
            else if (value.Length == 14)
                return DateTime.ParseExact(value.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            else
                return default(DateTime);
        }

        public static DateTime? TryConvertToDateTime(string value)
        {
            try
            {
                if (value.Length == 8)
                    return DateTime.ParseExact(value, "yyyyMMdd", System.Globalization.CultureInfo.CurrentCulture);
                else if (value.Length == 14)
                    return DateTime.ParseExact(value.ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                //by zdp 日期格式没有有17位和19位的，此处加上17和19位的
                else if (value.Length == 15)
                    return DateTime.ParseExact(DateTime.Parse(value).ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                else if (value.Length == 16)
                    return DateTime.ParseExact(DateTime.Parse(value).ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                else if (value.Length == 17)
                    return DateTime.ParseExact(DateTime.Parse(value).ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                else if (value.Length == 19)
                    return DateTime.ParseExact(DateTime.Parse(value).ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                else if (value.Length == 18)
                    return DateTime.ParseExact(DateTime.Parse(value).ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
                else
                    return DateTime.ParseExact(DateTime.Parse(value).ToString("yyyyMMddHHmmss"), "yyyyMMddHHmmss", System.Globalization.CultureInfo.CurrentCulture);
            }
            catch
            {
                return null;
            }
        }


    }

}
