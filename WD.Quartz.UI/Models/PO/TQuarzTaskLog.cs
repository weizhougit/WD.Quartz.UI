using FreeSql.DataAnnotations;
using System.ComponentModel;
using System.Xml.Linq;

namespace WD.Quartz.UI.Models.PO
{
    [Table(Name = "quarz_task_log")]
    public class TQuarzTaskLog : TBaseEntity<long>
    {
        /// <summary>
        /// 任务名
        /// </summary>
        [Description("任务名")]
        public string TaskName { get; set; }

        /// <summary>
        /// 分组名
        /// </summary>
        [Description("分组名")]
        public string GroupName { get; set; }

        /// <summary>
        /// 任务开始时间
        /// </summary>
        [Description("任务开始时间")]
        public DateTime? BeginDate { get; set; }

        /// <summary>
        /// 任务结束时间
        /// </summary>
        [Description("任务结束时间")]
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 任务执行结果
        /// </summary>
        [Description("任务执行结果")]
        public string Msg { get; set; }
    }
}
