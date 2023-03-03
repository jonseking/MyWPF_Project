using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace CourseManagement.Common
{
    public class CommandBase : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            //只有IsCanExecute为True时才返回执行
            return IsCanExecute?.Invoke(parameter)==true;
        }

        public void Execute(object parameter)
        {
            //如果DoExecute不为空执行
            DoExecute?.Invoke(parameter);
        }

        //定义DoExecute
        public Action<object> DoExecute { get; set; }
        //定义IsCanExecute
        public Func<object,bool> IsCanExecute { get; set; }
    }
}
