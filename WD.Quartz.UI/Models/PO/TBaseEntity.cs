using System.ComponentModel;
using FreeSql.DataAnnotations;

namespace WD.Quartz.UI.Models.PO
{
    public class TBaseEntity<T>
    {
        [Column(Position = 1, IsIdentity = true, IsPrimary = true)]
        [Description("主键ID")]
        public T Id { get; set; }

        [Column(Position = -2, DbType = "TIMESTAMP NOT NULL DEFAULT current_timestamp() ON UPDATE current_timestamp()", CanInsert = false, CanUpdate = false)]
        [Description("修改时间")]
        public DateTime UpdateTime { get; set; }

        [Column(Position = -1, DbType = "TIMESTAMP NOT NULL DEFAULT current_timestamp()", CanInsert = false, CanUpdate = false)]
        [Description("创建时间")]
        public DateTime CreateTime { get; set; }
    }
}
