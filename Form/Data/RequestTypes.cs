using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Form.Data
{
    //请求类型
    public static class RequestTypes
    {
        public static string JSON { get { return "json"; } }

        public static string X_WWW_FORM_URLENCODED { get { return "x-www-form-urlencoded"; } }
    }
}
