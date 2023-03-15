using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CourseManagement.Demo
{
    public static class StringHeaper
    {
        public static int ChangeInt(this string str)
        {
            int res=-1;
            int.TryParse(str, out res);
            return res;
        }
    }
}
