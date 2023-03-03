using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace PORM.Data
{
    public class QueryParam
    {
        /// <summary>
        /// 获取查询条件
        /// </summary>
        /// <returns></returns>
        public static string GetWhereString<T>(T obj, bool hasWhere = false, bool trim = true) where T : class
        {
            string querywhere = "";

            Type t = typeof(T);
            foreach (System.Reflection.PropertyInfo pi in t.GetProperties())
            {
                object paraValue = pi.GetValue(obj, null);

                if (paraValue != null && (!string.IsNullOrEmpty(paraValue.ToString())))
                {
                    var customAttribute = pi.GetCustomAttributes(typeof(SerachColAttribute), false);
                    if (customAttribute.Count() == 1)
                    {
                        string strform = "";
                        string realColumnName = pi.Name;
                        if (!string.IsNullOrEmpty((customAttribute[0] as SerachColAttribute).RealColumnName))
                        {
                            realColumnName = (customAttribute[0] as SerachColAttribute).RealColumnName;
                        }

                        OperaSerach condition = (customAttribute[0] as SerachColAttribute).OpSerach;
                        switch (condition)
                        {
                            case OperaSerach.like: strform = " and {0} like '%{1}%' "; break;
                            case OperaSerach.lefLike: strform = " and {0} like '{1}%'"; break;
                            case OperaSerach.rightLike: strform = " and {0} like '%{1}'"; break;
                            case OperaSerach.等于: strform = " and {0} = '{1}'"; break;
                            case OperaSerach.不等于: strform = " and {0} <> '{1}'"; break;
                            case OperaSerach.大于: strform = " and {0} > '{1}'"; break;
                            case OperaSerach.大于等于: strform = " and {0} >= '{1}'"; break;
                            case OperaSerach.小于: strform = " and {0} < '{1}'"; break;
                            case OperaSerach.小于等于: strform = " and {0} <= '{1}'"; break;
                            case OperaSerach.包含: strform = " and {0} in ('{1}')"; break;
                            case OperaSerach.不包含: strform = " and {0} not in ('{1}')"; break;
                            case OperaSerach.产品名组合包含: strform = " and instr(PRODUCTNAME||NVL(DRUGGOODSNAME,'')||NVL(PYCODE,''),'{1}')>0"; break;
                            case OperaSerach.企业组合包含: strform = " and instr(COMPANYNAME_TB||COMPANYNAME_SC,'{1}')>0"; break;
                            default: break;
                        }

                        var DoesValues = (customAttribute[0] as SerachColAttribute).DoesValues == null ? "" : (customAttribute[0] as SerachColAttribute).DoesValues;

                        if (string.IsNullOrEmpty(DoesValues) || (!string.IsNullOrEmpty(DoesValues) && !DoesValues.Contains(paraValue.ToString())))
                        {
                            if (pi.PropertyType == typeof(DateTime))
                            {
                                DateTime dateTime = (DateTime)paraValue;
                                if (dateTime.Year > 1900)
                                {
                                    if (trim)
                                        querywhere += string.Format(strform.Replace("'", ""), realColumnName, "to_date('" + Convert.ToDateTime(paraValue.ToString().Trim()).ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')");
                                    else
                                        querywhere += string.Format(strform.Replace("'", ""), realColumnName, "to_date('" + Convert.ToDateTime(paraValue).ToString("yyyy-MM-dd HH:mm:ss") + "','YYYY-MM-DD HH24:MI:SS')");
                                }
                            }
                            else
                            {
                                string stringValue = paraValue.ToString();
                                if ((customAttribute[0] as SerachColAttribute).OrdinalIgnoreCase)
                                {
                                    realColumnName = " UPPER(" + realColumnName + ") ";
                                    stringValue = stringValue.ToUpper();
                                }

                                if (condition == OperaSerach.包含 || condition == OperaSerach.不包含)
                                {
                                    stringValue = stringValue.Replace(",", "','");
                                }

                                if (trim)
                                {
                                    if (realColumnName.Contains("DRUGNAME") || realColumnName.Contains("PRODUCTNAME"))
                                    {
                                        querywhere += string.Format(strform + " or UPPER(PYCODE) like '%{1}%')", "(" + realColumnName, stringValue.Trim());
                                    }
                                    else
                                    {
                                        querywhere += string.Format(strform, realColumnName, stringValue.Trim());
                                    }
                                }
                                else
                                {
                                    if (realColumnName.Contains("DRUGNAME") || realColumnName.Contains("PRODUCTNAME"))
                                    {
                                        querywhere += string.Format(strform + " UPPER(PYCODE) like '%{1}%')", "(" + realColumnName, stringValue);
                                    }
                                    else
                                    {
                                        querywhere += string.Format(strform, realColumnName, stringValue);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (hasWhere)
                return querywhere;
            else
                return " where 1=1 " + querywhere;
        }

        //public static T GetQueryModel<T>(HttpContext context)
        //{
        //    T model = Activator.CreateInstance<T>();

        //    foreach (PropertyInfo pi in model.GetType().GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
        //    {
        //        if (!(context.Request.QueryString[pi.Name] == null))
        //        {
        //            if (context.Request.QueryString[pi.Name] != "null")
        //            {
        //                if (!string.IsNullOrEmpty(context.Request.QueryString[pi.Name]))
        //                {
        //                    pi.SetValue(model, CommonMap.MapNullableType(context.Request.QueryString[pi.Name].Trim(), pi.PropertyType), null);
        //                }
        //                else
        //                {
        //                    if (pi.PropertyType == typeof(System.String) || pi.PropertyType == typeof(System.Object))
        //                    {
        //                        pi.SetValue(model, CommonMap.MapNullableType(context.Request.QueryString[pi.Name].Trim(), pi.PropertyType), null);
        //                    }
        //                }
        //            }
        //        }
        //        else if (!(context.Request.Form[pi.Name] == null))
        //        {
        //            if (context.Request.Form[pi.Name] != "null")
        //            {
        //                if (!string.IsNullOrEmpty(context.Request.Form[pi.Name]))
        //                {
        //                    pi.SetValue(model, CommonMap.MapNullableType(context.Request.Form[pi.Name].Trim(), pi.PropertyType), null);
        //                }
        //                else
        //                {
        //                    if (pi.PropertyType == typeof(System.String) || pi.PropertyType == typeof(System.Object))
        //                    {
        //                        pi.SetValue(model, CommonMap.MapNullableType(context.Request.Form[pi.Name].Trim(), pi.PropertyType), null);
        //                    }
        //                }
        //            }
        //        }
        //    }

        //    return model;
        //}

        /// <summary>
        /// 获取查询条件（包括JqGrid传过来的查询条件）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="context"></param>
        /// <returns></returns>
        //public static QueryCondition GetQueryCondition<T>(HttpContext context, bool trim = true) where T : class
        //{
        //    QueryConditionJq queryConditionJq = QueryParamJq.GetQueryCondition(context);

        //    T searchModel = GetQueryModel<T>(context);
        //    string whereString = GetWhereString<T>(searchModel, true, trim);

        //    if (string.IsNullOrEmpty(queryConditionJq.WhereString))
        //        whereString = " where 1 = 1 " + whereString;
        //    else
        //        whereString = queryConditionJq.WhereString + whereString;

        //    QueryCondition QueryModel = new QueryCondition()
        //    {
        //        WhereString = whereString,
        //        CurrentPageIndex = queryConditionJq.CurrentPageIndex,
        //        PageRows = queryConditionJq.PageRows,
        //        OrderString = queryConditionJq.OrderString
        //    };

        //    if (!string.IsNullOrEmpty(QueryModel.OrderString))
        //        QueryModel.WhereString = QueryModel.WhereString + " order by " + QueryModel.OrderString;

        //    return QueryModel;
        //}

        //public static QueryCondition GetQueryCondition<T>(HttpContext context, T searchModel, bool trim = true) where T : class
        //{
        //    QueryConditionJq queryConditionJq = QueryParamJq.GetQueryCondition(context);

        //    string whereString = GetWhereString<T>(searchModel, true, trim);

        //    if (string.IsNullOrEmpty(queryConditionJq.WhereString))
        //        whereString = " where 1 = 1 " + whereString;
        //    else
        //        whereString = queryConditionJq.WhereString + whereString;


        //    QueryCondition QueryModel = new QueryCondition()
        //    {
        //        WhereString = whereString,
        //        CurrentPageIndex = queryConditionJq.CurrentPageIndex,
        //        PageRows = queryConditionJq.PageRows,
        //        OrderString = queryConditionJq.OrderString
        //    };

        //    if (!string.IsNullOrEmpty(QueryModel.OrderString))
        //        QueryModel.WhereString = QueryModel.WhereString + " order by " + QueryModel.OrderString;

        //    return QueryModel;
        //}
    }


    public class QueryCondition
    {
        /// <summary>
        /// where 条件
        /// </summary>
        public string WhereString { get; set; }

        /// <summary>
        /// 当前页码
        /// </summary>
        public int CurrentPageIndex { get; set; }

        /// <summary>
        /// 没页显示的行数
        /// </summary>
        public int PageRows { get; set; }

        /// <summary>
        /// 排序String
        /// </summary>
        public string OrderString { get; set; }
    }
}
