using CourseManagement.Model.EntityModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Demo
{
    public class Test
    {
        public delegate void abc(object o, EventArgs e);
        public event abc a;

        public void DiaoYong()
        {
            //语法如果a！=null调用事件
            a?.Invoke(this, new EventArgs());
            //多播调用
            foreach (abc item in a.GetInvocationList())
            {
                item.Invoke(this, new EventArgs());
            }
        }

        public void test1() {
            string str = "1232";
            int i = str.ChangeInt();
        }


    }
}
