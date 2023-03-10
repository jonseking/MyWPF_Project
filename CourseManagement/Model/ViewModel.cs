using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Model
{
    public class ViewModel
    {
        public object D1 { get; set; }
        public object D2 { get; set; }
        public object D3 { get; set; }
        public object D4 { get; set; }
        public object D5 { get; set; }
        public object D6 { get; set; }
        public object D7 { get; set; }
        public object D8 { get; set; }
        public object D9 { get; set; }
        public object D10 { get; set; }
        public object D11 { get; set; }
        public object D12 { get; set; }
        public object D13 { get; set; }
        public object D14 { get; set; }
        public object D15 { get; set; }
        public object D16 { get; set; }
        public object D17 { get; set; }
        public object D18 { get; set; }
        public object D19 { get; set; }
        public object D20 { get; set; }
        public object D21 { get; set; }
        public object D22 { get; set; }
        public object D23 { get; set; }
        public object D24 { get; set; }
        public object D25 { get; set; }
        public object D26 { get; set; }
        public object D27 { get; set; }
        public object D28 { get; set; }
        public object D29 { get; set; }
        public object D30 { get; set; }
        public object D31 { get; set; }
        public object D32 { get; set; }
        public object D33 { get; set; }
        public object D34 { get; set; }
        public object D35 { get; set; }
        public object D36 { get; set; }
        public object D37 { get; set; }
        public object D38 { get; set; }
        public object D39 { get; set; }
        public object D40 { get; set; }
        public object D41 { get; set; }
        public object D42 { get; set; }
        public object D43 { get; set; }
        public object D44 { get; set; }
        public object D45 { get; set; }
        public object D46 { get; set; }
        public object D47 { get; set; }
        public object D48 { get; set; }
        public object D49 { get; set; }
        public object D50 { get; set; }
    }

    public sealed class ViewAttribute : System.Attribute
    {
        public string ViewName { get; set; }
    }

    public class ViewModelHelper
    {
        /// <summary>
        /// ViewModel 转成 DBModel
        /// </summary>
        public static T GetDBModel<T>(ViewModel viewModel) where T : class
        {
            T dbModel = Activator.CreateInstance<T>();

            Type dbModelType = dbModel.GetType();
            Type viewModelType = viewModel.GetType();

            object[] customAttribute = null;
            foreach (PropertyInfo piDB in dbModelType.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                customAttribute = piDB.GetCustomAttributes(typeof(ViewAttribute), false);
                if (customAttribute.Count() == 1)
                {
                    ViewAttribute viewAttribute = customAttribute[0] as ViewAttribute;
                    PropertyInfo piView = viewModelType.GetProperty(viewAttribute.ViewName);
                    object value = piView.GetValue(viewModel, null);
                    string typeName = piDB.PropertyType.FullName;
                    if (typeName.StartsWith("System.Nullable"))
                    {
                        if (typeName.Contains("DateTime"))
                        {
                            if ((value != null) && (!string.IsNullOrEmpty(value.ToString())))
                            {
                                value = DateTime.Parse(value.ToString());
                                piDB.SetValue(dbModel, value, null);
                            }
                        }
                        else
                        {
                            piDB.SetValue(dbModel, value, null);
                        }
                    }
                    else
                    {
                        piDB.SetValue(dbModel, value, null);
                    }
                    //switch (piDB.PropertyType.Name)
                    //{
                    //    case "DateTime?":
                    //        {
                    //            if ((value != null) && (!string.IsNullOrEmpty(value.ToString())))
                    //            {
                    //                value = DateTime.Parse(value.ToString());
                    //                piDB.SetValue(dbModel, value, null); 
                    //            }
                    //            break;
                    //        }
                    //    default: piDB.SetValue(dbModel, value, null); break;
                    //}
                }
            }

            return dbModel;
        }

        /// <summary>
        /// DBModel 转成 ViewModel
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dbModel"></param>
        /// <returns></returns>
        public static ViewModel GenerateViewModel<T>(T dbModel) where T : class
        {
            ViewModel viewModel = new ViewModel();

            Type dbModelType = dbModel.GetType();
            Type viewModelType = viewModel.GetType();

            object[] customAttribute = null;
            foreach (PropertyInfo piDB in dbModelType.GetProperties(BindingFlags.GetProperty | BindingFlags.Public | BindingFlags.Instance))
            {
                customAttribute = piDB.GetCustomAttributes(typeof(ViewAttribute), false);
                if (customAttribute.Count() == 1)
                {
                    ViewAttribute viewAttribute = customAttribute[0] as ViewAttribute;
                    PropertyInfo piView = viewModelType.GetProperty(viewAttribute.ViewName);
                    piView.SetValue(viewModel, piDB.GetValue(dbModel, null), null);
                }
            }

            return viewModel;
        }
    }
}
