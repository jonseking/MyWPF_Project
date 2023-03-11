using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CourseManagement.Modules.Views.Auxiliary
{
    /// <summary>
    /// CustomerGptView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomerGptView : UserControl
    {
        public CustomerGptView()
        {
            InitializeComponent();
        }

        //public static string GetRichText(DependencyObject obj)
        //{
        //    return (string)obj.GetValue(RichTextProperty);
        //}

        //public static void SetRichText(DependencyObject obj, string value)
        //{
        //    obj.SetValue(RichTextProperty, value);
        //}

        //public static readonly DependencyProperty RichTextProperty =
        //    DependencyProperty.RegisterAttached("RichText", typeof(string), typeof(CustomerGptView),
        //        new FrameworkPropertyMetadata
        //        {
        //            BindsTwoWayByDefault = true,
        //            PropertyChangedCallback = (obj, e) =>
        //            {
        //                var richTextBox = (RichTextBox)obj;
        //                var text = GetRichText(richTextBox);
        //                richTextBox.AppendText(text);
        //                richTextBox.AppendText(Environment.NewLine);
        //                richTextBox.ScrollToEnd();
        //            }
        //        });
    }
}
