using Form.DataAccess;
using Form.Data;

namespace CourseManagement.Model.EntityModel
{
    public class BASE_CUSTOMERGPT
    {
        [ColumnAttribute(PrimaryKey = true)]
        [ViewAttribute(ViewName = "主键GUID")]
        public string ID { get; set; }

        [ViewAttribute(ViewName = "消息信息")]
        public string MESSAGEINFO { get; set; }

        [ViewAttribute(ViewName = "消息类型")]
        public string MESSAGETYPE { get; set; }

        [ViewAttribute(ViewName = "添加息时间")]
        public string ADDTIME { get; set; }

        [ViewAttribute(ViewName = "用户编号")]
        public string USERID { get; set; }

    }
}
